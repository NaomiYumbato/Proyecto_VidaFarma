namespace PryVidaFarma.Models;

public class TbCliente
{
    public int IdCliente { get; set; }

    public int IdPersona { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string Contrasenia { get; set; } = null!;

    public TbPersona IdPersonaNavigation { get; set; } = null!;
}
