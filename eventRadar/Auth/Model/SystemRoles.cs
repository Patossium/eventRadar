namespace eventRadar.Auth.Model
{
    public static class SystemRoles
    {
        public const string Administrator = nameof(Administrator);
        public const string SystemUser = nameof(SystemUser);

        public static readonly IReadOnlyCollection<string> All = new[] { Administrator, SystemUser };
    }
}
