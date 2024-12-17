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

            // Obtener nombres de productos
            foreach (var item in listacarrito)
            {
                var producto = carritoDao.ObtenerProductoPorId(item.IdProducto);
                item.NombreProducto = producto?.nombre_producto ?? "Producto no encontrado";
            }

            ViewBag.Total = listacarrito.Sum(c => c.ImporteTotal);
            return View(listacarrito);
        }

        //17/12/2024
        [HttpGet]
        public ActionResult AgregarAlCarrito(int idProducto)
        {
            // Obtener un producto por su ID
            var producto = carritoDao.ObtenerProductoPorId(idProducto);

            if (producto == null)
            {
                TempData["mensaje"] = "Producto no encontrado.";
                return RedirectToAction("ListadoProductos", "Productos");
            }

            return View(producto);
        }
        //Fin

        // Agregar artículo al carrito
        [HttpPost]
        [HttpPost]
        public ActionResult AgregarAlCarrito(int? idCliente, int idProducto, int cantidad, decimal precio)
        {
            if (cantidad <= 0 || precio <= 0)
            {
                TempData["mensaje"] = "Cantidad o precio no válidos.";
                return RedirectToAction("ListadoProductos");
            }

            var carritoCompra = new CarritoCompra
            {
                IdCliente = idCliente ?? 0,
                IdProducto = idProducto,
                Cantidad = cantidad,
                Precio = precio
            };
            carritoCompra.CalcularImporte();

            if (idCliente.HasValue)
            {
                carritoDao.AgregarCarrito(carritoCompra);
            }
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

            TempData["mensaje"] = "Producto agregado correctamente.";
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


        //    

        [HttpGet]
        public ActionResult SeleccionarTipoPago(int idCliente)
        {
            ViewBag.IdCliente = idCliente;

            // Obtener lista de tipos de pago
            var tiposPago = carritoDao.ObtenerTiposPago();

            // Obtener el carrito del cliente
            List<CarritoCompra> listacarrito;
            if (idCliente > 0)
            {
                listacarrito = carritoDao.GetCarritoPorCliente(idCliente);
            }
            else
            {
                listacarrito = ObtenerCarritoDesdeSesion();
            }
            foreach (var item in listacarrito)
            {
                var producto = carritoDao.ObtenerProductoPorId(item.IdProducto);
                item.NombreProducto = producto?.nombre_producto ?? "Producto no encontrado";
            }
            // Calcular el importe total del carrito
            ViewBag.Total = listacarrito.Sum(c => c.ImporteTotal);
            ViewBag.Carrito = listacarrito;

            return View(tiposPago);
        }


        [HttpGet]
        public ActionResult DetalleCompra(int idCarritoCompra)
        {
            // Obtener los detalles de la compra
            var detalleCompra = carritoDao.ObtenerDetallesCompra(idCarritoCompra);
            if (detalleCompra == null || !detalleCompra.Any())
            {
                TempData["mensaje"] = "No hay detalles de compra disponibles.";
                return RedirectToAction("VerCarrito", new { idCarritoCompra });
            }

            TempData["DetalleCompra"] = JsonConvert.SerializeObject(detalleCompra);

            return View("DetalleCompra", detalleCompra);

        }



        /* [HttpPost]
         public ActionResult ConfirmarCompra(int idCarritoCompra, int idProducto, int cantidad, decimal importeTotal, int idTipoPago)
         {
             // Obtener el id_cliente de la sesión
             int? idCliente = Session["id_cliente"] as int?;

             // Verificar si el cliente está logueado
             if (idCliente == null || idCliente <= 0)
             {
                 TempData["mensaje"] = "Debe iniciar sesión para confirmar la compra.";
                 return RedirectToAction("Login", "Account");  // Redirige al login o donde sea apropiado
             }

             // Confirmar la compra
             var mensaje = carritoDao.ConfirmarCompra(idCarritoCompra, idCliente.Value, idProducto, cantidad, importeTotal, idTipoPago);
             TempData["mensaje"] = mensaje;

             // Redirigir a la acción de detalles de la compra
             return RedirectToAction("DetalleCompra", new { idCarritoCompra, idTipoPago });
         }*/
       



    }
}
