using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentDiscount.Entities
{
    public class ShippingContext
    {
        public ShippingItem shippingItem { get; set; }

        public decimal totalDiscount { get; set; }

        public ProcessingResult processingResult { get; set; }

        public List<ShippingProvider> shippingProviders { get; set; }
    }
}
