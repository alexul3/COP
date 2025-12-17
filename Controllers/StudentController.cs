using Controllers;
using Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Storage;

namespace COP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Получить расписание студента
        [HttpGet("{studentId}/schedule")]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetStudentSchedule(int studentId)
        {
            // Находим студента и его группу
            var student = await _context.Students
                .Include(s => s.Group)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
            {
                return NotFound("Студент не найден");
            }

            // Получаем расписание для группы студента
            var schedules = await _context.Schedules
                .Include(s => s.Group)
                .Include(s => s.Subject)
                .Include(s => s.Teacher)
                .Where(s => s.GroupId == student.GroupId)
                .OrderBy(s => s.Date)
                .ThenBy(s => s.PairNumber)
                .ToListAsync();

            var scheduleDtos = schedules.Select(s => new ScheduleDto
            {
                Id = s.Id,
                Date = s.Date,
                PairNumber = s.PairNumber,
                Group = new GroupDto { Id = s.Group.Id, Name = s.Group.Name },
                Subject = new SubjectDto { Id = s.Subject.Id, Name = s.Subject.Name },
                Teacher = new TeacherDto { Id = s.Teacher.Id, Name = s.Teacher.Name },
                Classroom = s.Classroom
            });

            return Ok(scheduleDtos);
        }

        // Получить оценки студента
        [HttpGet("{studentId}/grades")]
        public async Task<ActionResult<IEnumerable<ExamDto>>> GetStudentGrades(int studentId)
        {
            // Проверяем существование студента
            var studentExists = await _context.Students.AnyAsync(s => s.Id == studentId);
            if (!studentExists)
            {
                return NotFound("Студент не найден");
            }

            // Получаем все оценки студента
            var exams = await _context.Exams
                .Include(e => e.Subject)
                .Include(e => e.Student)
                .Include(e => e.Teacher)
                .Where(e => e.StudentId == studentId)
                .OrderByDescending(e => e.Id) // Сначала новые
                .ToListAsync();

            var examDtos = exams.Select(e => new ExamDto
            {
                Id = e.Id,
                ExamScore = e.ExamScore,
                Subject = new SubjectDto { Id = e.Subject.Id, Name = e.Subject.Name },
                Student = new StudentDto { Id = e.Student.Id, Name = e.Student.Name },
                Teacher = new TeacherDto { Id = e.Teacher.Id, Name = e.Teacher.Name }
            });

            return Ok(examDtos);
        }

        // Получить информацию о студенте
        [HttpGet("{studentId}")]
        public async Task<ActionResult<StudentDto>> GetStudent(int studentId)
        {
            var student = await _context.Students
                .Include(s => s.Group)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
            {
                return NotFound("Студент не найден");
            }

            var studentDto = new StudentDto
            {
                Id = student.Id,
                Name = student.Name,
                Group = new GroupDto { Id = student.Group.Id, Name = student.Group.Name }
            };

            return Ok(studentDto);
        }
    }
}