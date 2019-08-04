using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentDiscount.Entities
{
    public class ShippingItem
    {
        public DateTime? date { get; set; }

        public string packageSize { get; set; }

        public string providerCode { get; set; }

        public int shippingNumber { get; set; }
    }
}
