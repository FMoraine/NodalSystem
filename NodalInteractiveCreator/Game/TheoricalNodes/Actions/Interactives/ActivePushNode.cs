using System;
using System.Collections;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Game.TheoricalNodes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using NodalInteractiveCreator.HUD;
using NodalInteractiveCreator.Objects;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools
{
    [NodeStyle("ActivePush", PathCategory.ACTIONS, "Interactives")]
    public class ActivePushNode : ActionNode
    {
        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void ActiveNode()
        {
            base.ActiveNode();
            ((PushObject)_target.Subject).ActivePush();
        }
    }
}