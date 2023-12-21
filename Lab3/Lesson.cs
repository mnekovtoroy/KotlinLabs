using System.Text;

namespace Lab3
{
    public class Lesson
    {
        public string teacher { get; set; }
        public string subjectType { get; set; }
        public int week { get; set; }
        public string name { get; set; }
        public TimeOnly start_time { get; set; }
        public TimeOnly end_time { get; set; }
        public string room { get; set; }

        public override string ToString()
        {
            StringBuilder shortname_builder = new StringBuilder();
            shortname_builder.Append(teacher.Split(' ')[0]);
            for (int i = 1; i < teacher.Split(' ').Length; i++)
            {
                shortname_builder.Append(" " + teacher.Split(' ')[i].First() + ".");
            }
            string teacherStr = teacher != "" ? shortname_builder.Append(", ").ToString() : "";
            string roomStr = room != "" ? $"{room}, " : "";
            return $"{name}, {roomStr}{subjectType}.\n" +
                $"{teacherStr}{start_time}-{end_time}";
        }
    }
}
