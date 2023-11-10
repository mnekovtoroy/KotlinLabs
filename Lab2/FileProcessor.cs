using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Lab2
{
    public class FileProcessor
    {
        public static void Process(string path)
        {
            DateTime start = DateTime.Now;

            var info = new FileInfo(path);

            Dictionary<Address, int> uniqueAddresses;
            Dictionary<string, int[]> floorCount;
            if(info.Extension == ".xml")
            {
                using (var reader = XmlReader.Create(path))
                {
                    GoThroughFile(new XmlFileReader(reader), out uniqueAddresses, out floorCount);
                }
            } else if(info.Extension == ".csv")
            {
                using (var reader = new StreamReader(path))
                {
                    GoThroughFile(new CsvFileReader(reader), out uniqueAddresses, out floorCount);
                }
            } else
            {
                throw new ArgumentException("Path is not valid");
            }

            DateTime finish = DateTime.Now;

            DisplayRepeatedAddresses(uniqueAddresses);
            DisplayFloorCount(floorCount);
            Console.WriteLine($"Обработка файла заняла {(finish - start).TotalSeconds} секунд.");
        }

        private static void GoThroughFile(IFileReader reader,
            out Dictionary<Address, int> uniqueAddresses,
            out Dictionary<string, int[]> floorCount)
        {
            uniqueAddresses = new Dictionary<Address, int>();
            floorCount = new Dictionary<string, int[]>();
            int i = 0;
            while (!reader.EndOfFile())
            {
                i++;
                Address curr = reader.Next();
                if (!uniqueAddresses.ContainsKey(curr))
                {
                    uniqueAddresses.Add(curr, 1);

                    if (!floorCount.ContainsKey(curr.city))
                    {
                        floorCount.Add(curr.city, new int[5] { 0, 0, 0, 0, 0 });
                    }
                    floorCount[curr.city][int.Parse(curr.floor) - 1]++;
                } else
                {
                    uniqueAddresses[curr]++;
                }
                if (i % 100000 == 0)
                {
                    Console.WriteLine(i + " адресов обработано...");
                };
            }
        }

        private static void DisplayRepeatedAddresses(Dictionary<Address, int> uniqueAddresses)
        {
            Console.WriteLine("Дублирующиеся записи:");
            foreach(var address in uniqueAddresses)
            {
                if(address.Value > 1)
                {
                    Console.WriteLine($"{address.Key.ToString()}: повторяется {address.Value} раз(а).");
                }
            }
        }

        private static void DisplayFloorCount(Dictionary<string, int[]> floorCount)
        {
            Console.WriteLine("Посчёт количества зданий разной этажности, исключая дубликаты (город: кол-во 1/2/3/4/5 этажных зданий): ");
            foreach(var city in floorCount)
            {
                Console.WriteLine($"{city.Key}: {city.Value[0]}/{city.Value[1]}/{city.Value[2]}/{city.Value[3]}/{city.Value[4]}");
            }
        }

    }
}
