// Machinika Museum
// © Littlefield Studio
// Writted by Franck-Olivier FILIN - 2017
//
// TranslatableObjectEditor.cs

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using NodalInteractiveCreator.Objects;
using NodalInteractiveCreator.Tools;

namespace NodalInteractiveCreator.Editor.Objects
{
    [CustomEditor(typeof(TranslatableObject))]
    [CanEditMultipleObjects]
    public class TranslatableObjectEditor : TouchableElementEditor
    {
        TranslatableObject _component;
        bool _limiteRotOpenned;
        bool _checkStepOpenned;

        #region Inspector
        private void OnEnable()
        {
            _component = target as TranslatableObject;
        }

        public override void OnInspectorGUI()
        {
            if (null != _component)
            {
                base.OnInspectorGUI();

                ShowOnInspectorNonSpecificVariables();

                EditorGUILayout.Space();

                ShowOnInspectorAudio();

                EditorGUILayout.Space();

                DrawInspectorTranslateLimits();

                EditorGUILayout.Space();

                ShowOnInspectorTranslateChecks();

                EditorGUILayout.Space();

                ShowOnInspectorMaterial(_component);

                ClampValues();

                SceneView.RepaintAll();
            }
        }

        private void ShowOnInspectorNonSpecificVariables()
        {
            EditorGUILayout.LabelField("Translation :", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            _component.AxisToTranslate = (Axis)EditorGUILayout.EnumPopup("Axis to Translate", _component.AxisToTranslate);
            _component._backToStart = EditorGUILayout.ToggleLeft("Back To Init Position", _component._backToStart);
            if (_component._backToStart)
            {
                EditorGUI.indentLevel++;
                _component._activeActionBackToStart = EditorGUILayout.Toggle("Active Action Start ?", _component._activeActionBackToStart);
                EditorGUI.indentLevel--;
            }

            if (_component.CheckTranslation || _component._backToStart)
                _component.SnapDuration = EditorGUILayout.FloatField("Snap Duration", _component.SnapDuration);

            EditorGUI.indentLevel--;
        }

        private void DrawInspectorTranslateLimits()
        {
            if (GUILayout.Toggle(_limiteRotOpenned, "Limites Translation", EditorStyles.foldout))
            {
                _limiteRotOpenned = true;

                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Show Debug :", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                _component.ShowMinTranslation = EditorGUILayout.Toggle("Show Start Pos", _component.ShowMinTranslation);
                _component.ShowMaxTranslation = EditorGUILayout.Toggle("Show Final Pos", _component.ShowMaxTranslation);

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Start position :", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                _component.MinNumberOfUnit = EditorGUILayout.FloatField("Start Pos", _component.MinNumberOfUnit);
                _component.PrecisionMin = EditorGUILayout.Slider("Limite Start Pos", _component.PrecisionMin, 0, (_component.MaxNumberOfUnit - _component.MinNumberOfUnit) - _component.PrecisionMax);
                _component._audioClipFinal = EditorGUILayout.ObjectField(new GUIContent("Start Audio"), _component._audioClipFinal, typeof(AudioClip), false) as AudioClip;
                _component.ActiveStartOnMove = EditorGUILayout.Toggle("Active Action On Move", _component.ActiveStartOnMove);

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Final position :", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                _component.MaxNumberOfUnit = EditorGUILayout.FloatField("Final Pos", _component.MaxNumberOfUnit);
                _component.PrecisionMax = EditorGUILayout.Slider("Limite Final Pos", _component.PrecisionMax, 0, (_component.MaxNumberOfUnit - _component.MinNumberOfUnit) - _component.PrecisionMin);
                _component._audioClipStart = EditorGUILayout.ObjectField(new GUIContent("Final Audio"), _component._audioClipStart, typeof(AudioClip), false) as AudioClip;
                _component.ActiveFinalOnMove = EditorGUILayout.Toggle("Active Action On Move", _component.ActiveFinalOnMove);

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Snap :", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                _component.SnapOnMinPosition = EditorGUILayout.Toggle("Snap To Start Pos", _component.SnapOnMinPosition);
                _component.SnapOnMaxPosition = EditorGUILayout.Toggle("Snap To Final Pos", _component.SnapOnMaxPosition);

                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
            else
            {
                _limiteRotOpenned = false;
                if (_component.MinNumberOfUnit != 0 || _component.MaxNumberOfUnit != 0)
                    EditorGUILayout.HelpBox("Limites translation configured", MessageType.None);
            }
        }

        private void ShowOnInspectorTranslateChecks()
        {
            if (GUILayout.Toggle(_checkStepOpenned, "Check Rotation", EditorStyles.foldout))
            {
                _checkStepOpenned = true;

                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;

                if (_component.MinNumberOfUnit == 0 && _component.MaxNumberOfUnit == 0)
                {
                    EditorGUILayout.HelpBox("Limites translation aren't configured", MessageType.Warning);
                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                    return;
                }

                _component.CheckTranslation = EditorGUILayout.ToggleLeft("Active ?", _component.CheckTranslation);
                if (_component.CheckTranslation)
                {
                    EditorGUI.indentLevel++;

                    _component.ShowCheckTranslation = EditorGUILayout.Toggle("Show Check", _component.ShowCheckTranslation);
                    _component.SnapOnCheckPosition = EditorGUILayout.Toggle("Snap To Check", _component.SnapOnCheckPosition);
                    _component.NbCheck = EditorGUILayout.DelayedIntField("Total Checks", _component.NbCheck);

                    CheckConfig();

                    EditorGUI.indentLevel--;
                }
                else
                {
                    _component.CheckTranslation = false;
                    _component.NbCheck = 0;
                    _component.ListCheck.Clear();
                }

                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
            else
            {
                _checkStepOpenned = false;
                if (_component.ListCheck.Count > 0)
                    EditorGUILayout.HelpBox("There have " + _component.ListCheck.Count.ToString() + " Checks configured", MessageType.None);
            }
        }

        private void CheckConfig()
        {
            int cptCheck = 0;

            if (_component.NbCheck <= 0)
            {
                _component.ListCheck.Clear();
                _component.ListCheck.Add(new Check(0, 0, 0, 0, 0));
                _component.NbCheck = 1;
            }

            while (_component.ListCheck.Count != _component.NbCheck)
            {
                if (_component.ListCheck.Count < _component.NbCheck)
                {
                    _component.ListCheck.Add(new Check(0, 0, _component.ListCheck[_component.ListCheck.Count - 1].Distance, 0, 0));
                }
                else if (_component.ListCheck.Count > _component.NbCheck)
                {
                    _component.ListCheck.RemoveAt(_component.NbCheck);
                }
            }

            EditorGUILayout.BeginVertical();
            EditorGUI.indentLevel++;

            foreach (Check check in _component.ListCheck)
            {
                check.Id = _component.ListCheck.IndexOf(check);

                CheckIsLimited(check, cptCheck);

                check.AudioCheck = EditorGUILayout.ObjectField(new GUIContent("Audio"), check.AudioCheck, typeof(AudioClip), false) as AudioClip;
                if (check.AudioCheck == null)
                    check.AudioCheck = _component._audioClipDefault;

                check.Block = EditorGUILayout.Toggle("Blocker ?", check.Block);
                check.ActiveCheckOnMove = EditorGUILayout.Toggle("Active Action On Move", check.ActiveCheckOnMove);

                cptCheck++;
                EditorGUILayout.LabelField("_________________________________________");
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        private void CheckIsLimited(Check check, int cptCheck)
        {
            check.Distance = EditorGUILayout.Slider("Check_" + check.Id, check.Distance, _component.MinNumberOfUnit, _component.MaxNumberOfUnit);

            if (_component.ListCheck.Count > 1)
            {
                if (check.Id < _component.ListCheck.Count - 1)
                {
                    if (_component.ListCheck[_component.ListCheck.LastIndexOf(check) + 1].Distance <= check.Distance)
                        check.Distance = _component.ListCheck[_component.ListCheck.LastIndexOf(check) + 1].Distance;
                }

                EditorGUI.indentLevel++;

                if (_component.SnapOnCheckPosition)
                    EditorGUILayout.LabelField("Limits Snap :");
                else
                    EditorGUILayout.LabelField("Limits Check :");

                float initMin = cptCheck == 0 ? check.Distance - _component.MinNumberOfUnit - _component.PrecisionMin : _component.ListCheck[cptCheck].Distance - _component.ListCheck[cptCheck - 1].Distance - _component.ListCheck[cptCheck - 1].LimitSnapMax;
                float initMax = cptCheck == _component.ListCheck.Count - 1 ? _component.MaxNumberOfUnit - check.Distance - _component.PrecisionMax : _component.ListCheck[cptCheck + 1].Distance - _component.ListCheck[cptCheck].Distance - _component.ListCheck[cptCheck + 1].LimitSnapMin;

                check.LimitSnapMin = EditorGUILayout.Slider("Min", check.LimitSnapMin, 0, initMin);
                check.LimitSnapMin = cptCheck == _component.ListCheck.Count - 1 && _component.PrecisionMax >= check.Distance ? 0 : check.LimitSnapMin;
                check.LimitSnapMin = cptCheck == 0 && _component.PrecisionMax >= _component.MaxNumberOfUnit - check.Distance ? (_component.MaxNumberOfUnit - _component.PrecisionMax) : check.LimitSnapMin;

                check.LimitSnapMax = EditorGUILayout.Slider("Max", check.LimitSnapMax, 0, initMax);
                check.LimitSnapMax = cptCheck == 0 && _component.PrecisionMin >= check.Distance ? 0 : check.LimitSnapMax;
                check.LimitSnapMax = cptCheck == _component.ListCheck.Count - 1 && _component.PrecisionMin >= check.Distance + _component.MinNumberOfUnit ? _component.MaxNumberOfUnit - (_component.PrecisionMin - _component.MinNumberOfUnit) : check.LimitSnapMax;

                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUI.indentLevel++;

                if (_component.SnapOnCheckPosition)
                    EditorGUILayout.LabelField("Limits Snap :");
                else
                    EditorGUILayout.LabelField("Limits Check :");

                check.LimitSnapMin = EditorGUILayout.Slider("Min", check.LimitSnapMin, 0, check.Distance - _component.MinNumberOfUnit - _component.PrecisionMin);
                check.LimitSnapMin = _component.PrecisionMax >= check.Distance ? 0 : check.LimitSnapMin;
                check.LimitSnapMax = EditorGUILayout.Slider("Max", check.LimitSnapMax, 0, _component.MaxNumberOfUnit - check.Distance - _component.PrecisionMax);
                check.LimitSnapMax = _component.PrecisionMin >= check.Distance ? 0 : check.LimitSnapMax;
                EditorGUI.indentLevel--;
            }
        }

        //private void ShowOnInspectorActionEventCheck(TranslatableObject Component, Check check)
        //{
        //    if (Component.ListCheck[check.Id].Action <= 0 || Component.ListCheck[check.Id].Action >= _myListActions.Count)
        //        Component.ListCheck[check.Id].Action = EditorGUILayout.Popup("Action", 0, _myListActions.ToArray());
        //    else
        //        Component.ListCheck[check.Id].Action = EditorGUILayout.Popup("Action", Component.ListCheck[check.Id].Action, _myListActions.ToArray());
        //}

        private void ClampValues()
        {
            if (_component.MinNumberOfUnit > 0.0f)
            {
                _component.MinNumberOfUnit = 0.0f;
            }
            if (_component.MaxNumberOfUnit < 0.0f)
            {
                _component.MaxNumberOfUnit = 0.0f;
            }
        }
        #endregion

        #region Create

        [MenuItem("Machinika/Create/Object/Translate")]
        static void InitCreateTranslate()
        {
            GameObject objSeleted = Selection.activeGameObject;
            GameObject newObj = CreateTranslate(objSeleted);

            if (null != newObj)
            {
                Selection.activeGameObject = newObj;
            }
        }

        private static GameObject CreateTranslate(GameObject parent)
        {
            GameObject newObjTranslate = new GameObject();

            if (null != newObjTranslate)
            {
                if (parent) newObjTranslate.transform.SetParent(parent.transform);
                newObjTranslate.tag = Tags.InteractiveObject;
                newObjTranslate.name = "New_Object_Translate";
                newObjTranslate.transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));

                MeshFilter meshFilter = newObjTranslate.AddComponent<MeshFilter>();
                if (null == meshFilter)
                {
                    EditorUtility.DisplayDialog("Create Translate Object Error", "Failed to add Mesh Filter in Translate object", "Ok");
                    Destroy(newObjTranslate);
                    return null;
                }

                MeshRenderer meshRenderer = newObjTranslate.AddComponent<MeshRenderer>();
                if (null != meshRenderer)
                {
                    meshRenderer.enabled = true;
                }
                else
                {
                    EditorUtility.DisplayDialog("Create Translate Object Error", "Failed to add Mesh Renderer in Translate object", "Ok");
                    Destroy(newObjTranslate);
                    return null;
                }

                BoxCollider boxCollider = newObjTranslate.AddComponent<BoxCollider>();
                if (null != boxCollider)
                {
                    boxCollider.isTrigger = true;
                }
                else
                {
                    EditorUtility.DisplayDialog("Create Translate Object Error", "Failed to add box collider in Translate object", "Ok");
                    Destroy(newObjTranslate);
                    return null;
                }

                TranslatableObject translatableObjScript = newObjTranslate.AddComponent<TranslatableObject>();
                if (null != translatableObjScript)
                {
                    //Initialization if you need 
                }
                else
                {
                    EditorUtility.DisplayDialog("Create Translate Object Error", "Failed to add 'Translatable Script' in Translate object", "Ok");
                    Destroy(newObjTranslate);
                    return null;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Create Translate Object Error", "Create Translate Object failed", "Ok");
            }
            return newObjTranslate;
        }


        #endregion

        #region Change
        [MenuItem("Machinika/Change/To Translate Object")]
        static void InitChangeToTranslate()
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
                option = EditorUtility.DisplayDialog("Change to Translate Object ?",
                    "WARNING ! Some your components can to be deleted ! \nAre you sure to change this object ?",
                    "Yes",
                    "No");
            }
            else
            {
                EditorUtility.DisplayDialog("Change to Translate Object Error", "Failed because you don't selected a object to change.", "Ok");
                option = false;
            }

            switch (option)
            {
                // With linking
                case true:
                    foreach (GameObject go in objSeleted)
                        ChangeToTranslate(go);
                    break;

                // Cancel.
                case false:
                    Debug.Log("The 'Translate Object' creation has been canceled.");
                    break;

                default:
                    Debug.LogError("Translate Object Error.");
                    break;
            }
        }

        private static GameObject ChangeToTranslate(GameObject objToChange)
        {
            if (null != objToChange)
            {
                objToChange.tag = Tags.InteractiveObject;
                objToChange.name += "_Translate";

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

                if (null == objToChange.GetComponent<TranslatableObject>())
                    objToChange.AddComponent<TranslatableObject>();

                if (null == objToChange.GetComponent<AudioSource>())
                    objToChange.AddComponent<AudioSource>();
            }
            else
                EditorUtility.DisplayDialog("Update Translate Object Error", "Update Translate Object failed", "Ok");

            return objToChange;
        }
        #endregion
    }
}
#endif
