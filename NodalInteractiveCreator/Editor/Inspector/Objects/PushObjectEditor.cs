// Machinika Museum
// © Littlefield Studio
//
// Writted by Franck-Olivier FILIN - 2017
//
// PushObjectEditor.cs

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using NodalInteractiveCreator.Objects;

namespace NodalInteractiveCreator.Editor.Objects
{
    [CustomEditor(typeof(PushObject))]
    [CanEditMultipleObjects]
    public class PushObjectEditor : TouchableElementEditor
    {
        PushObject _component;

        private void OnEnable()
        {
            _component = target as PushObject;
        }

        #region Inspector
        public override void OnInspectorGUI()
        {
            if (null != _component)
            {
                base.OnInspectorGUI();

                DrawInspectorPushObj();

                EditorGUILayout.Space();

                ShowOnInspectorAudio();

                EditorGUILayout.Space();

                ShowOnInspectorMaterial(_component);

                SceneView.RepaintAll();
            }
        }

        private void DrawInspectorPushObj()
        {
            EditorGUILayout.LabelField("Pushable :", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            _component._showDebug = EditorGUILayout.Toggle("Show Debug ?", _component._showDebug);
            _component._axisTranslate = (Axis)EditorGUILayout.EnumPopup("Axis", _component._axisTranslate);
            _component._offsetPos = EditorGUILayout.FloatField("Distance ", _component._offsetPos);
            _component._axisRotate = (Axis)EditorGUILayout.EnumPopup("Axis", _component._axisRotate);
            _component._offsetRot = EditorGUILayout.FloatField("Angle ", _component._offsetRot);
            _component._duration = EditorGUILayout.Slider("Duration (s) ", _component._duration, 0f, 1f);
            _component._twoStep = EditorGUILayout.ToggleLeft("Two Step ?", _component._twoStep);

            if (!_component._twoStep)
            {
                EditorGUI.indentLevel++;
                _component._onPress = EditorGUILayout.Toggle(new GUIContent("OnPress ?"), _component._onPress);
                EditorGUI.indentLevel--;
            }
            else
                _component._onPress = false;

            EditorGUI.indentLevel--;
        }
        #endregion

        #region Create
        [MenuItem("Machinika/Create/Object/Push")]
        static void InitCreatePush()
        {
            GameObject objSeleted = Selection.activeGameObject;
            GameObject newObj = CreatePush(objSeleted);

            if (null != newObj)
            {
                Selection.activeGameObject = newObj;
            }
        }

        private static GameObject CreatePush(GameObject parent)
        {
            GameObject newObjPush = new GameObject();

            if (null != newObjPush)
            {
                if (parent) newObjPush.transform.SetParent(parent.transform);
                newObjPush.tag = Tags.InteractiveObject;
                newObjPush.name = "New_Item_Push";
                newObjPush.transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));

                MeshFilter meshFilter = newObjPush.AddComponent<MeshFilter>();
                if (null == meshFilter)
                {
                    EditorUtility.DisplayDialog("Create Push Object Error", "Failed to add Mesh Filter in Push object", "Ok");
                    Destroy(newObjPush);
                    return null;
                }

                MeshRenderer meshRenderer = newObjPush.AddComponent<MeshRenderer>();
                if (null != meshRenderer)
                {
                    meshRenderer.enabled = true;
                }
                else
                {
                    EditorUtility.DisplayDialog("Create Push Object Error", "Failed to add Mesh Renderer in Push object", "Ok");
                    Destroy(newObjPush);
                    return null;
                }

                BoxCollider boxCollider = newObjPush.AddComponent<BoxCollider>();
                if (null != boxCollider)
                {
                    boxCollider.isTrigger = true;
                }
                else
                {
                    EditorUtility.DisplayDialog("Create Push Object Error", "Failed to add box collider in Push object", "Ok");
                    Destroy(newObjPush);
                    return null;
                }

                PushObject pickableObjScript = newObjPush.AddComponent<PushObject>();
                if (null != pickableObjScript)
                {
                    //Initialization if you need 
                }
                else
                {
                    EditorUtility.DisplayDialog("Create Push Object Error", "Failed to add 'Push Script' in Push object", "Ok");
                    Destroy(newObjPush);
                    return null;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Create Push Object Error", "Create Push Object failed", "Ok");
            }
            return newObjPush;
        }


        #endregion

        #region Change
        [MenuItem("Machinika/Change/To Push Object")]
        static void InitChangeToPush()
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
                option = EditorUtility.DisplayDialog("Change to Push Object ?",
                    "WARNING ! Some your components can to be deleted ! \nAre you sure to change this object ?",
                    "Yes",
                    "No");
            }
            else
            {
                EditorUtility.DisplayDialog("Change to Push Object Error", "Failed because you don't selected a object to change.", "Ok");
                option = false;
            }

            switch (option)
            {
                // With linking
                case true:
                    foreach (GameObject go in objSeleted)
                        ChangeToPush(go);
                    break;

                // Cancel.
                case false:
                    Debug.Log("The 'Push Object' creation has been canceled.");
                    break;

                default:
                    Debug.LogError("Push Object Error.");
                    break;
            }
        }

        private static GameObject ChangeToPush(GameObject objToChange)
        {
            if (null != objToChange)
            {
                objToChange.tag = Tags.InteractiveObject;
                objToChange.name += "_Push";

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

                if (null == objToChange.GetComponent<PushObject>())
                    objToChange.AddComponent<PushObject>();

                if (null == objToChange.GetComponent<AudioSource>())
                    objToChange.AddComponent<AudioSource>();
            }
            else
                EditorUtility.DisplayDialog("Update Push Object Error", "Update Push Object failed", "Ok");

            return objToChange;
        }
        #endregion
    }
#endif
}
