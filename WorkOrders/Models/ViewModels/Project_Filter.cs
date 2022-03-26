using Microsoft.AspNetCore.Mvc.Rendering;

namespace WorkOrders.Models.ViewModels
{
    public class Project_Filter
    {
        public int? CustomerId { get; set; }
        public int? ProjectId { get; set; }
        public List<Project>? Project { get; set; }
        public SelectList? Customers { get; internal set; }
        public SelectList? Projects { get; set; }
        //public bool? IsActive { get; set; }


        //public SelectList? Customers { get; set; }

    }
}
