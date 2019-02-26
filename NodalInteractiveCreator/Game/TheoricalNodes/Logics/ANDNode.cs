using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools
{
    [Serializable]
    [NodeStyle("AND", PathCategory.LOGICS, "")]
    public class ANDNode : TheoricalNode
    {
        [NodeConnectStyle("A", ColorStyle.WHITE, "A")]
        public NodeReceiver A;
        [NodeConnectStyle("B", ColorStyle.WHITE, "B")]
        public NodeReceiver B;

        [NodeConnectStyle("", ColorStyle.GREEN, "Called when 2 inputs are fired")]
        public NodeEmitter positive;
        [NodeConnectStyle("", ColorStyle.RED, "Called when 1 or more inputs are not fired")]
        public NodeEmitter negative;

        public bool aTester = false;
        public bool bTester = false;

        public override void Initialize()
        {
            base.Initialize();
            A.Receive = OnReceiveA;
            B.Receive = OnReceiveB;
        }

        void OnReceiveA()
        {
            aTester = true;
            Emit();
        }

        void OnReceiveB()
        {
            bTester = true;
            Emit();
        }

        void Emit()
        {
            if (bTester && aTester)
            {
                bTester = aTester = false;
                positive.Emit();
            }
            else
            {
                negative.Emit();
            }
        }
    }
}
