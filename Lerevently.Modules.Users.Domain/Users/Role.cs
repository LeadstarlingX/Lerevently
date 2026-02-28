namespace Lerevently.Modules.Users.Domain.Users;

public sealed class Role
{
    public static readonly Role Administrator = new("Administrator");
    public static readonly Role Member = new("Member");

    private Role(string name)
    {
        Name = name;
    }
#pragma warning disable CS8618
    private Role()
    {
    }
#pragma warning restore CS8618

    public string Name { get; private set; }
}