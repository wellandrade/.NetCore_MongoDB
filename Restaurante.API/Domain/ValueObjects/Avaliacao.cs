using FluentValidation;
using FluentValidation.Results;

namespace Restaurante.API.Domain.ValueObjects
{
    public class Avaliacao : AbstractValidator<Avaliacao>
    {
        public Avaliacao(int estrelas, string comentario)
        {
            Estrelas = estrelas;
            Comentario = comentario;
        }

        public int Estrelas { get; private set; }
        public string Comentario { get; private set; }
        public ValidationResult ValidationResult { get; set; }

        public bool Validar()
        {
            ValidarEstrelas();
            ValidarComentarios();

            ValidationResult = Validate(this);

            return ValidationResult.IsValid;
        }

        private void ValidarEstrelas()
        {
            RuleFor(e => e.Estrelas)
                .GreaterThan(0).WithMessage("Número de estrelas deve ser maior que zero")
                .LessThanOrEqualTo(5).WithMessage("Número de estrelas deve ser menor ou igual a cinco");
        }

        private void ValidarComentarios()
        {
            RuleFor(c => c.Comentario)
                .NotEmpty().WithMessage("Comentario nao pode ser vazio")
                .MaximumLength(100).WithMessage("Comentario pode ter no maximo 100 caracteres");
        }
    }
}
