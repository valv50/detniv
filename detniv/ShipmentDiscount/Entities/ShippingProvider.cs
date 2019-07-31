using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentDiscount.Entities
{
    public class ShippingProvider
    {
        public string Providerode { get; set; }

        public decimal Price { get; set; }

        public string PackageSize { get; set; }
    }
}
