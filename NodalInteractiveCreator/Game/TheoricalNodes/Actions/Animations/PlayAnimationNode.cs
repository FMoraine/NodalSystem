using System;
using System.Collections;
using System.Collections.Generic;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Game.TheoricalNodes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using NodalInteractiveCreator.HUD;
using NodalInteractiveCreator.Managers;
using NodalInteractiveCreator.Objects;
using NodalInteractiveCreator.Tools;
using NodalInteractiveCreator.Viewpoints;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools
{
    [Serializable]
    [NodeStyle("PlayAnimation", PathCategory.ACTIONS, "Animation")]
    public class PlayAnimationNode : ActionNode
    {
        public SerializerObject<Animation> _animation = new SerializerObject<Animation>();
        public SerializerObject<AnimationClip> _clipAnime = new SerializerObject<AnimationClip>();
        public int _idAnim = 0;
        public bool _reverse;
        public bool _replay;
        public bool _lockInteractivity;
        public bool _lockPinch;

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void ActiveNode()
        {
            base.ActiveNode();
             
            if (_lockInteractivity)
                GameManager.GetInputsManager().LockInteractivity = true;
            if (_lockPinch)
                GameManager.GetInputsManager().LockPinch = true;

            if (_animation.Subject.isPlaying && _replay)
                _animation.Subject.Stop();

            if (_reverse)
            {
                _animation.Subject[_clipAnime.Subject.name].speed = -1;
                _animation.Subject[_clipAnime.Subject.name].normalizedTime = 1;
            }
            else
            {
                _animation.Subject[_clipAnime.Subject.name].speed = 1;
                _animation.Subject[_clipAnime.Subject.name].normalizedTime = 0;
            }

            _animation.Subject.Play(_clipAnime.Subject.name);
        }
    }
}