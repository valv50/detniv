using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace detniv
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = "";
            if (args.Length == 0)
            {
                fileName = "C:\\TEMP\\input.txt";
            }
            else
            {
                fileName = args[0];
            }

            if (!File.Exists(fileName))
            {
                Console.WriteLine("File does not exist");
            }

            CalculateFee calculateFee = new CalculateFee();
            var lines = File.ReadAllLines(fileName);
            string period = "";
            foreach (string line in lines)
            {
                string[] elements = line.Split('\t');
                if (elements.Length < 3)
                {
                    Console.WriteLine();
                    continue;
                }
                if (period != "" && period != elements[0].Substring(5, 2))
                {
                    calculateFee = new CalculateFee();
                }
                decimal clientFee = calculateFee
                    .CalculateClientFees(
                    elements[1], decimal.Parse(elements[2]));

                Console.WriteLine(
                    string.Format(
                    "{0}\t{1}\t{2}"
                    , elements[0]
                    , elements[1]
                    , clientFee.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)));
                period = elements[0].Substring(5, 2);
            }
            Console.ReadLine();

        }
    }
}
