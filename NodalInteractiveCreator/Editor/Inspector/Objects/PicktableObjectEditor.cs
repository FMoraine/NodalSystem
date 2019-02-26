// Machinika Museum
// © Littlefield Studio
//
// Writted by Franck-Olivier FILIN - 2017
//
// PicktableObjectEditor.cs

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using NodalInteractiveCreator.Objects;
using NodalInteractiveCreator.Database;

namespace NodalInteractiveCreator.Editor.Objects
{
    [CustomEditor(typeof(PickableObject))]
    [CanEditMultipleObjects]
    public class PicktableObjectEditor : TouchableElementEditor
    {
        //Value to set
        private bool _setValue;
        private string _nameObj;
        private bool _printableObj;
        private int _nbPrintableMax;
        private Sprite _iconeInvObj;
        private GameObject _goGameObj;
        private GameObject _goInvObj;

        //Others
        PickableObject _component;

        private void OnEnable()
        {
            _component = target as PickableObject;
            _component._myItemData = FindObjectOfType<Inventory.Inventory>().ItemDatabase;
        }

        #region Inspector
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ShowOnInspectorInitItemDatabase();

            EditorGUILayout.Space();

            ShowOnInspectorAudio();
        }

        private void ShowOnInspectorInitItemDatabase()
        {
            if (_component._myItemData == null)
                _component._myItemData = EditorGUILayout.ObjectField("Link to ItemDatabase", _component._myItemData, typeof(ItemDatabase), false) as ItemDatabase;
            else
            {
                EditorGUI.BeginDisabledGroup(_component._myItemData != null);
                _component._myItemData = EditorGUILayout.ObjectField("Link to ItemDatabase", _component._myItemData, typeof(ItemDatabase), false) as ItemDatabase;
                EditorGUI.EndDisabledGroup();
            }

            if (null != _component && null != _component._myItemData)
            {
                if (_component._myItemData._items.Count > 0)
                    DrawInspectorPickableObj();
                else
                    Debug.Log("This Database is empty..");
            }
        }

        private void DrawInspectorPickableObj()
        {
            EditorGUILayout.LabelField("Picktable :", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            if (EditorGUILayout.ToggleLeft("Set new value", _setValue))
            {
                ShowOnInspectorAddItemDatabase();
            }
            else
            {
                ShowOnInspectorLoadItemDatabase();
            }

            EditorGUI.indentLevel--;
        }


        private void ShowOnInspectorAddItemDatabase()
        {
            EditorGUI.indentLevel++;

            _nameObj = _nameObj != null ? _nameObj : _component.name;
            _nameObj = EditorGUILayout.TextField("Name", _nameObj);

            _printableObj = EditorGUILayout.Toggle("Printable ?", _printableObj);
            if (_printableObj)
            {
                EditorGUI.indentLevel++;
                _nbPrintableMax = EditorGUILayout.IntSlider("Number Max", _nbPrintableMax, 2, 4);
                EditorGUI.indentLevel--;
            }
            else
                _nbPrintableMax = 1;

            _iconeInvObj = EditorGUILayout.ObjectField("Icone Inventory", _iconeInvObj, typeof(Sprite), false) as Sprite;

            _goGameObj = _goGameObj != null ? _goGameObj : null;
            _goGameObj = _component.gameObject.GetComponent<MeshFilter>().sharedMesh != null ? _component.gameObject : null;
            _goGameObj = EditorGUILayout.ObjectField("GameObject In Game", _goGameObj, typeof(GameObject), true) as GameObject;

            _goInvObj = _goInvObj != null ? _goInvObj : null;
            _goInvObj = _component.gameObject.GetComponent<MeshFilter>().sharedMesh != null ? _component.gameObject : null;
            _goInvObj = EditorGUILayout.ObjectField("GameObject In Inventory", _goInvObj, typeof(GameObject), true) as GameObject;

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            if (GUILayout.Button("Create new Item !"))
                if (_component._myItemData != null && _iconeInvObj != null && (_goGameObj != null || _goInvObj != null))
                    _component._myItemData.CreateItem(_nameObj, _printableObj, _nbPrintableMax, _iconeInvObj, _goGameObj, _goInvObj);
                else
                    EditorUtility.DisplayDialog("Create New Item Error", "Failed to create a new item because you don't completed correctly the infos !", "Ok");
        }

        private void ShowOnInspectorLoadItemDatabase()
        {
            EditorGUI.indentLevel++;

            if (_component._myItemData != null)
                _component.itemID = EditorGUILayout.Popup("Item Inventory ", _component.itemID, _component._myItemData.GetAllItemsName().ToArray());

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            if (GUILayout.Button("Load this Item !"))
                if (_component._myItemData != null)
                {
                    _component.LoadValue(_component._myItemData._items[_component.itemID]._name, _component._myItemData._items[_component.itemID]._objectInGame);
                }
                else
                    EditorUtility.DisplayDialog("Load Item Error", "You don't selected item !", "Ok");
        }
        #endregion

        #region Create
        [MenuItem("Machinika/Create/Object/Inventory")]
        static void InitCreatePickable()
        {
            //        _isCreate = true;
            GameObject objSeleted = Selection.activeGameObject;
            GameObject newObj = CreatePickable(objSeleted);

            if (null != newObj)
            {
                Selection.activeGameObject = newObj;
            }
        }

        private static GameObject CreatePickable(GameObject parent)
        {
            GameObject newObjInventory = new GameObject();

            if (null != newObjInventory)
            {
                if (parent) newObjInventory.transform.SetParent(parent.transform);
                newObjInventory.tag = Tags.InteractiveObject;
                newObjInventory.name = "New_Item_Inventory";
                newObjInventory.transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));

                MeshFilter meshFilter = newObjInventory.AddComponent<MeshFilter>();
                if (null == meshFilter)
                {
                    EditorUtility.DisplayDialog("Create Inventory Object Error", "Failed to add Mesh Filter in Inventory object", "Ok");
                    Destroy(newObjInventory);
                    return null;
                }

                MeshRenderer meshRenderer = newObjInventory.AddComponent<MeshRenderer>();
                if (null != meshRenderer)
                {
                    meshRenderer.enabled = true;
                }
                else
                {
                    EditorUtility.DisplayDialog("Create Inventory Object Error", "Failed to add Mesh Renderer in Inventory object", "Ok");
                    Destroy(newObjInventory);
                    return null;
                }

                BoxCollider boxCollider = newObjInventory.AddComponent<BoxCollider>();
                if (null != boxCollider)
                {
                    boxCollider.isTrigger = true;
                }
                else
                {
                    EditorUtility.DisplayDialog("Create Inventory Object Error", "Failed to add box collider in Inventory object", "Ok");
                    Destroy(newObjInventory);
                    return null;
                }

                PickableObject pickableObjScript = newObjInventory.AddComponent<PickableObject>();
                if (null != pickableObjScript)
                {
                    //Initialization if you need 
                }
                else
                {
                    EditorUtility.DisplayDialog("Create Inventory Object Error", "Failed to add 'Pickable Script' in Inventory object", "Ok");
                    Destroy(newObjInventory);
                    return null;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Create Inventory Object Error", "Create Inventory Object failed", "Ok");
            }
            return newObjInventory;
        }


