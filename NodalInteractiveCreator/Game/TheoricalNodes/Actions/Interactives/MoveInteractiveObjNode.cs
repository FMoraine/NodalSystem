using System;
using System.Collections;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Game.TheoricalNodes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using NodalInteractiveCreator.Controllers;
using NodalInteractiveCreator.HUD;
using NodalInteractiveCreator.Objects;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools
{
    [Serializable]
    [NodeStyle("MoveInteractiveObj", PathCategory.ACTIONS, "Interactives")]
    public class MoveInteractiveObjNode : ActionNode
    {
        [NodeConnectStyle("R", ColorStyle.WHITE, "Active node")]
        public NodeReceiver<float> actNodeFloat;

        [NodalField]
        public SerializerObject<InteractiveObject> _test = new SerializerObject<InteractiveObject>();

        private float value;
        public override void Initialize()
        {
            _test.Subject.act_Progress += actNodeFloat.Receive;

            base.Initialize();
        }

        protected override void ActiveNode()
        {
            base.ActiveNode();

            if (_test.Subject is TranslatableObject)
            {
                TranslatableObject t = _test.Subject as TranslatableObject;
                //t.MoveObjectProcuration();

            }

        }
    }
}
