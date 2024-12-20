namespace PryVidaFarma.Models
{
    public class DetalleCompra
    {
      
        public string IdCarritoCompra { get; set; } = string.Empty;
        public string NombreCliente { get; set; } = string.Empty;
        public string DireccionCliente { get; set; } = string.Empty;
        public string TipoPago { get; set; } = string.Empty;
        public DateTime FechaCompra { get; set; }
        public List<ProductoCarrito> Productos { get; set; }


    }

   
}
