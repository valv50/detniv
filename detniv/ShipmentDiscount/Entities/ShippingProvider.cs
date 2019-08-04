using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentDiscount.Entities
{
    public class ShippingProvider
    {
        public string providerCode { get; set; }

        public List<(string packageSize, decimal price)> prices = new List<(string packageSize, decimal price)>();
    }
}
