using Microsoft.EntityFrameworkCore;
using Models;

namespace Storage
{
    public class ApplicationDbContext : DbContext
    {
        // Для миграций используем конструктор без параметров
        public ApplicationDbContext() { }

        // Для runtime используем конструктор с параметрами
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<DecanatWorker> DecanatWorkers { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Exam> Exams { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Строка подключения для миграций (разработки)
                // Для продакшена нужно использовать appsettings.json
                var connectionString = "Host=localhost;Port=5432;Database=DecanatDB;Username=postgres;Password=123;";

                optionsBuilder.UseNpgsql(connectionString);

                // Для отладки SQL запросов (можно удалить в продакшене)
                optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
                optionsBuilder.EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. User конфигурация
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();

                // Связь User -> Student (один-к-одному)
                entity.HasOne(u => u.Student)
                    .WithOne(s => s.User)
                    .HasForeignKey<User>(u => u.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Связь User -> Teacher (один-к-одному)
                entity.HasOne(u => u.Teacher)
                    .WithOne(t => t.User)
                    .HasForeignKey<User>(u => u.TeacherId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Связь User -> DecanatWorker (один-к-одному)
                entity.HasOne(u => u.DecanatWorker)
                    .WithOne(d => d.User)
                    .HasForeignKey<User>(u => u.DecanatWorkerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 2. Student конфигурация
            modelBuilder.Entity<Student>(entity =>
            {
                // Связь Student -> Group (многие-к-одному)
                entity.HasOne(s => s.Group)
                    .WithMany(g => g.Students)
                    .HasForeignKey(s => s.GroupId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Связь Student -> Exams (один-ко-многим)
                entity.HasMany(s => s.Exams)
                    .WithOne(e => e.Student)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 3. Teacher конфигурация
            modelBuilder.Entity<Teacher>(entity =>
            {
                // Связь Teacher -> Schedules (один-ко-многим)
                entity.HasMany(t => t.Schedules)
                    .WithOne(s => s.Teacher)
                    .HasForeignKey(s => s.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Связь Teacher -> Exams (один-ко-многим)
                entity.HasMany(t => t.Exams)
                    .WithOne(e => e.Teacher)
                    .HasForeignKey(e => e.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // 4. Group конфигурация
            modelBuilder.Entity<Group>(entity =>
            {
                // Связь Group -> Students (один-ко-многим) уже настроена в Student

                // Связь Group -> Schedules (один-ко-многим)
                entity.HasMany(g => g.Schedules)
                    .WithOne(s => s.Group)
                    .HasForeignKey(s => s.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 5. Schedule конфигурация
            modelBuilder.Entity<Schedule>(entity =>
            {
                // Связь Schedule -> Group (многие-к-одному)
                entity.HasOne(s => s.Group)
                    .WithMany(g => g.Schedules)
                    .HasForeignKey(s => s.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Связь Schedule -> Teacher (многие-к-одному)
                entity.HasOne(s => s.Teacher)
                    .WithMany(t => t.Schedules)
                    .HasForeignKey(s => s.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Связь Schedule -> Subject (многие-к-одному)
                entity.HasOne(s => s.Subject)
                    .WithMany(sub => sub.Schedules)
                    .HasForeignKey(s => s.SubjectId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // 6. Subject конфигурация
            modelBuilder.Entity<Subject>(entity =>
            {
                // Связь Subject -> Schedules (один-ко-многим) уже настроена в Schedule

                // Связь Subject -> Exams (один-ко-многим)
                entity.HasMany(sub => sub.Exams)
                    .WithOne(e => e.Subject)
                    .HasForeignKey(e => e.SubjectId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // 7. Exam конфигурация
            modelBuilder.Entity<Exam>(entity =>
            {
                // Связь Exam -> Subject (многие-к-одному)
                entity.HasOne(e => e.Subject)
                    .WithMany(sub => sub.Exams)
                    .HasForeignKey(e => e.SubjectId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Связь Exam -> Student (многие-к-одному)
                entity.HasOne(e => e.Student)
                    .WithMany(s => s.Exams)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Связь Exam -> Teacher (многие-к-одному)
                entity.HasOne(e => e.Teacher)
                    .WithMany(t => t.Exams)
                    .HasForeignKey(e => e.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Ограничение для оценки (0-100)
                entity.Property(e => e.ExamScore)
                    .IsRequired();
            });

            // 8. DecanatWorker конфигурация
            modelBuilder.Entity<DecanatWorker>(entity =>
            {
                // Связь DecanatWorker -> User (один-к-одному) уже настроена в User
            });
        }
    }
}