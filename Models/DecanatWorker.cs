namespace Models
{
    public class DecanatWorker
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Связь с пользователем
        public User User { get; set; }

    }
}