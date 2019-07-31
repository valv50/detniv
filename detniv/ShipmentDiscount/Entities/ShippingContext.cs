using ShipmentDiscount.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentDiscount.Entities
{
    public class ShippingContext
    {
        public int ShippingNumber { get; set; }

        public decimal TotalDiscount { get; set; }

        public decimal Price { get; set; }

        public IShippingProvider ShippingProvider { get; set; }

        public List<IShippingProvider> ShippingProviders { get; set; }
    }
}
