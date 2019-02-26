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
    [NodeStyle("GetValueStory", PathCategory.ACTIONS, "")]
    public class GetValueStoryNode : ActionNode
    {
        public SerializerObject<Machine> _machineStory = new SerializerObject<Machine>();
        public int _idStoryEvent = 0;
        public int _idStoryAction = 0;

        public override void Initialize()
        {
            base.Initialize();

            actNode.Receive = ActiveNode;
        }

        protected override void ActiveNode()
        {
            Machine.EVENTS myEvent = _machineStory.Subject.Events[_idStoryEvent]._event;
            string myAct = _machineStory.Subject.Events[_idStoryEvent]._listActions[_idStoryAction]._name;

            if (_machineStory.Subject.GetConditionValue(myEvent, myAct))
                actStart.Emit();
            else
                actFinal.Emit();
        }
    }
}
