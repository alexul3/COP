using Controllers;
using Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Storage;

namespace COP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TeacherController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Получить расписание преподавателя
        [HttpGet("{teacherId}/schedule")]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetTeacherSchedule(int teacherId)
        {
            // Проверяем существование преподавателя
            var teacherExists = await _context.Teachers.AnyAsync(t => t.Id == teacherId);
            if (!teacherExists)
            {
                return NotFound("Преподаватель не найден");
            }

            // Получаем расписание преподавателя
            var schedules = await _context.Schedules
                .Include(s => s.Group)
                .Include(s => s.Subject)
                .Include(s => s.Teacher)
                .Where(s => s.TeacherId == teacherId)
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
                Teacher = new TeacherDto { Id = s.Teacher.Id, Name = s.Teacher.Name }
            });

            return Ok(scheduleDtos);
        }

        // Выставить оценку студенту
        [HttpPost("grade")]
        public async Task<ActionResult<ExamDto>> AddGrade([FromBody] CreateExamDto createExamDto)
        {
            // Проверяем существование студента
            var student = await _context.Students.FindAsync(createExamDto.StudentId);
            if (student == null)
            {
                return NotFound("Студент не найден");
            }

            // Проверяем существование преподавателя
            var teacher = await _context.Teachers.FindAsync(createExamDto.TeacherId);
            if (teacher == null)
            {
                return NotFound("Преподаватель не найден");
            }

            // Проверяем существование предмета
            var subject = await _context.Subjects.FindAsync(createExamDto.SubjectId);
            if (subject == null)
            {
                return NotFound("Предмет не найден");
            }

            // Проверяем оценку (0-100)
            if (createExamDto.ExamScore < 0 || createExamDto.ExamScore > 100)
            {
                return BadRequest("Оценка должна быть в диапазоне от 0 до 100");
            }

            // Создаем новую запись об оценке
            var exam = new Exam
            {
                StudentId = createExamDto.StudentId,
                TeacherId = createExamDto.TeacherId,
                SubjectId = createExamDto.SubjectId,
                ExamScore = createExamDto.ExamScore
            };

            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();

            // Загружаем связанные данные для ответа
            await _context.Entry(exam)
                .Reference(e => e.Student).LoadAsync();
            await _context.Entry(exam)
                .Reference(e => e.Teacher).LoadAsync();
            await _context.Entry(exam)
                .Reference(e => e.Subject).LoadAsync();

            var examDto = new ExamDto
            {
                Id = exam.Id,
                ExamScore = exam.ExamScore,
                Subject = new SubjectDto { Id = exam.Subject.Id, Name = exam.Subject.Name },
                Student = new StudentDto { Id = exam.Student.Id, Name = exam.Student.Name },
                Teacher = new TeacherDto { Id = exam.Teacher.Id, Name = exam.Teacher.Name }
            };

            return CreatedAtAction(nameof(AddGrade), new { id = exam.Id }, examDto);
        }

        // Получить студентов для выставления оценки
        [HttpGet("students")]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents()
        {
            var students = await _context.Students
                .Include(s => s.Group)
                .ToListAsync();

            var studentDtos = students.Select(s => new StudentDto
            {
                Id = s.Id,
                Name = s.Name,
                Group = new GroupDto { Id = s.Group.Id, Name = s.Group.Name }
            });

            return Ok(studentDtos);
        }

        // Получить список предметов
        [HttpGet("subjects")]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetSubjects()
        {
            var subjects = await _context.Subjects.ToListAsync();

            var subjectDtos = subjects.Select(s => new SubjectDto
            {
                Id = s.Id,
                Name = s.Name,
            });

            return Ok(subjectDtos);
        }
    }
}