namespace FreedomFridayServerless.Domain.Core
{
    public sealed class AccountDescriptor : ValueObject<AccountDescriptor>
    {
        public static AccountDescriptor As(string sourceId, string code, string name)
        {
            return new AccountDescriptor(sourceId, code, name);
        }

        public string SourceId { get; private set; }
        public string Code { get; private set; }
        public string Name { get; private set; }

        private AccountDescriptor(string sourceId, string code, string name)
        {
            SourceId = sourceId;
            Code = !string.IsNullOrEmpty(code) ? code.Trim() : code;
            Name = !string.IsNullOrEmpty(name) ? name.Trim() : name;
        }

		protected override bool EqualsCore(AccountDescriptor other)
		{
            return SourceId == other.SourceId;
        }

		protected override int GetHashCodeCore()
        {
            unchecked
            {
                return SourceId.GetHashCode();
            }
        }
    }
}