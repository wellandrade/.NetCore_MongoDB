using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Restaurante.API.Domain.Entities;
using Restaurante.API.Domain.Enums;
using Restaurante.API.Domain.ValueObjects;

namespace Restaurante.API.Data.Schemas
{
    public class RestauranteSchema
    {
        [BsonRepresentation(BsonType.ObjectId)] // Para gerar o id automaticamente 
        public string Id { get; set; } 
        public string Nome { get; set; }
        public ECozinha Cozinha { get; set; }
        public EnderecoSchema Endereco { get; set; }
    }
}
