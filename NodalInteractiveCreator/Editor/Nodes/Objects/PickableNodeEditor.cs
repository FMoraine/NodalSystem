using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.CustomEditing;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using NodalInteractiveCreator.Tools;
using System;
using UnityEngine;
using UnityEditor;
using NodalInteractiveCreator.Objects;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;

#if UNITY_EDITOR
[CustomNodalEditor(typeof(PickableNode))]
public class PickableNodeEditor : InteractiveNodeEditor<PickableNode, PickableObject>
{
    public PickableNodeEditor(Type t) : base(t) { }

    public override Rect GetCustomBounds()
    {
        bounds.size = new Vector2(200, 100);
        return bounds;
    }

    public override void OnGUI()
    {
        base.OnGUI();
    }

    protected override void ShowOnNodeInit()
    {
        base.ShowOnNodeInit();
    }

    protected override void ChangeNodeEmitterStyle()
    {
        base.ChangeNodeEmitterStyle();
    }
}
#endif