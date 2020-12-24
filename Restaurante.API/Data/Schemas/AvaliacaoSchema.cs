using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Restaurante.API.Data.Schemas
{
    public class AvaliacaoSchema
    {
        public ObjectId Id { get; set; } 
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string RestauranteId { get; set; } // Como string para facilitar o uso de comparacao nas querys
        
        public int Estrelas { get; set; }
        
        public string Comentarios { get; set; }

    }
}
