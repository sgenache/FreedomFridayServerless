using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FreedomFridayServerless.Contracts
{
    [DataContract]
    public class TransactionDTO
    {
        [DataMember]
        public Guid Id { get; private set; }

		[DataMember]
        public List<TransactionLineDTO> Lines { get; set; }

        private TransactionDTO()
        {
            
        }

        public TransactionDTO(Guid id)
        {
            Id = id;
            Lines = new List<TransactionLineDTO>();
        }

        public TransactionDTO(Guid id, List<TransactionLineDTO> lines)
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