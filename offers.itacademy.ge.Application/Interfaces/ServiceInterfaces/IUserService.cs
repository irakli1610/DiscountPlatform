using offers.itacademy.ge.Application.Models.Images;
using offers.itacademy.ge.Application.Models.Users;
using offers.itacademy.ge.Domain.Users;

namespace offers.itacademy.ge.Application.Interfaces.ServiceInterfaces
{
    public interface IUserService
    {
        Task<List<TResponse>> GetUsersAsync<TUserEntity, TResponse>(int pageNumber, int pageSize, CancellationToken token)
              where TUserEntity : User
              where TResponse : UserResponseModel;

        Task<bool> ActivateCompanyAsync(int companyId, CancellationToken token);
        Task<bool> UpdateCustomerCategoriesAsync(int customerId, List<int> categoryIds, CancellationToken token);
        Task<string> AuthenticateAsync(UserLoginRequestModel model, CancellationToken token);
        Task<string> UploadCompanyImage(int companyId, ImageRequestModel fileForm, CancellationToken token);
        Task<TResponse> RegisterAsync<TRequest, TResponse>(TRequest model, CancellationToken token)
                where TRequest : PasswordRequestModel
                where TResponse : UserResponseModel;
        Task<TResponse> UpdateUserAsync<TRequest, TResponse>(int userId, TRequest model, CancellationToken token)
          where TRequest : UserRequestModel
          where TResponse : UserResponseModel;

        Task<UserResponseModel> GetByIdAsync(int userId, CancellationToken token);
        Task<Customer> GetCustomerWithCategories(int customerId, CancellationToken token);
    }
}
