using Aplicacao.DTOs.Carrapato;
using FluentValidation;

namespace Aplicacao.Validacoes
{
    public class CarrapatoCriarDtoValidator : AbstractValidator<CarrapatoCriarDto>
    {
        public CarrapatoCriarDtoValidator()
        {
            RuleFor(x => x.CodigoSerial)
                .NotEmpty().WithMessage("CodigoSerial é obrigatório.")
                .MaximumLength(100).WithMessage("CodigoSerial deve ter no máximo 100 caracteres.");

            RuleFor(x => x.IdPatio)
                .GreaterThan(0).WithMessage("IdPatio deve ser um número positivo.");
        }
    }
}

