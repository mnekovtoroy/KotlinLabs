namespace Lab2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                //Ввод пути до файла
                string path;
                bool flag = false;
                do
                {

                    Console.WriteLine("Введите путь до .csv или .xml файла (или \"exit\" для выхода из программы).");
                    path = Console.ReadLine();
                    if (path == "exit")
                    {
                        Console.WriteLine("Выход из программы...");
                        return;
                    }
                    if (string.IsNullOrEmpty(path))
                    {
                        Console.WriteLine("Введите хоть что-то!");
                        continue;
                    }
                    FileInfo fileInfo = new FileInfo(path);
                    if (fileInfo.Exists && (fileInfo.Extension == ".csv" || fileInfo.Extension == ".xml"))
                    {
                        flag = true;
                    }
                    else
                    {
                        Console.WriteLine("Файл либо не существует, либо не подходит. Введите путь заново.");
                    }
                } while (!flag);
                Console.WriteLine("Начанаем обработку файла...");
                //Исполнение
                FileProcessor.Process(path);

                Console.WriteLine("Вы можете обработать другой файл или выйти из программы.");
            }
        }
    }
}