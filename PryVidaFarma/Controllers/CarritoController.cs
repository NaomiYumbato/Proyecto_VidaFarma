using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PryVidaFarma.DAO;
using PryVidaFarma.Models;

namespace PryVidaFarma.Controllers
{
    public class CarritoController : Controller
    {
        private readonly CarritoDao carritoDao;

        public CarritoController(CarritoDao _carritoDao)
        {
            carritoDao = _carritoDao;
        }

        // Serializar el carrito temporalmente en sesión
        void GuardarCarritoEnSesion(List<CarritoCompra> carrito)
        {
            HttpContext.Session.SetString("CarritoTemporal", JsonConvert.SerializeObject(carrito));
        }

        // Deserializar el carrito temporal desde sesión
         List<CarritoCompra> ObtenerCarritoDesdeSesion()
        {
            var carritoJson = HttpContext.Session.GetString("CarritoTemporal");
            //devolver una lista vacia en lugar de null
            return string.IsNullOrEmpty(carritoJson)
             ? new List<CarritoCompra>()
             : JsonConvert.DeserializeObject<List<CarritoCompra>>(carritoJson) ?? new List<CarritoCompra>();
        }

        // Mostrar el carrito del cliente logueado o no logueado
        public ActionResult VerCarrito(int? idCliente)
        {
            List<CarritoCompra> listacarrito;
            // Cliente logueado
            if (idCliente.HasValue) 
            {
                listacarrito = carritoDao.GetCarritoPorCliente(idCliente.Value);
            }
            // Cliente no logueado
            else
            {
                listacarrito = ObtenerCarritoDesdeSesion();
            }

            ViewBag.total = listacarrito.Sum(c => c.ImporteTotal);
            return View(listacarrito);
        }

        // Agregar artículo al carrito
        [HttpPost]
        public ActionResult AgregarAlCarrito(int? idCliente, int idProducto, int cantidad, decimal precio)
        {
            var carritoCompra = new CarritoCompra
            {
                IdCliente = idCliente ?? 0,
                IdProducto = idProducto,
                Cantidad = cantidad,
                Precio = precio
            };
            carritoCompra.CalcularImporte();

            // Cliente logueado
            if (idCliente.HasValue) 
            {
                carritoDao.AgregarCarrito(carritoCompra);
            }
            // Cliente no logueado
            else
            {
                var carrito = ObtenerCarritoDesdeSesion();
                var existente = carrito.FirstOrDefault(c => c.IdProducto == idProducto);

                if (existente == null)
                {
                    carrito.Add(carritoCompra);
                }
                else
                {
                    existente.Cantidad += cantidad;
                    existente.CalcularImporte();
                }

                GuardarCarritoEnSesion(carrito);
            }

            TempData["mensaje"] = "Producto agregado al carrito correctamente.";
            return RedirectToAction("VerCarrito", new { idCliente });
        }

        // Eliminar artículo del carrito
        public ActionResult EliminarArticulo(int idCarritoCompra, int? idCliente, int idProducto)
        // Cliente logueado
        {
            if (idCliente.HasValue) 
            {
                carritoDao.EliminarArticulo(idCarritoCompra);
            }
            // Cliente no logueado
            else
            {
                var carrito = ObtenerCarritoDesdeSesion();
                carrito.RemoveAll(c => c.IdProducto == idProducto);
                GuardarCarritoEnSesion(carrito);
            }

            TempData["mensaje"] = "Producto eliminado del carrito.";
            return RedirectToAction("VerCarrito", new { idCliente });
        }
    }
}
