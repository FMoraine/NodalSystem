using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using NodalInteractiveCreator.Objects;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Game.TheoricalNodes
{
    [Serializable]
    public abstract class MonoBehaviourNode : TheoricalNode 
    {
        public virtual void SetSubject(MonoBehaviour mono)
        {

        }
    }
    [Serializable]
    public abstract class MonoBehaviourNode<TMono> : MonoBehaviourNode where TMono : MonoBehaviour
    {
        public override void SetSubject(MonoBehaviour mono)
        {

        }
    }
}
        