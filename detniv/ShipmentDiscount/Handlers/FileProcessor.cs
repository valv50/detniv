using ShipmentDiscount.Entities;
using ShipmentDiscount.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentDiscount.Handlers
{
    public class FileProcessor
    {
        public List<string> Process(string filePath)
        {
            List<string> processResult = new List<string>();

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

            var lines = File.ReadAllLines(filePath);

            var shippingProcessor = new ShippingProcessor();

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
                                , price : 1.5M
                            )
                            ,(
                                packageSize : "M"
                                , price : 4.9M
                            )
                            ,(
                                packageSize : "L"
                                , price : 6.9M
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
                                , price : 2M
                            )
                            ,(
                                packageSize : "M"
                                , price : 3M
                            )
                            ,(
                                packageSize : "L"
                                , price : 4M
                            )
                        }
                    }
                });

            shippingProcessor.SetShippingProvider(shippingProvider);

            var rules = new List<Rule>();

            rules.AddRange(
                //  All S shipments should always match the lowest S package price among the providers.
                new[]
                {
                    new Rule(
                        r =>
                    {
                        ProcessingResult processingResult = new ProcessingResult();

                        if (r.shippingItem.packageSize == "S")
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
                    // Third L shipment via LP should be free, but only once a calendar month.
                    new Rule(
                        r =>
                    {
                        ProcessingResult processingResult = new ProcessingResult();

                        if (r.shippingItem.packageSize == "L"
                            && r.shippingItem.providerCode == "LP"
                            && r.shippingItem.shippingNumber == 3)
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
                    // Accumulated discounts cannot exceed 10 € in a calendar month. If there are not enough funds to fully cover a discount this calendar month, it should be covered partially.
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

            shippingProcessor.Process(items)
                .ForEach(s =>
                {
                    if (!s.correctLine)
                    {
                        processResult.Add($"{s.line} Ignored");
                        return;
                    }

                    string discount = s.ruleProcessingResult.discount == 0 
                        ? "-" 
                        : s.ruleProcessingResult.discount.ToString("F");

                    processResult.Add($"{s.line} {s.ruleProcessingResult.price:0.00} {discount}");
                });

            return processResult;
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
