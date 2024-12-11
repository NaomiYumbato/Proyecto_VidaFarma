namespace PryVidaFarma.Models
{
    
    public class Productos
    {
        public int id_producto { get; set; }
        public string nombre_producto { get; set; } = string.Empty;
        public string detalles {  get; set; } = string.Empty;
        public int stock { get; set; }
        public decimal precio { get; set; }
        public Categorias categoria { get; set; }
        public string imagen { get; set;} = string.Empty;
        public int estado { get; set; } = 1;

    }
}
