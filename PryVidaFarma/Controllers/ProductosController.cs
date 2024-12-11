using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PryVidaFarma.DAO;
using PryVidaFarma.Data;
using PryVidaFarma.Models;
using System.Drawing;

namespace PryVidaFarma.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ProductosDAO productsDAO;
        private readonly CategoriasDAO categoriasDAO;
        private readonly AplicationContext context;
        private readonly IWebHostEnvironment host;
        public ProductosController(ProductosDAO productsDAO, CategoriasDAO categoriasDAO,
                                    IWebHostEnvironment host, AplicationContext context)
        {
            this.productsDAO = productsDAO;
            this.categoriasDAO = categoriasDAO;
            this.host = host;
            this.context = context;
        }
        // GET: ProductosController
        public ActionResult ListadoProductos()
        {
            var listado = productsDAO.ListadoProductos();
            //
            return View(listado);
        }

        private Productos BuscarProducto(int id)
        {
            var buscado = productsDAO.ListadoProductos().Find(p => p.id_producto.Equals(id));

            if (buscado != null && buscado.categoria == null)
            {
                buscado.categoria = new Categorias(); 
            }

            return buscado!;
        }

        // GET: ProductosController/Details/5
        public ActionResult DetailsProducto(int id)
        {
            return View(BuscarProducto(id));
        }

        // GET: ProductosController/Create
        public ActionResult CreateProductos()
        {
            ViewBag.categorias = new SelectList(
                categoriasDAO.ListadoCategorias(), "id_categoria", "nombre_categoria");
            return View();
        }

        // POST: ProductosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProductos(Productos obj, IFormFile imagen)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.categorias = new SelectList(categoriasDAO.ListadoCategorias(), "id_categoria", "nombre_categoria");
                    return View(obj);
                }
                if (imagen != null && imagen.Length > 0)
                {
                    string rutaImagen = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "productos", imagen.FileName);

                    using (var stream = new FileStream(rutaImagen, FileMode.Create))
                    {
                        imagen.CopyTo(stream);
                    }

                    obj.imagen = "/img/productos/" + imagen.FileName;


                    TempData["mensaje"] = productsDAO.RegistrarProductos(obj);
                    return RedirectToAction(nameof(CreateProductos));
                }
                else
                {
                    ViewBag.mensaje = "Debe proporcionar una imagen válida.";
                    ViewBag.categorias = new SelectList(categoriasDAO.ListadoCategorias(), "id_categoria", "nombre_categoria");
                    return View(obj);
                }
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = "Ocurrió un error: " + ex.Message;
                ViewBag.categorias = new SelectList(categoriasDAO.ListadoCategorias(), "id_categoria", "nombre_categoria");
                return View(obj);
            }
        }


        // GET: ProductosController/Edit/5
        public ActionResult EditProducto(int id)
        {
            var producto = BuscarProducto(id);
            if (producto == null)
            {
                TempData["mensaje"] = "El producto no existe.";
                return RedirectToAction(nameof(ListadoProductos));
            }

            int categoriaId = producto.categoria != null ? producto.categoria.id_categoria : 0;

            ViewBag.categorias = new SelectList(
                categoriasDAO.ListadoCategorias(),
                "id_categoria",       
                "nombre_categoria",   
                categoriaId);
            return View(producto);
        }


        // POST: ProductosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProducto(Productos obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TempData["mensaje"] = productsDAO.RegistrarProductos(obj, 2);
                    return RedirectToAction(nameof(ListadoProductos));
                }
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ex.Message;
            }

            ViewBag.categorias = new SelectList(
                categoriasDAO.ListadoCategorias(), "id_categoria", "nombre_categoria", obj.categoria.id_categoria);

            return View(obj);
        }

        // GET: ProductosController/Delete/5
        public ActionResult DeleteProducto(int id_producto)
        {
            try
            {
                string resultado = productsDAO.EliminarProducto(id_producto);  
                TempData["mensaje"] = resultado;
                return RedirectToAction(nameof(ListadoProductos));
            }
            catch (Exception ex)
            {
                TempData["mensaje"] = $"Error al eliminar el producto: {ex.Message}";
                return RedirectToAction(nameof(ListadoProductos));
            }
        }

    }
}
