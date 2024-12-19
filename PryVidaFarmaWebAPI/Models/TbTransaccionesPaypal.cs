using System;
using System.Collections.Generic;

namespace PryVidaFarmaWebAPI.Models;

public partial class TbTransaccionesPaypal
{
    public int IdTransaccion { get; set; }

    public int IdCarritoCompra { get; set; }

    public string PaypalTransactionId { get; set; } = null!;

    public decimal Monto { get; set; }

    public DateTime? FechaTransaccion { get; set; }

    public string NombreTitular { get; set; } = null!;

    public string Dni { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public int IdBanco { get; set; }

    public int IdTipoPago { get; set; }

    public virtual TbBanco IdBancoNavigation { get; set; } = null!;

    public virtual TbCarritoCompra IdCarritoCompraNavigation { get; set; } = null!;

    public virtual TbTiposPago IdTipoPagoNavigation { get; set; } = null!;
}
