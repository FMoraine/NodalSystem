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
    [NodeStyle("SetValueStory", PathCategory.ACTIONS, "")]
    public class SetValueStoryNode : ActionNode
    {

        public SerializerObject<Machine> _machineStory = new SerializerObject<Machine>();
        public int _idStoryEvent;
        public int _idStoryAction;
        public bool _value;

        public override void Initialize()
        {
            base.Initialize();

            actNode.Receive = ActiveNode;
        }

        protected override void ActiveNode()
        {
            Machine.EVENTS myEvent = _machineStory.Subject.Events[_idStoryEvent]._event;
            string myAct = _machineStory.Subject.Events[_idStoryEvent]._listActions[_idStoryAction]._name;

            _machineStory.Subject.SetConditionValue(myEvent, myAct, _value);
        }
    }
}