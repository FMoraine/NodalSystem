using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.CustomEditing;
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
[CustomNodalEditor(typeof(GetValueStoryNode))]
public class GetValueStoryNodeEditor : ActionNodeEditor<GetValueStoryNode>
{
    List<string> labelsEvent = new List<string>();
    List<string> labelsAction = new List<string>();
    public GetValueStoryNodeEditor(Type t) : base(t) { }

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
        if (_node._machineStory.Subject == null)
            _node._machineStory.Assign(UnityEngine.Object.FindObjectOfType<Machine>());

        EditorGUILayout.BeginVertical();

        if (_node._machineStory.Subject == null)
        {
            EditorGUILayout.LabelField("Machine", EditorStyles.centeredGreyMiniLabel);
            _node._machineStory.Assign(EditorGUILayout.ObjectField(_node._machineStory.Subject, typeof(Machine), true) as Machine);
        }
        else
        {
            EditorGUILayout.LabelField("Story", EditorStyles.centeredGreyMiniLabel);

            foreach (Machine.EventsMachine eve in _node._machineStory.Subject.Events)
                labelsEvent.Add(eve._event.ToString());

            _node._idStoryEvent = EditorGUILayout.Popup(_node._idStoryEvent, labelsEvent.ToArray());

            if (_node._machineStory.Subject.Events[_node._idStoryEvent]._listActions.Count > 0)
            {
                foreach (Machine.EventsMachine.Action act in _node._machineStory.Subject.Events[_node._idStoryEvent]._listActions)
                    labelsAction.Add(act._name);

                _node._idStoryAction = EditorGUILayout.Popup(_node._idStoryAction, labelsAction.ToArray());

                EditorGUILayout.LabelField("Current value : " + _node._machineStory.Subject.Events[_node._idStoryEvent]._listActions[_node._idStoryAction]._value, EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                EditorGUILayout.LabelField("You haven't 'story value' in this category", EditorStyles.helpBox);
            }
        }

        EditorGUILayout.EndVertical();
    }

    protected override void ChangeNodeEmitterStyle()
    {
        NodeConnector.SetStyleAt(_node.actStart.MainLayer, "T", "Emit if value is true", Color.green);
        NodeConnector.SetStyleAt(_node.actFinal.MainLayer, "F", "Emit if value is false", Color.red);
        NodeConnector.HiddenLayer(_node.actOnActive.MainLayer, true);
        NodeConnector.HiddenLayer(_node.actCheck.MainLayer, true);
        NodeConnector.HiddenLayer(_node.actProgress.MainLayer, true);
        NodeConnector.HiddenLayer(_node.actTouch.MainLayer, true);
    }

}
#endif

