using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M3ales.RelationshipTooltips.Relationships;

namespace M3ales.RelationshipTooltips.API
{
    public interface IRelationshipAPI
    {
        event EventHandler<EventArgsRegisterRelationships> RegisterRelationships;
    }
}
