using Mapster;

namespace offers.itacademy.ge.API.Infrastructure.Mappings
{
    public static class MapsterConfiguration
    {
        public static void RegisterMaps(this IServiceCollection services)
        {
            //This method Should hold mapster adaptation configurations if necessary, one is provided below

            //TypeAdapterConfig<Book, BookResponseModel>.NewConfig().TwoWays();

        }
    }
}
