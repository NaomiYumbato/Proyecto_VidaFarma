using System;
using System.Collections.Generic;

namespace PryVidaFarmaWebAPI.Models;

public partial class TbPersona
{
    public int IdPersona { get; set; }

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Dni { get; set; } = null!;

    public DateOnly FechaNacimiento { get; set; }

    public string Direccion { get; set; } = null!;

    public string CorreoElectronico { get; set; } = null!;

    public virtual ICollection<TbCliente> TbClientes { get; set; } = new List<TbCliente>();

    public virtual ICollection<TbEmpleado> TbEmpleados { get; set; } = new List<TbEmpleado>();
}
