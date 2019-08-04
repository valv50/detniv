using ShipmentDiscount.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentDiscount.Helpers
{
    public class Rule
    {
        public Func<ShippingContext, ProcessingResult> Func { get; set; }

        public Rule(Func<ShippingContext, ProcessingResult> func)
        {     
            Func = func;
        }

    }
}
