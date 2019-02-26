using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal
{
    [Serializable]
    public class NodalLink 
    {
        public string guid;

        public static implicit operator string(NodalLink n)
        {
            return n.guid;
        }
    }

    [Serializable]
    public class ReceiverLink : NodalLink
    {
        public NodeReceiver receiver;
    }

    [Serializable]
    public class EmitterLink : NodalLink
    {
        public List<string> linkedReceivers = new List<string>();
        
        [SerializeField]
        public List<NodalLayer> layers = new List<NodalLayer>();

        public void AddReceiver(string guid, NodalLayer layer)
        {
            linkedReceivers.Add(guid);
            layers.Add(layer);
        }

        public void RemoveReceiver(string guid, NodalLayer layer , bool useLayer = true)
        {
            for (int i = 0; i < linkedReceivers.Count ; i+= 0)
            {
                if (linkedReceivers[i] == guid && (layers[i] == layer || !useLayer))
                {
                    linkedReceivers.RemoveAt(i);
                    layers.RemoveAt(i);
                    
                }
                else
                {
                    i++;
                }
            }
        }

        public void RemoveReceiver(string guid)
        {
            for (int i = 0; i < linkedReceivers.Count; i += 0)
            {
                if (linkedReceivers[i] == guid)
                {
                    linkedReceivers.RemoveAt(i);
                    layers.RemoveAt(i);

                }
                else
                {
                    i++;
                }
            }
        }
    }

    [Serializable]
    public class NodalLayer
    {
        [HideInInspector] public string layerID = "";
        [HideInInspector]
        public string content;
        [HideInInspector]
        public string desc;
        [HideInInspector]
        public Color color;
        [HideInInspector]
        public bool hide;

        public NodalLayer()
        {
        }

        public NodalLayer(string content, string desc, Color c)
        {
            #if UNITY_EDITOR
            layerID = GUID.Generate().ToString();
            #endif
            this.content = content;
            this.desc = desc;
            color = c;
        }

        public void ResetStyle()
        {
            color = Color.clear;
            content = "";
            desc = "";
        }

        public bool Equals(NodalLayer other)
        {
            return layerID.Equals(other.layerID);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is NodalLayer && Equals((NodalLayer) obj);   
        }

        public override int GetHashCode()
        {
            return layerID.GetHashCode();
        }

        public static bool operator ==(NodalLayer a, NodalLayer b)
        {
            return a.layerID == b.layerID;
        }

        public static bool operator !=(NodalLayer a, NodalLayer b)
        {
            return a.layerID != b.layerID;
        }
    }
}
