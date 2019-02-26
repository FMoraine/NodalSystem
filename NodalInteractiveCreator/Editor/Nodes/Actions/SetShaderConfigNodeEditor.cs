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
[CustomNodalEditor(typeof(SetShaderConfigNode))]
public class SetShaderConfigNodeEditor : ActionNodeEditor<SetShaderConfigNode>
{
    public SetShaderConfigNodeEditor(Type t) : base(t) { }

    public override Rect GetCustomBounds()
    {
        bounds.size = new Vector2(250, 100);
        return bounds;
    }

    public override void OnGUI()
    {
        base.OnGUI();
    }

    protected override void ShowOnNodeInit()
    {
        base.ShowOnNodeInit();

        if (NodalInteractionCreatorEditor.targetMono != null && _node._target.Subject != null)
            EditorGUILayout.HelpBox("Look at 'Nodal Interative System' Script --> Shader Node #" + (NodalInteractionCreatorEditor.targetMono.nodes.FindIndex(x => x.Subject == Subject as SetShaderConfigNode)) + " to configure.", MessageType.Info);
    }
}
#endif

