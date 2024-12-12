using System.ComponentModel.DataAnnotations.Schema;

namespace PryVidaFarma.Models
{
    [Table("tb_categorias")]
    public class Categorias
    {
        public int id_categoria { get; set; }
        public string nombre_categoria { get; set; } = string.Empty;
    }
}
