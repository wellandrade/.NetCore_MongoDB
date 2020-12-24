using Restaurante.API.Domain.Entities;
using Restaurante.API.Domain.ValueObjects;

namespace Restaurante.API.Data.Schemas
{
    public static class RestauranteSchemaExtensao
    {
        public static Restaurantes ConverterParaDomain(this RestauranteSchema document)
        {
            var restaurante = new Restaurantes(document.Id.ToString(), document.Nome, document.Cozinha);
            var endereco = new Endereco(document.Endereco.Logradouro, document.Endereco.Numero, document.Endereco.Cidade, document.Endereco.UF, document.Endereco.CEP);

            restaurante.AtribuirEndereco(endereco);

            return restaurante;
        }
    }
}
