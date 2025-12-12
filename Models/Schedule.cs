namespace Models
{
    public class Schedule
    {
        public int Id { get; set; }

        // Связь с группами
        public int GroupId { get; set; }
        public Group Group { get; set; } = new();

        // Связь с преподавателем
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        // Дата и время
        public DateOnly Date { get; set; }
        public int PairNumber { get; set; } // 1-8 пар

        // Предмет
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        // Аудитория
        public string Classroom { get; set; }
    }
}