#region IMPORTAÇÕES
using API.Data;
using Application.Service;
using Domain.Entities;
using Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Contextos;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class EstacionamentoController : ControllerBase
    {
        private readonly IVagaService _vagaService;
        private readonly DataContext _dataContext;

        public EstacionamentoController(IVagaService vagaService, DataContext dataContext)
        {
            _vagaService = vagaService;
            _dataContext = dataContext;
        }

        [HttpPost("verificarVagasDisponiveis")]
        public IActionResult VerificarVagasDisponiveis()
        {
            try
            {
                var vagasDisponiveis = _vagaService.ObterVagasDisponiveis();
                if (vagasDisponiveis == null || !vagasDisponiveis.Any())
                {
                    return Ok("Não há vagas disponíveis no momento.");
                }

                var vagasFormatadas = vagasDisponiveis.Select(v => $"Vaga Nº {v.IdVaga}, Tipo de Vaga {v.TipoVaga.ToString()}").ToList();

                return Ok($"Vagas disponíveis:\n{string.Join("\n", vagasFormatadas)}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("estacionar")]
        public IActionResult EstacionarVeiculo(Veiculo veiculo)
        {
            if (veiculo == null)
            {
                return BadRequest("Dados do veículo inválidos.");
            }

            try
            {

                var (mensagem, vagaIdsUtilizadas) = _vagaService.EstacionarVeiculo(veiculo);
                if (mensagem == null || mensagem != null)
                {
                    string sucessoMessage = $"Veículo Placa {veiculo.Placa} cor {veiculo.Cor} estacionado com sucesso";

                    if (vagaIdsUtilizadas != null && vagaIdsUtilizadas.Any())
                    {
                        sucessoMessage += $" nas vagas {string.Join(", ", vagaIdsUtilizadas)}";
                    }

                    sucessoMessage += $" às {DateTime.Now}";

                    return Ok(sucessoMessage);
                }
                else
                {
                    return BadRequest(mensagem);
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("listarVeiculosEstacionados")]
        public IActionResult ListarVeiculos()
        {
            try
            {
                var veiculosEstacionados = new List<object>();

                _vagaService.ListarVeiculosEstacionados(veiculosEstacionados, veiculo => true);

                return Ok(veiculosEstacionados);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("removerVeiculo")]
        public IActionResult RemoverVeiculo(string vagaIds, int idVeiculo)
        {
            try
            {
                List<int> vagaIdList = vagaIds.Split(',').Select(int.Parse).ToList();

                if (vagaIdList.Any(v => _dataContext.Vagas.FirstOrDefault(vaga => vaga.IdVaga == v) == null))
                {
                    return BadRequest($"Alguma(s) vaga(s) especificada(s) não foi(ram) encontrada(s).");
                }

                Veiculo veiculo = _dataContext.Veiculos.FirstOrDefault(v => v.IdVeiculo == idVeiculo);
                if (veiculo == null)
                {
                    return BadRequest($"O veículo com ID {idVeiculo} não foi encontrado.");
                }

                Vaga vaga = _dataContext.Vagas.FirstOrDefault(v => v.IdVaga == vagaIdList.First());
                decimal precoTotal = vaga.CalcularSaida(veiculo.HoraEntrada);

                _vagaService.RemoverVeiculo(vagaIdList, veiculo.IdVeiculo);

                return StatusCode(200, $"Obrigado, você pagou R${precoTotal}. Vaga {string.Join(", ", vagaIdList)} liberada. Volte Sempre!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("consultarTotalVagas")]
        public IActionResult TotalVagas()
        {
            try
            {
                var totalVagas = _vagaService.TotalVagas();

                return Ok(totalVagas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("verificarEstacionamento-vazio-cheio")]
        public IActionResult EstacionamentoCheio()
        {
            try
            {
                var estacionamentoCheio = _vagaService.EstacionamentoVazioCheio();

                return Ok(estacionamentoCheio ? "Desculpe, o estacionamento no momento está cheio, volte outra hora." : "Olá, seja bem-vindo! Temos vagas disponíveis no momento. Escolha a sua e tenha um ótimo dia!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("tiposDeVeiculosEstacionados")]
        public IActionResult VagasRestantes(TipoVeiculo tipoVeiculo)
        {
            try
            {
                if (!Enum.IsDefined(typeof(TipoVeiculo), tipoVeiculo))
                {
                    return BadRequest("Esse tipo de veículo não confere.");
                }

                var quantidadeVeiculos = _vagaService.TipoVeiculo(tipoVeiculo);
                var mensagem = $"Contém {quantidadeVeiculos} {(quantidadeVeiculos == 1 ? "veículo" : "veículos")} do tipo {tipoVeiculo} estacionados no momento.";

                return Ok(mensagem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
