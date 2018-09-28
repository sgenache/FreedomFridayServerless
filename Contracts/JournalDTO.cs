using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FreedomFridayServerless.Contracts
{
    [DataContract]
    public class JournalDTO
    {
        [DataMember]
        public string Id { get; set; }

		[DataMember]
        public List<JournalLineDTO> Lines { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public string Reference { get; set; }
        
        [DataMember]
        public string Number { get; set; }
        
        [DataMember]
        public DateTime? UpdatedAt { get; set; }
    }
}