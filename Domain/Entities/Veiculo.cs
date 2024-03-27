#region IMPORTAÇÕES
using Domain.Enum;
using Domain.Validation;
using System;
using System.ComponentModel.DataAnnotations;
#endregion

namespace Domain.Entities
{
    public class Veiculo
    {
        [Key]
        public int IdVeiculo { get; set; }
        public TipoVeiculo Tipo { get; set; }

        [StringLength(7, ErrorMessage = "O campo Placa deve ter no máximo 7 caracteres.")]
        [Validadores(ErrorMessage = "O campo Placa deve conter apenas letras.")]
        public string Placa { get; set; }

        [Validadores(ErrorMessage = "O campo Cor deve conter apenas letras.")]
        public string Cor { get; set; }

        public int VagaId { get; set; }
        public DateTime HoraEntrada { get; set; }

        public Veiculo()
        {

        }

        public Veiculo(TipoVeiculo tipo, string placa, string cor, int vagaId)
        {
            this.Tipo = tipo;
            Placa = placa;
            Cor = cor;

            if (!VerificarTipoVeiculoValido(tipo))
            {
                throw new ArgumentException("Tipo de veículo inválido.");
            }
            VagaId = vagaId;
        }

        public bool VerificarTipoVeiculoValido(TipoVeiculo tipoVeiculo)
        {
            return TipoVeiculo.IsDefined(typeof(TipoVeiculo), tipoVeiculo);
        }
    }
}