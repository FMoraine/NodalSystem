using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Game.TheoricalNodes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using NodalInteractiveCreator.Objects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools
{
    [NodeStyle("ActiveAction", PathCategory.ACTIONS, "Interactives")]
    public class ActiveActionNode : ActionNode
    {
        public SerializerObject<InteractiveObject> _interactiveObjects = new SerializerObject<InteractiveObject>();
        public string _tagAction;
        public int _idAction;
        public int _idCheck;

        public override void Initialize()
        {
            actNode.Receive = ActiveNode;
        }

        protected override void ActiveNode()
        {
            base.ActiveNode();

            switch (_tagAction)
            {
                case "Start":
                    _target.Subject.act_Start.Invoke();
                    break;

                case "Unblock":
                    _target.Subject.act_Start.Invoke();
                    break;

                case "Final":
                    _target.Subject.act_Final.Invoke();

                    break;

                case "Block":
                    _target.Subject.act_Final.Invoke();
                    break;

                case "Touch":
                    _target.Subject.act_Touch.Invoke();
                    break;

                case "Check":
                    _target.Subject.act_Check.Invoke(_idCheck);
                    break;
            }
        }
    }
}