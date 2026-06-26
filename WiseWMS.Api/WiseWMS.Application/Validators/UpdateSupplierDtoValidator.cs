using FluentValidation;
using WiseWMS.Application.DTOs;

namespace WiseWMS.Application.Validators;

public class UpdateSupplierDtoValidator : AbstractValidator<UpdateSupplierDto>
{
    public UpdateSupplierDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("供应商名称不能为空")
            .MaximumLength(100).WithMessage("名称不能超过100个字符");

        RuleFor(x => x.Contact)
            .MaximumLength(50).WithMessage("联系人不能超过50个字符");

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("电话不能超过20个字符");

        RuleFor(x => x.Address)
            .MaximumLength(200).WithMessage("地址不能超过200个字符");
    }
}
