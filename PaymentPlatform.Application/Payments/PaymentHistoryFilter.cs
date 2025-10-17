using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentPlatform.Application.Payments
{
    public class PaymentHistoryFilter
    {
        public string? Status { get; set; } = null; // "Success", "Failed", "Pending"
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
