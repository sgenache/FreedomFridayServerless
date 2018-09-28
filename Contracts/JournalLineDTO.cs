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

        public JournalPostedEvent ToPostedEvent(string journalId, DateTime date)
        {
            return new JournalPostedEvent
            {
                EventId = Guid.NewGuid().ToString(),
                Date = date,
                JournalId = journalId,
                AccountId = this.AccountId,
                AmountCredit = this.AmountCredit,
                AmountDebit = this.AmountDebit, 
                Description = this.Description               
            };
        }
    }
}