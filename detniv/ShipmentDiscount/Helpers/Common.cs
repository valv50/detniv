using ShipmentDiscount.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentDiscount.Helpers
{
    public class Common
    {
        public static ProcessingResult ProcessRules(ShippingContext shippingContext, List<Rule> rules)
        {
            ProcessingResult ruleProcessingResult = new ProcessingResult();

            rules.ForEach(rule =>
            {
                ruleProcessingResult = rule.Func(shippingContext);
                shippingContext.processingResult.discount = ruleProcessingResult.discount;
            });

            shippingContext.totalDiscount += ruleProcessingResult.discount;

            return ruleProcessingResult;
        }

        public static decimal GetPrice(List<ShippingProvider> shippingProviders
            , string providerCode, string packageSize)
        {
            decimal price = 
                shippingProviders
                .Where(w => w.providerCode == providerCode)
                .Select(s => s.prices)
                .FirstOrDefault()
                .Where(w => w.packageSize == packageSize)
                .Select(s => s.price)
                .FirstOrDefault();

            return price;
        }
    }
}
