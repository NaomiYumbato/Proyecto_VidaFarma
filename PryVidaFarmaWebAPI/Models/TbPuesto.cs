using System;
using System.Collections.Generic;

namespace PryVidaFarmaWebAPI.Models;

public partial class TbPuesto
{
    public int IdPuesto { get; set; }

    public string NombrePuesto { get; set; } = null!;

    public virtual ICollection<TbEmpleado> TbEmpleados { get; set; } = new List<TbEmpleado>();
}
