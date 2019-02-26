using System;
using System.Collections;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.TheoricalNodes
{
    [Serializable]
    [NodeStyle("Delay", PathCategory.LOGICS, "")]
    public class DelayNode : TheoricalNode
    {
        [NodeConnectStyle("", ColorStyle.GREEN, "")]
        public NodeReceiver B;
        
        [NodeConnectStyle("", ColorStyle.GREEN, "Called after a certain amount of time")]
        public NodeEmitter delayEmit;

        [NodeConnectStyle("", ColorStyle.RED, "Called after a certain amount of time")]
        public NodeEmitter delayEmit2;

        [NodalField]
        public float delayAmount = 1f;

        [NodalField] public SerializerObject<GameObject> toPush;

        public override void Initialize()
        {
            base.Initialize();
            B.Receive = OnReceive;
        }

        void OnReceive()
        {
            nic.StartTheoricalCoroutine(Delay(delayAmount));
        }

        IEnumerator Delay(float amount)
        {
            yield return new WaitForSeconds(amount);
            delayEmit.Emit();
            
            if(toPush.Subject)
                GameObject.Instantiate(toPush.Subject);
        }
    }
}
