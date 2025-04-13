using offers.itacademy.ge.Domain.Categories;
using offers.itacademy.ge.Domain.Purchases;

namespace offers.itacademy.ge.Domain.Users
{
    public class Customer : User
    {
        public decimal Balance { get; set; } = 0;
        public List<Purchase> Purchases { get; set; } = [];
        public List<Category> SelectedCategories { get; set; } = [];

    }
}
