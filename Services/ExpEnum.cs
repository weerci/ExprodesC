using Func.Meta;

namespace ExprodesC.Services;

/// <inheritdoc/>
internal record ExpEnum : FuncEnum
{
    /// <summary>
    /// Если генотип загружен из файла - true
    /// </summary>
    public static readonly ExpEnum FromFile = new(1);

    protected ExpEnum(int internalValue) : base(internalValue) { }
}
