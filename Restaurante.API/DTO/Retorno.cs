namespace Restaurante.API.DTO
{
    public class Retorno
    {
        public Retorno(bool sucesso, string mensagem)
        {
            Sucesso = sucesso;
            Mensagem = mensagem;
        }

        public bool Sucesso { get; private set; }
        public string Mensagem { get; private set; }
    }
}
