namespace PryVidaFarma.Models
{
    public class CarritoCompra
    {
        public int IdCarritoCompra { get; set; }
        public int IdCliente { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal ImporteTotal { get; set; }

        public void CalcularImporte()
        {
            ImporteTotal = Precio * Cantidad;
        }
    }

}

