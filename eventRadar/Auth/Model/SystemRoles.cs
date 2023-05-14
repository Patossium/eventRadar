using System.Diagnostics.CodeAnalysis;

namespace eventRadar.Auth.Model
{
    [ExcludeFromCodeCoverage]
    public static class SystemRoles
    {
        public const string Administrator = nameof(Administrator);
        public const string SystemUser = nameof(SystemUser);

        public static readonly IReadOnlyCollection<string> All = new[] { Administrator, SystemUser };
    }
}
