using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using NodalInteractiveCreator.Objects;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Game.TheoricalNodes
{
    [Serializable]
    public abstract class ActionNode : TheoricalNode
    {
        [NodeConnectStyle("R", ColorStyle.WHITE, "Active node")]
        public NodeReceiver actNode;

        [NodeConnectStyle("E", ColorStyle.MAGENTA, "Emit on Active node")]
        public NodeEmitter actOnActive;
        [NodeConnectStyle("S", ColorStyle.GREEN, "Emit on Start position target")]
        public NodeEmitter actStart;
        [NodeConnectStyle("F", ColorStyle.RED, "Emit on Final position target")]
        public NodeEmitter actFinal;
        [NodeConnectStyle("C", ColorStyle.BLUE, "Emit on Check position target")]
        public NodeEmitter actCheck;
        [NodeConnectStyle("P", ColorStyle.YELLOW, "Emit on Progress target")]
        public NodeEmitter<float> actProgress;
        [NodeConnectStyle("T", ColorStyle.CYAN, "Emit on Touch target")]
        public NodeEmitter actTouch;

        public SerializerObject<InteractiveObject> _target = new SerializerObject<InteractiveObject>();

        public override void Initialize()
        {
            base.Initialize();

            actNode.Receive = ActiveNode;

            if (_target.Subject != null)
            {
                _target.Subject.act_Start += actStart.Emit;
                _target.Subject.act_Final += actFinal.Emit;
                _target.Subject.act_Touch += actTouch.Emit;
                _target.Subject.act_Progress += actProgress.Emit;

                for(int i =0; i < actCheck.LayerCount; i++)
                    _target.Subject.act_Check += actCheck.Emit;
            }
        }

        protected virtual void ActiveNode()
        {
            actOnActive.Emit();
        }
    }
}
