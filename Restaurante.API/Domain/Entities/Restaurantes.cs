using FluentValidation;
using FluentValidation.Results;
using Restaurante.API.Domain.Enums;
using Restaurante.API.Domain.ValueObjects;
using System.Collections.Generic;

namespace Restaurante.API.Domain.Entities
{
    public class Restaurantes : AbstractValidator<Restaurantes>
    {
        public Restaurantes(string nome, ECozinha cozinha)
        {
            Nome = nome;
            Cozinha = cozinha;
            Avaliacoes = new List<Avaliacao>();
        }

        public Restaurantes(string id, string nome, ECozinha cozinha)
        {
            Id = id;
            Nome = nome;
            Cozinha = cozinha;
            Avaliacoes = new List<Avaliacao>();
        }

        public string Id { get; private set; }
        public string Nome { get; private set; }
        public ECozinha Cozinha { get; private set; }
        public Endereco Endereco { get; private set; }
        public List<Avaliacao> Avaliacoes { get; private set; }

        public ValidationResult ValidationResult { get; set; }

        public void AtribuirEndereco(Endereco endereco)
        {
            Endereco = endereco;
        }

        public void InserirAvaliacao(Avaliacao avaliacao)
        {
            Avaliacoes.Add(avaliacao);
        }

        public bool Validar()
        {
            ValidarNome();
            ValidarEndereco();

            ValidationResult = Validate(this);

            return ValidationResult.IsValid;
        }

        public void ValidarNome()
        {
            RuleFor(r => r.Nome)
              .NotEmpty().WithMessage("Nome não pode ser vazio")
              .MaximumLength(30).WithMessage("Nome pode ter no maximo 30 caracteres");
        }

        public void ValidarEndereco()
        {
            if (Endereco.Validar())
            {
                return;
            }

            foreach (var erro in Endereco.ValidationResult.Errors)
            {
                ValidationResult.Errors.Add(erro);
            }
        }
    }
}
