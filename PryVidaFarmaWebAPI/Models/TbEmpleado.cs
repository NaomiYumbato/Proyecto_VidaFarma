using System;
using System.Collections.Generic;

namespace PryVidaFarmaWebAPI.Models;

public partial class TbEmpleado
{
    public int IdEmpleado { get; set; }

    public int IdPersona { get; set; }

    public int IdPuesto { get; set; }

    public decimal Salario { get; set; }

    public DateOnly FechaContratacion { get; set; }

    public virtual TbPersona IdPersonaNavigation { get; set; } = null!;

    public virtual TbPuesto IdPuestoNavigation { get; set; } = null!;
}
