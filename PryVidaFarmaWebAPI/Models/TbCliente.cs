using System;
using System.Collections.Generic;

namespace PryVidaFarmaWebAPI.Models;

public partial class TbCliente
{
    public int IdCliente { get; set; }

    public int IdPersona { get; set; }

    public DateOnly FechaRegistro { get; set; }

    public string Contrasenia { get; set; } = null!;

    public virtual TbPersona IdPersonaNavigation { get; set; } = null!;

    public virtual ICollection<TbCarritoCompra> TbCarritoCompras { get; set; } = new List<TbCarritoCompra>();

    public virtual ICollection<TbTarjeta> TbTarjeta { get; set; } = new List<TbTarjeta>();
}
