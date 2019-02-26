// Machinika Museum
// © Littlefield Studio
// Writted by Rémi Carreira - 2015 - 2016
//
// Edited by Franck-Olivier FILIN - 2017
//
// PickableObject.cs

using System.Collections;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using NodalInteractiveCreator.Database;
using UnityEngine;
#if UNITY_EDITOR

#endif

namespace NodalInteractiveCreator.Objects
{
    public class PickableObject : InteractiveObject
    {
        public ItemDatabase _myItemData = null;
        public int itemID = 0;
        private bool _isPicked = false;

        protected override void Awake()
        {
            base.Awake();
        }

        public override void SelectObject(Camera Camera, Vector3 InputPosition)
        {
            base.SelectObject(Camera, InputPosition);
            ActivePickUp();
        }

        public void ActivePickUp()
        {
            if (_isPicked)
                return;

            if (null != Inventory.Inventory.Instance)
            {
                ActiveMeshRenderer(false);

                Inventory.Inventory.Instance.AddItem(itemID);

                if(act_Start != null)
                    act_Start.Invoke();

                if (_objectAudio != null)
                    StartCoroutine(PlayAudioToValidate(_audioClipDefault));
                else
                {
                    enabled = false;
                    gameObject.SetActive(false);
                    ActiveMeshRenderer(true);
                }
            }
        }

        public void LoadValue(string newName, GameObject newMesh)
        {
            BoxCollider currentColl = this.GetComponent<BoxCollider>();

            this.name = newName;
            this.GetComponent<MeshFilter>().mesh = newMesh.GetComponent<MeshFilter>().sharedMesh;
            this.GetComponent<MeshRenderer>().materials = newMesh.GetComponent<MeshRenderer>().sharedMaterials;
            this.gameObject.AddComponent<BoxCollider>();
            DestroyImmediate(currentColl);
        }

        protected override IEnumerator PlayAudioToValidate(AudioClip audio)
        {
            yield return base.PlayAudioToValidate(audio);

            enabled = false;
            gameObject.SetActive(false);
            ActiveMeshRenderer(true);
        }

        private void ActiveMeshRenderer(bool value)
        {
            _objectCollider.enabled = value;
            _isPicked = !value;

            if (GetComponent<MeshRenderer>() != null)
            {
                _objectTransform.GetComponent<MeshRenderer>().enabled = value;

                if (_objectTransform.childCount > 0)
                {
                    foreach (SpriteRenderer spriteRend in _objectTransform.GetComponentsInChildren<SpriteRenderer>())
                        spriteRend.enabled = value;

                    foreach (MeshRenderer meshRend in _objectTransform.GetComponentsInChildren<MeshRenderer>())
                        meshRend.enabled = value;
                }
            }
        }
    }
}
