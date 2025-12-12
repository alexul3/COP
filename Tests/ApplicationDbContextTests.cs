using Microsoft.EntityFrameworkCore;
using Models;
using Storage;
using Xunit;

namespace COP.Tests.Data
{
    public class ApplicationDbContextTests
    {
        [Fact]
        public async Task CanAddAndRetrieveStudent()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_CanAddStudent")
                .Options;

            // Act & Assert
            using (var context = new ApplicationDbContext(options))
            {
                var group = new Group { Name = "ИВТ-101" };
                context.Groups.Add(group);
                await context.SaveChangesAsync();

                var student = new Student
                {
                    Name = "Тестовый студент",
                    GroupId = group.Id
                };
                context.Students.Add(student);
                await context.SaveChangesAsync();
            }

            // Проверяем, что данные сохранились
            using (var context = new ApplicationDbContext(options))
            {
                var student = await context.Students
                    .Include(s => s.Group)
                    .FirstOrDefaultAsync(s => s.Name == "Тестовый студент");

                Assert.NotNull(student);
                Assert.Equal("ИВТ-101", student.Group.Name);
            }
        }

        [Fact]
        public async Task StudentDeletion_CascadesCorrectly()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_CascadeDelete")
                .Options;

            int studentId;
            using (var context = new ApplicationDbContext(options))
            {
                var student = new Student
                {
                    Name = "Студент для удаления",
                    GroupId = 1
                };
                context.Students.Add(student);
                await context.SaveChangesAsync();
                studentId = student.Id;

                // Добавляем оценку для студента
                var exam = new Exam
                {
                    StudentId = student.Id,
                    TeacherId = 1,
                    SubjectId = 1,
                    ExamScore = 90
                };
                context.Exams.Add(exam);
                await context.SaveChangesAsync();
            }

            // Act - удаляем студента
            using (var context = new ApplicationDbContext(options))
            {
                var student = await context.Students.FindAsync(studentId);
                context.Students.Remove(student);
                await context.SaveChangesAsync();
            }

            // Assert - проверяем, что оценки тоже удалились
            using (var context = new ApplicationDbContext(options))
            {
                var remainingExams = await context.Exams
                    .Where(e => e.StudentId == studentId)
                    .ToListAsync();

                Assert.Empty(remainingExams);
            }
        }

        [Fact]
        public async Task UniqueUsername_Enforced()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_UniqueUsername")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                var user1 = new User
                {
                    Username = "uniqueuser",
                    Password = "hash1",
                    Role = UserRole.Student
                };
                context.Users.Add(user1);
                await context.SaveChangesAsync();

                // Act & Assert - попытка добавить пользователя с тем же именем
                var user2 = new User
                {
                    Username = "uniqueuser", // То же имя
                    Password = "hash2",
                    Role = UserRole.Teacher
                };
                context.Users.Add(user2);

                // Должно вызвать исключение
                await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
            }
        }
    }
}