        #endregion

        #region Change
        [MenuItem("Machinika/Change/To Inventory Object")]
        static void InitChangeToPickable()
        {
            List<GameObject> objSeleted = new List<GameObject>();

            if (Selection.gameObjects.Length > 0)
                for (int i = 0; i < Selection.gameObjects.Length; i++)
                    objSeleted.Add(Selection.gameObjects[i]);
            else
                objSeleted = null;

            bool option;

            if (null != objSeleted)
            {
                option = EditorUtility.DisplayDialog("Change to Inventory Object ?",
                    "WARNING ! Some your components can to be deleted ! \nAre you sure to change this object ?",
                    "Yes",
                    "No");
            }
            else
            {
                EditorUtility.DisplayDialog("Change to Inventory Object Error", "Failed because you don't selected a object to change.", "Ok");
                option = false;
            }

            switch (option)
            {
                // With linking
                case true:
                    foreach (GameObject go in objSeleted)
                        ChangeToPickable(go);
                    break;

                // Cancel.
                case false:
                    Debug.Log("The 'Inventory Object' creation has been canceled.");
                    break;

                default:
                    Debug.LogError("Inventory Object Error.");
                    break;
            }
        }

        private static GameObject ChangeToPickable(GameObject objToChange)
        {
            if (null != objToChange)
            {
                objToChange.tag = Tags.InteractiveObject;
                objToChange.name += "_Inventory";

                if (null == objToChange.GetComponent<MeshFilter>())
                    objToChange.AddComponent<MeshFilter>();

                if (null == objToChange.GetComponent<MeshRenderer>())
                    objToChange.AddComponent<MeshRenderer>();

                if (null != objToChange.GetComponent<TouchableElement>())
                {
                    DestroyImmediate(objToChange.GetComponent<TouchableElement>());
                    DestroyImmediate(objToChange.GetComponent<Collider>());
                }

                if (null == objToChange.GetComponent<BoxCollider>())
                    objToChange.AddComponent<BoxCollider>(); objToChange.GetComponent<BoxCollider>().isTrigger = true;

                if (null == objToChange.GetComponent<PickableObject>())
                    objToChange.AddComponent<PickableObject>();

                if (null == objToChange.GetComponent<AudioSource>())
                    objToChange.AddComponent<AudioSource>();
            }
            else
                EditorUtility.DisplayDialog("Update Inventory Object Error", "Update Inventory Object failed", "Ok");

            return objToChange;
        }
        #endregion

    }
#endif
}