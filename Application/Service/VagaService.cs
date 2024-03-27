#region IMPORTA��ES
using System;
using System.Linq;
using Domain.Entities;
using Application.Service;
using Domain.Enum;
using System.Collections.Generic;
using Persistence.Contextos;
using Application.Method;
using Microsoft.EntityFrameworkCore;
#endregion

namespace Application
{
    public class VagaService : IVagaService
    {
        private readonly Context context;

        public VagaService(Context Context)
        {
            this.context = Context;
        }

        private List<int> _vagaIdsUtilizadas;

        public List<Vaga> ObterVagasDisponiveis()
        {
            return context.Vagas.Where(v => !v.Ocupada).ToList();
        }

        public List<object> ListarVeiculosEstacionados(List<object> lista, Func<Veiculo, bool> filtro)
        {
            var veiculos = context.Veiculos.Where(filtro).ToList();

            foreach (var veiculo in veiculos)
            {
                var vagasOcupadas = context.Vagas.Include(x => x.Veiculo)
                    .Where(v => v.Veiculo.IdVeiculo == veiculo.IdVeiculo && v.Ocupada).ToList();

                foreach (var vaga in vagasOcupadas)
                {
                    lista.Add(new
                    {
                        IdVeiculo = veiculo.IdVeiculo,
                        Placa = veiculo.Placa,
                        Cor = veiculo.Cor,
                        VagaId = vaga.IdVaga,
                        HoraEntrada = veiculo.HoraEntrada
                    });
                }
            }

            return lista;
        }

