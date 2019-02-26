using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.CustomEditing;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using NodalInteractiveCreator.Viewpoints;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomNodalEditor(typeof(PlayAnimationNode))]
public class PlayAnimationNodeEditor : ActionNodeEditor<PlayAnimationNode>
{
    public PlayAnimationNodeEditor(Type t) : base(t) { }

    public override Rect GetCustomBounds()
    {
        bounds.size = new Vector2(250, 150);
        return bounds;
    }

    public override void OnGUI()
    {
        base.OnGUI();
    }

    protected override void ShowOnNodeInit()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Animation", EditorStyles.centeredGreyMiniLabel);
        _node._animation.Assign(EditorGUILayout.ObjectField(_node._animation.Subject, typeof(Animation), true) as Animation);

        if (null != _node._animation.Subject && _node._animation.Subject.GetClipCount() > 0)
        {
            List<string> listAnim = new List<string>();

            foreach (AnimationState animClip in _node._animation.Subject)
                listAnim.Add(animClip.name);

            EditorGUILayout.LabelField("Clip name", EditorStyles.centeredGreyMiniLabel);
            _node._idAnim = EditorGUILayout.Popup(_node._idAnim, listAnim.ToArray());
            _node._clipAnime.Assign(_node._animation.Subject[listAnim[_node._idAnim]].clip);

            _node._reverse = EditorGUILayout.Toggle(new GUIContent("Reverse", "Play reverse animation"), _node._reverse);
            _node._replay = EditorGUILayout.Toggle(new GUIContent("Replay", "Replay animation foreach push"), _node._replay);
            _node._lockInteractivity = EditorGUILayout.Toggle(new GUIContent("Lock Interactive input"), _node._lockInteractivity);
            _node._lockPinch = EditorGUILayout.Toggle(new GUIContent("Lock Pinch out"), _node._lockPinch);
        }

        EditorGUILayout.EndVertical();
    }

    protected override void ChangeNodeEmitterStyle()
    {
        base.ChangeNodeEmitterStyle();
    }
}
#endif

