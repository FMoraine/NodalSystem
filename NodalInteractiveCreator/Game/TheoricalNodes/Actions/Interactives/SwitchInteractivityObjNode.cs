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
    [NodeStyle("SwitchInteractivityObj", PathCategory.ACTIONS, "Interactives")]
    public class SwitchInteractivityObjNode : ActionNode
    {
        public int _idInteractiveObj1;
        public int _idInteractiveObj2;
        public bool _isBetween;

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void ActiveNode()
        {
            base.ActiveNode();

            if(_isBetween)
            {
                _target.Subject.GetComponents<InteractiveObject>()[_idInteractiveObj1].OnActiveScript = _target.Subject.GetComponents<InteractiveObject>()[_idInteractiveObj2].OnActiveScript;
                _target.Subject.GetComponents<InteractiveObject>()[_idInteractiveObj2].OnActiveScript = !_target.Subject.GetComponents<InteractiveObject>()[_idInteractiveObj1].OnActiveScript;
            }
            else
            {
                _target.Subject.GetComponents<InteractiveObject>()[_idInteractiveObj1].OnActiveScript = false;
                _target.Subject.GetComponents<InteractiveObject>()[_idInteractiveObj2].OnActiveScript = true;
            }

        }
    }
}