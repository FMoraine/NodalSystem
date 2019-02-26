using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.CustomEditing;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using NodalInteractiveCreator.Controllers;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomNodalEditor(typeof(PlayCutsceneNode))]
public class PlayCutsceneNodeEditor : ActionNodeEditor<PlayCutsceneNode>
{
    public PlayCutsceneNodeEditor(Type t) : base(t) { }

    public override Rect GetCustomBounds()
    {
        bounds.size = new Vector2(250, 75);
        return bounds;
    }

    public override void OnGUI()
    {
        base.OnGUI();
    }

    protected override void ShowOnNodeInit()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Animator", EditorStyles.centeredGreyMiniLabel);
        _node._animator.Assign(EditorGUILayout.ObjectField(_node._animator.Subject, typeof(Animator), true) as Animator);

        if (null != _node._animator.Subject)
        {
            Cutscene myCutscene = UnityEngine.Object.FindObjectOfType<Cutscene>();
            List<string> listState = new List<string>();

            foreach (Cutscene.CheckpointCutscene ck in myCutscene._pointCutscene)
                listState.Add(ck._nameCuts);

            if (listState.Count > 0)
            {
                EditorGUILayout.LabelField("Cutscene name", EditorStyles.centeredGreyMiniLabel);
                _node._idState = EditorGUILayout.Popup(_node._idState, listState.ToArray());
                _node._cutsceneName = listState[_node._idState];
            }
            else
                EditorGUILayout.HelpBox("You haven't cutscene configured", MessageType.None);
        }

        EditorGUILayout.EndVertical();
    }

    protected override void ChangeNodeEmitterStyle()
    {
        base.ChangeNodeEmitterStyle();
    }
}
#endif

