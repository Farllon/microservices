namespace Basket.API.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class IgnoreOnHashAttribute : Attribute { }