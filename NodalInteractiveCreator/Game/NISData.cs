using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts
{
    public partial class NodalInteractionSystem
    {
        public void AddNodeBahviour(INodal toAdd)
        {
            List<NodeEmitter> lemit = NodalTools.GetAllConnections<NodeEmitter>(toAdd, true);
            foreach (var emit in lemit)
                AddEmitter(emit);

            List<NodeReceiver> lreceiv = NodalTools.GetAllConnections<NodeReceiver>(toAdd, true);
            foreach (var receiv in lreceiv)
                AddReceiver(receiv);

            toAdd.AssignGM(this);
            nodes.Add(new NodalInfos() { Subject = toAdd });
        }

        public bool AsNode(INodal toTest)
        {
            foreach (var n in nodes)
            {
                if (n.Subject == toTest)
                    return true;
            }

            return false;
        }

        public EmitterLink GetEmitterLinkByGUID(string guid)
        {
            foreach (var rT in emitters)
            {
                if (rT.guid == guid)
                    return rT;
            }
            
            return new EmitterLink();
        }

        public bool AsEmitterGUID(string guid)
        {
            foreach (var rT in emitters)
            {
                if (rT.guid == guid)
                    return true;
            }

            return false;
        }

        public ReceiverLink GetReceiverLinkByGUID(string guid)
        {
           
            if (string.IsNullOrEmpty(guid))
            {
                Debug.LogError("No guid on this  receiver");
                return new ReceiverLink();
            }

            if (guidToReceivers.ContainsKey(guid))
            {

                return guidToReceivers[guid];
            }
            
            return new ReceiverLink();
        }

        void AddReceiver(NodeReceiver r)
        {
            if (guidToReceivers.ContainsKey(r.guid))
            {
              
                guidToReceivers[r.guid].receiver = r;
            }
            else
            {
                guidToReceivers.Add(r.guid, new ReceiverLink() { receiver = r });
            }
        }

        void AddEmitter(NodeEmitter r)
        {
            RegisterEmitter(r);
            foreach (var rT in emitters)
            {
                if (rT.guid == r.guid)
                    return;
            }
            emitters.Add(new EmitterLink() { guid = r.guid });
        }

        void RegisterEmitter(NodeEmitter r)
        {
            if (guidToEmitter.ContainsKey(r.guid))
                guidToEmitter[r.guid] = r;
            else
            {
                guidToEmitter.Add(r.guid , r);
            }

        }

        public void RemoveAllNodes()
        {
            nodes.Clear();
            emitters.Clear();
            guidToReceivers.Clear();
        }

        public void RemoveNode(INodal n)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].Subject == n)
                {
                    nodes.RemoveAt(i);
                    break;
                }
            }

            List<NodeEmitter> emitter = NodalTools.GetAllConnections<NodeEmitter>(n);
            foreach (var e in emitter)
                RemoveEmitterGUID(e.guid);

            List<NodeReceiver> receiver = NodalTools.GetAllConnections<NodeReceiver>(n);
         
            foreach (var r in receiver)
                RemoveReceiverGUID(r.guid);

        }

        public void RefreshEmitters()
        {
            for (var i = emitters.Count-1; i >=0 ; i--)
            {
                if (!GUIDEmitterExist(emitters[i].guid))
                    emitters.RemoveAt(i);
            }

            foreach (var n in nodes)
            {
                List<NodeEmitter> lemit = NodalTools.GetAllConnections<NodeEmitter>(n.Subject, false);
               
                foreach (var emit in lemit)
                    AddEmitter(emit);
            }
        }

        public void RefreshDynamicNodesSizes()
        {

        }
          
        bool GUIDEmitterExist(string guid)
        {
            foreach (var n in nodes)
            {
                List<NodeEmitter> lemit = NodalTools.GetAllConnections<NodeEmitter>(n.Subject, false);
                foreach (var emit in lemit)
                {
                     if (emit.guid == guid)
                         return true;
                }
                
            }

            return false;
        }

        public bool Connect(string guidEmit, string guidConnect, NodalLayer layerEmit , NodalLayer layerReceiv)
        {
            if (string.IsNullOrEmpty(guidConnect) || string.IsNullOrEmpty(guidEmit))
            {
                Debug.LogError("no guid on this connection : emitter " + guidEmit + " receiver :" + guidConnect);
                return false;
            }

            NodeEmitter e = guidToEmitter[guidEmit];
            EmitterLink link = GetEmitterLinkByGUID(guidEmit);
            ReceiverLink r = GetReceiverLinkByGUID(guidConnect);
            Type rT = r.receiver.GetType();
            Type eT = e.GetType();

            if (!eT.IsGenericType && !rT.IsGenericType)
            {
                return Connect(link, r, guidConnect, layerEmit);
            }

            if (eT.IsGenericType && rT.IsGenericType)
            {
                if(eT.GetGenericArguments()[0] == rT.GetGenericArguments()[0])
                    return Connect(link, r, guidConnect, layerEmit);
            }
           
            return false;
        }

        bool Connect(EmitterLink link, ReceiverLink r , string guidConnect, NodalLayer layer)
        {
            if (EmitterContainsReceiver(link, guidConnect, layer))
                return false;

            link.AddReceiver(guidConnect, layer);
            return true;
        }

        public bool Disconnect(string guidEmit, string guidConnect, NodalLayer layer)
        {
            EmitterLink e = GetEmitterLinkByGUID(guidEmit);

            e.RemoveReceiver(guidConnect, layer);
            return true;
        }

        public void RemoveEmitterGUID(string guid)
        {
            for (int i = 0; i < emitters.Count; i++)
            {
                if (emitters[i].guid == guid)
                {
                    emitters.RemoveAt(i);
                    return;
                }
                  
            }
           
        }

        public void RemoveReceiverGUID(string guid)
        {
            if(guidToReceivers.ContainsKey(guid))
                 guidToReceivers.Remove(guid);


            for (int i = 0; i < emitters.Count; i++)
            {
                emitters[i].RemoveReceiver(guid);
            }
        }

        bool EmitterContainsReceiver(EmitterLink emit, string guidReceiver, NodalLayer layer)
        {
            for (int i = 0; i < emit.linkedReceivers.Count; i++)
            {
                if (emit.linkedReceivers[i] == guidReceiver && emit.layers[i] == layer)
                    return true;
            }
         
            return false;
        }
    }
}
