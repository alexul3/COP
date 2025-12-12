using Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Storage;

namespace COP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DecanatController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DecanatController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Получить все расписания
        [HttpGet("schedules")]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetAllSchedules()
        {
            var schedules = await _context.Schedules
                .Include(s => s.Group)
                .Include(s => s.Subject)
                .Include(s => s.Teacher)
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

        // Создать новое расписание
        [HttpPost("schedule")]
        public async Task<ActionResult<ScheduleDto>> CreateSchedule([FromBody] CreateScheduleDto createScheduleDto)
        {
            // Проверяем существование группы
            var group = await _context.Groups.FindAsync(createScheduleDto.GroupId);
            if (group == null)
            {
                return NotFound("Группа не найдена");
            }

            // Проверяем существование преподавателя
            var teacher = await _context.Teachers.FindAsync(createScheduleDto.TeacherId);
            if (teacher == null)
            {
                return NotFound("Преподаватель не найден");
            }

            // Проверяем существование предмета
            var subject = await _context.Subjects.FindAsync(createScheduleDto.SubjectId);
            if (subject == null)
            {
                return NotFound("Предмет не найден");
            }

            // Проверяем номер пары (1-8)
            if (createScheduleDto.PairNumber < 1 || createScheduleDto.PairNumber > 8)
            {
                return BadRequest("Номер пары должен быть от 1 до 8");
            }

            // Проверяем, нет ли уже занятия в это время для группы
            var existingSchedule = await _context.Schedules
                .FirstOrDefaultAsync(s => s.GroupId == createScheduleDto.GroupId &&
                                         s.Date == createScheduleDto.Date &&
                                         s.PairNumber == createScheduleDto.PairNumber);

            if (existingSchedule != null)
            {
                return BadRequest("В это время у группы уже есть занятие");
            }

            // Создаем новое расписание
            var schedule = new Schedule
            {
                GroupId = createScheduleDto.GroupId,
                TeacherId = createScheduleDto.TeacherId,
                SubjectId = createScheduleDto.SubjectId,
                Date = createScheduleDto.Date,
                PairNumber = createScheduleDto.PairNumber,
                Classroom = createScheduleDto.Classroom
            };

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            // Загружаем связанные данные для ответа
            await _context.Entry(schedule)
                .Reference(s => s.Group).LoadAsync();
            await _context.Entry(schedule)
                .Reference(s => s.Subject).LoadAsync();
            await _context.Entry(schedule)
                .Reference(s => s.Teacher).LoadAsync();

            var scheduleDto = new ScheduleDto
            {
                Id = schedule.Id,
                Date = schedule.Date,
                PairNumber = schedule.PairNumber,
                Group = new GroupDto { Id = schedule.Group.Id, Name = schedule.Group.Name },
                Subject = new SubjectDto { Id = schedule.Subject.Id, Name = schedule.Subject.Name },
                Teacher = new TeacherDto { Id = schedule.Teacher.Id, Name = schedule.Teacher.Name }
            };

            return CreatedAtAction(nameof(GetScheduleById), new { id = schedule.Id }, scheduleDto);
        }

        // Обновить расписание
        [HttpPut("schedule/{id}")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] CreateScheduleDto updateScheduleDto)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound("Расписание не найдено");
            }

            // Проверяем существование группы
            var group = await _context.Groups.FindAsync(updateScheduleDto.GroupId);
            if (group == null)
            {
                return NotFound("Группа не найдена");
            }

            // Проверяем существование преподавателя
            var teacher = await _context.Teachers.FindAsync(updateScheduleDto.TeacherId);
            if (teacher == null)
            {
                return NotFound("Преподаватель не найден");
            }

            // Проверяем существование предмета
            var subject = await _context.Subjects.FindAsync(updateScheduleDto.SubjectId);
            if (subject == null)
            {
                return NotFound("Предмет не найден");
            }

            // Проверяем номер пары (1-8)
            if (updateScheduleDto.PairNumber < 1 || updateScheduleDto.PairNumber > 8)
            {
                return BadRequest("Номер пары должен быть от 1 до 8");
            }

            // Проверяем, нет ли уже занятия в это время для группы (кроме текущего)
            var existingSchedule = await _context.Schedules
                .FirstOrDefaultAsync(s => s.GroupId == updateScheduleDto.GroupId &&
                                         s.Date == updateScheduleDto.Date &&
                                         s.PairNumber == updateScheduleDto.PairNumber &&
                                         s.Id != id);

            if (existingSchedule != null)
            {
                return BadRequest("В это время у группы уже есть занятие");
            }

            // Обновляем расписание
            schedule.GroupId = updateScheduleDto.GroupId;
            schedule.TeacherId = updateScheduleDto.TeacherId;
            schedule.SubjectId = updateScheduleDto.SubjectId;
            schedule.Date = updateScheduleDto.Date;
            schedule.PairNumber = updateScheduleDto.PairNumber;
            schedule.Classroom = updateScheduleDto.Classroom;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Удалить расписание
        [HttpDelete("schedule/{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound("Расписание не найдено");
            }

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Получить расписание по ID
        [HttpGet("schedule/{id}")]
        public async Task<ActionResult<ScheduleDto>> GetScheduleById(int id)
        {
            var schedule = await _context.Schedules
                .Include(s => s.Group)
                .Include(s => s.Subject)
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (schedule == null)
            {
                return NotFound("Расписание не найдено");
            }

            var scheduleDto = new ScheduleDto
            {
                Id = schedule.Id,
                Date = schedule.Date,
                PairNumber = schedule.PairNumber,
                Group = new GroupDto { Id = schedule.Group.Id, Name = schedule.Group.Name },
                Subject = new SubjectDto { Id = schedule.Subject.Id, Name = schedule.Subject.Name },
                Teacher = new TeacherDto { Id = schedule.Teacher.Id, Name = schedule.Teacher.Name }
            };

            return Ok(scheduleDto);
        }

        // Получить список групп
        [HttpGet("groups")]
        public async Task<ActionResult<IEnumerable<GroupDto>>> GetGroups()
        {
            var groups = await _context.Groups.ToListAsync();

            var groupDtos = groups.Select(g => new GroupDto
            {
                Id = g.Id,
                Name = g.Name
            });

            return Ok(groupDtos);
        }

        // Получить список преподавателей
        [HttpGet("teachers")]
        public async Task<ActionResult<IEnumerable<TeacherDto>>> GetTeachers()
        {
            var teachers = await _context.Teachers.ToListAsync();

            var teacherDtos = teachers.Select(t => new TeacherDto
            {
                Id = t.Id,
                Name = t.Name
            });

            return Ok(teacherDtos);
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