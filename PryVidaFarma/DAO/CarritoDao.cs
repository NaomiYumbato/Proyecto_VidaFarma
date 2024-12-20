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


        /// Agregar los productos al carrito mediante el id
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

        //OBTENER LAS FORMAS DE PAGO DESDE LA BASE DE DATOS
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

        //
        //CONFIRMAR COMPRA
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



        //OBTENER EL DETALLE DE LA COMPRA DESPUES DE HABER CONFIRMADO LA COMPRA 
       

        public DetalleCompra ObtenerDetalleBoleta(int idCarritoCompra)
        {
            DetalleCompra detalleBoleta = null;

            using (var dr = SqlHelper.ExecuteReader(
                cad_cn,
                CommandType.StoredProcedure,
                "PA_OBTENER_DETALLES_BOLETA",
                new SqlParameter("@id_carrito_compra", idCarritoCompra)))
            {
                if (dr.Read())
                {
                    detalleBoleta = new DetalleCompra
                    {
                        IdCarritoCompra = dr["id_carrito_compra"].ToString(),
                        NombreCliente = $"{dr["nombres"]} {dr["apellidos"]}",
                        DireccionCliente = dr["direccion"].ToString(),
                        TipoPago = dr["tipo_pago"].ToString(),
                        FechaCompra = Convert.ToDateTime(dr["fecha_compra"]),
                        Productos = JsonConvert.DeserializeObject<List<ProductoCarrito>>(dr["productos_json"].ToString())
                    };
                }
            }

            return detalleBoleta;
        }





    }
}
