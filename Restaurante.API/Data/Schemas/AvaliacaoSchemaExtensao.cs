using Restaurante.API.Domain.ValueObjects;

namespace Restaurante.API.Data.Schemas
{
    public static class AvaliacaoSchemaExtensao
    {
        public static Avaliacao ConverterParaDomain(this AvaliacaoSchema documento)
        {
            return new Avaliacao(documento.Estrelas, documento.Comentarios);
        }
    }
}
