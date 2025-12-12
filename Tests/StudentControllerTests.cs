using Controllers;
using System.Net.Http.Json;
using Tests;
using Xunit;

namespace COP.Tests.Controllers
{
    public class StudentControllerTests : IClassFixture<TestApplication>
    {
        private readonly HttpClient _client;

        public StudentControllerTests(TestApplication factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetStudentSchedule_WithValidStudentId_ReturnsSchedule()
        {
            // Arrange
            int studentId = 1;

            // Act
            var response = await _client.GetAsync($"/api/student/{studentId}/schedule");

            // Assert
            response.EnsureSuccessStatusCode();

            var schedule = await response.Content.ReadFromJsonAsync<List<ScheduleDto>>();
            Assert.NotNull(schedule);
            Assert.NotEmpty(schedule);
            Assert.Equal("Программирование", schedule[0].Subject.Name);
        }

        [Fact]
        public async Task GetStudentSchedule_WithInvalidStudentId_ReturnsNotFound()
        {
            // Arrange
            int invalidStudentId = 999;

            // Act
            var response = await _client.GetAsync($"/api/student/{invalidStudentId}/schedule");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetStudent_WithValidId_ReturnsStudent()
        {
            // Arrange
            int studentId = 1;

            // Act
            var response = await _client.GetAsync($"/api/student/{studentId}");

            // Assert
            response.EnsureSuccessStatusCode();

            var student = await response.Content.ReadFromJsonAsync<StudentDto>();
            Assert.NotNull(student);
            Assert.Equal("Иван Иванов", student.Name);
            Assert.Equal("ИВТ-101", student.Group.Name);
        }
    }
}