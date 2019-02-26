using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.CustomEditing;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using System;
using UnityEngine;
using UnityEditor;
using NodalInteractiveCreator.Objects;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using NodalInteractiveCreator.Endoscop;

#if UNITY_EDITOR
[Serializable]
public abstract class InteractiveNodeEditor<TNode, TMono> : NodalEditorCustom where TNode : InteractiveNode<TMono> where TMono : InteractiveObject
{
    protected TNode _node;
    protected TMono _mono;

    public InteractiveNodeEditor(Type t) : base(t) { }

    public override Rect GetCustomBounds()
    {
        bounds.size = new Vector2(200, 100);
        return bounds;
    }

    public override void OnGUI()
    {
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

        EditorGUILayout.LabelField("Object source", EditorStyles.centeredGreyMiniLabel);
        _node._source.Assign(EditorGUILayout.ObjectField(_node._source.Subject, typeof(TMono), true) as TMono);
        _mono = _node._source.Subject as TMono;

        EditorGUILayout.EndVertical();
    }

    protected virtual void ChangeNodeEmitterStyle()
    {
        if (_mono == null)
            return;

        switch (_mono.GetType().Name)
        {
            case "EndoscopeEntrence" :
                EndoscopeEntrence ee = _mono as EndoscopeEntrence;

                NodeConnector.HiddenLayer(_node.actStart.MainLayer, true);
                NodeConnector.HiddenLayer(_node.actFinal.MainLayer, true);
                NodeConnector.HiddenLayer(_node.actProgress.MainLayer, true);
                NodeConnector.HiddenLayer(_node.actTouch.MainLayer, true);

                if (ee.linkedScene != null)
                {
                    int nbPath = ee.linkedScene.specs.path.Count;
                    if (nbPath > 0)
                    {
                        NodeConnector.HiddenLayer(_node.actCheck.MainLayer, false);

                        while (_node.actCheck.LayerCount != nbPath)
                        {
                            if (_node.actCheck.LayerCount < nbPath)
                                _node.actCheck.AddLayer((_node.actCheck.LayerCount).ToString(), "Node path point #" + (_node.actCheck.LayerCount).ToString(), Color.cyan);
                            else if (_node.actCheck.LayerCount > nbPath)
                                _node.actCheck.RemoveLayerAt(_node.actCheck.LayerCount - 1);
                        }
                    }
                    else if (nbPath <= 0)
                    {
                        _node.actCheck.ResetLayers();
                        NodeConnector.HiddenLayer(_node.actCheck.MainLayer, true);
                    }
                }
                else
                    NodeConnector.HiddenLayer(_node.actCheck.MainLayer, true);
                break;

            case "InteractiveObject":
                NodeConnector.HiddenLayer(_node.actStart.MainLayer, true);
                NodeConnector.HiddenLayer(_node.actFinal.MainLayer, true);
                NodeConnector.HiddenLayer(_node.actCheck.MainLayer, true);
                NodeConnector.HiddenLayer(_node.actProgress.MainLayer, true);
                NodeConnector.HiddenLayer(_node.actTouch.MainLayer, false);
                break;

            case "RotatableObject":
                RotatableObject ro = _mono as RotatableObject;

                NodeConnector.HiddenLayer(_node.actStart.MainLayer, false);
                NodeConnector.HiddenLayer(_node.actFinal.MainLayer, false);
                NodeConnector.HiddenLayer(_node.actProgress.MainLayer, false);
                NodeConnector.HiddenLayer(_node.actTouch.MainLayer, _mono._interactable);

                if (ro.NbCheck > 0)
                {
                    NodeConnector.HiddenLayer(_node.actCheck.MainLayer, false);

                    while (_node.actCheck.LayerCount != ro.NbCheck)
                    {
                        if(_node.actCheck.LayerCount < ro.NbCheck)
                            _node.actCheck.AddLayer();
                        else if (_node.actCheck.LayerCount > ro.NbCheck)
                            _node.actCheck.RemoveLayerAt(_node.actCheck.LayerCount - 1);
                    }
                }
                else if (ro.NbCheck <= 0)
                {
                    _node.actCheck.ResetLayers();
                    NodeConnector.HiddenLayer(_node.actCheck.MainLayer, true);
                }
                break;

            case "TranslatableObject":
                TranslatableObject to = _mono as TranslatableObject;

                NodeConnector.HiddenLayer(_node.actStart.MainLayer, false);
                NodeConnector.HiddenLayer(_node.actFinal.MainLayer, false);
                NodeConnector.HiddenLayer(_node.actProgress.MainLayer, false);
                NodeConnector.HiddenLayer(_node.actTouch.MainLayer, _mono._interactable);

                if (to.NbCheck > 0)
                {
                    NodeConnector.HiddenLayer(_node.actCheck.MainLayer, false);

                    while (_node.actCheck.LayerCount != to.NbCheck)
                    {
                        if (_node.actCheck.LayerCount < to.NbCheck)
                            _node.actCheck.AddLayer();
                        else if (_node.actCheck.LayerCount > to.NbCheck)
                            _node.actCheck.RemoveLayerAt(_node.actCheck.LayerCount - 1);
                    }
                }
                else if(to.NbCheck <= 0)
                {
                    _node.actCheck.ResetLayers();
                    NodeConnector.HiddenLayer(_node.actCheck.MainLayer, true);
                }
                break;

            case "PushObject":
                PushObject po = _mono as PushObject;

                NodeConnector.HiddenLayer(_node.actStart.MainLayer, false);
                NodeConnector.HiddenLayer(_node.actProgress.MainLayer, false);
                NodeConnector.HiddenLayer(_node.actCheck.MainLayer, true);
                NodeConnector.HiddenLayer(_node.actTouch.MainLayer, _mono._interactable);

                if (po._twoStep)
                {
                    NodeConnector.HiddenLayer(_node.actFinal.MainLayer, false);
                    NodeConnector.SetStyleAt(_node.actFinal.MainLayer, "F", "Emit if on final", Color.red);
                }
                else
                {
                    NodeConnector.HiddenLayer(_node.actFinal.MainLayer, !po._onPress);

                    if (po._onPress)
                        NodeConnector.SetStyleAt(_node.actFinal.MainLayer, "P", "Emit if pressed", Color.red);
                }
                break;

            case "PickableObject":
                NodeConnector.HiddenLayer(_node.actStart.MainLayer, false);
                NodeConnector.SetStyleAt(_node.actStart.MainLayer, "P", "Emit if Pick up", Color.green);
                NodeConnector.HiddenLayer(_node.actFinal.MainLayer, true);
                NodeConnector.HiddenLayer(_node.actCheck.MainLayer, true);
                NodeConnector.HiddenLayer(_node.actProgress.MainLayer, true);
                NodeConnector.HiddenLayer(_node.actTouch.MainLayer, true);
                break;

            case "ItemTarget":
                ItemTarget it = _mono as ItemTarget;

                NodeConnector.HiddenLayer(_node.actStart.MainLayer, false);
                NodeConnector.SetStyleAt(_node.actStart.MainLayer, "U", "Emit if Unblock", Color.green);
                NodeConnector.HiddenLayer(_node.actFinal.MainLayer, !it._blockEnabled);
                if(it._blockEnabled)
                    NodeConnector.SetStyleAt(_node.actFinal.MainLayer, "B", "Emit if Block", Color.red);

                NodeConnector.HiddenLayer(_node.actProgress.MainLayer, true);
                NodeConnector.HiddenLayer(_node.actCheck.MainLayer, true);
                NodeConnector.HiddenLayer(_node.actTouch.MainLayer, false);
                break;

            default:
                NodeConnector.HiddenLayer(_node.actStart.MainLayer, false);
                NodeConnector.HiddenLayer(_node.actFinal.MainLayer, true);
                NodeConnector.HiddenLayer(_node.actCheck.MainLayer, true);
                NodeConnector.HiddenLayer(_node.actProgress.MainLayer, true);
                NodeConnector.HiddenLayer(_node.actTouch.MainLayer, true);
                break;
        }
    }

}

[CustomNodalEditor(typeof(InteractiveNode))]
public class InteractiveNodeEditor : InteractiveNodeEditor<InteractiveNode, InteractiveObject>
{
    public InteractiveNodeEditor(Type t) : base(t) { }

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
