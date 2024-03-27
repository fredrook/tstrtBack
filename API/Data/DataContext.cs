#region IMPORTAÇÕES
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
#endregion

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Vaga> Vagas { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<Estacionamento> Estacionamentos { get; set; }
    }
}