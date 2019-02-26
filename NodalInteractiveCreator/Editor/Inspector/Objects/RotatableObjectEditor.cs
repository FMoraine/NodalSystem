// Machinika Museum
// © Littlefield Studio
// Writted by Rémi Carreira - 13/02/2016
//
// Edited by Franck-Olivier FILIN - 2017
//
// RotatableObjectEditor.cs

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using NodalInteractiveCreator.Objects;
using NodalInteractiveCreator.Tools;

namespace NodalInteractiveCreator.Editor.Objects
{
    [CustomEditor(typeof(RotatableObject))]
    [CanEditMultipleObjects]
    public class RotatableObjectEditor : TouchableElementEditor
    {
        RotatableObject _component;
        bool _autoGeneration;
        bool _limiteRotOpenned;
        bool _checkStepOpenned;

        private void OnEnable()
        {
            _component = target as RotatableObject;
        }

        #region Inspector
        public override void OnInspectorGUI()
        {
            if (null != _component)
            {
                base.OnInspectorGUI();

                DrawInspectorForNonSpecificVariables();

                EditorGUILayout.Space();

                ShowOnInspectorAudio();

                EditorGUILayout.Space();

                DrawInspectorForLimits();

                EditorGUILayout.Space();

                DrawInspectorForCheckRotation();

                EditorGUILayout.Space();

                ShowOnInspectorMaterial(_component);

                ClampValues();

                SceneView.RepaintAll();
            }
        }

