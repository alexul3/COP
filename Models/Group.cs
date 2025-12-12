namespace Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Student> Students { get; set; } = new();
        public List<Schedule> Schedules { get; set; } = new(); // Добавлено
    }
}