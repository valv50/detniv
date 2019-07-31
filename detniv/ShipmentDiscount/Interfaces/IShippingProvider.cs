using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentDiscount.Interfaces
{
    public class IShippingProvider
    {
        public string Name { get; set; }

        public decimal SSizePrice { get; set; }

        public decimal MSizePrice { get; set; }

        public decimal LSizePrice { get; set; }
    }
}
