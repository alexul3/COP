namespace Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Расписание по предмету
        public List<Schedule> Schedules { get; set; } = new();

        // Экзамены по предмету
        public List<Exam> Exams { get; set; } = new();
    }
}