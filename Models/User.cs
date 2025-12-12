namespace Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }

        // Внешние ключи для связи с конкретными сущностями
        public int? StudentId { get; set; }
        public Student? Student { get; set; }

        public int? TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        public int? DecanatWorkerId { get; set; }
        public DecanatWorker? DecanatWorker { get; set; }
    }

    public enum UserRole
    {
        Student,
        Teacher,
        DecanatWorker
    }
}