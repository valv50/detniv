using ShipmentDiscount.Entities;
using ShipmentDiscount.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentDiscount.Handlers
{
    public class ShippingProcessor
    {
        private List<ShippingProvider> _shippingProviders = new List<ShippingProvider>();

        private List<Rule> _rules = new List<Rule>();

        public ShippingProcessor()
        {

        }

        public ShippingProcessor(List<ShippingProvider> shippingProviders, List<Rule> rules)
        {
            _shippingProviders = shippingProviders;

            _rules = rules;
        }

        public void SetShippingProvider(List<ShippingProvider> shippingProviders)
        {
            _shippingProviders = shippingProviders;
        }

        public void SetRules(List<Rule> rules)
        {
            _rules = rules;
        }

        public List<(ProcessingResult ruleProcessingResult, bool correctLine, string line)>
            Process(List<(ShippingItem shippingItem, bool correctLine, string line)> items)
        {
            List<(ProcessingResult ruleProcessingResult, bool correctLine, string line)> processingResultItems =
                new List<(ProcessingResult ruleProcessingResult, bool correctLine, string line)>();

            ShippingContext shippingContext = new ShippingContext();

            shippingContext.shippingProviders = _shippingProviders;

            DateTime? lastShippingDate = null;

            items.ForEach(item =>
            {
                (ProcessingResult ruleProcessingResult, bool correctLine, string line) processingResultItem;

                processingResultItem = (new ProcessingResult(), item.correctLine, item.line);
                
                if (!item.correctLine)
                {
                    processingResultItems.Add(processingResultItem);

                    return;
                }

                if (lastShippingDate == null ||
                    ((DateTime)lastShippingDate).Month != ((DateTime)(item.shippingItem.date)).Month)
                {
                    shippingContext.totalDiscount = 0M;

                    lastShippingDate = item.shippingItem.date;
                }

                processingResultItem.ruleProcessingResult.price = 
                    Common.GetPrice(_shippingProviders
                    , item.shippingItem.providerCode
                    , item.shippingItem.packageSize);

                shippingContext.shippingItem = item.shippingItem;

                shippingContext.processingResult =
                    processingResultItem.ruleProcessingResult;

                ProcessingResult processingResult = 
                    Common.ProcessRules(shippingContext, _rules);

                shippingContext.processingResult = processingResult;

                processingResultItem.ruleProcessingResult.discount = processingResult.discount;

                processingResultItem.ruleProcessingResult.price -= processingResult.discount;

                processingResultItems.Add(processingResultItem);
            });

            return processingResultItems;
        }


    }
}
