using System;
using System.Runtime.Serialization;

namespace FreedomFridayServerless.Contracts
{
    [DataContract]
    public class JournalLineDTO
    {
        [DataMember]
		public string AccountId { get; set; }
        [DataMember]
		public string AccountCode { get; set; }
        [DataMember]
        public string AccountName { get; set; }
        [DataMember]
		public decimal AmountDebit { get; set; }
		[DataMember]
		public decimal AmountCredit { get; set; }
		[DataMember]
		public string Description { get; set; }
    }
}