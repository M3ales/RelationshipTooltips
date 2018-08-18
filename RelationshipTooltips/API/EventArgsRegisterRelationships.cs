using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelationshipTooltips.Relationships;

namespace RelationshipTooltips.API
{
    public class EventArgsRegisterRelationships : EventArgs
    {
        public EventArgsRegisterRelationships()
        {
            Relationships = new List<IRelationship>();
        }
        public Dictionary<int, int> Priorities
        {
            get
            {
                Dictionary<int, int> dups = new Dictionary<int, int>();
                foreach(IRelationship r in Relationships)
                {
                    if(dups.TryGetValue(r.Priority, out int v))
                    {
                        dups[r.Priority]++;
                    }
                    else
                    {
                        dups.Add(r.Priority, 1);
                    }
                }
                return dups;
            }
        }
        public List<IRelationship> Relationships { get; }
    }
}
