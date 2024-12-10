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
    }
}
