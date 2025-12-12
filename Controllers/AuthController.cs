using Controllers;
using Models;
using Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserInfo>> Login([FromBody] LoginRequest loginRequest)
        {
            // Находим пользователя по имени
            var user = await _context.Users
                .Include(u => u.Student)
                .Include(u => u.Teacher)
                .Include(u => u.DecanatWorker)
                .FirstOrDefaultAsync(u => u.Username == loginRequest.Username);



            // Возвращаем информацию о пользователе
            return new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role.ToString(),
                StudentId = user.StudentId,
                TeacherId = user.TeacherId,
                DecanatWorkerId = user.DecanatWorkerId
            };
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserInfo>> Register([FromBody] LoginRequest registerRequest)
        {
            // Проверяем, существует ли пользователь с таким именем
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == registerRequest.Username);

            if (existingUser != null)
            {
                return BadRequest("Пользователь с таким именем уже существует");
            }

            // Создаем нового пользователя (без привязки к конкретной роли)
            var user = new User
            {
                Username = registerRequest.Username,
                Password = registerRequest.Password,
                Role = UserRole.Student // По умолчанию регистрируем как студента
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role.ToString()
            };
        }
    }
}