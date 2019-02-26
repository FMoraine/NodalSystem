// Machinika Museum
// © Littlefield Studio
//
// Writted by Franck-Olivier FILIN - 2017
//
// ItemTriggerEditor.cs

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using NodalInteractiveCreator.Objects;
using NodalInteractiveCreator.Database;
using NodalInteractiveCreator.Endoscop;

namespace NodalInteractiveCreator.Editor.Objects
{
    [CustomEditor(typeof(ItemTarget))]
    [CanEditMultipleObjects]
    public class ItemTriggerEditor : TouchableElementEditor
    {
        public static readonly int ENDOSCOPE_ID = -2;
        ItemTarget _component;

        private void OnEnable()
        {
            _component = target as ItemTarget;
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
                    ShowOnInspectorItemTarget();
                else
                    Debug.Log("This Database is empty..");
            }
        }

        private void ShowOnInspectorItemTarget()
        {
            if (_component._myItemData._items.Count > 0)
            {
                EditorGUILayout.LabelField("Targetable :", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                _component.ItemId = EditorGUILayout.Popup("Item Target", _component.ItemId, _component._myItemData.GetAllItemsName().ToArray());
                _component._blockEnabled = EditorGUILayout.Toggle(new GUIContent("Target Block ?"), _component._blockEnabled);

                _component._activeEndoscope = EditorGUILayout.ToggleLeft(new GUIContent("Endoscope Target ?", ""), _component._activeEndoscope);
                if (_component._activeEndoscope)
                {
                    EditorGUI.indentLevel++;

                    _component._myEndoEntrance = EditorGUILayout.ObjectField(new GUIContent("Scene to link", ""), _component._myEndoEntrance, typeof(EndoscopeEntrence), true) as EndoscopeEntrence;
                    if (null != _component._myEndoEntrance)
                    {
                        _component._myEndoEntrance.animIndex = EditorGUILayout.DelayedIntField(new GUIContent("Index Anim", ""), _component._myEndoEntrance.animIndex);
                        _component._myEndoEntrance.linkedScene = EditorGUILayout.ObjectField(new GUIContent("Scene to link", ""), _component._myEndoEntrance.linkedScene, typeof(EndoscopeScene), true) as EndoscopeScene;
                    }

                    EditorGUI.indentLevel--;
                }

                _component._activeScrewdriver = EditorGUILayout.ToggleLeft(new GUIContent("Screwdriver Target ?", ""), _component._activeScrewdriver);
                if(_component._activeScrewdriver)
                {

                }

                EditorGUI.indentLevel--;
            }
        }
        #endregion

        #region Create
        [MenuItem("Machinika/Create/Object/Item Target")]
        static void InitCreateItemTarget()
        {
            GameObject objSeleted = Selection.activeGameObject;
            GameObject newObj = CreateItemTarget(objSeleted);

            if (null != newObj)
            {
                Selection.activeGameObject = newObj;
            }
        }

        private static GameObject CreateItemTarget(GameObject parent)
        {
            GameObject newObj = new GameObject();

            if (null != newObj)
            {
                if (parent) newObj.transform.SetParent(parent.transform);
                newObj.tag = Tags.InteractiveObject;
                newObj.name = "New_Item_Target";
                newObj.transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));

                MeshFilter meshFilter = newObj.AddComponent<MeshFilter>();
                if (null == meshFilter)
                {
                    EditorUtility.DisplayDialog("Create Item Target Object Error", "Failed to add Mesh Filter in Item Target object", "Ok");
                    Destroy(newObj);
                    return null;
                }

                MeshRenderer meshRenderer = newObj.AddComponent<MeshRenderer>();
                if (null != meshRenderer)
                {
                    meshRenderer.enabled = true;
                }
                else
                {
                    EditorUtility.DisplayDialog("Create Item Target Object Error", "Failed to add Mesh Renderer in Item Target object", "Ok");
                    Destroy(newObj);
                    return null;
                }

                BoxCollider boxCollider = newObj.AddComponent<BoxCollider>();
                if (null != boxCollider)
                {
                    boxCollider.isTrigger = true;
                }
                else
                {
                    EditorUtility.DisplayDialog("Create Item Target Object Error", "Failed to add box collider in Item Target object", "Ok");
                    Destroy(newObj);
                    return null;
                }

                ItemTarget ItemTriggerObjScript = newObj.AddComponent<ItemTarget>();
                if (null != ItemTriggerObjScript)
                {
                    //Initialization if you need 
                }
                else
                {
                    EditorUtility.DisplayDialog("Create Item Target Object Error", "Failed to add 'Item Trigger Script' in Item Target object", "Ok");
                    Destroy(newObj);
                    return null;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Create Item Target Object Error", "Create Item Target Object failed", "Ok");
            }
            return newObj;
        }


        #endregion

        #region Change
        [MenuItem("Machinika/Change/To Item Target Object")]
        static void InitChangeToItemTarget()
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
                option = EditorUtility.DisplayDialog("Change to Item Target Object ?",
                    "WARNING ! Some your components can to be deleted ! \nAre you sure to change this object ?",
                    "Yes",
                    "No");
            }
            else
            {
                EditorUtility.DisplayDialog("Change to Item Target Object Error", "Failed because you don't selected a object to change.", "Ok");
                option = false;
            }

            switch (option)
            {
                // With linking
                case true:
                    foreach (GameObject go in objSeleted)
                        ChangeToItemTarget(go);
                    break;

                // Cancel.
                case false:
                    Debug.Log("The 'Item Target Object' creation has been canceled.");
                    break;

                default:
                    Debug.LogError("Item Target Object Error.");
                    break;
            }
        }

        private static GameObject ChangeToItemTarget(GameObject objToChange)
        {
            if (null != objToChange)
            {
                objToChange.tag = Tags.InteractiveObject;
                objToChange.name += "_Target";

                if (null == objToChange.GetComponent<MeshFilter>())
                    objToChange.AddComponent<MeshFilter>();

                if (null == objToChange.GetComponent<MeshRenderer>())
                    objToChange.AddComponent<MeshRenderer>();

                if (null != objToChange.GetComponent<InteractiveObject>())
                {
                    DestroyImmediate(objToChange.GetComponent<InteractiveObject>());
                    DestroyImmediate(objToChange.GetComponent<Collider>());
                }

                if (null == objToChange.GetComponent<BoxCollider>())
                    objToChange.AddComponent<BoxCollider>(); objToChange.GetComponent<BoxCollider>().isTrigger = true;

                if (null == objToChange.GetComponent<ItemTarget>())
                    objToChange.AddComponent<ItemTarget>();

                if (null == objToChange.GetComponent<AudioSource>())
                    objToChange.AddComponent<AudioSource>();
            }
            else
                EditorUtility.DisplayDialog("Update Item Target Object Error", "Update Item Target Object failed", "Ok");

            return objToChange;
        }
        #endregion
    }

#endif
}