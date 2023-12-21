namespace Lab3
{
    public class ApiRequester
    {
        public static async Task<Schedule> GetScheduleAsync(string group, string day = null)
        {
            string requestUri = $"https://digital.etu.ru/api/mobile/schedule?groupNumber={group}";
            if(day != null)
            {
                requestUri += $"&weekDay={day}";
            }
            string responseBody;
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(requestUri);
                if (!response.IsSuccessStatusCode)
                    throw new Exception("Ошибка при выполнении запроса: "+response.StatusCode.ToString());
                responseBody = await response.Content.ReadAsStringAsync();
            }
            return Parser.Parse(responseBody, group);            
        }
    }
}