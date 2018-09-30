using System.Runtime.Serialization;

namespace FreedomFridayServerless.Contracts
{
    [DataContract]
    public class TrialBalanceLineDTO
    {
        [DataMember()]
        public string AccountId { get; private set; }

        [DataMember]
        public string AccountCode { get; private set; }

		[DataMember]
		public string AccountName { get; private set; }

		[DataMember]
		public decimal Balance { get; private set; }

		[DataMember]
		public bool IsPnl { get; private set; }
    }
}