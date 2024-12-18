using System;
using System.Collections.Generic;

namespace PryVidaFarmaWebAPI.Models;

public partial class TbCarritoCompra
{
    public int IdCarritoCompra { get; set; }

    public int IdCliente { get; set; }

    public int IdProducto { get; set; }

    public int Cantidad { get; set; }

    public decimal ImporteTotal { get; set; }

    public virtual TbCliente IdClienteNavigation { get; set; } = null!;

    public virtual TbProducto IdProductoNavigation { get; set; } = null!;

    public virtual ICollection<TbDetalleCompra> TbDetalleCompras { get; set; } = new List<TbDetalleCompra>();

    public virtual ICollection<TbTransaccionesPaypal> TbTransaccionesPaypals { get; set; } = new List<TbTransaccionesPaypal>();
}
