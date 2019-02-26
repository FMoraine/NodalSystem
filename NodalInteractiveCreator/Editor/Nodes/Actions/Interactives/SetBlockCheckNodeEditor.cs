using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.CustomEditing;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.Settings;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Game.TheoricalNodes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using NodalInteractiveCreator.Controllers;
using NodalInteractiveCreator.Objects;
using NodalInteractiveCreator.Tools;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomNodalEditor(typeof(SetBlockCheckNode))]
public class SetBlockCheckNodeEditor : ActionNodeEditor<SetBlockCheckNode>
{
    private TranslatableObject _translatableObject;
    public SetBlockCheckNodeEditor(Type t) : base(t) { }

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

        EditorGUILayout.LabelField("Object target", EditorStyles.centeredGreyMiniLabel);
        _node._target.Assign(EditorGUILayout.ObjectField(_node._target.Subject, typeof(TranslatableObject), true) as TranslatableObject);

        EditorGUILayout.Space();

        if (_node._target.Subject != null)
        {
            _translatableObject = _node._target.Subject as TranslatableObject;

            if (_translatableObject.ListCheck.Count <= 0)
            {
                EditorGUILayout.HelpBox("There aren't have checks configured on this object", MessageType.None);
                EditorGUILayout.EndVertical();
                return;
            }
            else
            {
                if (_node._values.Count != _translatableObject.ListCheck.Count)
                    foreach (Check c in _translatableObject.ListCheck)
                        _node._values.Add(c.Block);

                EditorGUILayout.BeginHorizontal();

                _node._allCheck = EditorGUILayout.ToggleLeft("All Check ?", _node._allCheck, NICStettings.Settings.nodeEditorStyle, GUILayout.Width(100));

                if (_node._allCheck)
                {
                    _node._values[0] = PopupForLabelsBool("", _node._values[0], "True (Block)", "False (Unblock)");

                    for (int i = 0; i < _node._values.Count; i++)
                        _node._values[i] = _node._values[0];

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Height(125));

                    for (int i = 0; i < _translatableObject.NbCheck; i++)
                        EditorGUILayout.LabelField("Check #" + i + " Current value : " + _translatableObject.ListCheck[i].Block.ToString(), EditorStyles.centeredGreyMiniLabel);

                    EditorGUILayout.EndScrollView();
                }
                else
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Height(125));

                    for (int i = 0; i < _translatableObject.NbCheck; i++)
                    {
                        _node._values[i] = PopupForLabelsBool("Check #" + i + " Current value : " + _translatableObject.ListCheck[i].Block.ToString(), _node._values[i], "True (Block)", "False (Unblock)");
                    }

                    EditorGUILayout.EndScrollView();
                }
            }
        }
        EditorGUILayout.EndVertical();
    }

    protected override void ChangeNodeEmitterStyle()
    {
        base.ChangeNodeEmitterStyle();
    }

}
#endif