        private void DrawInspectorForNonSpecificVariables()
        {
            EditorGUILayout.LabelField("Rotatable :", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            _component.AxisToRotate = (Axis)EditorGUILayout.EnumPopup("Axis to Rotate", _component.AxisToRotate);
            _component.RotationSpeed = EditorGUILayout.FloatField("Rotation Speed", _component.RotationSpeed);

            _component._backToStart = EditorGUILayout.ToggleLeft("Back to Start ?", _component._backToStart);
            if (_component._backToStart)
            {
                EditorGUI.indentLevel++;
                _component._activeActionBackToStart = EditorGUILayout.Toggle("Active Action Start ?", _component._activeActionBackToStart);
                EditorGUI.indentLevel--;
            }

            _component.Swipe = EditorGUILayout.ToggleLeft("Swipe Object", _component.Swipe);
            if (true == _component.Swipe)
            {
                EditorGUI.indentLevel++;
                _component.HorizontalSwipe = EditorGUILayout.Toggle("Horizontal Swipe", _component.HorizontalSwipe);
                EditorGUI.indentLevel--;
            }

            if (_component.LimitRotation || _component.CheckRotation || _component._backToStart)
                _component.SnapDuration = EditorGUILayout.FloatField("Snap Duration", _component.SnapDuration);

            EditorGUI.indentLevel--;
        }

        private void DrawInspectorForLimits()
        {
            if (GUILayout.Toggle(_limiteRotOpenned, "Limites Rotation", EditorStyles.foldout))
            {
                _limiteRotOpenned = true;
                EditorGUI.indentLevel++;

                _component.LimitRotation = EditorGUILayout.Toggle("Active ?", _component.LimitRotation);
                if (true == _component.LimitRotation)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField(new GUIContent("Show Debug :"), EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;

                    _component.ShowMinRotation = EditorGUILayout.Toggle("Show Start", _component.ShowMinRotation);
                    _component.ShowMaxRotation = EditorGUILayout.Toggle("Show Final", _component.ShowMaxRotation);

                    EditorGUILayout.Space();
                    EditorGUI.indentLevel--;
                    EditorGUILayout.LabelField(new GUIContent("Start position :"), EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;

                    _component.MinAngle = EditorGUILayout.FloatField("Angle", _component.MinAngle);
                    if (!_component.SnapToMinRotation)
                        _component.PrecisionMin = EditorGUILayout.Slider("Limite Angle", _component.PrecisionMin, 0, (-_component.MinAngle + _component.MaxAngle) - _component.PrecisionMax);
                    else
                        _component.PrecisionMin = EditorGUILayout.Slider("Limite Snap", _component.PrecisionMin, 0, (-_component.MinAngle + _component.MaxAngle) - _component.PrecisionMax);

                    _component._audioClipStart = EditorGUILayout.ObjectField(new GUIContent("Audio"), _component._audioClipStart, typeof(AudioClip), false) as AudioClip;

                    EditorGUILayout.Space();
                    EditorGUI.indentLevel--;
                    EditorGUILayout.LabelField(new GUIContent("Final position :"), EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;

                    _component.MaxAngle = EditorGUILayout.FloatField("Angle", _component.MaxAngle);
                    if (!_component.SnapToMaxRotation)
                        _component.PrecisionMax = EditorGUILayout.Slider("Limite Angle", _component.PrecisionMax, 0, (-_component.MinAngle + _component.MaxAngle) - _component.PrecisionMin);
                    else
                        _component.PrecisionMax = EditorGUILayout.Slider("Limite Snap", _component.PrecisionMax, 0, (-_component.MinAngle + _component.MaxAngle) - _component.PrecisionMin);

                    _component._audioClipFinal = EditorGUILayout.ObjectField(new GUIContent("Audio"), _component._audioClipFinal, typeof(AudioClip), false) as AudioClip;

                    EditorGUILayout.Space();
                    EditorGUI.indentLevel--;
                    EditorGUILayout.LabelField(new GUIContent("Snap :"), EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;

                    _component.SnapToMinRotation = EditorGUILayout.Toggle("Snap To Start", _component.SnapToMinRotation);
                    _component.SnapToMaxRotation = EditorGUILayout.Toggle("Snap To Final", _component.SnapToMaxRotation);

                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }
            else if (_component.LimitRotation)
            {
                _limiteRotOpenned = false;
                EditorGUILayout.HelpBox("Limites rotation configured", MessageType.None);
            }
        }

        private void DrawInspectorForCheckRotation()
        {
            if (GUILayout.Toggle(_checkStepOpenned, "Check Rotation", EditorStyles.foldout))
            {
                _checkStepOpenned = true;
                EditorGUI.indentLevel++;

                _component.CheckRotation = EditorGUILayout.Toggle("Active ?", _component.CheckRotation);
                if (_component.CheckRotation)
                {
                    EditorGUI.indentLevel++;

                    _component.ShowCheckRotation = EditorGUILayout.Toggle("Show Check Rotation", _component.ShowCheckRotation);
                    _component.SnapToCheckRotation = EditorGUILayout.Toggle("Snap To Check Rotation", _component.SnapToCheckRotation);
                    _component.AntiHoraire = EditorGUILayout.Toggle(new GUIContent("Anticlockwise", "Inverse the orientation of the check's angles"), _component.AntiHoraire);
                    _component.NbCheck = EditorGUILayout.DelayedIntField("Total Checks", _component.NbCheck);
                    _component.CheckRotation = true;
                    CheckConfig();

                    EditorGUI.indentLevel--;
                }
                else
                {
                    _component.CheckRotation = false;
                    _component.NbCheck = 0;
                    _component.ListCheck.Clear();
                }

                EditorGUI.indentLevel--;
            }
            else if (_component.ListCheck.Count > 0)
            {
                _checkStepOpenned = false;
                EditorGUILayout.HelpBox("There have " + _component.ListCheck.Count.ToString() + " Checks configured", MessageType.None);
            }
        }

        private void DrawInspectorForSpecificRotation()
        {
            _component.SpecificRotation = GUILayout.Toggle(_component.SpecificRotation, "Specific Rotation", EditorStyles.foldout);
            if (true == _component.SpecificRotation)
            {
                _component.ShowSpecificRotation = EditorGUILayout.Toggle("Show Specific Rotation", _component.ShowSpecificRotation);
                _component.SnapToSpecificRotation = EditorGUILayout.Toggle("Snap To Specific Rotation", _component.SnapToSpecificRotation);
                _component.SpecificAngle = EditorGUILayout.FloatField("Specific Angle", _component.SpecificAngle);
            }
        }

        private void ClampValues()
        {
            if (_component.PrecisionMin < 0.0f)
            {
                _component.PrecisionMin = 0.0f;
            }
            if (_component.PrecisionMax < 0.0f)
            {
                _component.PrecisionMax = 0.0f;
            }
            //if (Component.RotationSpeed < 0.0f)
            //{
            //    Component.RotationSpeed = 0.0f;
            //}
            if (_component.SnapDuration < 0.0f)
            {
                _component.SnapDuration = 0.0f;
            }
            if (_component.MinAngle > 0.0f)
            {
                _component.MinAngle = 0.0f;
            }
            if (_component.MaxAngle < 0.0f)
            {
                _component.MaxAngle = 0.0f;
            }
            if (true == _component.LimitRotation)
            {
                if (_component.SpecificAngle > _component.MaxAngle)
                {
                    _component.SpecificAngle = _component.MaxAngle;
                }
                else if (_component.SpecificAngle < _component.MinAngle)
                {
                    _component.SpecificAngle = _component.MinAngle;
                }
            }
            if (_component.SpecificAngle > 360.0f)
            {
                _component.SpecificAngle = 360.0f;
            }
            else if (_component.SpecificAngle < -360.0f)
            {
                _component.SpecificAngle = -360.0f;
            }
        }

        private void CheckConfig()
        {
            bool isLimited;
            if (_component.LimitRotation)
                isLimited = true;
            else
                isLimited = false;

            if (_component.NbCheck <= 0)
            {
                _component.ListCheck.Clear();
                _component.ListCheck.Add(new Check(0, 0, 0, 0, 0));
                _component.NbCheck = 1;
            }

            if (!isLimited && _component.NbCheck > 1 && GUILayout.Button(new GUIContent("Auto Configuration", "Configure yours checks angle's, min and max limit's")))
            {
                for (int i = 0; i < _component.NbCheck; i++)
                {
                    float q = (System.Convert.ToSingle(i) / System.Convert.ToSingle(_component.NbCheck - 1));
                    _component.ListCheck[i].Angle = 360 * q;
                    _component.ListCheck[i].LimitSnapMin = Mathf.RoundToInt(360 / (_component.NbCheck - 1)) / 2;
                    _component.ListCheck[i].LimitSnapMax = Mathf.RoundToInt(360 / (_component.NbCheck - 1)) / 2;
                }
            }
            else
            {
                while (_component.ListCheck.Count != _component.NbCheck)
                {
                    if (_component.ListCheck.Count < _component.NbCheck)
                    {
                        _component.ListCheck.Add(new Check(0, _component.ListCheck[_component.ListCheck.Count - 1].Angle, 0, 0, 0));
                    }
                    else if (_component.ListCheck.Count > _component.NbCheck)
                    {
                        _component.ListCheck.RemoveAt(_component.NbCheck);
                    }
                }
            }

            EditorGUILayout.BeginVertical();
            EditorGUI.indentLevel++;

            int cptCheck = 0;

            foreach (Check check in _component.ListCheck)
            {
                check.Id = _component.ListCheck.IndexOf(check);

                if (isLimited)
                    CheckIsLimited(check, cptCheck);
                else
                    CheckIsNotLimited(check, cptCheck);

                check.AudioCheck = EditorGUILayout.ObjectField(new GUIContent("Audio check"), check.AudioCheck, typeof(AudioClip), false) as AudioClip;
                if (check.AudioCheck == null)
                    check.AudioCheck = _component._audioClipDefault;

                cptCheck++;
                EditorGUILayout.LabelField("_________________________________________");
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

        }

        private void CheckIsLimited(Check check, int cptCheck)
        {
            check.Angle = EditorGUILayout.Slider("Check_" + check.Id + " Angle", check.Angle, _component.MinAngle + _component.PrecisionMin, _component.MaxAngle - _component.PrecisionMax);

            if (_component.ListCheck.Count > 1)
            {
                if (check.Id < _component.ListCheck.Count - 1)
                {
                    if (_component.ListCheck[_component.ListCheck.LastIndexOf(check) + 1].Angle <= check.Angle)
                        check.Angle = _component.ListCheck[_component.ListCheck.LastIndexOf(check) + 1].Angle;
                }

                EditorGUI.indentLevel++;

                if (_component.SnapToCheckRotation)
                    EditorGUILayout.LabelField("Limits Snap :");
                else
                    EditorGUILayout.LabelField("Limits Check :");

                float initMin = cptCheck == 0 ? check.Angle - _component.MinAngle - _component.PrecisionMin : _component.ListCheck[cptCheck].Angle - _component.ListCheck[cptCheck - 1].Angle - _component.ListCheck[cptCheck - 1].LimitSnapMax;
                float initMax = cptCheck == _component.ListCheck.Count - 1 ? _component.MaxAngle - check.Angle - _component.PrecisionMax : _component.ListCheck[cptCheck + 1].Angle - _component.ListCheck[cptCheck].Angle - _component.ListCheck[cptCheck + 1].LimitSnapMin;

                check.LimitSnapMin = EditorGUILayout.Slider("Min", check.LimitSnapMin, 0, Mathf.Clamp(initMin, 0, 360));
                check.LimitSnapMax = EditorGUILayout.Slider("Max", check.LimitSnapMax, 0, Mathf.Clamp(initMax, 0, 360));

                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUI.indentLevel++;

                if (_component.SnapToCheckRotation)
                    EditorGUILayout.LabelField("Limits Snap :");
                else
                    EditorGUILayout.LabelField("Limits Check :");

                check.LimitSnapMin = EditorGUILayout.Slider("Min", check.LimitSnapMin, 0, check.Angle - _component.MinAngle - _component.PrecisionMin);
                check.LimitSnapMax = EditorGUILayout.Slider("Max", check.LimitSnapMax, 0, _component.MaxAngle - check.Angle - _component.PrecisionMax);

                EditorGUI.indentLevel--;
            }
        }

        private void CheckIsNotLimited(Check check, int cptCheck)
        {
            check.Angle = EditorGUILayout.Slider("Check_" + check.Id + " Angle", check.Angle, 0, 360);

            if (_component.ListCheck.Count > 1)
            {
                if (check.Id < _component.ListCheck.Count - 1)
                {
                    if (_component.ListCheck[_component.ListCheck.LastIndexOf(check) + 1].Angle <= check.Angle)
                        check.Angle = _component.ListCheck[_component.ListCheck.LastIndexOf(check) + 1].Angle;
                }
                EditorGUI.indentLevel++;

                if (_component.SnapToCheckRotation)
                    EditorGUILayout.LabelField("Limits Snap :");
                else
                    EditorGUILayout.LabelField("Limits Check :");
                float initMin = cptCheck == 0 ? 360f : _component.ListCheck[cptCheck].Angle - _component.ListCheck[cptCheck - 1].Angle - _component.ListCheck[cptCheck - 1].LimitSnapMax;
                float initMax = cptCheck == _component.ListCheck.Count - 1 ? 360f : _component.ListCheck[cptCheck + 1].Angle - _component.ListCheck[cptCheck].Angle - _component.ListCheck[cptCheck + 1].LimitSnapMin;

                check.LimitSnapMin = EditorGUILayout.Slider("Min", check.LimitSnapMin, 0, initMin);
                check.LimitSnapMax = EditorGUILayout.Slider("Max", check.LimitSnapMax, 0, initMax);

                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUI.indentLevel++;

                if (_component.SnapToCheckRotation)
                    EditorGUILayout.LabelField("Limits Snap :");
                else
                    EditorGUILayout.LabelField("Limits Check :");

                check.LimitSnapMin = Mathf.Clamp(EditorGUILayout.FloatField("Min", check.LimitSnapMin), 0, 360);
                check.LimitSnapMax = Mathf.Clamp(EditorGUILayout.FloatField("Max", check.LimitSnapMax), 0, 360);

                EditorGUI.indentLevel--;
            }
        }
        #endregion

        #region Create

        [MenuItem("Machinika/Create/Object/Rotable")]
        static void InitCreateRotable()
        {
            GameObject objSeleted = Selection.activeGameObject;
            GameObject newObj = CreateRotable(objSeleted);

            if (null != newObj)
            {
                Selection.activeGameObject = newObj;
            }
        }

        private static GameObject CreateRotable(GameObject parent)
        {
            GameObject newObjRotable = new GameObject();

            if (null != newObjRotable)
            {
                if (parent) newObjRotable.transform.SetParent(parent.transform);
                newObjRotable.tag = Tags.InteractiveObject;
                newObjRotable.name = "New_Object_Rotable";
                newObjRotable.transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));

                MeshFilter meshFilter = newObjRotable.AddComponent<MeshFilter>();
                if (null == meshFilter)
                {
                    EditorUtility.DisplayDialog("Create Rotable Object Error", "Failed to add Mesh Filter in Rotable object", "Ok");
                    Destroy(newObjRotable);
                    return null;
                }

                MeshRenderer meshRenderer = newObjRotable.AddComponent<MeshRenderer>();
                if (null != meshRenderer)
                {
                    meshRenderer.enabled = true;
                }
                else
                {
                    EditorUtility.DisplayDialog("Create Rotable Object Error", "Failed to add Mesh Renderer in Rotable object", "Ok");
                    Destroy(newObjRotable);
                    return null;
                }

                SphereCollider sphereCollider = newObjRotable.AddComponent<SphereCollider>();
                if (null != sphereCollider)
                {
                    sphereCollider.isTrigger = true;
                }
                else
                {
                    EditorUtility.DisplayDialog("Create Rotable Object Error", "Failed to add box collider in Rotable object", "Ok");
                    Destroy(newObjRotable);
                    return null;
                }

                RotatableObject rotatableObjScript = newObjRotable.AddComponent<RotatableObject>();
                if (null != rotatableObjScript)
                {
                    //Initialization if you need 
                }
                else
                {
                    EditorUtility.DisplayDialog("Create Rotable Object Error", "Failed to add 'Rotatable Script' in Rotable object", "Ok");
                    Destroy(newObjRotable);
                    return null;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Create Rotable Object Error", "Create Rotable Object failed", "Ok");
            }
            return newObjRotable;
        }


        #endregion

        #region Change
        [MenuItem("Machinika/Change/To Rotable Object")]
        static void InitChangeToRotable()
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
                option = EditorUtility.DisplayDialog("Change to Rotable Object ?",
                    "WARNING ! Some your components can to be deleted ! \nAre you sure to change this object ?",
                    "Yes",
                    "No");
            }
            else
            {
                EditorUtility.DisplayDialog("Change to Rotable Object Error", "Failed because you don't selected a object to change.", "Ok");
                option = false;
            }

            switch (option)
            {
                // With linking
                case true:
                    foreach (GameObject go in objSeleted)
                        ChangeToRotable(go);
                    break;

                // Cancel.
                case false:
                    Debug.Log("The 'Rotable Object' creation has been canceled.");
                    break;

                default:
                    Debug.LogError("Rotable Object Error.");
                    break;
            }
        }

        private static GameObject ChangeToRotable(GameObject objToChange)
        {
            if (null != objToChange)
            {
                objToChange.tag = Tags.InteractiveObject;
                objToChange.name += "_Rotable";

                if (null == objToChange.GetComponent<MeshFilter>())
                    objToChange.AddComponent<MeshFilter>();

                if (null == objToChange.GetComponent<MeshRenderer>())
                    objToChange.AddComponent<MeshRenderer>();

                if (null != objToChange.GetComponent<TouchableElement>())
                {
                    DestroyImmediate(objToChange.GetComponent<TouchableElement>());
                    DestroyImmediate(objToChange.GetComponent<Collider>());
                }

                if (null == objToChange.GetComponent<SphereCollider>())
                    objToChange.AddComponent<SphereCollider>(); objToChange.GetComponent<SphereCollider>().isTrigger = true;

                if (null == objToChange.GetComponent<RotatableObject>())
                    objToChange.AddComponent<RotatableObject>();

                if (null == objToChange.GetComponent<AudioSource>())
                    objToChange.AddComponent<AudioSource>();
            }
            else
                EditorUtility.DisplayDialog("Update Rotable Object Error", "Update Rotable Object failed", "Ok");

            return objToChange;
        }
        #endregion
    }
}
#endif
