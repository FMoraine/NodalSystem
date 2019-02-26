using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal
{
    [Serializable]
    public abstract class NodeConnector
    {
        [SerializeField]
        protected string _guid;

        [SerializeField]
        protected List<NodalLayer> layers = new List<NodalLayer>() { };

        /// <summary>
        /// The main Layer, use when a connector as no layers defined
        /// </summary>
        public NodalLayer MainLayer
        {
            get { return _mainLayer; }
        }

        protected NodalLayer _mainLayer = new NodalLayer();

        public int LayerCount {
            get { return layers.Count; }
        }


        public void GenerateGUID()
        {

            guid = Guid.NewGuid().ToString();
        }

        public void ForceAffectGUID(string newGUID)
        {
            guid = newGUID; 
        }

        public string guid {
            get { return _guid; }
            private set { _guid = value; }
        }
          

        public void AddLayer()
        {
           layers.Add(new NodalLayer("", "", Color.clear));
        }   

        public void AddLayer(string content , string desc , Color c)
        {
            layers.Add(new NodalLayer(content, desc, c));
        }

        public void RemoveLayerAt(int index)
        {
           layers.RemoveAt(index);
        }

        public void SetStyleAt(int index, string content, string desc, Color c)
        {
            if (layers.Count > index)
            {
                layers[index].color = c;
                layers[index].content = content;
                layers[index].desc = desc;
            }
        }

        public static void SetStyleAt(NodalLayer layer, string content, string desc, Color c)
        {
            if(layer.color != c)
                layer.color = c;

            if(layer.content != content)
                layer.content = content;

            if (layer.desc != desc)
                layer.desc = desc;
        }

        public void ResetLayers()
        {
            layers = new List<NodalLayer>() { };
        }

        public NodalLayer GetLayerAt(int index)
        {
            return layers[index];
        }

        public static void HiddenLayer(NodalLayer layer, bool value)
        {
            if (layer.hide != value)
                layer.hide = value;
        }
    }

    [Serializable]
    public class NodeEmitter : NodeConnector
    {
        protected Action<NodeEmitter, NodalLayer> OnEmit;
        public object value;

        public virtual void Emit()
        {
            if (OnEmit != null)
                OnEmit.Invoke(this, MainLayer);
        }

        public virtual void Emit(int index)
        {

            if (index >= layers.Count)
            {
                Debug.LogError("Index " + index + " does not exist inside this emitter !");
                return;
            }

            if (OnEmit != null)
                OnEmit.Invoke(this, layers[index]);
            
        }

        public void SetListener(Action<NodeEmitter, NodalLayer> listener)
        {
            OnEmit = listener;
        }
    }

    [Serializable]
    public class NodeReceiver : NodeConnector
    {
        public Action Receive;

        public virtual void OnReceive(object val)
        {
            if (Receive != null)
                Receive.Invoke();
        }
    }

    [Serializable]
    public class NodeEmitter<T> : NodeEmitter
    {
        public virtual void Emit(T pVal)
        {
            value = pVal;
            Emit();

        }

        public virtual void Emit(T pVal, int layer)
        {
            value = pVal;
            Emit(layer);

        }
    }

    [Serializable]
    public class NodeReceiver<T> : NodeReceiver
    {
        public new Action<T> Receive;

        public override void OnReceive(object val)
        {

            if (Receive != null)
                Receive.Invoke((T)val);
        }
    }


}
