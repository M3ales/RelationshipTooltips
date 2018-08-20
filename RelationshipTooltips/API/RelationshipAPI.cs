using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelationshipTooltips.Relationships;

namespace RelationshipTooltips.API
{
    public class RelationshipAPI : IRelationshipAPI
    {
        public event EventHandler<EventArgsRegisterRelationships> RegisterRelationships;
        public event EventHandler<EventArgsRegisterRelationships> OnRegisterRelationshipsComplete;
        internal List<IRelationship> FireRegistrationEvent()
        {
            EventArgsRegisterRelationships toRegister = new EventArgsRegisterRelationships();
            RegisterRelationships?.Invoke(null, toRegister);
            OnRegisterRelationshipsComplete?.Invoke(null, toRegister);
            return toRegister.Relationships;
        }
    }
}