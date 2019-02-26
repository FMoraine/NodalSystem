using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.CustomEditing;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using NodalInteractiveCreator.Objects;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomNodalEditor(typeof(SetValueShaderNode))]
public class SetValueShaderNodeEditor : ActionNodeEditor<SetValueShaderNode>
{
    public SetValueShaderNodeEditor(Type t) : base(t) { }

    public override Rect GetCustomBounds()
    {
        bounds.size = new Vector2(250, 200);
        return bounds;
    }

    public override void OnGUI()
    {
        base.OnGUI();
    }

    protected override void ShowOnNodeInit()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("Game Object", EditorStyles.centeredGreyMiniLabel);
        _node._go.Assign(EditorGUILayout.ObjectField(_node._go.Subject, typeof(GameObject), true) as GameObject);

        if (_node._go.Subject != null)
        {
            if (_node._go.Subject.GetComponent<MeshRenderer>() == null)
            {
                EditorGUILayout.EndVertical();
                return;
            }
            
            _node._meshRenderer.Assign(_node._go.Subject.GetComponent<MeshRenderer>());

            ShowOnNodeNameMaterial();

            if (_node._meshRenderer.Subject.sharedMaterials[_node._idMat] != null)
            {
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Height(150));

                ShowOnNodeNameProperty();

                EditorGUILayout.EndScrollView();
            }
        }

        EditorGUILayout.EndVertical();
    }

    protected override void ChangeNodeEmitterStyle()
    {
        base.ChangeNodeEmitterStyle();
    }

    private void ShowOnNodeNameMaterial()
    {
        List<string> nameMat = new List<string>();
        foreach (Material mat in _node._meshRenderer.Subject.sharedMaterials)
            nameMat.Add(mat.name);

        if (_node._idMat > _node._meshRenderer.Subject.sharedMaterials.Length-1)
            _node._idMat = 0;

        EditorGUILayout.LabelField("Material", EditorStyles.centeredGreyMiniLabel);
        _node._idMat = EditorGUILayout.Popup("",_node._idMat, nameMat.ToArray());
    }

    private void ShowOnNodeNameProperty()
    {
        List<string> listNameProps = new List<string>();
        int idName = 0;
        
        for (int i = 0; i < ShaderUtil.GetPropertyCount(_node._meshRenderer.Subject.sharedMaterials[_node._idMat].shader); i++)
            listNameProps.Add(ShaderUtil.GetPropertyName(_node._meshRenderer.Subject.sharedMaterials[_node._idMat].shader, i));

        EditorGUILayout.LabelField("Property", EditorStyles.centeredGreyMiniLabel);

        if (string.IsNullOrEmpty(_node._propertyName) || !listNameProps.Exists(x => x == _node._propertyName))
            idName = EditorGUILayout.Popup(idName, listNameProps.ToArray());
        else
            idName = EditorGUILayout.Popup(listNameProps.IndexOf(_node._propertyName), listNameProps.ToArray());

        _node._propertyName = listNameProps[idName];

        ShowShaderConfigOf(idName);
    }

    private void ShowShaderConfigOf(int idName)
    {
        if (ShaderUtil.GetPropertyType(_node._meshRenderer.Subject.sharedMaterials[_node._idMat].shader, idName) == ShaderUtil.ShaderPropertyType.TexEnv)
        {
            _node._texture = (Texture)EditorGUILayout.ObjectField(_node._texture, typeof(Texture), true);

            EditorGUILayout.LabelField(new GUIContent("Tiling"), EditorStyles.centeredGreyMiniLabel);
            _node._tiling = EditorGUILayout.Vector2Field("", _node._tiling);

            EditorGUILayout.LabelField(new GUIContent("Offset"), EditorStyles.centeredGreyMiniLabel);
            _node._offset = EditorGUILayout.Vector2Field("", _node._offset);
        }
        else if (ShaderUtil.GetPropertyType(_node._meshRenderer.Subject.sharedMaterials[_node._idMat].shader, idName) == ShaderUtil.ShaderPropertyType.Color)
        {
            if (_node._meshRenderer.Subject.sharedMaterials[_node._idMat].HasProperty(_node._propertyName))
            {
                EditorGUILayout.LabelField(new GUIContent("Color"), EditorStyles.centeredGreyMiniLabel);
                _node._color = EditorGUILayout.ColorField(_node._color);
            }
        }
        else if (ShaderUtil.GetPropertyType(_node._meshRenderer.Subject.sharedMaterials[_node._idMat].shader, idName) == ShaderUtil.ShaderPropertyType.Float)
        {
            EditorGUILayout.LabelField(new GUIContent("Value"), EditorStyles.centeredGreyMiniLabel);
            _node._value = EditorGUILayout.FloatField(_node._value);
        }
        else if (ShaderUtil.GetPropertyType(_node._meshRenderer.Subject.sharedMaterials[_node._idMat].shader, idName) == ShaderUtil.ShaderPropertyType.Range)
        {
            EditorGUILayout.LabelField(new GUIContent("Range"), EditorStyles.centeredGreyMiniLabel);
            _node._value = EditorGUILayout.Slider(_node._value,0,1);
        }
    }
}
#endif

