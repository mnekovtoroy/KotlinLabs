using Newtonsoft.Json.Linq;

namespace Lab3
{
    public class Parser
    {    
        public static Schedule Parse(string jsonSchedule, string group)
        {
            JObject jObject = JObject.Parse(jsonSchedule);
            var schedule = new Schedule();

            schedule.group = group;
            schedule.days = new List<Day>();
            for(int i = 0; i < 7; i++)
            {
                var day = new Day()
                {
                    day_number = i,
                    name = jObject[group]["days"][$"{i}"]["name"].Value<string>(),
                    lessons_byWeek = new List<List<Lesson>>()
                    {
                        new List<Lesson>(),
                        new List<Lesson>()
                    }
                };
                var jLessons = jObject[group]["days"][$"{i}"]["lessons"] as JArray;
                for(int j = 0; j < jLessons.Count; j++)
                {
                        
                    var lesson = new Lesson()
                    {
                        teacher = jLessons[j]["teacher"].Value<string>(),
                        subjectType = jLessons[j]["subjectType"].Value<string>(),
                        week = jLessons[j]["week"].Value<int>(),
                        name = jLessons[j]["name"].Value<string>(),
                        room = jLessons[j]["room"].Value<string>(),
                        start_time = TimeOnly.Parse(jLessons[j]["start_time"].Value<string>()),
                        end_time = TimeOnly.Parse(jLessons[j]["end_time"].Value<string>())
                    };
                    if(lesson.week == 1)
                    {
                        day.lessons_byWeek[1].Add(lesson);
                    } else
                    {
                        day.lessons_byWeek[0].Add(lesson);
                    }
                }
                schedule.days.Add(day);
            }
            return schedule;
        }
    }
}
