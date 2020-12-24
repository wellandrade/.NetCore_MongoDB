using FluentValidation;
using FluentValidation.Results;

namespace Restaurante.API.Domain.ValueObjects
{
    public class Endereco : AbstractValidator<Endereco>
    {
        public Endereco(string logradouro, string numero, string cidade, string uf, string cep)
        {
            Logradouro = logradouro;
            Numero = numero;
            Cidade = cidade;
            UF = uf;
            CEP = cep;
        }

        public string Logradouro { get; private set; }
        public string Numero { get; private set; }
        public string Cidade { get; private set; }
        public string UF { get; private set; }
        public string CEP { get; private set; }

        public ValidationResult ValidationResult { get; set; }

        public bool Validar()
        {
            ValidarLogradouro();
            ValidarCidade();
            ValidarUF();
            ValidarCEP();

            ValidationResult = Validate(this);
            return ValidationResult.IsValid;
        }

        private void ValidarLogradouro()
        {
            RuleFor(c => c.Logradouro)
                .NotEmpty().WithMessage("Logradouro não pode ser vazio")
                .MaximumLength(50).WithMessage("Logradouro pode ter no maximo 50 caracteres");
        }

        private void ValidarCidade()
        {
            RuleFor(c => c.Cidade)
                .NotEmpty().WithMessage("Cidade não pode ser vazio")
                .MaximumLength(100).WithMessage("Logradouro pode ter no maximo 100 caracteres");
        }

        private void ValidarUF()
        {
            RuleFor(c => c.UF)
                .NotEmpty().WithMessage("UF não pode ser vazio")
                .Length(2).WithMessage("UF deve ter 2 caracteres");
        }

        private void ValidarCEP()
        {
            RuleFor(c => c.CEP)
                .NotEmpty().WithMessage("CEP não pode ser vazio")
                .Length(8).WithMessage("CEP deve ter 8 caracteres");
        }
    }
}
