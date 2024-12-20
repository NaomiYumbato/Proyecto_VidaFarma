// CarritoController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PryVidaFarma.DAO;
using PryVidaFarma.Models;

namespace PryVidaFarma.Controllers
{
    public class CarritoController : BaseController
    {
        private readonly CarritoDao carritoDao;

        public CarritoController(CarritoDao _carritoDao, CategoriasDAO categoriasDAO) : base(categoriasDAO)
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
            return string.IsNullOrEmpty(carritoJson)
                ? new List<CarritoCompra>()
                : JsonConvert.DeserializeObject<List<CarritoCompra>>(carritoJson) ?? new List<CarritoCompra>();

        }

        // Mostrar el carrito del cliente logueado o no logueado
        [HttpGet]
        public ActionResult VerCarrito(int? idCliente)
        {
            CarritoCompra carritoCompra;

            if (idCliente.HasValue && idCliente > 0)
            {
                // Obtener carrito para usuarios logueados desde la base de datos
                carritoCompra = carritoDao.ObtenerCarritoCompletoPorCliente(idCliente.Value);
            }
            else
            {
                // Obtener carrito desde sesión para usuarios no logueados
                var productosSesion = ObtenerCarritoDesdeSesion();

                // Deserializar los productos y consolidarlos
                var productos = productosSesion
                    .SelectMany(c => JsonConvert.DeserializeObject<List<ProductoCarrito>>(c.ProductosJson))
                    .ToList();

                // Construir el carrito con los productos deserializados
                carritoCompra = new CarritoCompra
                {
                    ProductosJson = JsonConvert.SerializeObject(productos),
                    ImporteTotal = productos.Sum(p => p.Cantidad * p.Precio)
                };
            }

            // Validar si el carrito tiene productos
            if (string.IsNullOrEmpty(carritoCompra.ProductosJson) || !carritoCompra.ObtenerProductos().Any())
            {
                TempData["mensaje"] = "El carrito está vacío.";
                return RedirectToAction("ListadoProductos");
            }

            // Pasar los datos necesarios a la vista
            ViewBag.Total = carritoCompra.ImporteTotal;

            return View(carritoCompra); // Enviar CarritoCompra a la vista
        }




        [HttpGet]
        public ActionResult AgregarAlCarrito(int idProducto)
        {
            var producto = carritoDao.ObtenerProductoPorId(idProducto);

            if (producto == null)
            {
                TempData["mensaje"] = "Producto no encontrado.";
                return RedirectToAction("ListadoProductos", "Productos");
            }

            return View(producto);
        }

        [HttpPost]
        public ActionResult AgregarAlCarrito(int? idCliente, int idProducto, int cantidad, decimal precio, decimal importeTotal)
        {
            if (cantidad <= 0 || precio <= 0)
            {
                TempData["mensaje"] = "Cantidad o precio no válidos.";
                return RedirectToAction("ListadoProductos");
            }

            var nuevoProducto = new ProductoCarrito
            {
                IdProducto = idProducto,
                Cantidad = cantidad,
                Precio = precio,
                ImporteTotal = precio * cantidad,
                NombreProducto = carritoDao.ObtenerProductoPorId(idProducto)?.nombre_producto ?? "Producto desconocido"
            };

            if (idCliente.HasValue && idCliente > 0)
            {
                var carritoExistente = carritoDao.ObtenerCarritoCompletoPorCliente(idCliente.Value);

                if (carritoExistente == null)
                {
                    var nuevoCarrito = new CarritoCompra
                    {
                        IdCliente = idCliente.Value,
                        ProductosJson = JsonConvert.SerializeObject(new List<ProductoCarrito> { nuevoProducto }),
                        ImporteTotal = nuevoProducto.ImporteTotal
                    };
                    carritoDao.AgregarCarrito(nuevoCarrito);
                }
                else
                {
                    var productos = carritoExistente.ObtenerProductos();
                    var existente = productos.FirstOrDefault(p => p.IdProducto == idProducto);

                    if (existente == null)
                    {
                        productos.Add(nuevoProducto);
                    }
                    else
                    {
                        existente.Cantidad += cantidad;
                    }

                    carritoExistente.ProductosJson = JsonConvert.SerializeObject(productos);
                    carritoExistente.CalcularImporteTotal();
                    carritoDao.ActualizarCarrito(carritoExistente);
                }
            }
            else
            {
                var carrito = ObtenerCarritoDesdeSesion();
                var existente = carrito.FirstOrDefault(c => c.ProductosJson.Contains(idProducto.ToString()));

                if (existente == null)
                {
                    carrito.Add(new CarritoCompra
                    {
                        ProductosJson = JsonConvert.SerializeObject(new List<ProductoCarrito> { nuevoProducto }),
                        ImporteTotal = nuevoProducto.ImporteTotal
                    });
                }
                else
                {
                    var productos = JsonConvert.DeserializeObject<List<ProductoCarrito>>(existente.ProductosJson);
                    var prodExistente = productos.FirstOrDefault(p => p.IdProducto == idProducto);

                    if (prodExistente == null)
                    {
                        productos.Add(nuevoProducto);
                    }
                    else
                    {
                        prodExistente.Cantidad += cantidad;
                    }

                    existente.ProductosJson = JsonConvert.SerializeObject(productos);
                    existente.CalcularImporteTotal();
                }

                GuardarCarritoEnSesion(carrito);
            }

            TempData["mensaje"] = "Producto agregado correctamente.";
            return RedirectToAction("VerCarrito", new { idCliente });
        }

