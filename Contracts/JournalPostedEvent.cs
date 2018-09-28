using System;
using System.Runtime.Serialization;

namespace FreedomFridayServerless.Contracts
{
    [DataContract]
    public class JournalPostedEvent
    {
        [DataMember]
		public string EventId  { get; set; }

        [DataMember]
        public DateTime Date {get;set;}

        [DataMember]
		public string JournalId { get; set; }

        [DataMember]
		public string AccountId { get; set; }
        
        [DataMember]
		public decimal AmountDebit { get; set; }

		[DataMember]
		public decimal AmountCredit { get; set; }

        [DataMember]
        public string Description {get;set;}
    }
}