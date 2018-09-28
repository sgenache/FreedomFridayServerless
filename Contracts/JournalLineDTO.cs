using System;
using System.Runtime.Serialization;

namespace FreedomFridayServerless.Contracts
{
    [DataContract]
    public class JournalLineDTO
    {
        private JournalLineDTO()
        {
            
        }

        public JournalLineDTO(Guid accountId, string accountCode, decimal amountDebit, decimal amountCredit)
        {
            AccountId = accountId;
            AccountCode = accountCode;
            AmountDebit = amountDebit;
            AmountCredit = amountCredit;
        }

        [DataMember]
		public Guid AccountId { get; private set; }
        [DataMember]
		public string AccountCode { get; private set; }
        [DataMember]
        public string AccountName { get; set; }
        [DataMember]
		public decimal AmountDebit { get; private set; }
		[DataMember]
		public decimal AmountCredit { get; private set; }
		[DataMember]
		public string Description { get; set; }
    }
}