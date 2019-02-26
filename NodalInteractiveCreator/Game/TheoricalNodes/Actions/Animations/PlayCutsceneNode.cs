using System;
using System.Collections;
using System.Collections.Generic;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Game.TheoricalNodes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using NodalInteractiveCreator.Controllers;
using NodalInteractiveCreator.HUD;
using NodalInteractiveCreator.Managers;
using NodalInteractiveCreator.Objects;
using NodalInteractiveCreator.Tools;
using NodalInteractiveCreator.Viewpoints;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools
{
    [Serializable]
    [NodeStyle("PlayCutscene", PathCategory.ACTIONS, "Animation")]
    public class PlayCutsceneNode : ActionNode
    {
        public SerializerObject<Animator> _animator = new SerializerObject<Animator>();
        public int _idState = 0;
        public string _cutsceneName = null;

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void ActiveNode()
        {
            base.ActiveNode();

            GameManager.GetGameState()._myCutscene.CurrentCutscene = Cutscene.CUTSCENE.IN_GAME;
            Cutscene._myCutsceneName = _cutsceneName;
            Cutscene._cutsceneIsPlaying = true;
        }
    }
}