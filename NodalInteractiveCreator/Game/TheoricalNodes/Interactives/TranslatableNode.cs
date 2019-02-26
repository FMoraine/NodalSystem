using System;
using System.Collections;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using NodalInteractiveCreator.HUD;
using NodalInteractiveCreator.Objects;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools
{
    [Serializable]
    [NodeStyle("Translatable", PathCategory.MONO, "")]
    public class TranslatableNode : InteractiveNode<TranslatableObject>
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