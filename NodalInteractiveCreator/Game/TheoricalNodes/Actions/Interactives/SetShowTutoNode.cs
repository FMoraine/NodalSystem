using System;
using System.Collections;
using System.Collections.Generic;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Game.TheoricalNodes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using NodalInteractiveCreator.HUD;
using NodalInteractiveCreator.Objects;
using NodalInteractiveCreator.Tools;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools
{
    [NodeStyle("SetShowTuto", PathCategory.ACTIONS, "Interactives")]
    public class SetShowTutoNode : ActionNode
    {
        public bool _showTuto;
        public Tuto.TUTO_STEP _step;

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void ActiveNode()
        {
            base.ActiveNode();

            Tuto mytuto = HUDManager.GetTuto();

            if (_showTuto)
            {
                if (!mytuto.gameObject.activeSelf)
                    mytuto.gameObject.SetActive(true);

                mytuto.StartCoroutine(mytuto.TutoStep(_step));
            }
            else
            {
                mytuto.UnshowPanel();
            }
        }
    }
}