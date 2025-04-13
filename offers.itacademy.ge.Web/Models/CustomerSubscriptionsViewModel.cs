using offers.itacademy.ge.Application.Models.Categories;

namespace offers.itacademy.ge.Web.Models
{
    public class CustomerSubscriptionsViewModel
    {
        public List<CategoryResponseModel> AvailableCategories { get; set; }
        public List<int> SubscribedCategoryIds { get; set; }
    }
}
