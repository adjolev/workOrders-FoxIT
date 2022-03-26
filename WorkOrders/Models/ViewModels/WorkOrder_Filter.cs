using Microsoft.AspNetCore.Mvc.Rendering;

namespace WorkOrders.Models.ViewModels
{
    public class WorkOrder_Filter
    {
        public string? CustomerNote { get; set; }
        public int? CustomerId { get; set; }
        public int? ProjectId { get; set; }
        public string? UserId { get; set; }

        public List<WorkOrder>? WorkOrders { get; set; }
        public SelectList? Customers { get; set; }
        public SelectList? Projects { get; set; }
        public SelectList? Users { get; set; }

    }
}
