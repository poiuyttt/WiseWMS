using FluentValidation;
using WiseWMS.Application.DTOs;

namespace WiseWMS.Application.Validators;

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("商品名称不能为空")
            .MaximumLength(100).WithMessage("商品名称不能超过100个字符");

        RuleFor(x => x.Spec)
            .NotEmpty().WithMessage("规格不能为空")
            .MaximumLength(100).WithMessage("规格不能超过100个字符");

        RuleFor(x => x.Unit)
            .NotEmpty().WithMessage("单位不能为空")
            .MaximumLength(10).WithMessage("单位不能超过10个字符");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("请选择分类");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("价格必须大于0")
            .LessThanOrEqualTo(999999).WithMessage("价格不能超过999999");

        RuleFor(x => x.MinStock)
            .GreaterThanOrEqualTo(0).WithMessage("预警库存不能小于0");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("描述不能超过500个字符");
    }
}
