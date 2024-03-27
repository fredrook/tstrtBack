#region IMPORTAÇÕES
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#endregion

namespace Domain.Entities
{
    public class Estacionamento
    {
        [Key]
        public int IdEstacionamento { get; set; }
        public required List<Vaga> Vagas { get; set; }
    }
}