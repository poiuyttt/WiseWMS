using FluentValidation;
using WiseWMS.Application.DTOs;

namespace WiseWMS.Application.Validators;

public class CreateOutboundDtoValidator : AbstractValidator<CreateOutboundDto>
{
    public CreateOutboundDtoValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0).WithMessage("请选择客户");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("出库明细不能为空");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId)
                .GreaterThan(0).WithMessage("请选择商品");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("数量必须大于0");

            item.RuleFor(i => i.SalePrice)
                .GreaterThan(0).WithMessage("售价必须大于0");
        });
    }
}
