namespace Controllers
{
    // DTO для входа в систему
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    // DTO для информации о пользователе
    public class UserInfo
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public int? StudentId { get; set; }
        public int? TeacherId { get; set; }
        public int? DecanatWorkerId { get; set; }
    }

    // DTO для расписания
    public class ScheduleDto
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public int PairNumber { get; set; }
        public GroupDto Group { get; set; }
        public SubjectDto Subject { get; set; }
        public TeacherDto Teacher { get; set; }
        public string Classroom { get; set; }
    }

    // DTO для оценки
    public class ExamDto
    {
        public int Id { get; set; }
        public int ExamScore { get; set; }
        public SubjectDto Subject { get; set; }
        public StudentDto Student { get; set; }
        public TeacherDto Teacher { get; set; }
    }

    // DTO для студента
    public class StudentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public GroupDto Group { get; set; }
    }

    // DTO для преподавателя
    public class TeacherDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    // DTO для группы
    public class GroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    // DTO для предмета
    public class SubjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    // DTO для создания/обновления расписания
    public class CreateScheduleDto
    {
        public int GroupId { get; set; }
        public int TeacherId { get; set; }
        public int SubjectId { get; set; }
        public DateOnly Date { get; set; }
        public int PairNumber { get; set; }
        public string Classroom { get; set; }
    }

    // DTO для выставления оценки
    public class CreateExamDto
    {
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
        public int ExamScore { get; set; }
    }
}