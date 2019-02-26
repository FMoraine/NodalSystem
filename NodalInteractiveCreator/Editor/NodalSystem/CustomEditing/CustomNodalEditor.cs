using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.CustomEditing
{
    
    public class CustomNodalEditor : Attribute
    {
        public Type subjectType;

        public CustomNodalEditor(Type pTypedCustom)
        {
            subjectType = pTypedCustom;
        }
    }
}
