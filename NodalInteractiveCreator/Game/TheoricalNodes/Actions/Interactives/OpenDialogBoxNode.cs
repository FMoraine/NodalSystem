using System;
using System.Collections;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Game.TheoricalNodes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using NodalInteractiveCreator.Controllers;
using NodalInteractiveCreator.HUD;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools
{
    [Serializable]
    [NodeStyle("OpenDialogBox", PathCategory.ACTIONS, "Interactives")]
    public class OpenDialogBoxNode : ActionNode
    {
        public SerializerObject<SentenceTranslate> _sentenceTranslate = new SerializerObject<SentenceTranslate>();
        public int _idSentence = 0;
        public bool _oneShot = false;

        private bool res = false;

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void ActiveNode()
        {
            base.ActiveNode();
            if (res)
                return;

            _sentenceTranslate.Subject.OpenBox(_idSentence);
            actStart.Emit();
            nic.StartCoroutine(WaitToClose());

            res = _oneShot;
        }

        private IEnumerator WaitToClose()
        {
            yield return new WaitUntil(() => !InfoBox._infoBoxEnabled);
            actFinal.Emit();
        }
    }
}
