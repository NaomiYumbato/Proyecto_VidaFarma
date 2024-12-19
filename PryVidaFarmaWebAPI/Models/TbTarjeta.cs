using System;
using System.Collections.Generic;

namespace PryVidaFarmaWebAPI.Models;

public partial class TbTarjeta
{
    public int IdTarjeta { get; set; }

    public string NombreTitular { get; set; } = null!;

    public string NumeroTarjeta { get; set; } = null!;

    public DateOnly FechaVencimiento { get; set; }

    public string Cvv { get; set; } = null!;

    public int IdCliente { get; set; }

    public int? IdBanco { get; set; }

    public int IdTipoPago { get; set; }

    public virtual TbBanco? IdBancoNavigation { get; set; }

    public virtual TbCliente IdClienteNavigation { get; set; } = null!;

    public virtual TbTiposPago IdTipoPagoNavigation { get; set; } = null!;
}