        public ActionResult EliminarArticulo(int idCarritoCompra, int? idCliente, int idProducto)
        {
            if (idCliente.HasValue)
            {
                carritoDao.EliminarArticulo(idCarritoCompra);
            }
            else
            {
                var carrito = ObtenerCarritoDesdeSesion();
                carrito.RemoveAll(c => c.ProductosJson.Contains(idProducto.ToString()));
                GuardarCarritoEnSesion(carrito);
            }

            TempData["mensaje"] = "Producto eliminado del carrito.";
            return RedirectToAction("VerCarrito", new { idCliente });
        }


        [HttpGet]
        public ActionResult SeleccionarTipoPago(int idCliente)
        {
            ViewBag.IdCliente = idCliente;

            // Obtener lista de tipos de pago
            var tiposPago = carritoDao.ObtenerTiposPago();

            // Obtener el carrito desde sesión
            var carritoSesion = ObtenerCarritoDesdeSesion();

            if (carritoSesion == null || !carritoSesion.Any())
            {
                TempData["mensaje"] = "El carrito está vacío.";
                return RedirectToAction("VerCarrito");
            }

            // Obtener productos del carrito actual desde sesión
            var productos = carritoSesion
                .SelectMany(c => c.ObtenerProductos())
                .ToList();

            if (!productos.Any())
            {
                TempData["mensaje"] = "No hay productos en el carrito.";
                return RedirectToAction("VerCarrito");
            }

            // Calcular importe total y completar información
            foreach (var producto in productos)
            {
                producto.ImporteTotal = producto.Cantidad * producto.Precio;
            }

            // Asignar datos a ViewBag para la vista
            ViewBag.Total = productos.Sum(p => p.ImporteTotal);
            ViewBag.ProductosJson = JsonConvert.SerializeObject(productos);

            return View(tiposPago);
        }




        void SincronizarCarritoEnSesion(CarritoCompra carrito)
        {
            var carritosSesion = ObtenerCarritoDesdeSesion();
            var carritoExistente = carritosSesion.FirstOrDefault(c => c.IdCarritoCompra == carrito.IdCarritoCompra);

            if (carritoExistente != null)
            {
                carritosSesion.Remove(carritoExistente);
            }
            carritosSesion.Add(carrito);

            GuardarCarritoEnSesion(carritosSesion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarCompra(string productosJson, decimal importeTotal, int idTipoPago)
        {
            // Obtener el cliente desde la sesión
            string usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (!int.TryParse(usuarioIdStr, out int idCliente) || idCliente <= 0)
            {
                TempData["mensaje"] = "Debe iniciar sesión para confirmar la compra.";
                return RedirectToAction("Login", "Usuario");
            }

            // Validar que el carrito tenga productos y el importe sea válido
            if (string.IsNullOrEmpty(productosJson) || importeTotal <= 0)
            {
                TempData["mensaje"] = "El carrito está vacío o el importe total no es válido.";
                return RedirectToAction("VerCarrito");
            }

            // Validar que el tipo de pago sea válido
            if (idTipoPago <= 0)
            {
                TempData["mensaje"] = "Debe seleccionar un tipo de pago válido.";
                return RedirectToAction("SeleccionarTipoPago");
            }

            // Confirmar la compra
            var (idCarritoCompra, mensaje) = carritoDao.ConfirmarCompra(idCliente, productosJson, importeTotal, idTipoPago);

            TempData["mensaje"] = mensaje;

            if (idCarritoCompra > 0)
            {
                HttpContext.Session.Remove("CarritoTemporal");
                return RedirectToAction("DetalleCompra", new { idCarritoCompra });
            }

            TempData["mensaje"] = mensaje ?? "Error al confirmar la compra.";
            return RedirectToAction("VerCarrito", new { idCliente });
        }








        [HttpGet]
        public IActionResult DetalleCompra(int idCarritoCompra)
        {
            try
            {
                if (idCarritoCompra <= 0)
                {
                    TempData["mensaje"] = "ID de carrito inválido.";
                    return RedirectToAction("VerCarrito");
                }

                var detalleCompra = carritoDao.ObtenerDetallesCompra(idCarritoCompra);

                if (detalleCompra == null || !detalleCompra.Any())
                {
                    TempData["mensaje"] = "No se encontraron detalles para el carrito especificado.";
                    return RedirectToAction("VerCarrito");
                }

                return View("DetalleCompra", detalleCompra);
            }
            catch (Exception)
            {
                TempData["mensaje"] = "Ocurrió un error inesperado al obtener los detalles de la compra.";
                return RedirectToAction("VerCarrito");
            }
        }
    }
}
