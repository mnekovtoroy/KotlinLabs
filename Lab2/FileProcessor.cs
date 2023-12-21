using System.Xml;

namespace Lab2
{

    public class FileProcessor
    {
        public static void Process(string path)
        {
            TimeSpan processTime;
            AddressMapper addressMapper = new AddressMapper();

            var info = new FileInfo(path);
            try
            {
                if (info.Extension == ".xml")
                {
                    ProcessXML(path, addressMapper, out processTime);
                }
                else if (info.Extension == ".csv")
                {
                    ProcessCSV(path, addressMapper, out processTime);
                }
                else
                {
                    throw new ArgumentException("Invalid argument: wrong path");
                }
            } catch(Exception ex)
            {
                Console.WriteLine("Возникла ошибка при выполнении прокраммы. Проверьте правильность введенного пути.");
                return;
            }

            DisplayDuplicates(addressMapper.duplicates);
            DisplayFloorCount(addressMapper.floors);
            Console.WriteLine($"Обработка файла заняла {processTime} секунд.");
        }

        private static void ProcessXML(string path,
            AddressMapper addressMapper,
            out TimeSpan processTime)
        { 
            var document = new XmlDocument();
            document.Load(path);

            XmlNodeList addresses = document.SelectNodes("/root/item");
            var start = DateTime.Now;
            foreach (XmlNode address in addresses)
            {
                Address curr = new Address()
                {
                    city = address.Attributes["city"].Value,
                    street = address.Attributes["street"].Value,
                    house = int.Parse(address.Attributes["house"].Value),
                    floor = int.Parse(address.Attributes["floor"].Value)
                };
                addressMapper.MapBuilding(curr);
            }
            var finish = DateTime.Now;
            processTime = finish - start;
        }



        private static void ProcessCSV(string path,
            AddressMapper addressMapper,
            out TimeSpan processTime)
        {
            var start = DateTime.Now;

            using (var reader = new StreamReader(path))
            {
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var address = line.Split(';');
                    Address curr = new Address()
                    {
                        city = address[0],
                        street = address[1],
                        house = int.Parse(address[2]),
                        floor = int.Parse(address[3])
                    };
                    addressMapper.MapBuilding(curr);
                }
            }
            var finish = DateTime.Now;

            processTime = finish - start;
        }

        private static void DisplayDuplicates(Dictionary<Address, int> uniqueAddresses)
        {
            Console.WriteLine("Дублирующиеся записи:");
            foreach (var address in uniqueAddresses)
            {
                if (address.Value > 1)
                {
                    Console.WriteLine($"{address.Key.ToString()}: повторяется {address.Value} раз(а).");
                }
            }
        }

        private static void DisplayFloorCount(Dictionary<string, int[]> floorCount)
        {
            Console.WriteLine("Посчёт количества зданий разной этажности, исключая дубликаты (город: кол-во 1/2/3/4/5 этажных зданий): ");
            foreach (var city in floorCount)
            {
                Console.WriteLine($"{city.Key}: {city.Value[0]}/{city.Value[1]}/{city.Value[2]}/{city.Value[3]}/{city.Value[4]}");
            }
        }
    }
}
