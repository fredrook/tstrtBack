#region IMPORTAÇÕES
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
#endregion

namespace Persistence.Contextos
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
        public DbSet<Vaga> Vagas { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<Estacionamento> Estacionamentos { get; set; }

    }
}
