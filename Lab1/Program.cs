using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Web;

namespace Lab1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //Считать введенные пользователем данные
            Console.WriteLine("Введите поисковой запрос:");
            string query = Console.ReadLine();

            //Сделать запрос к серверу
            string encoded_query = HttpUtility.UrlEncode(query);
            string searchUrl = $"https://ru.wikipedia.org/w/api.php?action=query&list=search&utf8=&format=json&srsearch=\"{encoded_query}\"";
            string resopnseStr;
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(searchUrl);
                if (!response.IsSuccessStatusCode)
                {
                    Console.Write("Что-то пошло не так при выполнении запроса. Выход из программы...");
                    return;
                }
                resopnseStr = await response.Content.ReadAsStringAsync();
            }

            //Распарсить ответ
            JObject resopnseJson = JObject.Parse(resopnseStr);
            JArray searchResults = resopnseJson["query"]["search"] as JArray;

            //Вывести результат поиска
            if(searchResults.Count == 0)
            {
                Console.WriteLine("К сожалений, по вашему запросу ничего не нашлось :(");
                Console.WriteLine("Выход из программы...");
                return;
            }
            Console.WriteLine("Результаты поиска:\n");
            for (int i = 0; i < searchResults.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {searchResults[i]["title"]}.\n");
            }

            //Открыть нужную страницу в браузере
            Console.WriteLine($"Какую страницу вы хотите открыть (1-{searchResults.Count})?\n" +
                $"Ввод не числа или числа не из списка приведет к выходу из программы.");
            string jumpToInput = Console.ReadLine();
            int jumpTo ;
            if (!int.TryParse(jumpToInput, out jumpTo) || jumpTo < 1 || jumpTo > searchResults.Count)
            {
                Console.WriteLine("Выход из программы...");
                return;
            }
            string jumpToUrl = $"https://ru.wikipedia.org/w/index.php?curid={searchResults[jumpTo - 1]["pageid"]}";
            Process.Start(new ProcessStartInfo
            {
                FileName = jumpToUrl,
                UseShellExecute = true
            });
            Console.WriteLine($"Открытие браузера и выход из программы...");
        }
    }
}