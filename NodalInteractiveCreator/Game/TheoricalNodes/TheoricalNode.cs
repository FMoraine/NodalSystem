using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools
{
    [Serializable]
    public class TheoricalNode : INodal
    {
        protected NodalInteractionSystem nic;

        public string GetName()
        {
            object[] o = this.GetType().GetCustomAttributes(typeof(NodeStyleAttribute), true);
            for (int i = 0; i < o.Length; i++)
            {
                return ((NodeStyleAttribute) o[i]).name;
            }
            return this.GetType().Name;
        }

        public NodalInteractionSystem GetNic()
        {
            return nic;
        }

        public void AssignGM(NodalInteractionSystem nic)
        {
            this.nic = nic;
        }
        
        public virtual void Initialize()
        {

        }
    }

    public interface INodal
    {
        string GetName();
        void Initialize();
        NodalInteractionSystem GetNic();
        void AssignGM(NodalInteractionSystem nic);
    }
}
