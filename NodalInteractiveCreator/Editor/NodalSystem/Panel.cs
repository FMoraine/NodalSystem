using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts;
using UnityEditor;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem
{
    public class Panel
    {
        public NodalInteractionSystem target;
        public SerializedObject objTarget;

        public virtual void Clear()
        {

        }

        public virtual void InitNecessities()
        {
             
        }
    }
}
