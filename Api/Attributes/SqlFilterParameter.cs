namespace Api.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class SqlFilterParameter : Attribute
{
    public SqlFilterParameter(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
