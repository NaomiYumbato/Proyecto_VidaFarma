using PryVidaFarma.Models;
using System.Data.SqlClient;

namespace PryVidaFarma.DAO
{
    public class CategoriasDAO
    {
        private string cad_cn;
        public CategoriasDAO(IConfiguration cfg)
        {
            cad_cn = cfg.GetConnectionString("cad_cn");
        }

        public List<Categorias> ListadoCategorias()
        {
            var lista = new List<Categorias>();
            var dr = SqlHelper.ExecuteReader(cad_cn, "usp_ListarCategorias");
            while (dr.Read())
            {
                lista.Add(
                    new Categorias()
                    {
                        id_categoria = dr.GetInt32(0),
                        nombre_categoria = dr.GetString(1)
                    });
            }
            dr.Close();
            return lista;
        }

        public Categorias GetCategoriaById(int id_categoria)
        {
            using (var connection = new SqlConnection(cad_cn))
            {
                connection.Open();
                string query = "SELECT * FROM tb_categorias WHERE id_categoria = @id_categoria";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_categoria", id_categoria);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Categorias
                            {
                                id_categoria = Convert.ToInt32(reader["id_categoria"]),
                                nombre_categoria = reader["nombre_categoria"].ToString()
                            };
                        }
                    }
                }
            }

            return null;  
        }
    }
}
