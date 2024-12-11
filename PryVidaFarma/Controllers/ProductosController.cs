using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        public ActionResult ListadoProductos(int? categoriaId)
        {
            var listado = productsDAO.ListadoProductos();
            //
            return View(listado);
        }

        private Productos BuscarProducto(int id)
        {
            var buscado = productsDAO.ListadoProductos().Find(p => p.id_producto.Equals(id));

            return buscado!;
        }

        // GET: ProductosController/Details/5
        public ActionResult DetailsProducto(int id)
        {
            var producto = BuscarProducto(id);
            if (producto == null)
            {
                TempData["mensaje"] = "El producto no existe.";
                return RedirectToAction(nameof(ListadoProductos));
            }
            return View(producto);
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
        public ActionResult EditProducto(Productos obj, IFormFile imagen)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.categorias = new SelectList(categoriasDAO.ListadoCategorias(), "id_categoria", "nombre_categoria");
                    return View(obj);
                }

                // Si se seleccionó una nueva imagen
                if (imagen != null && imagen.Length > 0)
                {
                    string rutaImagen = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "productos", imagen.FileName);

                    using (var stream = new FileStream(rutaImagen, FileMode.Create))
                    {
                        imagen.CopyTo(stream);
                    }

                    // Actualiza la propiedad 'imagen' con la nueva ruta
                    obj.imagen = "/img/productos/" + imagen.FileName;
                }
                else
                {
                    // Si no se seleccionó una nueva imagen, mantenemos la imagen actual
                    obj.imagen = obj.imagen ?? ""; // Asegúrate de que no sea null
                }

                // Registrar el producto, sin importar si la imagen ha cambiado o no
                TempData["mensaje"] = productsDAO.RegistrarProductos(obj, 2);
                return RedirectToAction(nameof(ListadoProductos));
            }
            catch (Exception ex)
            {
                TempData["mensaje"] = "Ocurrió un error: " + ex.Message;
                ViewBag.categorias = new SelectList(categoriasDAO.ListadoCategorias(), "id_categoria", "nombre_categoria");
                return View(obj);
            }
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

        public ActionResult ListadoProductosPorCategoria(int? categoriaId)
        {
            ViewBag.Categorias = new SelectList(
                categoriasDAO.ListadoCategorias(),
                "id_categoria",
                "nombre_categoria",
                categoriaId ?? 0
            );
            var listado = productsDAO.ListadoProductos();
            //
            return View(listado);
        }

        [HttpPost]
        public IActionResult ListadoProductosPorCategoria(int id_categoria)
        {
            var productos = productsDAO.ListadoProductosPorCategoria(id_categoria);
            if (productos != null && productos.Any())
            {
                ViewBag.Categorias = new SelectList(categoriasDAO.ListadoCategorias(), "id_categoria", "nombre_categoria", id_categoria);
                return View("ListadoProductosPorCategoria", productos);
            }
            else 
            {
                TempData["mensaje"] = "No hay productos en la categoria seleccionada";
                return RedirectToAction(nameof(ListadoProductosPorCategoria));
            }
        }

        public ActionResult ListadoProductosPorPalabra()
        {
            var listado = productsDAO.ListadoProductos();
            //
            return View(listado);
        }

        [HttpPost]
        public IActionResult ListadoProductosPorPalabra(string palabra_clave)
        {
            var productos = productsDAO.ListadoProductosPorPalabra(palabra_clave);
            if (productos != null && productos.Any())
            {
                return View("ListadoProductosPorPalabra", productos);
            }
            else
            {
                TempData["mensaje"] = "Producto no encontrado";
                return RedirectToAction(nameof(ListadoProductosPorPalabra));
            }
        }

    }
}
