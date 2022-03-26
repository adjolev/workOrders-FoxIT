using Microsoft.AspNetCore.Mvc.Rendering;

namespace WorkOrders.Models.ViewModels
{
    public class Customers_Filter
    {
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? Edb { get; set; }
        public List<Customer> Custumer { get; set; }
        public SelectList? CustomersNames { get; internal set; }
        public SelectList? CustomersCity { get; internal set; }
        public SelectList? CustomersEdb { get; internal set; }
    }
}
