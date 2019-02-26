using System;
using System.Collections;
using System.Collections.Generic;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using NodalInteractiveCreator.HUD;
using NodalInteractiveCreator.Objects;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools
{
    [Serializable]
    [NodeStyle("Rotatable", PathCategory.MONO, "")]
    public class RotatableNode : InteractiveNode<RotatableObject>
    {
        public override void Initialize()
        {
            base.Initialize();
        }

        public override void SetSubject(MonoBehaviour mono)
        {
            base.SetSubject(mono);
        }
    }
}