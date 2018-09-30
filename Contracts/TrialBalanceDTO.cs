using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FreedomFridayServerless.Contracts
{
    [DataContract]
    public class TrialBalanceDTO
    {
        private TrialBalanceDTO()
        {
            
        }

        public TrialBalanceDTO(List<TrialBalanceLineDTO> lines)
        {
            Lines = lines;
        }

		[DataMember]
        public List<TrialBalanceLineDTO> Lines { get; private set; }
    }
}