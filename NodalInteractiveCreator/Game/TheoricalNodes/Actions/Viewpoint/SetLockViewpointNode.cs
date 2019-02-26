using System;
using System.Collections;
using System.Collections.Generic;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Game.TheoricalNodes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using NodalInteractiveCreator.HUD;
using NodalInteractiveCreator.Objects;
using NodalInteractiveCreator.Tools;
using NodalInteractiveCreator.Viewpoints;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools
{
    [Serializable]
    [NodeStyle("SetLockViewpoint", PathCategory.ACTIONS, "Viewpoint")]
    public class SetLockViewpointNode : ActionNode
    {
        public SerializerObject<ViewPoint> _vp = new SerializerObject<ViewPoint>();
        public bool _value;

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void ActiveNode()
        {
            base.ActiveNode();

            if (_value)
                _vp.Subject.LockViewpoint();
            else
                _vp.Subject.UnlockViewpoint();
        }
    }
}