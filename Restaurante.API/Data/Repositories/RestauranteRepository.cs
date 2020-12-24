using MongoDB.Driver;
using Restaurante.API.Data.Schemas;
using Restaurante.API.Domain.Entities;
using Restaurante.API.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Restaurante.API.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Driver.Linq;

namespace Restaurante.API.Data.Repositories
{
    public class RestauranteRepository
    {
        IMongoCollection<RestauranteSchema> _restaurantes;

        IMongoCollection<AvaliacaoSchema> _avaliacao;

        public RestauranteRepository(MongoDB mongoDB)
        {
            _restaurantes = mongoDB.DB.GetCollection<RestauranteSchema>("restaurante"); // restaurante é o nome da collection no banco
            _avaliacao = mongoDB.DB.GetCollection<AvaliacaoSchema>("avaliacao");
        }

        public void Inserir(Restaurantes restaurante)
        {
            var documento = new RestauranteSchema
            {
                Nome = restaurante.Nome,
                Cozinha = restaurante.Cozinha,
                Endereco = new EnderecoSchema
                {
                    Logradouro = restaurante.Endereco.Logradouro,
                    Numero = restaurante.Endereco.Numero,
                    Cidade = restaurante.Endereco.Cidade,
                    CEP = restaurante.Endereco.CEP,
                    UF = restaurante.Endereco.UF,
                }
            };

            _restaurantes.InsertOne(documento);
        }
        public async Task<IEnumerable<Restaurantes>> ObterTodos()
        {
            var restaurantes = new List<Restaurantes>();

            // Usando os AsQueryable para fazer a consulta com linq 
            await _restaurantes.AsQueryable().ForEachAsync(d =>
           {
               var restaurante = new Restaurantes(d.Id.ToString(), d.Nome, d.Cozinha);
               var endereco = new Endereco(d.Endereco.Logradouro, d.Endereco.Numero, d.Endereco.Cidade, d.Endereco.UF, d.Endereco.CEP);

               restaurante.AtribuirEndereco(endereco);
               restaurantes.Add(restaurante);
           });

            return restaurantes;
        }
        public Restaurantes ObterPorId(string id)
        {
            var document = _restaurantes.AsQueryable().FirstOrDefault(x => x.Id == id);

            if (document == null)
                return null;

            return document.ConverterParaDomain();
        }
        public bool AlterarCompleto(Restaurantes restaurante)
        {
            var documento = new RestauranteSchema
            {
                Id = restaurante.Id,
                Nome = restaurante.Nome,
                Cozinha = restaurante.Cozinha,
                Endereco = new EnderecoSchema
                {
                    Logradouro = restaurante.Endereco.Logradouro,
                    Numero = restaurante.Endereco.Numero,
                    Cidade = restaurante.Endereco.Cidade,
                    CEP = restaurante.Endereco.CEP,
                    UF = restaurante.Endereco.UF,
                }
            };

            var resultado = _restaurantes.ReplaceOne(r => r.Id == documento.Id, documento); // Alterar todos os dados do documento pelo o que veio 

            return resultado.ModifiedCount > 0;
        }
        public bool AlterarCozinha(string id, ECozinha cozinha)
        {
            var cozinhaNova = Builders<RestauranteSchema>.Update.Set(r => r.Cozinha, cozinha); // representa o SET do update 

            var resultado = _restaurantes.UpdateOne(r => r.Id == id, cozinhaNova); // efetua o updat e

            return resultado.ModifiedCount > 0;
        }
        public IEnumerable<Restaurantes> ObterPorNome(string nome)
        {
            var restaurantes = new List<Restaurantes>();

            #region Sintaxe Mongo

            /* 
            Sintaxe Mongo 

            var filtro = new BsonDocument { { "nome", new BsonDocument { { "regex", nome } }, { "$options", "i" } }; // options para ser case sensitive 
            _restaurantes.Find(filtro).ToList().ForEach(d => restaurantes.Add(d.ConverterParaDomain()));
            */

            #endregion

            _restaurantes.AsQueryable()
                .Where(d => d.Nome.ToLower().Contains(nome.ToLower()))
                .ToList()
                .ForEach(d => restaurantes.Add(d.ConverterParaDomain()));

            return restaurantes;
        }
        public void Avaliar(string restauranteId, Avaliacao avaliacao)
        {
            var documento = new AvaliacaoSchema
            {
                RestauranteId = restauranteId,
                Estrelas = avaliacao.Estrelas,
                Comentarios = avaliacao.Comentario
            };

            _avaliacao.InsertOne(documento);
        }
        public async Task<Dictionary<Restaurantes, double>> ObterTop3()
        {
            var retorno = new Dictionary<Restaurantes, double>();

            var top3 = _avaliacao.Aggregate()
                .Group(x => x.RestauranteId, g => new { RestauranteId = g.Key, MediaEstrelas = g.Average(e => e.Estrelas) })
                .SortByDescending(x => x.MediaEstrelas)
                .Limit(3);

            await top3.ForEachAsync(x =>
            {
                var restaurante = ObterPorId(x.RestauranteId);

                _avaliacao.AsQueryable()
                    .Where(r => r.RestauranteId == x.RestauranteId)
                    .ToList()
                    .ForEach(a => restaurante.InserirAvaliacao(a.ConverterParaDomain()));

                retorno.Add(restaurante, x.MediaEstrelas);

            });

            return retorno;
        }
        public (long, long) Remover(string restauranteId)
        {
            var resultadoAvaliacoes = _avaliacao.DeleteMany(x => x.RestauranteId == restauranteId);
            var resultadoRestaurante = _avaliacao.DeleteOne(x => x.RestauranteId == restauranteId);

            return (resultadoRestaurante.DeletedCount, resultadoAvaliacoes.DeletedCount);
        }

        public async Task<IEnumerable<Restaurantes>> ObterPorBuscaTextual(string nome)
        {
            var restaurantes = new List<Restaurantes>();

            var filtro = Builders<RestauranteSchema>.Filter.Text(nome);

            await _restaurantes
                .AsQueryable()
                .Where(x => filtro.Inject())
                .ForEachAsync(d => restaurantes.Add(d.ConverterParaDomain()));

            return restaurantes;
        }
    }
}
