using System;
using System.Collections.Generic;

namespace PryVidaFarmaWebAPI.Models;

public partial class TbBanco
{
    public int IdBanco { get; set; }

    public string NombreBanco { get; set; } = null!;

    public virtual ICollection<TbTarjeta> TbTarjeta { get; set; } = new List<TbTarjeta>();

    public virtual ICollection<TbTransaccionesPaypal> TbTransaccionesPaypals { get; set; } = new List<TbTransaccionesPaypal>();
}
