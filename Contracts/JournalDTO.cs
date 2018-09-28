using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FreedomFridayServerless.Contracts
{
    [DataContract]
    public class JournalDTO
    {
        [DataMember]
        public Guid Id { get; private set; }

		[DataMember]
        public List<JournalLineDTO> Lines { get; set; }

        private JournalDTO()
        {
            
        }

        public JournalDTO(Guid id)
        {
            Id = id;
            Lines = new List<JournalLineDTO>();
        }

        public JournalDTO(Guid id, List<JournalLineDTO> lines)
        {
            Id = id;
            Lines = lines;
        }

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