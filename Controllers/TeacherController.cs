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

        [HttpGet("{teacherId}/grades")]
        public async Task<ActionResult<IEnumerable<ExamDto>>> GetTeacherGrades(int teacherId)
        {
            var exams = await _context.Exams
                .Include(e => e.Student)
                    .ThenInclude(s => s.Group)
                .Include(e => e.Subject)
                .Include(e => e.Teacher)
                .Where(e => e.TeacherId == teacherId)
                .OrderByDescending(e => e.Id)
                .ToListAsync();

            var examDtos = exams.Select(e => new ExamDto
            {
                Id = e.Id,
                ExamScore = e.ExamScore,
                Subject = new SubjectDto { Id = e.Subject.Id, Name = e.Subject.Name },
                Student = new StudentDto
                {
                    Id = e.Student.Id,
                    Name = e.Student.Name,
                    Group = e.Student.Group != null ? new GroupDto
                    {
                        Id = e.Student.Group.Id,
                        Name = e.Student.Group.Name
                    } : null
                },
                Teacher = new TeacherDto { Id = e.Teacher.Id, Name = e.Teacher.Name }
            });

            return Ok(examDtos);
        }

        [HttpGet("exams")]
        public async Task<ActionResult<IEnumerable<ExamDto>>> GetAllExams()
        {
            var exams = await _context.Exams
                .Include(e => e.Student)
                    .ThenInclude(s => s.Group) // ДОБАВЬТЕ ЭТО!
                .Include(e => e.Subject)
                .Include(e => e.Teacher)
                .OrderByDescending(e => e.Id)
                .ToListAsync();

            var examDtos = exams.Select(e => new ExamDto
            {
                Id = e.Id,
                ExamScore = e.ExamScore,
                Subject = new SubjectDto { Id = e.Subject.Id, Name = e.Subject.Name },
                Student = new StudentDto
                {
                    Id = e.Student.Id,
                    Name = e.Student.Name,
                    Group = e.Student.Group != null ? new GroupDto
                    {
                        Id = e.Student.Group.Id,
                        Name = e.Student.Group.Name
                    } : null
                },
                Teacher = new TeacherDto { Id = e.Teacher.Id, Name = e.Teacher.Name }
            });

            return Ok(examDtos);
        }

        // PUT: /api/teacher/grade/{id}
        [HttpPut("grade/{id}")]
        public async Task<IActionResult> UpdateGrade(int id, [FromBody] CreateExamDto updateDto)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam == null)
                return NotFound("Оценка не найдена");

            exam.ExamScore = updateDto.ExamScore;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: /api/teacher/grade/{id}
        [HttpDelete("grade/{id}")]
        public async Task<IActionResult> DeleteGrade(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam == null)
                return NotFound("Оценка не найдена");

            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();

            return NoContent();
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
                Teacher = new TeacherDto { Id = s.Teacher.Id, Name = s.Teacher.Name },
                Classroom = s.Classroom
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
                .Include(s => s.Group) // Включаем группу
                .OrderBy(s => s.Name)
                .ToListAsync();

            var studentDtos = students.Select(s => new StudentDto
            {
                Id = s.Id,
                Name = s.Name,
                Group = s.Group != null ? new GroupDto
                {
                    Id = s.Group.Id,
                    Name = s.Group.Name
                } : null
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