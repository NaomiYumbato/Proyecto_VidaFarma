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
        //VER CARRITO
        // Mostrar el carrito 
        [HttpGet]
        public ActionResult VerCarrito(int? idCliente)
        {
            CarritoCompra carritoCompra;

            // Obtenemos el carrito desde sesión 
            var productosSesion = ObtenerCarritoDesdeSesion();

            // Deserializar los productos y consolidarlos
            var productos = productosSesion
                .SelectMany(c => JsonConvert.DeserializeObject<List<ProductoCarrito>>(c.ProductosJson))
                .ToList();

            // Construir --> carrito con los productos deserializados
            carritoCompra = new CarritoCompra
            {
                ProductosJson = JsonConvert.SerializeObject(productos),
                ImporteTotal = productos.Sum(p => p.Cantidad * p.Precio)
            };

            // Validamos si el carrito tiene productos
            if (string.IsNullOrEmpty(carritoCompra.ProductosJson) || !carritoCompra.ObtenerProductos().Any())
            {
                TempData["mensaje"] = "El carrito está vacío.";
                return RedirectToAction("ListadoProductos", "Productos");
            }

            // Pasar los datos  a la vista
            ViewBag.Total = carritoCompra.ImporteTotal;

            // --> CarritoCompra a la vista
            return View(carritoCompra);
        }


        //AGREGAR AL CARRITO --> OBTENER
        //obtenemos los productos por id 
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

        //
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
                NombreProducto = carritoDao.ObtenerProductoPorId(idProducto)?.nombre_producto ?? "Producto desconocido",
                Imagen = carritoDao.ObtenerProductoPorId(idProducto)?.imagen??"Imagen"
            };

            
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
            

            TempData["mensaje"] = "Producto agregado correctamente.";
            return RedirectToAction("VerCarrito");
        }


        //ELIMINAR ARTICULO DEL CARRITO
        public ActionResult EliminarArticulo(int idProducto)
        {
            var carrito = ObtenerCarritoDesdeSesion();
            carrito.RemoveAll(c => c.ProductosJson.Contains(idProducto.ToString()));
            GuardarCarritoEnSesion(carrito);

            TempData["mensaje"] = "Producto eliminado del carrito.";
            return RedirectToAction("VerCarrito");
        }


        //SELECCIONAR FORMA DE PAGO
        [HttpGet]
        public ActionResult SeleccionarTipoPago(int idCliente)
        {
            ViewBag.IdCliente = idCliente;

            // Obtenemos la lista de tipos de pago
            var tiposPago = carritoDao.ObtenerTiposPago();

            // Obtenemos el carrito desde sesión
            var carritoSesion = ObtenerCarritoDesdeSesion();

            if (carritoSesion == null || !carritoSesion.Any())
            {
                TempData["mensaje"] = "El carrito está vacío.";
                return RedirectToAction("VerCarrito");
            }

            // Obtenemos los productos del carrito actual desde sesión
            var productos = carritoSesion
                .SelectMany(c => c.ObtenerProductos())
                .ToList();

            if (!productos.Any())
            {
                TempData["mensaje"] = "No hay productos en el carrito.";
                return RedirectToAction("VerCarrito");
            }

            // Calcular importe total
            foreach (var producto in productos)
            {
                producto.ImporteTotal = producto.Cantidad * producto.Precio;
            }

            // ViewBag
            ViewBag.Total = productos.Sum(p => p.ImporteTotal);
            ViewBag.ProductosJson = JsonConvert.SerializeObject(productos);

            return View(tiposPago);
        }

        //CONFIRMAR COMPRA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarCompra(string productosJson, decimal importeTotal, int idTipoPago)
        {
            // Obtenemos el cliente desde la sesión
            string usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (!int.TryParse(usuarioIdStr, out int idCliente) || idCliente <= 0)
            {
                TempData["mensaje"] = "Debe iniciar sesión para confirmar la compra.";
                return RedirectToAction("Login", "Usuario");
            }

            // validamos que el carrito tenga productos y que el importe sea válido
            if (string.IsNullOrEmpty(productosJson) || importeTotal <= 0)
            {
                TempData["mensaje"] = "El carrito está vacío o el importe total no es válido.";
                return RedirectToAction("VerCarrito");
            }

            // validamos que el tipo de pago sea válido
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



      
         //Detalle-BoletaCompra
        [HttpGet]
        public IActionResult DetalleCompra(int idCarritoCompra)
        {
            try
            {
                // Validación del ID del carrito
                if (idCarritoCompra <= 0)
                {
                    TempData["mensaje"] = "ID de carrito inválido.";
                    return RedirectToAction("VerCarrito");
                }

                // Llamar al DAO para obtener los datos del carrito y del cliente
                var detalleBoleta = carritoDao.ObtenerDetalleBoleta(idCarritoCompra);

                // Validación si no se encuentran datos
                if (detalleBoleta == null || detalleBoleta.Productos == null || !detalleBoleta.Productos.Any())
                {
                    TempData["mensaje"] = "No se encontraron detalles para el carrito especificado.";
                    return RedirectToAction("VerCarrito");
                }

                // Pasar los datos a la vista de la boleta
                return View("DetalleCompra", detalleBoleta);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                TempData["mensaje"] = $"Ocurrió un error inesperado: {ex.Message}";
                return RedirectToAction("VerCarrito");
            }
        }

    }
}
