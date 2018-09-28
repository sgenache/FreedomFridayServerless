using System;
using System.Runtime.Serialization;

namespace FreedomFridayServerless.Contracts
{
    [DataContract]
    public class AccountDTO
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsPnl { get; set; }

        [DataMember]
        public DateTime? UpdatedAt { get; set; }
    }
}