using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using UnityEngine;

#if UNITY_EDITOR
using NUnit.Framework.Interfaces;
#endif

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts
{
    public partial class NodalInteractionSystem 
    {
        //Test if this behaviour need to rebind is connections
        public bool isLinked { get; set; }

        void Awake()
        {
            RefreshLinks();
            InitializeLinks();
        }

        public void RefreshLinks()
        {
            guidToReceivers.Clear();
            guidToEmitter.Clear();

            foreach (var b in nodes)
            {
                if (b.Subject == null)
                {
                    Debug.LogError("no subject founded");
                    continue;
                }
                List<NodeReceiver> lreceiv = NodalTools.GetAllConnections<NodeReceiver>(b.Subject);

                foreach (var receiv in lreceiv)
                    AddReceiver(receiv);

                List<NodeEmitter> lEmit = NodalTools.GetAllConnections<NodeEmitter>(b.Subject);

                foreach (var emit in lEmit)
                    RegisterEmitter(emit);
            }

            foreach (var e in emitters)
            {
                for (int i = e.linkedReceivers.Count-1; i >= 0; i--)
                {
                    if(GetReceiverLinkByGUID(e.linkedReceivers[i]).receiver == null)
                        e.linkedReceivers.RemoveAt(i);
                }
            }

            isLinked = true;
        }

        void InitializeLinks()
        {
            foreach (var b in nodes) { 
                List<NodeEmitter> lEmit = NodalTools.GetAllConnections<NodeEmitter>(b.Subject);
                foreach (var emit in lEmit)
                    emit.SetListener(OnEmit);

                if (b.isTheorical)
                {
                    b.Subject.Initialize();

                    if(b.Subject is TheoricalNode)
                        ((TheoricalNode)b.Subject).AssignGM(this);
                }
                   
            }
        }

        public void OnBeforeSerialize()
        {
            foreach (var n in nodes)
            {
                if (n.isTheorical)
                    n.BeforeSerialize();
            }

         
        } 

        public void OnAfterDeserialize()
        {
            foreach (var n in nodes)
            {
                if (n.isTheorical)
                {
                    n.EndSerialize();
                }
            }
        }

    }
}
