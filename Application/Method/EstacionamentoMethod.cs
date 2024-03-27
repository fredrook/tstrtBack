#region IMPORTAÇÕES
using Domain.Entities;
using Domain.Enum;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Application.Method
{
    internal class EstacionamentoMethod
    {
        public static int CalcularVagasRestantes(List<Vaga> vagas, TipoVeiculo tipoVeiculo)
        {
            return vagas.Count(v => !v.Ocupada && v.TipoVaga == tipoVeiculo);
        }

        public static int CalcularTotalVagas(List<Vaga> vagas)
        {
            return vagas.Count;
        }

        public static bool VerificarLotacaoEstacionamento(List<Vaga> vagas)
        {
            return !vagas.Any(v => !v.Ocupada);
        }
         
        public static bool VerificarTodasVagasMotoOcupadas(List<Vaga> vagas)
        {
            return vagas.Where(v => v.TipoVaga == TipoVeiculo.Moto).All(v => v.Ocupada);
        }

        public static bool VerificarTodasVagasCarroOcupadas(List<Vaga> vagas)
        {
            return vagas.Where(v => v.TipoVaga == TipoVeiculo.Carro).All(v => v.Ocupada);
        }

        public static bool VerificarTodasVagasVanOcupadas(List<Vaga> vagas)
        {
            return vagas.Where(v => v.TipoVaga == TipoVeiculo.Van).All(v => v.Ocupada);
        }

        public static int CalcularVagasOcupadasPorMoto(List<Vaga> vagas)
        {
            return vagas.Count(v => v.Ocupada && v.TipoVaga == TipoVeiculo.Moto);
        }

        public static int CalcularVagasOcupadasPorCarros(List<Vaga> vagas)
        {
            return vagas.Count(v => v.Ocupada && v.TipoVaga == TipoVeiculo.Carro);
        }

        public static int CalcularVagasOcupadasPorVans(List<Vaga> vagas)
        {
            return vagas.Count(v => v.Ocupada && v.TipoVaga == TipoVeiculo.Van);
        }
    }
}
