using Newtonsoft.Json;
using PryVidaFarma.Models;
using System.Data;
using System.Data.SqlClient;

namespace PryVidaFarma.DAO
{
    public class CarritoDao
    {
        private readonly string cad_cn;

        public CarritoDao(IConfiguration cfg)
        {
            cad_cn = cfg.GetConnectionString("cad_cn") ?? throw new InvalidOperationException("Cadena de conexión no configurada.");
        }

        /// <summary>
        /// Agregar un artículo al carrito en la base de datos.
        /// </summary>
        public void AgregarCarrito(CarritoCompra carrito)
        {
            SqlHelper.ExecuteNonQuery(
                cad_cn,
                CommandType.StoredProcedure,
                "PA_CONFIRMAR_COMPRA",
                new SqlParameter("@id_cliente", carrito.IdCliente),
                new SqlParameter("@productos_json", carrito.ProductosJson),
                new SqlParameter("@importeTotal", carrito.ImporteTotal),
                new SqlParameter("@id_tipo_pago", carrito.IdTipoPago)
            );
        }

        /// <summary>
        /// Actualizar un carrito existente.
        /// </summary>
        public void ActualizarCarrito(CarritoCompra carrito)
        {
            SqlHelper.ExecuteNonQuery(
                cad_cn,
                CommandType.StoredProcedure,
                "PA_ACTUALIZAR_CARRITO",
                new SqlParameter("@id_carrito_compra", carrito.IdCarritoCompra),
                new SqlParameter("@productos_json", carrito.ProductosJson),
                new SqlParameter("@importe_total", carrito.ImporteTotal)
            );
        }

        public List<ProductoCarrito> ObtenerCarritoDetalladoPorCliente(int idCliente)
        {
            var productos = new List<ProductoCarrito>();

            using (var dr = SqlHelper.ExecuteReader(
                cad_cn,
                CommandType.StoredProcedure,
                "PA_OBTENER_CARRITO_CON_DETALLES",
                new SqlParameter("@id_cliente", idCliente)))
            {
                while (dr.Read())
                {
                    productos.Add(new ProductoCarrito
                    {
                        IdProducto = dr.GetInt32(dr.GetOrdinal("id_producto")),
                        NombreProducto = dr.GetString(dr.GetOrdinal("nombre_producto")),
                        Cantidad = dr.GetInt32(dr.GetOrdinal("cantidad")),
                        Precio = dr.GetDecimal(dr.GetOrdinal("precio")),
                        ImporteTotal = dr.GetDecimal(dr.GetOrdinal("importe_total"))
                    });
                }
            }

            // Agrupar y eliminar duplicados (por si el procedimiento devuelve productos duplicados)
            return productos;
        }



        //forma de pago <--para
        public CarritoCompra ObtenerCarritoCompletoPorCliente(int idCliente)
        {
            var carrito = new CarritoCompra { IdCliente = idCliente };
            var productos = new List<ProductoCarrito>();

            using (var dr = SqlHelper.ExecuteReader(
                cad_cn,
                CommandType.StoredProcedure,
                "PA_OBTENER_CARRITO_CON_DETALLES",
                new SqlParameter("@id_cliente", idCliente)))
            {
                while (dr.Read())
                {
                    Console.WriteLine($"Producto ID: {dr.GetInt32(2)}, Nombre: {dr.GetString(4)}"); // Agregar log
                    productos.Add(new ProductoCarrito
                    {
                        IdProducto = dr.GetInt32(2),
                        NombreProducto = dr.GetString(4),
                        Cantidad = dr.GetInt32(3),
                        Precio = dr.GetDecimal(5),
                        ImporteTotal = dr.GetDecimal(6)
                    });


                }


                // Serializar los productos en el JSON del carrito
                carrito.ProductosJson = JsonConvert.SerializeObject(productos);
                carrito.CalcularImporteTotal();


            }
            return carrito;
        }


        public List<ProductoCarrito> ObtenerProductosPorCliente(int idCliente)
        {
            var carrito = ObtenerCarritoCompletoPorCliente(idCliente);
            return carrito?.ObtenerProductos() ?? new List<ProductoCarrito>();
        }


