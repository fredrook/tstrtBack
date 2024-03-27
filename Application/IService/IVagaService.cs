#region IMPORTAÇÕES
using Domain.Entities;
using Domain.Enum;
using System;
using System.Collections.Generic;
#endregion

namespace Application.Service
{
    public interface IVagaService
    {
        List<Vaga> ObterVagasDisponiveis();
        (string mensagem, List<int> vagaIdsUtilizadas) EstacionarVeiculo(Veiculo veiculo);
        void RemoverVeiculo(List<int> vagaIds, int idVeiculo);
        int TipoVeiculo(TipoVeiculo tipoVeiculo);
        bool EstacionamentoVazioCheio();
        int TotalVagas();
        bool TodasVagasMotoOcupadas();
        bool TodasVagasCarroOcupadas();
        bool TodasVagasVanOcupadas();
        int VagasOcupadasPorMoto();
        int VagasOcupadasPorCarro();
        int VagasOcupadasPorVans();
        List<object> ListarVeiculosEstacionados(List<object> lista, Func<Veiculo, bool> filtro);
    }
}