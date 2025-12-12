namespace Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Связь с пользователем
        public User User { get; set; }

        // Расписание преподавателя
        public List<Schedule> Schedules { get; set; } = new();

        // Экзамены, которые принимает преподаватель
        public List<Exam> Exams { get; set; } = new();
    }
}