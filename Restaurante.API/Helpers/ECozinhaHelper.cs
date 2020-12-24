using Restaurante.API.Domain.Enums;
using System;

namespace Restaurante.API.Helpers
{
    public static class ECozinhaHelper
    {
        public static ECozinha ConverterDeInteiro(int idCozinha)
        {
            if (Enum.TryParse(idCozinha.ToString(), out ECozinha cozinha))
            {
                return cozinha;
            }

            throw new ArgumentOutOfRangeException("Opcao de cozinha invalida");
        }
    }
}
