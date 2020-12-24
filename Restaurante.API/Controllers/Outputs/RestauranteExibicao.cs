namespace Restaurante.API.Controllers.Outputs
{
    public class RestauranteExibicao
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public int Cozinha { get; set; }
        public EnderecoExibicao EnderecoExibicao { get; set; }
    }
}
