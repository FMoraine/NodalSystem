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
    [NodeStyle("SetActiveBackToStart", PathCategory.ACTIONS, "Interactives")]
    public class SetActiveBackToStartNode : ActionNode
    {
        public bool _value;

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void ActiveNode()
        {
            base.ActiveNode();

            if (_target.Subject is RotatableObject)
                ((RotatableObject)_target.Subject)._backToStart = _value;
            else if (_target.Subject is TranslatableObject)
                ((TranslatableObject)_target.Subject)._backToStart = _value;
        }
    }
}