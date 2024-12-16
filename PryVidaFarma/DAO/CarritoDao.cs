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


        /// Agregar un artículo al carrito en la base de datos.
        public void AgregarCarrito(CarritoCompra carrito)
        {
            SqlHelper.ExecuteNonQuery(
                cad_cn,
                CommandType.StoredProcedure,
                "PA_AGREGAR_CARRITO_COMPRA",
                new SqlParameter("@id_cliente", carrito.IdCliente),
                new SqlParameter("@id_producto", carrito.IdProducto),
                new SqlParameter("@cantidad", carrito.Cantidad),
                new SqlParameter("@importe_total", carrito.ImporteTotal)
            );
        }

        /// Obtener todos los artículos del carrito de un cliente específico.
        public List<CarritoCompra> GetCarritoPorCliente(int idCliente)
        {
            var lista = new List<CarritoCompra>();
            using (var dr = SqlHelper.ExecuteReader(
                cad_cn,
                CommandType.StoredProcedure,
                "PA_OBTENER_CARRITO_POR_CLIENTE",
                new SqlParameter("@id_cliente", idCliente)))
            {
                while (dr.Read())
                {
                    lista.Add(new CarritoCompra
                    {
                        IdCarritoCompra = dr.GetInt32(0),
                        IdCliente = dr.GetInt32(1),
                        IdProducto = dr.GetInt32(2),
                        NombreProducto = dr.GetString(3),
                        Precio= dr.GetDecimal(4),
                        Cantidad = dr.GetInt32(5),
                        ImporteTotal = dr.GetDecimal(6)
                    });
                }
            }
            return lista;
        }

        /// Eliminar un artículo del carrito por su ID.
        public void EliminarArticulo(int idCarritoCompra)
        {
            SqlHelper.ExecuteNonQuery(
                cad_cn,
                CommandType.StoredProcedure,
                "PA_ELIMINAR_ARTICULO_CARRITO",
                new SqlParameter("@id_carrito_compra", idCarritoCompra)
            );
        }

        //
        // Obtener tipos de pago
        public List<TipoPago> ObtenerTiposPago()
        {
            var lista = new List<TipoPago>();

            using (var dr = SqlHelper.ExecuteReader(
                cad_cn,
                CommandType.Text,
                "SELECT id_tipo_pago, descripcion FROM tb_tipos_pago"))
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


        //
        public List<DetalleCompra> ConfirmarCompra(int idCliente, int idTipoPago)
        {
            var lista = new List<DetalleCompra>();

            using (var dr = SqlHelper.ExecuteReader(
                cad_cn,
                CommandType.StoredProcedure,
                "PA_CONFIRMAR_COMPRA",
                new SqlParameter("@id_cliente", idCliente),
                new SqlParameter("@id_tipo_pago", idTipoPago)))
            {
                while (dr.Read())
                {
                    lista.Add(new DetalleCompra
                    {
                        NombreProducto = dr.GetString(0),
                        Cantidad = dr.GetInt32(1),
                        Precio = dr.GetDecimal(2),
                        ImporteTotal = dr.GetDecimal(3),
                        TipoPago = dr.GetString(4),
                        FechaCompra = dr.GetDateTime(5)
                    });
                }
            }
            return lista;
        }
    }
}
