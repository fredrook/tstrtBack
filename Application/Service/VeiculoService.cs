#region IMPORTAÇÕES
using System.Linq;
using Application.IService;
using Domain.Entities;
using Persistence.Contextos;
#endregion

namespace Application.Service
{
    public class VeiculoService : IVeiculoService
    {
        private readonly Context context;

        public VeiculoService(Context context)
        {
            this.context = context;
        }

        public Veiculo ObterVeiculo(int idVeiculo)
        {
            return context.Veiculos.FirstOrDefault(x => x.IdVeiculo == idVeiculo);
        }
    }
}