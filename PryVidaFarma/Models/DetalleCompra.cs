namespace PryVidaFarma.Models
{
    public class DetalleCompra
    {
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal ImporteTotal { get; set; }
        public string TipoPago { get; set; } = string.Empty;
        public DateTime FechaCompra { get; set; }
    }
}
