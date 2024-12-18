using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PryVidaFarmaWebAPI.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PryVidaFarmaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly BdFarmaciaContext _context;

        public UsuarioController(BdFarmaciaContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (_context.TbPersonas.Any(p => p.Dni == request.Dni || p.CorreoElectronico == request.CorreoElectronico))
            {
                return BadRequest("El usuario ya existe con el mismo DNI o correo electrónico.");
            }

            var persona = new TbPersona
            {
                Nombres = request.Nombres,
                Apellidos = request.Apellidos,
                Dni = request.Dni,
                FechaNacimiento = request.FechaNacimiento,
                Direccion = request.Direccion,
                CorreoElectronico = request.CorreoElectronico
            };

            string hashedPassword = HashPassword(request.Contrasenia);

            var cliente = new TbCliente
            {
                FechaRegistro = DateOnly.FromDateTime(DateTime.Now),
                Contrasenia = hashedPassword,
                IdPersonaNavigation = persona
            };

            _context.TbPersonas.Add(persona);
            _context.TbClientes.Add(cliente);
            _context.SaveChanges();

            return Ok("Usuario registrado correctamente.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null)
            {
                return BadRequest("El objeto de solicitud no puede ser nulo.");
            }

            var usuario = _context.TbClientes
                .Include(c => c.IdPersonaNavigation)
                .FirstOrDefault(u => u.IdPersonaNavigation.CorreoElectronico == request.CorreoElectronico);

            if (usuario == null)
            {
                return NotFound("El usuario no existe o las credenciales son incorrectas.");
            }

            if (!VerifyPassword(request.Contrasenia, usuario.Contrasenia))
            {
                return Unauthorized("La contraseña es incorrecta.");
            }

            return Ok(new
            {
                    usuario.IdCliente,
                    usuario.IdPersonaNavigation.Nombres,
                    usuario.IdPersonaNavigation.Apellidos,
                    usuario.IdPersonaNavigation.CorreoElectronico
                
            });
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private bool VerifyPassword(string inputPassword, string storedHashedPassword)
        {
            string inputHashed = HashPassword(inputPassword);
            return inputHashed == storedHashedPassword;
        }
    }

    // Clases DTO para solicitud
    public class RegisterRequest
    {
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Dni { get; set; }
        public DateOnly FechaNacimiento { get; set; }
        public string Direccion { get; set; }
        public string CorreoElectronico { get; set; }
        public string Contrasenia { get; set; }
    }

    public class LoginRequest
    {
        public string CorreoElectronico { get; set; }
        public string Contrasenia { get; set; }
    }
}
