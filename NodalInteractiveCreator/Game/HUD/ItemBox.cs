using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodalInteractiveCreator.Inventory;
using NodalInteractiveCreator.Controllers;

namespace NodalInteractiveCreator.HUD
{
    public class ItemBox : MonoBehaviour
    {
        public SentenceTranslate _myTranslate;
        public int _idSentence;
        private bool _isEnter;

        private Vector3 _mousePosInit;

        public void OnMouseDown()
        {
            _mousePosInit = Input.mousePosition;
            _isEnter = true;
        }

        public void OnMouseDrag()
        {
            if (Vector3.Distance(Input.mousePosition, _mousePosInit) > 0.5f)
                _isEnter = false;
        }

        private void OnMouseUpAsButton()
        {
            if (_isEnter)
                OpenBox();
        }

        public void OpenBox()
        {
            if (!InfoBox._infoBoxEnabled && _isEnter)
            {
                if (InspectSystem.IsInspecting())
                    InfoBox._changeAnchor = true;

                _myTranslate.OpenBox(_idSentence);
            }
        }
    }
}