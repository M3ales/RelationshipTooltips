using M3ales.RelationshipTooltips.Relationships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3ales.RelationshipTooltips.API
{
    public class RelationshipAPI : IRelationshipAPI
    {
        public event EventHandler<EventArgsRegisterRelationships> RegisterRelationships;
        internal List<IRelationship> FireRegistrationEvent()
        {
            EventArgsRegisterRelationships toRegister = new EventArgsRegisterRelationships();
            RegisterRelationships(null, toRegister);
            return toRegister.Relationships;
        }
    }
}