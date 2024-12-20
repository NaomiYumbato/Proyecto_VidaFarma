using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.MSIdentity.Shared;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PryVidaFarma.DAO;
using PryVidaFarma.Models;

namespace PryVidaFarma.Controllers
{
    public class UsuarioController : BaseController
    {
        private readonly HttpClient _httpClient;

        public UsuarioController(CategoriasDAO categoriasDAO) : base(categoriasDAO)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5251/api/Usuario/") };
        }

        // Página de registro
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Acción POST de registro
        [HttpPost]
        public async Task<IActionResult> Register(string nombres, string apellidos, string dni, DateTime fechaNacimiento, string direccion, string correoElectronico, string contrasenia)
        {
            var payload = new
            {
                Nombres = nombres,
                Apellidos = apellidos,
                Dni = dni,
                FechaNacimiento = fechaNacimiento.ToString("yyyy-MM-dd"),
                Direccion = direccion,
                CorreoElectronico = correoElectronico,
                Contrasenia = contrasenia
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("register", content);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.Mensaje = "Registro exitoso.";
                return RedirectToAction("Login");
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            ViewBag.Error = $"Error al registrar usuario: {errorResponse}";
            return View();
        }

        // Página de login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Acción POST de login
        [HttpPost]
        public async Task<IActionResult> Login(string correoElectronico, string contrasenia)
        {
            var payload = new
            {
                CorreoElectronico = correoElectronico,
                Contrasenia = contrasenia
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("login", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<LoginResponse>(jsonResponse);

                if (user == null)
                {
                    ViewBag.Error = "No se pudo deserializar la respuesta del servidor.";
                    return View();
                }

                if (!string.IsNullOrEmpty(user.IdCliente.ToString()) && !string.IsNullOrEmpty(user.Nombres))
                {
                    HttpContext.Session.SetString("UsuarioId", user.IdCliente.ToString());
                    HttpContext.Session.SetString("UsuarioNombre", $"{user.Nombres} {user.Apellidos}");


                    var refererUrl = Request.Headers["Referer"].ToString();
                    if (!string.IsNullOrEmpty(refererUrl))
                    {
                        return Redirect(refererUrl);  
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home"); 
                    }
                }
                else
                {
                    ViewBag.Error = "Los valores obtenidos del JSON son nulos.";
                    return View();
                }
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            ViewBag.Error = $"Correo o contraseña incorrectos: {errorResponse}";
            return View();
        }


        // Acción para cerrar sesión
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UsuarioId");
            HttpContext.Session.Remove("UsuarioNombre");

            var refererUrl = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(refererUrl))
            {
                return Redirect(refererUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult PruebaSesion()
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            var usuarioNombre = HttpContext.Session.GetString("UsuarioNombre");

            if (string.IsNullOrEmpty(usuarioId) || string.IsNullOrEmpty(usuarioNombre))
            {
                return Content("Los valores de la sesión están vacíos.");
            }

            return Content($"UsuarioId: {usuarioId}, UsuarioNombre: {usuarioNombre}");
        }

    }

    // Clase para deserializar la respuesta de login
    public class LoginResponse
    {
        [JsonProperty("idCliente")]
        public int IdCliente { get; set; }

        [JsonProperty("nombres")]
        public string Nombres { get; set; }

        [JsonProperty("apellidos")]
        public string Apellidos { get; set; }

        [JsonProperty("correoElectronico")]
        public string CorreoElectronico { get; set; }
    }

}
