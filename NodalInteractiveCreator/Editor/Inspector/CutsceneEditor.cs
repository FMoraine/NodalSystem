// Machinika Museum Prototype v.01
// © Littlefield Studio
// Writted by Franck-Olivier FILIN - 2017
//
// CutsceneEditor.cs

#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using NodalInteractiveCreator.Controllers;
using NodalInteractiveCreator.Viewpoints;

namespace NodalInteractiveCreator.Editor
{
    [CustomEditor(typeof(Cutscene))]
    public class CutsceneEditor : UnityEditor.Editor
    {
        Cutscene _myCutsceneTarget;

        private void OnEnable()
        {
            _myCutsceneTarget = (Cutscene)target;
        }

        public override void OnInspectorGUI()
        {
            _myCutsceneTarget._screenFade = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Screen for Fade"), _myCutsceneTarget._screenFade, typeof(GameObject), true);
            _myCutsceneTarget._cutsceneAnimator = EditorGUILayout.ObjectField(new GUIContent("Cutscene Animator"), _myCutsceneTarget._cutsceneAnimator, typeof(Animator), true) as Animator;

            _myCutsceneTarget._showToolsHUD = EditorGUILayout.Toggle("Show Tools HUD", _myCutsceneTarget._showToolsHUD);
            _myCutsceneTarget._showItemsHUD = EditorGUILayout.Toggle("Show Items HUD", _myCutsceneTarget._showItemsHUD);

            if (null != _myCutsceneTarget._cutsceneAnimator)
            {
                int sizeList;
                sizeList = _myCutsceneTarget._pointCutscene.Count;
                sizeList = EditorGUILayout.DelayedIntField("Checkpoint Cutscene", sizeList);

                while (_myCutsceneTarget._pointCutscene.Count != sizeList)
                {
                    if (_myCutsceneTarget._pointCutscene.Count > sizeList)
                        _myCutsceneTarget._pointCutscene.RemoveAt(_myCutsceneTarget._pointCutscene.Count - 1);
                    else if (sizeList > _myCutsceneTarget._pointCutscene.Count)
                        _myCutsceneTarget._pointCutscene.Add(new Cutscene.CheckpointCutscene());

                    if (_myCutsceneTarget._pointCutscene.Count == 0)
                        return;
                }

                List<string> listState = new List<string>();

                foreach (AnimationClip state in _myCutsceneTarget._cutsceneAnimator.runtimeAnimatorController.animationClips)
                    if (state.name != "INTRO")
                        listState.Add(state.name);

                EditorGUI.indentLevel++;

                foreach (Cutscene.CheckpointCutscene checkpoint in _myCutsceneTarget._pointCutscene)
                {
                    EditorGUILayout.LabelField("--------------------------------------", EditorStyles.boldLabel);

                    int id;
                    if (string.IsNullOrEmpty(checkpoint._nameCuts))
                    {
                        id = 0;
                        EditorGUILayout.HelpBox("You haven't created Cutscene ", MessageType.None);
                        return;
                    }
                    else
                        id = listState.IndexOf(checkpoint._nameCuts);

                    id = EditorGUILayout.Popup("Name Custscene", id, listState.ToArray());
                    checkpoint._nameCuts = listState[id];

                    checkpoint._switchToCutsceneCamera = EditorGUILayout.ToggleLeft(new GUIContent("Switch to Custscene"), checkpoint._switchToCutsceneCamera);

                    if (checkpoint._switchToCutsceneCamera)
                    {
                        ShowSwitchCameraConfig(checkpoint);
                    }

                    if (checkpoint._nameCuts != "OUTRO")
                        checkpoint._vpFinalCuts = (ViewPoint)EditorGUILayout.ObjectField(new GUIContent("Viewpoint Final ", "Viewpoint of the Main Camera to go at end of cutscene. If empty, the default viewpoint is the 'Default viewpoint' from Main Camera"), checkpoint._vpFinalCuts, typeof(ViewPoint), true);
                    else if (checkpoint._nameCuts == "OUTRO")
                    {
                        _myCutsceneTarget._screenValidate = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Screen for Validate"), _myCutsceneTarget._screenValidate, typeof(GameObject), true);

                        if (_myCutsceneTarget._screenValidate == null)
                            _myCutsceneTarget._nameSceneToLoad = EditorGUILayout.TextField("Scene to load", _myCutsceneTarget._nameSceneToLoad);
                        else
                            EditorGUILayout.HelpBox("Write the next level in 'ValidateScreen' GameObject", MessageType.None);
                    }

                    EditorGUILayout.Space();
                }
                EditorGUI.indentLevel--;
            }
        }

