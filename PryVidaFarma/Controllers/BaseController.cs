using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using PryVidaFarma.DAO;

namespace PryVidaFarma.Controllers
{
    public class BaseController : Controller
    {
        private readonly CategoriasDAO categoriasDAO;

        public BaseController(CategoriasDAO categoriasDAO)
        {
            this.categoriasDAO = categoriasDAO;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var categorias = new SelectList(
                categoriasDAO.ListadoCategorias(),
                "id_categoria",
                "nombre_categoria"
            );
            ViewBag.Categorias = categorias;

            base.OnActionExecuting(context);
        }
    }
}
