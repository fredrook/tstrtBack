#region IMPORTA��ES
using Domain.Entities;
#endregion

namespace Application.IService
{
    public interface IVeiculoService
    {
        Veiculo ObterVeiculo(int IdVeiculo);
    }
}