        public (string mensagem, List<int> vagaIdsUtilizadas) EstacionarVeiculo(Veiculo veiculo)
        {
            var vaga = ObterVagaDisponivelPorId(veiculo.VagaId);

            if (vaga is null)
            {
                throw new ArgumentException("Vaga especificada n�o est� dispon�vel ou n�o existe.");
            }

            if (vaga.Ocupada)
            {
                throw new InvalidOperationException($"A vaga {vaga.IdVaga} est� ocupada.");
            }

            var veiculoNaVaga = context.Veiculos.FirstOrDefault(v => v.VagaId == veiculo.VagaId);
            if (veiculoNaVaga != null)
            {
                throw new InvalidOperationException($"A vaga {veiculo.VagaId} j� est� sendo usada por outro ve�culo.");
            }

            _vagaIdsUtilizadas = new List<int>();

            if (veiculo.Tipo == Domain.Enum.TipoVeiculo.Moto)
            {
                if (!vaga.Ocupada)
                {
                    EstacionarVeiculoComum(veiculo, vaga);
                    _vagaIdsUtilizadas.Add(vaga.IdVaga);
                    return ($"Ve�culo Placa {veiculo.Placa} cor {veiculo.Cor} estacionado com sucesso na vaga {veiculo.VagaId} �s {DateTime.Now}", _vagaIdsUtilizadas);
                }
                else
                {
                    throw new InvalidOperationException("A vaga est� ocupada.");
                }
            }
            else if (veiculo.Tipo == Domain.Enum.TipoVeiculo.Carro)
            {
                if (vaga.TipoVaga == Domain.Enum.TipoVeiculo.Carro || vaga.TipoVaga == Domain.Enum.TipoVeiculo.Van)
                {
                    if (!vaga.Ocupada)
                    {
                        EstacionarVeiculoComum(veiculo, vaga);
                        _vagaIdsUtilizadas.Add(vaga.IdVaga);
                        return ($"Ve�culo Placa {veiculo.Placa} cor {veiculo.Cor} estacionado com sucesso na vaga {veiculo.VagaId} �s {DateTime.Now}", _vagaIdsUtilizadas);
                    }
                    else
                    {
                        throw new InvalidOperationException("A vaga est� ocupada.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Um carro s� pode estacionar em uma vaga para carro ou em uma vaga grande.");
                }
            }
            else if (veiculo.Tipo == Domain.Enum.TipoVeiculo.Van)
            {
                if (vaga.TipoVaga == Domain.Enum.TipoVeiculo.Carro || vaga.TipoVaga == Domain.Enum.TipoVeiculo.Van)
                {
                    var vagasDisponiveis = context.Vagas
                        .Where(v => !v.Ocupada && (v.TipoVaga == Domain.Enum.TipoVeiculo.Carro || v.TipoVaga == Domain.Enum.TipoVeiculo.Van))
                        .OrderBy(v => v.IdVaga)
                        .Take(3)
                        .ToList();

                    if (vagasDisponiveis.Count == 3)
                    {
                        foreach (var vagaDisponivel in vagasDisponiveis)
                        {
                            vagaDisponivel.Ocupada = true;
                            context.Vagas.Update(vagaDisponivel);
                            _vagaIdsUtilizadas.Add(vagaDisponivel.IdVaga);
                        }

                        var novaVan = new Veiculo
                        {
                            Tipo = Domain.Enum.TipoVeiculo.Van,
                            Placa = veiculo.Placa,
                            Cor = veiculo.Cor,
                            HoraEntrada = veiculo.HoraEntrada,
                            VagaId = vagasDisponiveis[0].IdVaga
                        };

                        context.Veiculos.Add(novaVan);
                        context.SaveChanges();

                        EstacionarVeiculoComum(veiculo, vaga);
                        return (_vagaIdsUtilizadas.Any() ? (null, _vagaIdsUtilizadas) : ($"Ve�culo Placa {veiculo.Placa} cor {veiculo.Cor} estacionado com sucesso na vaga {_vagaIdsUtilizadas} �s {DateTime.Now}", null));
                    }
                    else
                    {
                        throw new InvalidOperationException("N�o h� vagas suficientes para estacionar a Van.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Uma van s� pode estacionar em tr�s Vagas de Carro ou em uma vaga grande.");
                }
            }
            else
            {
                return ("Tipo de ve�culo inv�lido.", null);
            }
        }

        public void RemoverVeiculo(List<int> vagaIds, int idVeiculo)
        {
            var veiculo = context.Veiculos.FirstOrDefault(v => v.IdVeiculo == idVeiculo);
            if (veiculo == null)
            {
                throw new Exception($"O ve�culo com ID {idVeiculo} n�o existe.");
            }

            if (veiculo.VagaId == 0)
            {
                throw new Exception($"O ve�culo com ID {idVeiculo} n�o est� estacionado em nenhuma vaga.");
            }

            var vagasOcupadas = context.Vagas
                                    .Where(v => vagaIds.Contains(v.IdVaga) && v.Ocupada)
                                    .ToList();

            foreach (var vagaOcupada in vagasOcupadas)
            {
                vagaOcupada.Liberar();
            }

            context.SaveChanges();
        }

        private void EstacionarVeiculoComum(Veiculo veiculo, Vaga vaga)
        {
            veiculo.HoraEntrada = DateTime.UtcNow;
            veiculo.VagaId = vaga.IdVaga;
            vaga.Estacionar(veiculo);
            context.SaveChanges();
        }

        private Vaga ObterVagaDisponivelPorId(int vagaId)
        {
            return context.Vagas.FirstOrDefault(v => v.IdVaga == vagaId);
        }

        public int TipoVeiculo(TipoVeiculo tipoVeiculo)
        {
            var vagas = context.Vagas.Where(v => v.Veiculo != null && v.Veiculo.Tipo == tipoVeiculo).ToList();
            return vagas.Count;
        }

        public int TotalVagas()
        {
            var vagas = context.Vagas.ToList();
            return EstacionamentoMethod.CalcularTotalVagas(vagas);
        }

        public bool EstacionamentoVazioCheio()
        {
            var vagas = context.Vagas.ToList();
            return EstacionamentoMethod.VerificarLotacaoEstacionamento(vagas);
        }

        public bool TodasVagasMotoOcupadas()
        {
            var vagas = context.Vagas.ToList();
            return EstacionamentoMethod.VerificarTodasVagasMotoOcupadas(vagas);
        }

        public int VagasOcupadasPorMoto()
        {
            var vagas = context.Vagas.ToList();
            return EstacionamentoMethod.CalcularVagasOcupadasPorMoto(vagas);
        }

        public bool TodasVagasCarroOcupadas()
        {
            var vagas = context.Vagas.ToList();
            return EstacionamentoMethod.VerificarTodasVagasCarroOcupadas(vagas);
        }

        public bool TodasVagasVanOcupadas()
        {
            var vagas = context.Vagas.ToList();
            return EstacionamentoMethod.VerificarTodasVagasVanOcupadas(vagas);
        }

        public int VagasOcupadasPorCarro()
        {
            var vagas = context.Vagas.ToList();
            return EstacionamentoMethod.CalcularVagasOcupadasPorCarros(vagas);
        }

        public int VagasOcupadasPorVans()
        {
            var vagas = context.Vagas.ToList();
            return EstacionamentoMethod.CalcularVagasOcupadasPorVans(vagas);
        }
    }
}
