using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.CustomEditing;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Game.TheoricalNodes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using NodalInteractiveCreator.Controllers;
using NodalInteractiveCreator.Objects;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR
[Serializable]
public abstract class ActionNodeEditor<TNode> : NodalEditorCustom where TNode : ActionNode
{
    protected TNode _node;

    protected string[] _labelsBool = {"True","False"};
    protected List<string> _labelsEmitter = new List<string>();
    protected List<string> _listChecks = new List<string>();

    protected Vector2 _scrollPos;

    public ActionNodeEditor(Type t) : base(t) { }

    public override void OnGUI()
    {
        base.OnGUI();

        _node = Subject as TNode;

        if (_node != null)
        {
            ShowOnNodeInit();
            ChangeNodeEmitterStyle();
        }
    }

    protected virtual void ShowOnNodeInit()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("Object target", EditorStyles.centeredGreyMiniLabel);
        _node._target.Assign(EditorGUILayout.ObjectField(_node._target.Subject, typeof(InteractiveObject), true) as InteractiveObject);

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }


    protected virtual void ChangeNodeEmitterStyle()
    {
        NodeConnector.HiddenLayer(_node.actStart.MainLayer, true);
        NodeConnector.HiddenLayer(_node.actFinal.MainLayer, true);
        NodeConnector.HiddenLayer(_node.actCheck.MainLayer, true);
        NodeConnector.HiddenLayer(_node.actProgress.MainLayer, true);
        NodeConnector.HiddenLayer(_node.actTouch.MainLayer, true);
    }

    protected List<string> GetListNameOf(IList<UnityEngine.Object> list)
    {
        if (list == null)
            return null;

        List<string> names = new List<string>();

        foreach (UnityEngine.Object t in list)
           if(t != null)
                names.Add(t.name);

        return names;
    }

    #region MyPopup
    protected bool PopupForLabelsBool(string labelField, bool value, string newLabelTrue, string newLabelFalse)
    {
        _labelsBool[0] = string.IsNullOrEmpty(newLabelTrue) ? _labelsBool[0] : newLabelTrue;
        _labelsBool[1] = string.IsNullOrEmpty(newLabelFalse) ? _labelsBool[1] : newLabelFalse;

        if(!string.IsNullOrEmpty(labelField))
            EditorGUILayout.LabelField(labelField, EditorStyles.centeredGreyMiniLabel);

        int id = value ? 0 : 1;
        value = EditorGUILayout.Popup(id, _labelsBool) == 0 ? true : false;

        return value;
    }

    protected bool PopupForLabelsBool(GUIContent labelField, bool value, string newLabelTrue, string newLabelFalse)
    {
        _labelsBool[0] = string.IsNullOrEmpty(newLabelTrue) ? _labelsBool[0] : newLabelTrue;
        _labelsBool[1] = string.IsNullOrEmpty(newLabelFalse) ? _labelsBool[1] : newLabelFalse;

        EditorGUILayout.LabelField(labelField, EditorStyles.centeredGreyMiniLabel);
        int id = value ? 0 : 1;
        value = EditorGUILayout.Popup(id, _labelsBool) == 0 ? true : false;

        return value;
    }

    protected void InitLabelsEmitterNodeForPopup(out List<string> labelsEmitter, out List<string> listChecks)
    {
        labelsEmitter = new List<string>();
        listChecks = new List<string>();

        if (_node._target.Subject is ItemTarget)
        {
            labelsEmitter.Add("Unblock");

            if (((ItemTarget)_node._target.Subject)._blockEnabled)
                labelsEmitter.Add("Block");

            labelsEmitter.Add("Touch");
        }
        else if (_node._target.Subject is PickableObject)
        {
            labelsEmitter.Add("Start");
            labelsEmitter.Add("Final");
        }
        else if(_node._target.Subject is PushObject)
        {
            labelsEmitter.Add("Start");

            if (((PushObject)_node._target.Subject)._twoStep)
                labelsEmitter.Add("Final");
            else if (((PushObject)_node._target.Subject)._onPress)
                labelsEmitter.Add("Press");
        }
        else if(_node._target.Subject is TranslatableObject)
        {
            labelsEmitter.Add("Start");
            labelsEmitter.Add("Final");

            if (((TranslatableObject)_node._target.Subject).NbCheck > 0)
            {
                labelsEmitter.Add("Check");

                for (int i = 0; i < ((TranslatableObject)_node._target.Subject).NbCheck; i++)
                    listChecks.Add(i.ToString());
            }
        }
        else if(_node._target.Subject is RotatableObject)
        {
            labelsEmitter.Add("Start");
            labelsEmitter.Add("Final");
            labelsEmitter.Add("Check");

            if (((RotatableObject)_node._target.Subject).NbCheck > 0)
            {
                labelsEmitter.Add("Check");
                for (int i = 0; i < ((RotatableObject)_node._target.Subject).NbCheck; i++)
                    listChecks.Add(i.ToString());
            }
        }
        else if (_node._target.Subject is InteractiveObject)
        {
            labelsEmitter.Add("Start");
        }
    }
    #endregion

}
#endif
