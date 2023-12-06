using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ProyectoRedes.Models
{
    public class CreateGame
    {
         public string name { get; set; }
         public string owner { get; set; }
         public string? password { get; set; }
    }
}
