using Controllers;
using System.Net.Http.Json;
using Tests;
using Xunit;

namespace COP.Tests.Controllers
{
    public class DecanatControllerTests : IClassFixture<TestApplication>
    {
        private readonly HttpClient _client;

        public DecanatControllerTests(TestApplication factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllSchedules_ReturnsSchedules()
        {
            // Act
            var response = await _client.GetAsync("/api/decanat/schedules");

            // Assert
            response.EnsureSuccessStatusCode();

            var schedules = await response.Content.ReadFromJsonAsync<List<ScheduleDto>>();
            Assert.NotNull(schedules);
            Assert.NotEmpty(schedules);
        }

        [Fact]
        public async Task CreateSchedule_WithValidData_ReturnsCreatedSchedule()
        {
            // Arrange
            var scheduleDto = new CreateScheduleDto
            {
                GroupId = 1,
                TeacherId = 1,
                SubjectId = 1,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                PairNumber = 2,
                Classroom = "201"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/decanat/schedule", scheduleDto);

            // Assert
            response.EnsureSuccessStatusCode();

            var schedule = await response.Content.ReadFromJsonAsync<ScheduleDto>();
            Assert.NotNull(schedule);
            Assert.Equal(201, (int)response.StatusCode);
        }

        [Fact]
        public async Task CreateSchedule_WithConflict_ReturnsBadRequest()
        {
            // Arrange - создаем расписание с такими же датой, группой и парой
            var scheduleDto = new CreateScheduleDto
            {
                GroupId = 1,
                TeacherId = 1,
                SubjectId = 1,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)), // Та же дата
                PairNumber = 1, // Та же пара
                Classroom = "301"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/decanat/schedule", scheduleDto);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateSchedule_WithValidData_ReturnsNoContent()
        {
            // Arrange
            int scheduleId = 1;
            var updateDto = new CreateScheduleDto
            {
                GroupId = 1,
                TeacherId = 1,
                SubjectId = 1,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                PairNumber = 3,
                Classroom = "102"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/decanat/schedule/{scheduleId}", updateDto);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteSchedule_WithValidId_ReturnsNoContent()
        {
            // Arrange
            int scheduleId = 1;

            // Act
            var response = await _client.DeleteAsync($"/api/decanat/schedule/{scheduleId}");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task GetSubjects_ReturnsAllSubjects()
        {
            // Act
            var response = await _client.GetAsync("/api/decanat/subjects");

            // Assert
            response.EnsureSuccessStatusCode();

            var subjects = await response.Content.ReadFromJsonAsync<List<SubjectDto>>();
            Assert.NotNull(subjects);
            Assert.NotEmpty(subjects);
        }
    }
}