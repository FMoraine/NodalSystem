using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using UnityEditor;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem
{
    [CustomEditor(typeof(NodalInteractionSystem))]
    public class NodalInteractionCreatorEditor : UnityEditor.Editor
    {
        public static SerializedObject objTarget;
        public static NodalInteractionSystem targetMono;
        public static GUIStyle windowStyle;

        private List<bool> _materialConfigOpenned = new List<bool>();

        private void OnEnable()
        {
            objTarget = serializedObject;
        }

        public override void OnInspectorGUI()
        {
            NodalInteractionSystem y = (NodalInteractionSystem)target;

            if (y != targetMono || (y != null && targetMono == null))
            {
                targetMono = y;

                if (NodalEditor.instance)
                    NodalEditor.instance.Refresh();
            }

            //base.OnInspectorGUI();
            EditorGUILayout.Space();

            ShowOnInspectorMaterial();
        }

        private void ShowOnInspectorMaterial()
        {
            int sizeList;
            EditorGUILayout.LabelField(new GUIContent("Shader Configuration :"), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            if(targetMono.nodes.Count != _materialConfigOpenned.Count)
                foreach (NodalInfos ni in targetMono.nodes)
                    _materialConfigOpenned.Add(false);

            foreach (NodalInfos ni in targetMono.nodes)
            {
                if (ni.Subject.GetType() == typeof(SetShaderConfigNode) && ((SetShaderConfigNode)ni.Subject)._target.Subject != null)
                {
                    if (GUILayout.Toggle(_materialConfigOpenned[targetMono.nodes.IndexOf(ni)], new GUIContent("Shader Node #" + targetMono.nodes.IndexOf(ni) + " (" + ((SetShaderConfigNode)ni.Subject)._target.Subject.name + ")"), EditorStyles.foldout))
                    {
                        _materialConfigOpenned[targetMono.nodes.IndexOf(ni)] = true;

                        EditorGUILayout.BeginVertical();
                        EditorGUI.indentLevel++;

                        sizeList = ni.materialConfig.Count;
                        sizeList = EditorGUILayout.DelayedIntField("Number Material", sizeList);

                        while (ni.materialConfig.Count != sizeList)
                        {
                            if (ni.materialConfig.Count > sizeList)
                                ni.materialConfig.RemoveAt(ni.materialConfig.Count - 1);
                            else if (sizeList > ni.materialConfig.Count)
                                ni.materialConfig.Add(new MaterialConfig());

                            if (ni.materialConfig.Count == 0)
                                return;
                        }

                        EditorGUI.indentLevel++;

                        foreach (MaterialConfig matConf in ni.materialConfig)
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
                                        EditorGUILayout.PropertyField(serializedObject.FindProperty("nodes.Array.data[" + targetMono.nodes.IndexOf(ni) + "].materialConfig.Array.data[" + ni.materialConfig.IndexOf(matConf) + "]._gradient"), false);
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
                    else if (ni.materialConfig.Count > 0)
                    {
                        _materialConfigOpenned[targetMono.nodes.IndexOf(ni)] = false;
                        EditorGUILayout.HelpBox("There have " + ni.materialConfig.Count.ToString() + " Material configured", MessageType.None);
                    }
                }
            }
        }
    }
}