        /// <summary>
        /// Eliminar un artículo del carrito por su ID.
        /// </summary>
        public void EliminarArticulo(int idCarritoCompra)
        {
            SqlHelper.ExecuteNonQuery(
                cad_cn,
                CommandType.StoredProcedure,
                "PA_ELIMINAR_ARTICULO_CARRITO",
                new SqlParameter("@id_carrito_compra", idCarritoCompra)
            );
        }

        /// <summary>
        /// Obtener los tipos de pago disponibles.
        /// </summary>
        public List<TipoPago> ObtenerTiposPago()
        {
            var lista = new List<TipoPago>();

            using (var dr = SqlHelper.ExecuteReader(
                cad_cn,
                CommandType.StoredProcedure,
                "PA_ObtenerTiposPago"))
            {
                while (dr.Read())
                {
                    lista.Add(new TipoPago
                    {
                        IdTipoPago = dr.GetInt32(0),
                        Descripcion = dr.GetString(1)
                    });
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtener los detalles de una compra específica.
        /// </summary>
        public List<DetalleCompra> ObtenerDetallesCompra(int idCarritoCompra)
        {
            var lista = new List<DetalleCompra>();

            using (var dr = SqlHelper.ExecuteReader(
                cad_cn,
                CommandType.StoredProcedure,
                "PA_OBTENER_DETALLES_COMPRA",
                new SqlParameter("@id_carrito_compra", idCarritoCompra)))
            {
                while (dr.Read())
                {
                    lista.Add(new DetalleCompra
                    {
                        NombreProducto = dr["nombre_producto"].ToString(),
                        Cantidad = Convert.ToInt32(dr["cantidad"]),
                        Precio = Convert.ToDecimal(dr["precio"]),
                        ImporteTotal = Convert.ToDecimal(dr["ImporteTotal"]),
                        TipoPago = dr["TipoPago"].ToString(),
                        FechaCompra = Convert.ToDateTime(dr["fecha_compra"])
                    });

                }
            }

            return lista;
        }


        //
        //Confirmar comopra
        public (int idCarritoCompra, string mensaje) ConfirmarCompra(int idCliente, string productosJson, decimal importeTotal, int idTipoPago)
        {
            var outputIdParam = new SqlParameter("@id_carrito_compra", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            var mensajeParam = new SqlParameter("@mensaje", SqlDbType.NVarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            SqlHelper.ExecuteNonQuery(
                cad_cn,
                CommandType.StoredProcedure,
                "PA_CONFIRMAR_COMPRA",
                new SqlParameter("@id_cliente", idCliente),
                new SqlParameter("@productos_json", productosJson),
                new SqlParameter("@importeTotal", importeTotal),
                new SqlParameter("@id_tipo_pago", idTipoPago),
                outputIdParam,
                mensajeParam
            );

            int idCarritoCompra = outputIdParam.Value != DBNull.Value ? (int)outputIdParam.Value : 0;
            string mensaje = mensajeParam.Value as string ?? "Operación completada sin mensaje.";

            return (idCarritoCompra, mensaje);
        }


        ////
        ///
        public Productos ObtenerProductoPorId(int idProducto)
        {
            Productos producto = null;

            using (var dr = SqlHelper.ExecuteReader(
                cad_cn,
                CommandType.StoredProcedure,
                "PA_ObtenerProductoPorId",
                new SqlParameter("@id_producto", idProducto)))
            {
                if (dr.Read())
                {
                    producto = new Productos
                    {
                        id_producto = dr.GetInt32(0),
                        nombre_producto = dr.GetString(1),
                        detalles = dr.GetString(2),
                        stock = dr.GetInt32(3),
                        precio = dr.GetDecimal(4),
                        categoria = new Categorias { id_categoria = dr.GetInt32(5) },
                        imagen = dr.GetString(6),
                        estado = dr.GetInt32(7)
                    };
                }
            }

            return producto;
        }
    }
}
