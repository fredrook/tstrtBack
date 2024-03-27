#region IMPORTAÇÕES
using Domain.Enum;
using System;
using System.ComponentModel.DataAnnotations;
#endregion

namespace Domain.Entities
{
    public class Vaga
    {
        [Key]
        public int IdVaga { get; set; }
        public bool Ocupada { get; set; }
        public Veiculo Veiculo { get; set; }
        public TipoVeiculo TipoVaga { get; set; }
        public int? VeiculoId { get; set; }

        public Vaga()
        {

        }

        public Vaga(TipoVeiculo tipoVaga, int idVaga, int? veiculoId)
        {
            this.Ocupada = false;
            this.TipoVaga = tipoVaga;
            this.IdVaga = idVaga;
            this.VeiculoId = veiculoId;
        }

        public void Estacionar(Veiculo veiculo)
        {
            this.Veiculo = veiculo;
            this.Ocupada = true;

            if (veiculo.IdVeiculo < 0)
                this.VeiculoId = veiculo.IdVeiculo;

        }

        public void Liberar()
        {
            this.Ocupada = false;
            if (Veiculo != null)
            {
                this.Veiculo = null;
            }
        }

        public decimal CalcularSaida(DateTime horaEntrada)
        {
            if (!Ocupada)
            {
                throw new InvalidOperationException("A vaga está vazia, não é possível calcular a saída.");
            }

            if (Veiculo == null || horaEntrada == DateTime.MinValue)
            {
                throw new InvalidOperationException("A hora de entrada do veículo não está definida corretamente.");
            }

            horaEntrada = Veiculo.HoraEntrada;
            DateTime horaSaida = DateTime.UtcNow;
            TimeSpan tempoEstacionado = horaSaida - horaEntrada;

            int minutosEstacionado = (int)tempoEstacionado.TotalMinutes;

            decimal precoTotal = CalcularPreco(minutosEstacionado);

            return precoTotal;
        }

        private decimal CalcularPreco(int minutosEstacionado)
        {
            const decimal precoPorMinuto = 0.016666m;

            decimal precoTotal = minutosEstacionado * precoPorMinuto;

            precoTotal = Math.Ceiling(precoTotal * 100) / 100;

            return precoTotal;
        }
    }
}
