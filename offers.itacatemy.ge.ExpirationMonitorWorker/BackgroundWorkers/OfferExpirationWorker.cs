using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using offers.itacademy.ge.Application.Interfaces;
using offers.itacademy.ge.Domain.ProductOffers;

namespace offers.itacademy.ge.ExpirationMonitorWorker.BackgroundWorkers
{
    public class OfferExpirationWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<OfferExpirationWorker> _logger;
        private readonly TimeSpan _pollingInterval;

        public OfferExpirationWorker(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<OfferExpirationWorker> logger,
            IConfiguration configuration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;

            var intervalSeconds = configuration.GetValue<int>("WorkerSettings:PollingIntervalSeconds");
            _pollingInterval = TimeSpan.FromSeconds(intervalSeconds > 0 ? intervalSeconds : 60);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OfferExpirationWorker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Checking for expired offers at {Time}", DateTime.UtcNow);

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        using (var transaction = await unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.RepeatableRead, stoppingToken))
                        {
                            try
                            {
                                var expiredOffers = await unitOfWork.ProductOffers
                                    .GetExpiredOffersAsync(stoppingToken);

                                if (!expiredOffers.Any())
                                {
                                    _logger.LogInformation("No expired offers found.");
                                }
                                else
                                {
                                    foreach (var offer in expiredOffers)
                                    {
                                        _logger.LogInformation("Archiving offer {OfferId}: {OfferName}", offer.Id, offer.Name);
                                        offer.Status = OfferStatus.Archived;
                                        unitOfWork.ProductOffers.Update(offer);
                                    }

                                    await unitOfWork.SaveChangesAsync(stoppingToken);
                                    _logger.LogInformation("Archived {Count} expired offers.", expiredOffers.Count);
                                }

                                await transaction.CommitAsync(stoppingToken);
                            }
                            catch (Exception ex)
                            {
                                await transaction.RollbackAsync(stoppingToken);
                                throw;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing expired offers.");
                }

                _logger.LogDebug("Waiting for next check in {IntervalSeconds} seconds.", _pollingInterval.TotalSeconds);
                await Task.Delay(_pollingInterval, stoppingToken);
            }

            _logger.LogInformation("OfferExpirationWorker stopped due to cancellation.");
        }
    }
}