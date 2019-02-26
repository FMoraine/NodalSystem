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
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools
{
    [NodeStyle("SetBlockChecks", PathCategory.ACTIONS, "Interactives")]
    public class SetBlockCheckNode : ActionNode
    {
        public bool _allCheck;
        public List<bool> _values = new List<bool>();

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void ActiveNode()
        {
            base.ActiveNode();

            if (_target.Subject is TranslatableObject)
                foreach(Check c in ((TranslatableObject)_target.Subject).ListCheck)
                    c.Block = _values[((TranslatableObject)_target.Subject).ListCheck.IndexOf(c)];
        }
    }
}