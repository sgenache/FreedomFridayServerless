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

        private List<JournalLineDTO> _lines;
		[DataMember]
        public List<JournalLineDTO> Lines 
        { 
            get 
            {
                if (_lines == null) _lines = new List<JournalLineDTO>();
                return _lines;
            }
            set{
                _lines = value;
            } 
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