namespace Models
{
    public class Exam
    {
        public int Id { get; set; }

        // Предмет
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        // Студент
        public int StudentId { get; set; }
        public Student Student { get; set; }

        // Преподаватель
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        // Оценка
        public int ExamScore { get; set; }

        // Дата экзамена
        public DateOnly ExamDate { get; set; }

        // Тип оценки (экзамен, зачет и т.д.)
        public ExamType ExamType { get; set; }

        // Когда была поставлена оценка
        public DateTime GradedAt { get; set; }
    }

    public enum ExamType
    {
        Exam,      // Экзамен
        Test,      // Зачет
        Coursework // Курсовая работа
    }
}