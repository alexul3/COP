using Controllers;
using System.Net.Http.Json;
using Tests;
using Xunit;

namespace COP.Tests.Controllers
{
    public class TeacherControllerTests : IClassFixture<TestApplication>
    {
        private readonly HttpClient _client;

        public TeacherControllerTests(TestApplication factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetTeacherSchedule_WithValidTeacherId_ReturnsSchedule()
        {
            // Arrange
            int teacherId = 1;

            // Act
            var response = await _client.GetAsync($"/api/teacher/{teacherId}/schedule");

            // Assert
            response.EnsureSuccessStatusCode();

            var schedule = await response.Content.ReadFromJsonAsync<List<ScheduleDto>>();
            Assert.NotNull(schedule);
            Assert.NotEmpty(schedule);
        }

        [Fact]
        public async Task AddGrade_WithValidData_ReturnsExam()
        {
            // Arrange
            var examDto = new CreateExamDto
            {
                StudentId = 1,
                TeacherId = 1,
                SubjectId = 1,
                ExamScore = 85
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/teacher/grade", examDto);

            // Assert
            response.EnsureSuccessStatusCode();

            var exam = await response.Content.ReadFromJsonAsync<ExamDto>();
            Assert.NotNull(exam);
            Assert.Equal(85, exam.ExamScore);
            Assert.Equal(201, (int)response.StatusCode); // Created
        }

        [Fact]
        public async Task AddGrade_WithInvalidStudentId_ReturnsNotFound()
        {
            // Arrange
            var examDto = new CreateExamDto
            {
                StudentId = 999, // Не существует
                TeacherId = 1,
                SubjectId = 1,
                ExamScore = 85
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/teacher/grade", examDto);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task AddGrade_WithInvalidScore_ReturnsBadRequest()
        {
            // Arrange
            var examDto = new CreateExamDto
            {
                StudentId = 1,
                TeacherId = 1,
                SubjectId = 1,
                ExamScore = 150 // Неверная оценка (> 100)
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/teacher/grade", examDto);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}