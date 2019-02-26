using UnityEngine;
using NodalInteractiveCreator.Database;
using NodalInteractiveCreator.Endoscop;
using NodalInteractiveCreator.HUD;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using System;

namespace NodalInteractiveCreator.Objects
{
    [RequireComponent(typeof(Collider))]
    public class ItemTarget : InteractiveObject
    {
        public static bool _isTrigged = false;

        public ItemDatabase _myItemData = null;
        public int ItemId = 0;
        public bool _blockEnabled = false;
        public bool _activeEndoscope = false;
        public bool _activeScrewdriver = false;

        [SerializeField]
        public EndoscopeEntrence _myEndoEntrance = null;

        protected override void Awake()
        {
            base.Awake();
            _isTrigged = false;

        }

        public override void DeselectObject()
        {
            base.DeselectObject();

            if (act_Touch != null)
                act_Touch.Invoke();
        }

        public void ExecuteTrigger()
        {
            if (null != Inventory.Inventory.Instance && !TranslatableObject._isBacked && !IsBlocked())
            {
                _isTrigged = true;

                if (GetComponent<AudioSource>() != null)
                    StartCoroutine(PlayAudioToValidate(_audioClipDefault));

                Inventory.Inventory.Instance.RemoveItem(ItemId);

                if (act_Start != null)
                    act_Start.Invoke();
            }
            else
                TranslatableObject._isBacked = false;
        }

        public void ExecuteEndoscope()
        {
            //if(!IsBlocked())
                _myEndoEntrance.OnEntrance();
        }

        public void ExecuteScrewdriver()
        {
            //if (!IsBlocked())
                Debug.Log("Screwdriver Action");
        }

        private bool IsBlocked()
        {
            if (_blockEnabled)
            {
                if (act_Final != null)
                    act_Final.Invoke();
                return true;
            }
            return false;
        }
    }
}
