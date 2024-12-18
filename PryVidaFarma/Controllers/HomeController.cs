using Microsoft.AspNetCore.Mvc;
using PryVidaFarma.DAO;
using PryVidaFarma.Models;
using System.Diagnostics;

namespace PryVidaFarma.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly CategoriasDAO categoriasDAO;

        public HomeController(ILogger<HomeController> logger, CategoriasDAO _categoriasDAO)
        {
            _logger = logger;
            categoriasDAO = _categoriasDAO;
        }

        public IActionResult lstCategorias()
        {
            var lstCategorias = categoriasDAO.ListadoCategorias();
            return View(lstCategorias);
        }

        public IActionResult Index()
        {
            var lstCategorias = categoriasDAO.ListadoCategorias();
            return View(lstCategorias);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
