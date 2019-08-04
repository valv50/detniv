
using ShipmentDiscount.Handlers;
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

            FileProcessor fileProcessor = new FileProcessor();

            List<string> processResult =  fileProcessor.Process(fileName);

            processResult.ForEach(resultLine =>
            {
                Console.WriteLine(resultLine);
            });

            Console.ReadLine();
        }
    }
}
