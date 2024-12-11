using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PryVidaFarma.Models;
using System.Data;
using System.Data.SqlClient;

namespace PryVidaFarma.DAO
{
    public class ProductosDAO 
    {
        private string cad_cn;
        public ProductosDAO(IConfiguration cfg)
        {
            cad_cn = cfg.GetConnectionString("cad_cn");
        }

        public List<Productos> ListadoProductos()
        {
            var lista = new List<Productos>();
            //
            var dr =
                SqlHelper.ExecuteReader(cad_cn, "sp_ListarProductos");
            //
            while (dr.Read())
            {
                lista.Add(
                    new Productos()
                    {
                        id_producto = dr.GetInt32(0),
                        nombre_producto = dr.GetString(1),
                        detalles = dr.GetString(2),
                        stock = dr.GetInt32(3),
                        precio = dr.GetDecimal(4),
                        categoria = new Categorias() 
                        {
                            nombre_categoria = dr.GetString(5)
                        },
                        imagen = dr.GetString(6),
                        estado = dr.GetInt32(7) 

                    });
            }
            dr.Close();
            //
            return lista;
        }

        public string RegistrarProductos(Productos obj, int opcion = 1)
        {
            string mensaje = $"El Nuevo Producto fue Registrado correctamente";
            try
            {
                SqlHelper.ExecuteNonQuery(cad_cn, "sp_RegistrarProducto",
                    obj.id_producto,
                    obj.nombre_producto, obj.detalles,
                    obj.stock, obj.precio, obj.categoria.id_categoria,
                    obj.imagen, obj.estado);

                if (opcion == 2)
                {
                    mensaje = $"Los datos del Producto {obj.id_producto} fueron actualizado correctamente";
                }

                return mensaje;
            }
            catch (Exception ex)
            {
                mensaje = $"Ocurrió un error al registrar el producto: {ex.Message}";
                mensaje += $"Parametros: {obj.id_producto}, {obj.nombre_producto}, {obj.imagen}";
                return mensaje;
            }

        }

        public string EliminarProducto(int id_producto)
        {
            string mensaje = "";
            try
            {
                SqlHelper.ExecuteNonQuery(cad_cn, "sp_EliminarProducto", id_producto);
                mensaje = $"Se ha eliminado el Producto con ID: {id_producto}";
            }
            catch (Exception ex)
            {
                mensaje = $"Error al eliminar el producto: {ex.Message}";
            }
            return mensaje;
        }

        public List<Productos> ListadoProductosPorCategoria(int id_categoria)
        {
            var lista = new List<Productos>();

            using (var dr = SqlHelper.ExecuteReader(cad_cn, "sp_BuscarProductosPorCategoria", id_categoria))
            {
                while (dr.Read())
                {
                    lista.Add(
                    new Productos()
                    {
                        id_producto = dr.GetInt32(0),
                        nombre_producto = dr.GetString(1),
                        detalles = dr.GetString(2),
                        stock = dr.GetInt32(3),
                        precio = dr.GetDecimal(4),
                        categoria = new Categorias()
                        {
                            nombre_categoria = dr.GetString(5)
                        },
                        imagen = dr.GetString(6),
                        estado = dr.GetInt32(7)

                    });
                }
            }

            return lista;
        }

        public List<Productos> ListadoProductosPorPalabra(string palabra_clave)
        {
            var lista = new List<Productos>();

            using (var dr = SqlHelper.ExecuteReader(cad_cn, "sp_BuscarProductoPorPalabra", palabra_clave))
            {
                while (dr.Read())
                {
                    lista.Add(new Productos()
                    {
                        id_producto = dr.GetInt32(0),
                        nombre_producto = dr.GetString(1),
                        detalles = dr.GetString(2),
                        stock = dr.GetInt32(3),
                        precio = dr.GetDecimal(4),
                        categoria = new Categorias()
                        {
                            nombre_categoria = dr.GetString(5)
                        },
                        imagen = dr.GetString(6),
                        estado = dr.GetInt32(7)
                    });
                }
            }

            return lista;
        }
    }
}
