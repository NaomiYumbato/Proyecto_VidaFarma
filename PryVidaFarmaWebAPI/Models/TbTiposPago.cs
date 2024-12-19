using System;
using System.Collections.Generic;

namespace PryVidaFarmaWebAPI.Models;

public partial class TbTiposPago
{
    public int IdTipoPago { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<TbDetalleCompra> TbDetalleCompras { get; set; } = new List<TbDetalleCompra>();

    public virtual ICollection<TbTarjeta> TbTarjeta { get; set; } = new List<TbTarjeta>();

    public virtual ICollection<TbTransaccionesPaypal> TbTransaccionesPaypals { get; set; } = new List<TbTransaccionesPaypal>();
}
