namespace PryVidaFarma.Models;

public class TbPersona
{
    public int IdPersona { get; set; }

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Dni { get; set; } = null!;

    public DateOnly FechaNacimiento { get; set; }

    public string Direccion { get; set; } = null!;

    public string CorreoElectronico { get; set; } = null!;
}
