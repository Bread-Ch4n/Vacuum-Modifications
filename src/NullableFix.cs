namespace System.Runtime.CompilerServices;

[AttributeUsage(
    AttributeTargets.Class
        | AttributeTargets.Event
        | AttributeTargets.Field
        | AttributeTargets.GenericParameter
        | AttributeTargets.Parameter
        | AttributeTargets.Property
        | AttributeTargets.ReturnValue,
    Inherited = false
)]
internal sealed class NullableAttribute : Attribute
{
    public readonly byte[] NullableFlags;

    public NullableAttribute(byte P_0) => NullableFlags = new byte[1] { P_0 };

    public NullableAttribute(byte[] P_0) => NullableFlags = P_0;
}

// You will likely need this one too, as it pairs with NullableAttribute
[AttributeUsage(
    AttributeTargets.Class
        | AttributeTargets.Struct
        | AttributeTargets.Method
        | AttributeTargets.Interface
        | AttributeTargets.Delegate,
    Inherited = false
)]
internal sealed class NullableContextAttribute : Attribute
{
    public readonly byte Flag;

    public NullableContextAttribute(byte P_0) => Flag = P_0;
}
