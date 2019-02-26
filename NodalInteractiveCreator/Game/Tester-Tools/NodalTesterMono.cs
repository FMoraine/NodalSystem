using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tester_Tools
{
    public class NodalTesterMono : NodalBehaviour
    {
        [SerializeField]
        protected NodeEmitter send;
        [SerializeField]
        protected NodeEmitter<Rect> sendInt;
        public NodeReceiver receive;


        void Awake()
        {
            receive.Receive += ReceiveOrder;

        }

        void ReceiveOrder()
        {  
            send.Emit();
            sendInt.Emit(new Rect(0 , 10 , 10 , 5));
        }

        void OnMouseDown()
        {
            Debug.Log("send");
            send.Emit();
            sendInt.Emit(new Rect(0, 10, 10, 5));
        }

       
    }
}
