using Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Models;

namespace Tests
{
    public class TestApplication : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Удаляем реальный DbContext
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Добавляем InMemory Database
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });

                // Создаем scope и базу данных
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    db.Database.EnsureDeleted(); // Очищаем базу перед тестами
                    db.Database.EnsureCreated();
                    SeedTestData(db); // Заполняем тестовыми данными
                }
            });
        }

        private void SeedTestData(ApplicationDbContext context)
        {
            // Тестовые данные
            var group = new Group { Id = 1, Name = "ИВТ-101" };
            var student = new Student { Id = 1, Name = "Иван Иванов", GroupId = 1 };
            var teacher = new Teacher { Id = 1, Name = "Петр Петров" };
            var subject = new Subject { Id = 1, Name = "Программирование"};
            var schedule = new Schedule
            {
                Id = 1,
                GroupId = 1,
                TeacherId = 1,
                SubjectId = 1,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                PairNumber = 1,
                Classroom = "101"
            };

            context.Groups.Add(group);
            context.Students.Add(student);
            context.Teachers.Add(teacher);
            context.Subjects.Add(subject);
            context.Schedules.Add(schedule);

            // Тестовый пользователь
            context.Users.Add(new User
            {
                Id = 1,
                Username = "testuser",
                Password = "hashedpassword",
                Role = UserRole.Student,
                StudentId = 1
            });

            context.SaveChanges();
        }
    }
}