        private void ShowSwitchCameraConfig(Cutscene.CheckpointCutscene checkpoint)
        {
            EditorGUILayout.LabelField("Main Camera Configuration :", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            checkpoint._posInitCuts = EditorGUILayout.Vector3Field(new GUIContent("Position Init", "Initial position of the Main Camera go to before the cutscence"), checkpoint._posInitCuts);
            checkpoint._rotInitCuts = EditorGUILayout.Vector3Field(new GUIContent("Rotation Init", "Initial rotation of the Main Camera go to before the cutscence"), checkpoint._rotInitCuts);

            if (AnimationMode.InAnimationMode() && GUILayout.Button("Copy Position/Rotation", EditorStyles.miniButton))
            {
                checkpoint._posInitCuts = new Vector3(_myCutsceneTarget.transform.position.x, _myCutsceneTarget.transform.position.y, _myCutsceneTarget.transform.position.z);
                checkpoint._rotInitCuts = new Vector3(_myCutsceneTarget.transform.eulerAngles.x, _myCutsceneTarget.transform.eulerAngles.y, _myCutsceneTarget.transform.eulerAngles.z);
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            ShowBeforeCutsceneConfig(checkpoint);

            EditorGUILayout.LabelField("--------------------------------------", EditorStyles.centeredGreyMiniLabel);

            EditorGUILayout.Space();

            ShowAfterCutsceneConfig(checkpoint);

            EditorGUILayout.Space();
        }

        private void ShowBeforeCutsceneConfig(Cutscene.CheckpointCutscene checkpoint)
        {
            EditorGUILayout.LabelField("BEFORE CUTSCENE :", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            checkpoint._moveTo = EditorGUILayout.Toggle(new GUIContent("Move to"), checkpoint._moveTo, EditorStyles.radioButton);
            checkpoint._spawnTo = !checkpoint._moveTo;

            EditorGUI.indentLevel++;
            if (checkpoint._moveTo)
                checkpoint._durationMoveTo = EditorGUILayout.Slider(new GUIContent("Duration"), checkpoint._durationMoveTo, 1f, 5f);
            else
                checkpoint._durationMoveTo = 0;
            EditorGUI.indentLevel--;

            checkpoint._spawnTo = EditorGUILayout.Toggle(new GUIContent("Spawn to"), checkpoint._spawnTo, EditorStyles.radioButton);
            checkpoint._moveTo = !checkpoint._spawnTo;

            EditorGUI.indentLevel--;


            EditorGUILayout.LabelField("FADE IN :", EditorStyles.boldLabel);

            checkpoint._fadeToCutscene = EditorGUILayout.ToggleLeft(new GUIContent("Active Fade In ?"), checkpoint._fadeToCutscene);

            if (checkpoint._fadeToCutscene)
            {
                if (checkpoint._moveTo)
                {
                    checkpoint._fadeWithMoveTo = EditorGUILayout.Toggle(new GUIContent("Fade during Move To"), checkpoint._fadeWithMoveTo);

                    if (checkpoint._fadeWithMoveTo)
                        checkpoint._durationFadeTo = checkpoint._durationMoveTo;
                    else
                    {
                        EditorGUI.indentLevel++;
                        checkpoint._durationFadeTo = EditorGUILayout.Slider(new GUIContent("Duration"), checkpoint._durationFadeTo, 0.1f, 5f);
                        EditorGUI.indentLevel--;

                        EditorGUILayout.LabelField("The fade in will played after the move to during " + checkpoint._durationFadeTo + " seconds", EditorStyles.helpBox);
                    }
                }
                else
                {
                    checkpoint._fadeWithMoveTo = false;

                    EditorGUI.indentLevel++;
                    checkpoint._durationFadeTo = EditorGUILayout.Slider(new GUIContent("Duration"), checkpoint._durationFadeTo, 0.1f, 5f);
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                checkpoint._fadeWithMoveTo = false;
                checkpoint._durationFadeTo = 0;
            }
        }

        private void ShowAfterCutsceneConfig(Cutscene.CheckpointCutscene checkpoint)
        {
            EditorGUILayout.LabelField("AFTER CUTSCENE :", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            checkpoint._moveToEnd = EditorGUILayout.Toggle(new GUIContent("Move to"), checkpoint._moveToEnd, EditorStyles.radioButton);
            checkpoint._spawnToEnd = !checkpoint._moveToEnd;

            EditorGUI.indentLevel++;
            if (checkpoint._moveToEnd)
                checkpoint._durationMoveToEnd = EditorGUILayout.Slider(new GUIContent("Duration"), checkpoint._durationMoveToEnd, 1f, 5f);
            else
                checkpoint._durationMoveToEnd = 0;
            EditorGUI.indentLevel--;

            checkpoint._spawnToEnd = EditorGUILayout.Toggle(new GUIContent("Spawn to"), checkpoint._spawnToEnd, EditorStyles.radioButton);
            checkpoint._moveToEnd = !checkpoint._spawnToEnd;

            EditorGUI.indentLevel--;


            EditorGUILayout.LabelField("FADE OUT :", EditorStyles.boldLabel);

            checkpoint._fadeToCutsceneEnd = EditorGUILayout.ToggleLeft(new GUIContent("Active Fade Out ?"), checkpoint._fadeToCutsceneEnd);

            if (checkpoint._fadeToCutsceneEnd)
            {
                if (checkpoint._moveToEnd)
                {
                    checkpoint._fadeWithMoveToEnd = EditorGUILayout.Toggle(new GUIContent("Fade during Move To"), checkpoint._fadeWithMoveToEnd);

                    if (checkpoint._fadeWithMoveToEnd)
                        checkpoint._durationFadeToEnd = checkpoint._durationMoveToEnd;
                    else
                    {
                        EditorGUI.indentLevel++;
                        checkpoint._durationFadeToEnd = EditorGUILayout.Slider(new GUIContent("Duration"), checkpoint._durationFadeToEnd, 0.1f, 5f);
                        EditorGUI.indentLevel--;

                        EditorGUILayout.LabelField("The fade out will played before the move to during " + checkpoint._durationFadeToEnd + " seconds", EditorStyles.helpBox);
                    }
                }
                else
                {
                    checkpoint._fadeWithMoveToEnd = false;

                    EditorGUI.indentLevel++;
                    checkpoint._durationFadeToEnd = EditorGUILayout.Slider(new GUIContent("Duration"), checkpoint._durationFadeToEnd, 0.1f, 5f);
                    EditorGUI.indentLevel--;
                }
            }
            else
            {

                checkpoint._fadeWithMoveToEnd = false;
                checkpoint._durationFadeToEnd = 0;
            }
        }
    }
}
#endif
