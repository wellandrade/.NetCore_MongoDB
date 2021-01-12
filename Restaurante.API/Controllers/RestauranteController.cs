using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Restaurante.API.Controllers.Inputs;
using Restaurante.API.Controllers.Outputs;
using Restaurante.API.Data.Repositories;
using Restaurante.API.Domain.Entities;
using Restaurante.API.Domain.ValueObjects;
using Restaurante.API.DTO;
using Restaurante.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurante.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class RestauranteController : ControllerBase
    {
        private readonly RestauranteRepository _restauranteRepositorio;

        public RestauranteController(RestauranteRepository restauranteRepositorio)
        {
            _restauranteRepositorio = restauranteRepositorio;
        }

        [HttpPost("restaurante")]
        public ActionResult<Retorno> IncluirRestaurante([FromBody] RestauranteInclusao input)
        {
            var cozinha = ECozinhaHelper.ConverterDeInteiro(input.Cozinha);

            var restaurante = new Restaurantes(input.Nome, cozinha);
            var endereco = new Endereco(input.Logradouro, input.Numero, input.Cidade, input.UF, input.CEP);

            restaurante.AtribuirEndereco(endereco);

            if (!restaurante.Validar())
            {
                return BadRequest(new
                {
                    errors = restaurante.ValidationResult.Errors.Select(x => x.ErrorMessage)
                });
            }

            _restauranteRepositorio.Inserir(restaurante);

            return Ok(new
            {
                data = "Restaurante cadastrado com sucesso"
            });
        }

        [HttpGet("restaurante/todos")]
        public async Task<ActionResult<IList<RestauranteListagem>>> ObterRestaurantes()
        {
            var restaurantes = await _restauranteRepositorio.ObterTodos();

            var listagem = restaurantes.Select(r => new RestauranteListagem
            {
                Id = r.Id,
                Nome = r.Nome,
                Cozinha = (int)r.Cozinha,
                Cidade = r.Endereco.Cidade
            });

            return Ok(listagem);
        }

        [HttpGet("restaurante/{id}")]
        public ActionResult ObterRestaurante(string id)
        {
            var restaurante = _restauranteRepositorio.ObterPorId(id);

            if (restaurante == null)
                return NotFound();

            var exibicao = new RestauranteExibicao
            {
                Id = restaurante.Id,
                Nome = restaurante.Nome,
                Cozinha = (int)restaurante.Cozinha,
                EnderecoExibicao = new EnderecoExibicao
                {
                    Logradouro = restaurante.Endereco.Logradouro,
                    CEP = restaurante.Endereco.CEP,
                    Numero = restaurante.Endereco.Numero,
                    Cidade = restaurante.Endereco.Cidade,
                    UF = restaurante.Endereco.UF
                }
            };

            return Ok(new
            {
                data = exibicao
            }); ;
        }

        [HttpPut("restaurante")]
        public ActionResult AlterarRestaurante([FromBody] RestauranteAlteracaoCompleta restauranteAlteracaoCompleta)
        {
            var restaurante = _restauranteRepositorio.ObterPorId(restauranteAlteracaoCompleta.Id);

            if (restaurante == null)
                return NotFound();

            var cozinha = ECozinhaHelper.ConverterDeInteiro(restauranteAlteracaoCompleta.Cozinha);
            restaurante = new Restaurantes(restauranteAlteracaoCompleta.Id, restauranteAlteracaoCompleta.Nome, cozinha);

            var endereco = new Endereco(restauranteAlteracaoCompleta.Logradouro, restauranteAlteracaoCompleta.Numero,
                                        restauranteAlteracaoCompleta.Cidade, restauranteAlteracaoCompleta.UF, restauranteAlteracaoCompleta.CEP);

            restaurante.AtribuirEndereco(endereco);

            if (!restaurante.Validar())
            {
                return BadRequest(
                    new
                    {
                        errors = restaurante.ValidationResult.Errors.Select(e => e.ErrorMessage)
                    });
            }

            if (!_restauranteRepositorio.AlterarCompleto(restaurante))
            {
                return BadRequest(
                    new
                    {
                        errors = "Nenhum restaurante teve as informacoes alteradas"
                    });
            }

            return Ok(
                new
                {
                    data = "Restaurante alterado com sucesso"
                }
            );
        }

        [HttpPatch("restaurante/{id}")]
        public ActionResult AlterarCozinha(string id, [FromBody] RestauranteAlteracaoParcial restauranteAlteracaoParcial)
        {
            var restaurante = _restauranteRepositorio.ObterPorId(id);

            if (restaurante == null)
                return NotFound();

            var cozinha = ECozinhaHelper.ConverterDeInteiro(restauranteAlteracaoParcial.Cozinha);

            if (!restaurante.Validar())
            {
                return BadRequest(
                    new
                    {
                        errors = restaurante.ValidationResult.Errors.Select(e => e.ErrorMessage)
                    });
            }

            if (!_restauranteRepositorio.AlterarCozinha(id, cozinha))
            {
                return BadRequest(
                    new
                    {
                        errors = "Nenhum restaurante teve as informacoes alteradas"
                    });
            }

            return Ok(
                new
                {
                    data = "Restaurante alterado com sucesso"
                }
            );
        }

        [HttpGet("restaurante")]
        public ActionResult ObterRestaurantePorNome([FromQuery] string nome)
        {
            var restaurante = _restauranteRepositorio.ObterPorNome(nome);

            if (restaurante == null)
                return NotFound();

            var listagem = restaurante.Select(r => new RestauranteListagem
            {
                Id = r.Id,
                Nome = r.Nome,
                Cozinha = (int)r.Cozinha,
                Cidade = r.Endereco.Cidade
            });

            return Ok(new
            {
                data = listagem
            });
        }

        [HttpPatch("restaurante/{id}/avaliar")]
        public ActionResult AvaliarRestaurante(string id, [FromBody] AvaliacaoInclusao avaliacaoInclusao)
        {
            var restaurante = _restauranteRepositorio.ObterPorId(id);

            if (restaurante == null)
                return NotFound();

            var avaliacao = new Avaliacao(avaliacaoInclusao.Estrelas, avaliacaoInclusao.Comentario);

            if (!avaliacao.Validar())
            {
                return BadRequest(new
                {
                    errors = avaliacao.ValidationResult.Errors.Select(e => e.ErrorMessage)
                });
            };

            _restauranteRepositorio.Avaliar(id, avaliacao);

            return Ok(new
            {
                data = "Restaurante avaliado com sucesso"
            });
        }

        [HttpGet("restaurante/top3")]
        public async Task<ActionResult> ObterTop3Restaurante()
        {
            var top3 = await _restauranteRepositorio.ObterTop3();

            if (top3 == null)
                return NotFound();

            var listagem = top3.Select(x => new RestauranteTop3
            {
                Id = x.Key.Id,
                Nome = x.Key.Nome,
                Cozinha = (int)x.Key.Cozinha,
                Cidade = x.Key.Endereco.Cidade,
                Estrelas = x.Value
            });

            return Ok(new
            {
                data = listagem
            });
        }

        [HttpDelete("restaurante/{id}")]
        public ActionResult Remover(string id)
        {
            var restaurante = _restauranteRepositorio.ObterPorId(id);

            if (restaurante == null)
                return NotFound();

            (var totalRestauranteRemovido, var totalAvaliacoesRemovidas) = _restauranteRepositorio.Remover(id);

            return Ok(new
            {
                data = $"Total de exclusoes: { totalRestauranteRemovido } restaurante com { totalAvaliacoesRemovidas } avaliacoes"
            });
        }

        [HttpGet("restaurante/textual")]
        public async Task<ActionResult> ObterRestaurantePorBuscaTextual([FromQuery] string nome)
        {
            var restaurantes = await _restauranteRepositorio.ObterPorBuscaTextual(nome);

            var listagem = restaurantes.ToList().Select(x => new RestauranteListagem
            {
                Id = x.Id,
                Nome = x.Nome,
                Cozinha = (int)x.Cozinha,
                Cidade = x.Endereco.Cidade
            });

            return Ok(new
            {
                data = listagem
            });
        }

    }
}
