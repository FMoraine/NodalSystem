using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts
{
    
    public partial class NodalInteractionSystem : GUIDBehaviour , ISerializationCallbackReceiver
    {
        [Versioning("nodesInfos")]
        public List<NodalInfos> nodes = new List<NodalInfos>();

        [Versioning("emitters infos")]
        public List<EmitterLink> emitters = new List<EmitterLink>();

        protected Dictionary<string , ReceiverLink> guidToReceivers = new Dictionary<string, ReceiverLink>();
        protected Dictionary<string, NodeEmitter> guidToEmitter = new Dictionary<string, NodeEmitter>();

        void OnEmit(NodeEmitter node, NodalLayer layer)
        {
            EmitterLink link = GetEmitterLinkByGUID(node.guid);

            for (int i = 0; i < link.linkedReceivers.Count; i++)
            {
                if (link.layers[i].layerID == layer.layerID)
                {
                    ReceiverLink r = GetReceiverLinkByGUID(link.linkedReceivers[i]);
                    r.receiver.OnReceive(node.value);
                }
            }
        }

        static T Cast<T>(object entity) where T : class
        {
            return entity as T;
        }

        public void StartTheoricalCoroutine(IEnumerator coroutine)
        {
            StartCoroutine(coroutine);
        }

        public bool IsAnyEmptyNodes()
        {
            for (int i = nodes.Count-1; i >= 0; i--)
            {
                if (nodes[i].IsEmpty())
                    return true;
            }

            return false;
        }

        public bool RemoveEmptyNodes()
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                if (nodes[i].IsEmpty())
                    RemoveNode(nodes[i].Subject);
            }

            RefreshEmitters();
            return false;
        }
    }
}
