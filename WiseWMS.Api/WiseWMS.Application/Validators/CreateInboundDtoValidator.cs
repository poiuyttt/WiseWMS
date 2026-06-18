using FluentValidation;
using WiseWMS.Application.DTOs;

namespace WiseWMS.Application.Validators;

public class CreateInboundDtoValidator : AbstractValidator<CreateInboundDto>
{
    public CreateInboundDtoValidator()
    {
        RuleFor(x => x.SupplierId)
            .GreaterThan(0).WithMessage("请选择供应商");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("入库明细不能为空");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId)
                .GreaterThan(0).WithMessage("请选择商品");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("数量必须大于0");

            item.RuleFor(i => i.UnitPrice)
                .GreaterThan(0).WithMessage("单价必须大于0");
        });
    }
}
