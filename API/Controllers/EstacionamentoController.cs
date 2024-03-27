#region IMPORTA��ES
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
                    return Ok("N�o h� vagas dispon�veis no momento.");
                }

                var vagasFormatadas = vagasDisponiveis.Select(v => $"Vaga N� {v.IdVaga}, Tipo de Vaga {v.TipoVaga.ToString()}").ToList();

                return Ok($"Vagas dispon�veis:\n{string.Join("\n", vagasFormatadas)}");
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
                return BadRequest("Dados do ve�culo inv�lidos.");
            }

            try
            {

                var (mensagem, vagaIdsUtilizadas) = _vagaService.EstacionarVeiculo(veiculo);
                if (mensagem == null || mensagem != null)
                {
                    string sucessoMessage = $"Ve�culo Placa {veiculo.Placa} cor {veiculo.Cor} estacionado com sucesso";

                    if (vagaIdsUtilizadas != null && vagaIdsUtilizadas.Any())
                    {
                        sucessoMessage += $" nas vagas {string.Join(", ", vagaIdsUtilizadas)}";
                    }

                    sucessoMessage += $" �s {DateTime.Now}";

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
                    return BadRequest($"Alguma(s) vaga(s) especificada(s) n�o foi(ram) encontrada(s).");
                }

                Veiculo veiculo = _dataContext.Veiculos.FirstOrDefault(v => v.IdVeiculo == idVeiculo);
                if (veiculo == null)
                {
                    return BadRequest($"O ve�culo com ID {idVeiculo} n�o foi encontrado.");
                }

                Vaga vaga = _dataContext.Vagas.FirstOrDefault(v => v.IdVaga == vagaIdList.First());
                decimal precoTotal = vaga.CalcularSaida(veiculo.HoraEntrada);

                _vagaService.RemoverVeiculo(vagaIdList, veiculo.IdVeiculo);

                return StatusCode(200, $"Obrigado, voc� pagou R${precoTotal}. Vaga {string.Join(", ", vagaIdList)} liberada. Volte Sempre!");
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

                return Ok(estacionamentoCheio ? "Desculpe, o estacionamento no momento est� cheio, volte outra hora." : "Ol�, seja bem-vindo! Temos vagas dispon�veis no momento. Escolha a sua e tenha um �timo dia!");
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
                    return BadRequest("Esse tipo de ve�culo n�o confere.");
                }

                var quantidadeVeiculos = _vagaService.TipoVeiculo(tipoVeiculo);
                var mensagem = $"Cont�m {quantidadeVeiculos} {(quantidadeVeiculos == 1 ? "ve�culo" : "ve�culos")} do tipo {tipoVeiculo} estacionados no momento.";

                return Ok(mensagem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
