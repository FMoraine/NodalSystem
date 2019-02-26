using System;
using System.Collections;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Game.TheoricalNodes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using NodalInteractiveCreator.HUD;
using NodalInteractiveCreator.Objects;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools
{
    [Serializable]
    [NodeStyle("Interactive", PathCategory.MONO, "")]
    public abstract class InteractiveNode<TMono> : MonoBehaviourNode<TMono> where TMono : InteractiveObject
    {
        [NodeConnectStyle("S", ColorStyle.GREEN, "Emit on Start position")]
        public NodeEmitter actStart;
        [NodeConnectStyle("F", ColorStyle.RED, "Emit on Final position")]
        public NodeEmitter actFinal;
        [NodeConnectStyle("C", ColorStyle.BLUE, "Emit on Check position")]
        public NodeEmitter actCheck;
        [NodeConnectStyle("P", ColorStyle.YELLOW, "Emit on Progress object")]
        public NodeEmitter<float> actProgress;
        [NodeConnectStyle("T", ColorStyle.CYAN, "Emit on Touch object")]
        public NodeEmitter actTouch;

        public SerializerObject<TMono> _source = new SerializerObject<TMono>();

        public override void Initialize()
        {
            base.Initialize();
            if (_source.Subject != null)
            {
                _source.Subject.act_Start += actStart.Emit;
                _source.Subject.act_Final += actFinal.Emit;
                _source.Subject.act_Progress += actProgress.Emit;
                _source.Subject.act_Touch += actTouch.Emit;

                for (int i = 0; i < actCheck.LayerCount; i++)
                    _source.Subject.act_Check += actCheck.Emit;
            }
        }

        public override void SetSubject(MonoBehaviour mono)
        {
            _source.Assign(mono as TMono);
        }
    }

    public class InteractiveNode : InteractiveNode<InteractiveObject>
    {
        public override void Initialize()
        {
            base.Initialize();
        }

        public override void SetSubject(MonoBehaviour mono)
        {
            base.SetSubject(mono);
        }
    }
}