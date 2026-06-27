
using System.Collections.Generic;

namespace E_commerce.Models.ViewModels
{
    public class OrderResult
    {
        public bool Success { get; set; }
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public List<string> AdjustmentMessages { get; set; } = new List<string>();
    }
}