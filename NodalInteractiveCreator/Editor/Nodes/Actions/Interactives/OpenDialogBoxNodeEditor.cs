using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.CustomEditing;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using NodalInteractiveCreator.Controllers;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR
[CustomNodalEditor(typeof(OpenDialogBoxNode))]
public class OpenDialogBoxNodeEditor : ActionNodeEditor<OpenDialogBoxNode>
{
    public OpenDialogBoxNodeEditor(Type t) : base(t){}

    public override Rect GetCustomBounds()
    {
        bounds.size = new Vector2(250, 125);
        return bounds;
    }

    public override void OnGUI()
    {
        base.OnGUI();
    }

    protected override void ShowOnNodeInit()
    {
        if (_node._sentenceTranslate.Subject == null)
            _node._sentenceTranslate.Assign(UnityEngine.Object.FindObjectOfType<SentenceTranslate>());

        EditorGUILayout.BeginVertical();

        if (_node._sentenceTranslate.Subject == null)
        {
            EditorGUILayout.LabelField("Sentence Translate", EditorStyles.centeredGreyMiniLabel);
            _node._sentenceTranslate.Assign(EditorGUILayout.ObjectField(_node._sentenceTranslate.Subject, typeof(SentenceTranslate), true) as SentenceTranslate);
        }
        else
        {
            _node._sentenceTranslate.Subject.Load(_node._sentenceTranslate.Subject._file);

            List<string> listKeyMessage = new List<string>();
            foreach (SentenceTranslate.Row row in _node._sentenceTranslate.Subject.GetRowList())
                listKeyMessage.Add(row.KEY);

            EditorGUILayout.LabelField("Key Message", EditorStyles.centeredGreyMiniLabel);
            _node._idSentence = EditorGUILayout.Popup(_node._idSentence, listKeyMessage.ToArray());

            EditorGUILayout.LabelField(_node._sentenceTranslate.Subject.GetAt(_node._idSentence).SENTENCE_FR, EditorStyles.helpBox);
            EditorGUILayout.LabelField(_node._sentenceTranslate.Subject.GetAt(_node._idSentence).SENTENCE_EN, EditorStyles.helpBox);

            _node._oneShot = PopupForLabelsBool(new GUIContent("Only once ? ", "Show this DialogBox once."), _node._oneShot, null, null);
        }

        EditorGUILayout.EndVertical();
    }

    protected override void ChangeNodeEmitterStyle()
    {
        NodeConnector.SetStyleAt(_node.actStart.MainLayer, "O", "Called when the dialog box is openned", Color.green);
        NodeConnector.SetStyleAt(_node.actFinal.MainLayer, "C", "Called when the dialog box is closed", Color.red);
        NodeConnector.HiddenLayer(_node.actProgress.MainLayer, true);
        NodeConnector.HiddenLayer(_node.actCheck.MainLayer, true);
        NodeConnector.HiddenLayer(_node.actTouch.MainLayer, true);
    }

}
#endif
