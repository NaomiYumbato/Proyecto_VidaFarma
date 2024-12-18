using System;
using System.Collections.Generic;

namespace PryVidaFarmaWebAPI.Models;

public partial class TbDetalleCompra
{
    public int IdDetalleCompra { get; set; }

    public int IdCarritoCompra { get; set; }

    public int IdProducto { get; set; }

    public DateOnly FechaCompra { get; set; }

    public int IdTipoPago { get; set; }

    public virtual TbCarritoCompra IdCarritoCompraNavigation { get; set; } = null!;

    public virtual TbProducto IdProductoNavigation { get; set; } = null!;

    public virtual TbTiposPago IdTipoPagoNavigation { get; set; } = null!;
}
