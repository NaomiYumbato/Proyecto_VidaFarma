using System;
using System.Collections.Generic;

namespace PryVidaFarmaWebAPI.Models;

public partial class TbCategoria
{
    public int IdCategoria { get; set; }

    public string NombreCategoria { get; set; } = null!;

    public virtual ICollection<TbProducto> TbProductos { get; set; } = new List<TbProducto>();
}
