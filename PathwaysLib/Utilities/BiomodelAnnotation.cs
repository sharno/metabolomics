using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace PathwaysLib.Utilities
{
    public class BiomodelAnnotation
    {
        public BiomodelAnnotation(Guid entityId, int qualifierId)
        {
            this.entityId = entityId;
            this.qualifierIds = new LinkedList<int>();
            qualifierIds.AddFirst(qualifierId);
        }

        public BiomodelAnnotation(Guid entityId)
        {
            this.entityId = entityId;
            this.qualifierIds = new LinkedList<int>();            
        }

        #region member variables
        Guid entityId = Guid.Empty;
        LinkedList<int> qualifierIds;
        #endregion

        #region public members
        public Guid EntityId
        {
            get
            {
                return entityId;
            }
            set
            {
                entityId = value;
            }
        }

        public LinkedList<int> QualifierIds
        {
            get
            {
                return qualifierIds;
            }
            set
            {
                qualifierIds = value;
            }
        }

        #endregion
    }
}
