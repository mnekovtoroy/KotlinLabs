using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;

namespace Lab3
{
    public class CommandHander
    {
        public static TimeOnly CurrentTime
        {
            get
            {
                return TimeOnly.FromDateTime(DateTime.Now);
            }
        }
        public static int CurrentDay
        {
            get
            {
                var day = (int)DateTime.Now.DayOfWeek - 1;
                if (day == -1) day = 6;
                return day;
            }
        }
        public static int CurrentWeek
        {
            get
            {
                return CultureInfo
                    .CurrentCulture
                    .Calendar
                    .GetWeekOfYear(
                        DateTime.Now,
                        CalendarWeekRule.FirstFourDayWeek,
                        DayOfWeek.Monday) % 2;
            }
        }

        public static void HandleCommand(long chatId, string message, ITelegramBotClient client)
        {
            if (message.StartsWith("/start"))
            {
                StartCommand(chatId, client);
                return;
            }
            if(!(message.StartsWith("/nextlesson ") ||
                message.StartsWith("/tomorrow ") ||
                message.StartsWith("/week ") ||
                message.StartsWith("/day "))) {
                throw new ArgumentException("Неизвестная команда! Повторите попытку.");
            }


            var parts = message.Split(' ');
            if (parts.Length < 2)
                throw new ArgumentException("Ошибка: нехватка аргументов в команде");

            string command = parts[0];
            string group = parts[1];
            string pattern = @"^\d{4}$";
            if (!Regex.IsMatch(group, pattern))
                throw new ArgumentException("Ошибка: номер группы должен состоять из 4х цифр.");


            if (command == "/nextlesson")
            {
                NextLessonCommandAsync(chatId, group, client);
            }
            else if (command == "/tomorrow")
            {
                TomorrowCommandAsync(chatId, group, client);
            }
            else if (command == "/week")
            {
                WeekCommandAsync(chatId, group, client);
            }
            else if (command == "/day")
            {
                int day;
                if (parts.Count() < 3 || !int.TryParse(parts[2], out day) || day > 7 || day < 1)
                {
                    throw new ArgumentException("Ошибка в аргументе {day_number}: аргумент либо отсутсвует, либо не находится в пределах от 1 до 7.");
                }
                DayCommandAsync(chatId, group, day - 1, client);
            }
        }

        public static void StartCommand(long chatId, ITelegramBotClient client)
        {
            client.SendTextMessageAsync(
                chatId: chatId,
                text: "Добро пожаловать в бота!\n" +
                "Доступные комманды:\n" +
                "/nextlesson {group}\n" +
                "/tomorrow {group}\n" +
                "/week {group}\n" +
                "/day {group} {day_number}");
        }

        public static async Task NextLessonCommandAsync(long chat_id, string group, ITelegramBotClient client)
        {
            var schedule = await ApiRequester.GetScheduleAsync(group);

            if(schedule.days.All(d => d.lessons_byWeek.All(l => l.Count == 0)))
            {
                client.SendTextMessageAsync(chat_id, "Похоже, у вас нет предстоящих пар!");
                return;
            }
            var curr_time = CurrentTime;
            int curr_day = CurrentDay;
            var curr_week = CurrentWeek;

            if(!schedule.days[curr_day].lessons_byWeek[curr_week].Any(l => l.start_time > curr_time))
            {
                curr_day++;
                curr_time = TimeOnly.MinValue;
                if (curr_day == 7)
                {
                    curr_day = 0;
                    curr_week = 1 - curr_week;
                }
            }
            while(!schedule.days[curr_day].lessons_byWeek[curr_week].Any())
            {
                curr_day++;
                if(curr_day == 7)
                {
                    curr_day = 0;
                    curr_week = 1 - curr_week;
                }
            }

            var next_lesson = schedule.days[curr_day].lessons_byWeek[curr_week].First(l => l.start_time > curr_time);

            client.SendTextMessageAsync(chat_id, $"Следующая пара у группы {group}:\n" + next_lesson.ToString());
        }

        public static async Task TomorrowCommandAsync(long chat_id, string group, ITelegramBotClient client)
        {
            var schedule = await ApiRequester.GetScheduleAsync(group);

            int curr_day = CurrentDay;
            int curr_week = CurrentWeek;

            curr_day++;
            if (curr_day == 7)
            {
                curr_day = 0;
                curr_week = 1 - curr_week;
            }

            var lessons = schedule.days[curr_day].lessons_byWeek[curr_week];
            StringBuilder sb = new StringBuilder();
            sb.Append($"Пары на завтра у группы {group}:");
            for(int i = 0; i < lessons.Count; i++)
            {
                sb.Append($"\n{i+1}. ");
                sb.Append(lessons[i].ToString());
            }
            if(lessons.Count == 0)
            {
                sb.Append("\nПохоже, на этот день нет пар!");
            }
            client.SendTextMessageAsync(chat_id, sb.ToString());
        }

        public static async Task WeekCommandAsync(long chat_id, string group, ITelegramBotClient client)
        {
            var schedule = await ApiRequester.GetScheduleAsync(group);
            StringBuilder sb = new StringBuilder();
            sb.Append($"Расписание на неделю у группы {group}:");
            foreach(var day in schedule.days.Where(d => d.lessons_byWeek[CurrentWeek].Any()))
            {
                sb.Append($"\n    {day.name}:");
                for(int i = 0; i < day.lessons_byWeek[CurrentWeek].Count; i++)
                {
                    sb.Append($"\n{i + 1}. ");
                    sb.Append(day.lessons_byWeek[CurrentWeek][i].ToString());
                }
            }
            if(schedule.days.All(d => d.lessons_byWeek[CurrentWeek].Count == 0))
            {
                sb.Append("\nПохоже, на этой неделе нет пар!");
            }
            client.SendTextMessageAsync(chat_id, sb.ToString());
        }

        public static async Task DayCommandAsync(long chat_id, string group, int day, ITelegramBotClient client)
        {
            var schedule = await ApiRequester.GetScheduleAsync(group);
            var lessons = schedule.days[day].lessons_byWeek[CurrentWeek];
            StringBuilder sb = new StringBuilder();
            sb.Append($"{schedule.days[day].name} у группы {group}:");
            for (int i = 0; i < lessons.Count; i++)
            {
                sb.Append($"\n{i + 1}. ");
                sb.Append(lessons[i].ToString());
            }
            if (lessons.Count == 0)
            {
                sb.Append("\nПохоже, на этот день нет пар!");
            }
            client.SendTextMessageAsync(chat_id, sb.ToString());
        }
    }
}
