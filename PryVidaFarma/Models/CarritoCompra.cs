using Newtonsoft.Json;

namespace PryVidaFarma.Models
{
    public class CarritoCompra
    {
        public int IdCarritoCompra { get; set; }
        public int IdCliente { get; set; }
        public string ProductosJson { get; set; } = string.Empty;
        public decimal ImporteTotal { get; set; }
        public int IdTipoPago { get; set; }

        // Método para deserializar el JSON y obtener los productos
        public List<ProductoCarrito> ObtenerProductos()
        {
            return string.IsNullOrEmpty(ProductosJson)
                ? new List<ProductoCarrito>()
                : JsonConvert.DeserializeObject<List<ProductoCarrito>>(ProductosJson);
        }

        // Método para calcular el importe total desde el JSON
        public void CalcularImporteTotal()
        {
            var productos = ObtenerProductos();
            ImporteTotal = productos.Sum(p => p.Cantidad * p.Precio);
        }
    }

    public class ProductoCarrito
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal ImporteTotal { get; set; } // Permite asignación explícita

        //public decimal ImporteTotal => Cantidad * Precio; 
    }


}
