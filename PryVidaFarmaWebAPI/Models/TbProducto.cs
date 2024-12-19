using System;
using System.Collections.Generic;

namespace PryVidaFarmaWebAPI.Models;

public partial class TbProducto
{
    public int IdProducto { get; set; }

    public string NombreProducto { get; set; } = null!;

    public string Detalles { get; set; } = null!;

    public int Stock { get; set; }

    public decimal Precio { get; set; }

    public int IdCategoria { get; set; }

    public string Imagen { get; set; } = null!;

    public int? Estado { get; set; }

    public virtual TbCategoria IdCategoriaNavigation { get; set; } = null!;

    public virtual ICollection<TbCarritoCompra> TbCarritoCompras { get; set; } = new List<TbCarritoCompra>();

    public virtual ICollection<TbDetalleCompra> TbDetalleCompras { get; set; } = new List<TbDetalleCompra>();
}
