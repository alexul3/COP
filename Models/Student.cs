namespace Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Связь с группой
        public int GroupId { get; set; }
        public Group Group { get; set; }

        // Связь с пользователем
        public User User { get; set; }

        // Оценки студента
        public List<Exam> Exams { get; set; } = new();
    }
}