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
            bool isPresent = Database.IsPresent(chatId);
            ConversationState state = isPresent ? ConversationState.OngoingConversation : ConversationState.NewConversation;
            
            if(state == ConversationState.NewConversation)
            {
                if(message.StartsWith("/setgroup "))
                {
                    var m_parts = message.Split(' ');
                    if (m_parts.Length < 2)
                        throw new ArgumentException("Ошибка: нехватка аргументов в команде");
                    string m_group = m_parts[1];
                    string m_pattern = @"^\d{4}$";
                    if (!Regex.IsMatch(m_group, m_pattern))
                        throw new ArgumentException("Ошибка: номер группы должен состоять из 4х цифр.");
                    SetGroupCommandAsync(chatId, m_group, client);
                    HelpCommand(chatId, client);
                    return;
                } else
                {
                    client.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Пожалуйста, введите номер группы, используя /setgroup {group}");
                    return;
                }
            }


            if (message.StartsWith("/help"))
            {
                HelpCommand(chatId, client);
                return;
            }
            if(!(message.StartsWith("/nextlesson") ||
                message.StartsWith("/tomorrow") ||
                message.StartsWith("/week") ||
                message.StartsWith("/day") ||
                message.StartsWith("/updategroup "))) {
                throw new ArgumentException("Неизвестная команда! Повторите попытку.");
            }

            string group = Database.SelectGroup(chatId);
            string pattern = @"^\d{4}$";
            if (!Regex.IsMatch(group, pattern))
                throw new ArgumentException("Ошибка: номер группы должен состоять из 4х цифр.");


            if (message.StartsWith("/nextlesson"))
            {
                NextLessonCommandAsync(chatId, group, client);
            }
            else if (message.StartsWith("/tomorrow"))
            {
                TomorrowCommandAsync(chatId, group, client);
            }
            else if (message.StartsWith("/week"))
            {
                WeekCommandAsync(chatId, group, client);
            }
            else if (message.StartsWith("/day"))
            {
                int day;
                if (message.Split(' ').Count() < 2 || !int.TryParse(message.Split(' ')[1], out day) || day > 7 || day < 1)
                {
                    throw new ArgumentException("Ошибка в аргументе {day_number}: аргумент либо отсутсвует, либо не находится в пределах от 1 до 7.");
                }
                DayCommandAsync(chatId, group, day - 1, client);
            } else if(message.StartsWith("/updategroup "))
            {
                var m_parts = message.Split(' ');
                if (m_parts.Length < 2)
                    throw new ArgumentException("Ошибка: нехватка аргументов в команде");
                string m_group = m_parts[1];
                string m_pattern = @"^\d{4}$";
                if (!Regex.IsMatch(m_group, m_pattern))
                    throw new ArgumentException("Ошибка: номер группы должен состоять из 4х цифр.");
                UpdateGroupCommandAsync(chatId, m_group, client);
            }
        }

        public static void HelpCommand(long chatId, ITelegramBotClient client)
        {
            client.SendTextMessageAsync(
                chatId: chatId,
                text: "Доступные комманды:\n" +
                "/help - список команд;\n" +
                "/nextlesson - узнать следующую пару;\n" +
                "/tomorrow - посмотреть расписание на завтра;\n" +
                "/week - посмотреть расписание на всю неделю;\n" +
                "/day {day_number} - посмотреть расписание на день недели;\n" +
                "/updategroup {group} - выбрать другую группу.");
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

        public static async Task SetGroupCommandAsync(long chat_id, string group, ITelegramBotClient client)
        {
            Database.InsertGroup(chat_id, group);
            client.SendTextMessageAsync(chat_id, "Группа успешно сохранена!");
        }

        public static async Task UpdateGroupCommandAsync(long chat_id, string group, ITelegramBotClient client)
        {
            Database.UpdateGroup(chat_id, group);
            client.SendTextMessageAsync(chat_id, "Группа успешно сохранена!");
        }
    }
}
