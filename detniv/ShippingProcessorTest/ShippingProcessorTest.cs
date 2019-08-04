using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShipmentDiscount.Entities;
using ShipmentDiscount.Handlers;
using ShipmentDiscount.Helpers;

namespace ShipmentDiscountTest
{
    [TestClass]
    public class ShippingProcessorTest
    {
        [TestMethod]
        public void ShippingProcessorProcessTest()
        {            
            DateTime? date;
            string packageSize;
            string shippingProviderCode;

            List<(string packageSize, string shippingProviderCode, DateTime date, int shipmentNumber)> shipmentNumbers =
                new List<(string packageSize, string shippingProviderCode, DateTime date, int shipmentNumber)>();

            List<(ShippingItem shippingItem, bool correctLine, string line)> items =
                new List<(ShippingItem shippingItem, bool correctLine, string line)>();

            int shipmentNumber = 0;
            DateTime? previousDate = null;
            bool correctLine;
                        
            var shippingProcessor = new ShippingProcessor();

            SetShippingProvider(shippingProcessor);

            SetRules(shippingProcessor);

            items = SetItems(shippingProcessor);

            string s1 = "";

            shippingProcessor.Process(items)
                .ForEach(s =>
                {
                    if (!s.correctLine)
                    {
                        s1 += $"{s.line} Ignored\r\n";
                        return;
                    }

                    string discount = s.ruleProcessingResult.discount == 0
                        ? "-"
                        : s.ruleProcessingResult.discount.ToString("F");

                    s1 += $"{s.line} {s.ruleProcessingResult.price:0.00} {discount}\r\n";
                });

            Assert.AreEqual(s1,
                @"2015-02-01 S MR 3.00 -
2015-02-02 S MR 3.00 -
2015-02-03 L LP 2.50 5.40
2015-02-05 S LP 2.50 -
2015-02-06 S MR 3.00 -
2015-02-06 L LP 3.30 4.60
2015-02-07 L MR 5.00 -
2015-02-08 M MR 4.00 -
2015-02-09 L LP 7.90 -
2015-02-10 L LP 7.90 -
2015-02-10 S MR 3.00 -
2015-02-10 S MR 3.00 -
2015-02-11 L LP 7.90 -
2015-02-12 M MR 4.00 -
2015-02-13 M LP 5.90 -
2015-02-15 S MR 3.00 -
2015-02-17 L LP 7.90 -
2015-02-17 S MR 3.00 -
2015-02-24 L LP 7.90 -
2015-02-29 CUSPS Ignored
2015-03-01 S MR 3.00 -
");

        }

        private void SetShippingProvider(ShippingProcessor shippingProcessor)
        {
            var shippingProvider = new List<ShippingProvider>();

            shippingProvider.AddRange(
                new[]
                {
                new ShippingProvider
                {
                    providerCode = "LP"
                    , prices = new List<(string packageSize, decimal price)>
                    {
                        (
                            packageSize : "S"
                            , price : 2.5M
                        )
                        ,(
                            packageSize : "M"
                            , price : 5.9M
                        )
                        ,(
                            packageSize : "L"
                            , price : 7.9M
                        )
                    }
                }
                ,
                new ShippingProvider
                {
                    providerCode = "MR"
                    , prices = new List<(string packageSize, decimal price)>
                    {
                        (
                            packageSize : "S"
                            , price : 3M
                        )
                        ,(
                            packageSize : "M"
                            , price : 4M
                        )
                        ,(
                            packageSize : "L"
                            , price : 5M
                        )
                    }
                }
                });

            shippingProcessor.SetShippingProvider(shippingProvider);
        }

        private void SetRules(ShippingProcessor shippingProcessor)
        {
            var rules = new List<Rule>();

            rules.AddRange(
                new[]
                {
                new Rule(
                    r =>
                {
                    ProcessingResult processingResult = new ProcessingResult();

                    if (r.shippingItem.packageSize == "L")
                    {
                        processingResult.discount =
                            r.processingResult.price -
                            r.shippingProviders
                                .SelectMany(s => s.prices.Where(w => w.packageSize == "S")
                                , (shippingProviders, prices) =>
                                new
                                {
                                    prices.price
                                })
                                .Select(s => s.price)
                                .Min();
                    }

                    return processingResult;
                }
                )
            ,
                new Rule(
                    r =>
                {
                    ProcessingResult processingResult = new ProcessingResult();

                    if (r.shippingItem.packageSize == "L"
                        && r.shippingItem.providerCode == "LP"
                        && r.shippingItem.shippingNumber == 2)
                    {
                        processingResult.discount = r.processingResult.price;
                    }
                    else
                    {
                        processingResult.discount = r.processingResult.discount;
                    }

                    return processingResult;
                }
                )
            ,
                new Rule(
                    r =>
                {
                    ProcessingResult processingResult = new ProcessingResult();

                        processingResult.discount =
                            Math.Min(10 - r.totalDiscount, r.processingResult.discount);

                    return processingResult;
                }
                )
                }

                );

            shippingProcessor.SetRules(rules);
        }

        private List<(ShippingItem shippingItem, bool correctLine, string line)> 
            SetItems(ShippingProcessor shippingProcessor)
        {
            bool correctLine;
            DateTime? date;
            string packageSize;
            string shippingProviderCode;
            int shipmentNumber = 0;
            DateTime? previousDate = null;

            List<(string packageSize, string shippingProviderCode, DateTime date, int shipmentNumber)> shipmentNumbers =
                new List<(string packageSize, string shippingProviderCode, DateTime date, int shipmentNumber)>();

            List<(ShippingItem shippingItem, bool correctLine, string line)> items =
                new List<(ShippingItem shippingItem, bool correctLine, string line)>();

            List<string> lines = new List<string>();

            lines.AddRange(
                new[]
                {
                    "2015-02-01 S MR"
                    , "2015-02-02 S MR"
                    , "2015-02-03 L LP"
                    , "2015-02-05 S LP"
                    , "2015-02-06 S MR"
                    , "2015-02-06 L LP"
                    , "2015-02-07 L MR"
                    , "2015-02-08 M MR"
                    , "2015-02-09 L LP"
                    , "2015-02-10 L LP"
                    , "2015-02-10 S MR"
                    , "2015-02-10 S MR"
                    , "2015-02-11 L LP"
                    , "2015-02-12 M MR"
                    , "2015-02-13 M LP"
                    , "2015-02-15 S MR"
                    , "2015-02-17 L LP"
                    , "2015-02-17 S MR"
                    , "2015-02-24 L LP"
                    , "2015-02-29 CUSPS"
                    , "2015-03-01 S MR"
                }
);
            
            foreach (string line in lines)
            {
                correctLine = ProcessLine(line, out date, out packageSize, out shippingProviderCode);

                if (correctLine)
                {
                    var shipmentDateNumber
                        = shipmentNumbers
                        .Where(w => w.shippingProviderCode == shippingProviderCode
                            && w.packageSize == packageSize)
                        .Select(s => new System.Tuple<DateTime, int>(s.date, s.shipmentNumber))
                        .FirstOrDefault();

                    if (shipmentDateNumber != null)
                    {
                        previousDate = shipmentDateNumber.Item1;

                        shipmentNumber = shipmentDateNumber.Item2;

                        if (previousDate != null && ((DateTime)previousDate).Month != ((DateTime)date).Month)
                        {
                            shipmentNumber = 0;
                        }
                    }
                    else
                    {
                        shipmentNumber = 0;
                    }

                    shipmentNumber++;

                    shipmentNumbers
                        .RemoveAll(w => w.shippingProviderCode == shippingProviderCode
                            && w.packageSize == packageSize);

                    shipmentNumbers.Add((packageSize, shippingProviderCode, (DateTime)date, shipmentNumber));
                }

                ShippingItem shippingItem = new ShippingItem();

                shippingItem.date = date;

                shippingItem.packageSize = packageSize;

                shippingItem.providerCode = shippingProviderCode;

                shippingItem.shippingNumber = shipmentNumber;

                items.Add((shippingItem, correctLine, line));
            }

            return items;
        }

        private bool ProcessLine(string line,
            out DateTime? date, out string packageSize, out string shippingProviderCode)
        {
            bool lineCorrect = true;

            packageSize = shippingProviderCode = null;

            date = null;

            try
            {
                string[] lineValues = line.Split(new string[] { "/", " " }, 0);

                if (lineValues.Length >= 3)
                {
                    date = Convert.ToDateTime(lineValues[0]);

                    packageSize = lineValues[1];

                    shippingProviderCode = lineValues[2];
                }
                else
                {
                    lineCorrect = false;
                }
            }
            catch
            {
                lineCorrect = false;
            }

            return lineCorrect;
        }
    }
}
                                                 