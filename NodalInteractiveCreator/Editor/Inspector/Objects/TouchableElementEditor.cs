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
    [CustomEditor(typeof(TouchableElement))]
    [CanEditMultipleObjects]
    public class TouchableElementEditor : UnityEditor.Editor
    {
        TouchableElement _componentParent;
        bool _materialConfigOpenned;

        #region Inspector
        public override void OnInspectorGUI()
        {
            if (target != null)
                _componentParent = target as TouchableElement;

            if (null != _componentParent)
                ShowOnInspectorTouchableInit();

            EditorGUILayout.Space();
        }

        private void ShowOnInspectorTouchableInit()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("versionUtility"));

            EditorStyles.label.fontStyle = FontStyle.Bold;
            _componentParent._interactable = EditorGUILayout.ToggleLeft(new GUIContent("Interactable On Start", "Initialize your object on lock or unlock."), _componentParent._interactable);
            EditorStyles.label.fontStyle = FontStyle.Normal;
        }

        protected void ShowOnInspectorAudio()
        {
            EditorGUILayout.LabelField("Audio :", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            _componentParent._audioClipDefault = EditorGUILayout.ObjectField(new GUIContent("Default audio"), _componentParent._audioClipDefault, typeof(AudioClip), false) as AudioClip;
            _componentParent._volume = EditorGUILayout.Slider(new GUIContent("Volume max"), _componentParent._volume, 0, 1);

            EditorGUI.indentLevel--;
        }

        protected void ShowOnInspectorMaterial(InteractiveObject myComponentInteractive)
        {
            int sizeList;

            if (GUILayout.Toggle(_materialConfigOpenned, new GUIContent("Shader Configuration"), EditorStyles.foldout))
            {
                _materialConfigOpenned = true;

                EditorGUILayout.BeginVertical();
                EditorGUI.indentLevel++;

                sizeList = myComponentInteractive._materials.Count;
                sizeList = EditorGUILayout.DelayedIntField("Number Material", sizeList);

                while (myComponentInteractive._materials.Count != sizeList)
                {
                    if (myComponentInteractive._materials.Count > sizeList)
                        myComponentInteractive._materials.RemoveAt(myComponentInteractive._materials.Count - 1);
                    else if (sizeList > myComponentInteractive._materials.Count)
                        myComponentInteractive._materials.Add(new MaterialConfig());

                    if (myComponentInteractive._materials.Count == 0)
                        return;
                }

                EditorGUI.indentLevel++;

                foreach (MaterialConfig matConf in myComponentInteractive._materials)
                {
                    EditorGUILayout.LabelField("--------------------------------------");

                    matConf._meshRenderer = (MeshRenderer)EditorGUILayout.ObjectField(new GUIContent("Renderer Target"), matConf._meshRenderer, typeof(MeshRenderer), true);

                    if (null != matConf._meshRenderer)
                    {
                        List<string> nameMat = new List<string>();
                        int idMat;

                        foreach (Material mat in matConf._meshRenderer.sharedMaterials)
                            nameMat.Add(mat.name);

                        if (null == matConf._material || !nameMat.Contains(matConf._material.name))
                            idMat = 0;
                        else
                            idMat = nameMat.IndexOf(matConf._material.name);

                        idMat = EditorGUILayout.Popup("Material ", idMat, nameMat.ToArray());

                        matConf._material = matConf._meshRenderer.sharedMaterials[idMat];

                        List<string> listNameProps = new List<string>();

                        for (int i = 0; i < ShaderUtil.GetPropertyCount(matConf._meshRenderer.sharedMaterial.shader); i++)
                            listNameProps.Add(ShaderUtil.GetPropertyName(matConf._meshRenderer.sharedMaterial.shader, i));

                        int idName = 0;

                        if (string.IsNullOrEmpty(matConf._propertyName))
                            idName = EditorGUILayout.Popup("Property Name", idName, listNameProps.ToArray());
                        else
                            idName = EditorGUILayout.Popup("Property Name", listNameProps.IndexOf(matConf._propertyName), listNameProps.ToArray());

                        matConf._propertyName = listNameProps[idName];

                        if (ShaderUtil.GetPropertyType(matConf._meshRenderer.sharedMaterial.shader, idName) == ShaderUtil.ShaderPropertyType.TexEnv)
                        {
                            matConf._texture = (Texture)EditorGUILayout.ObjectField(matConf._texture, typeof(Texture), true);
                            matConf._tiling = EditorGUILayout.Vector2Field(new GUIContent("Tiling"), matConf._tiling);
                            matConf._offset = EditorGUILayout.Vector2Field(new GUIContent("Offset"), matConf._offset);
                        }
                        else if (ShaderUtil.GetPropertyType(matConf._meshRenderer.sharedMaterial.shader, idName) == ShaderUtil.ShaderPropertyType.Color)
                        {
                            if (matConf._material.HasProperty("_RendererColor") || matConf._material.HasProperty("_Color"))
                            {
                                serializedObject.Update();
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("_materials.Array.data[" + myComponentInteractive._materials.IndexOf(matConf) + "]._gradient"), false);
                                serializedObject.ApplyModifiedProperties();
                            }
                        }
                        else if (ShaderUtil.GetPropertyType(matConf._meshRenderer.sharedMaterial.shader, idName) == ShaderUtil.ShaderPropertyType.Float)
                        {
                            matConf._value = EditorGUILayout.FloatField(new GUIContent("Value"), matConf._value);
                        }
                    }
                }
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }
            else if (myComponentInteractive._materials.Count > 0)
            {
                _materialConfigOpenned = false;
                EditorGUILayout.HelpBox("There have " + myComponentInteractive._materials.Count.ToString() + " Material configured", MessageType.None);
            }
        }
        #endregion
    }
}

#endif
