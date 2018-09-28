using System;
using System.Runtime.Serialization;

namespace FreedomFridayServerless.Contracts
{
    [DataContract]
    public class TransactionLineDTO
    {
        private TransactionLineDTO()
        {
            
        }

        public TransactionLineDTO(string nominalSourceId, string accountCode, decimal amountDebit, decimal amountCredit)
        {
            AccountCode = accountCode;
            AmountDebit = amountDebit;
            AmountCredit = amountCredit;
        }

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