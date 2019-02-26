using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using NodalInteractiveCreator.Objects.Puzzle;
using NodalInteractiveCreator.Editor.Objects;

namespace NodalInteractiveCreator.Editor.Puzzle
{
    [CustomEditor(typeof(PuzzleSystem))]
    public partial class PuzzleSystemEditor : InteractiveObjectEditor
    {
        int currentCell = 0;
        string[] options = new string[] { };

        static bool editConnection = false;
        private static bool editPosition = false;
        PuzzleSystem _target;
        private Vector2 previewSize;
        private Vector2 _scrollView;

        bool showElements = true;

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            //base.OnInspectorGUI();
            _target = (PuzzleSystem)target;
            if (_target == null)
                return;

            if (!TestNecessities(_target))
                return;

            serializedObject.Update();
            options = new string[_target.elements.Count];
            for (var i = 0; i < _target.elements.Count; i++)
                options[i] = _target.elements[i].type;


            DisplayGridProperties();
            DisplayGrid();
            DisplayDispositionMode();
            DisplayCell(currentCell);
            DisplayElements();
            DisplayActions();
            serializedObject.ApplyModifiedProperties();
            _target.OnValidate();

            if (EditorGUI.EndChangeCheck())
            {
                PrefabUtility.DisconnectPrefabInstance(target);
            }

        }

        void DisplayGridProperties()
        {
            EditorGUILayout.BeginVertical("Box");
            PuzzleSystem.displayElements = GUILayout.Toggle(PuzzleSystem.displayElements, "Display Wire elements");
            PuzzleSystem.displayConnectors = GUILayout.Toggle(PuzzleSystem.displayConnectors, "Display Connections");
            editPosition = GUILayout.Toggle(editPosition, "Edit elements positions");
            editConnection = GUILayout.Toggle(editConnection, "Edit connections");

            EditorGUILayout.Space();
            _target.tileSize = EditorGUILayout.FloatField("tile size", _target.tileSize);
            EditorGUILayout.LabelField("Map size : " + _target.gridSize.x + "  " + _target.gridSize.y);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("new map size", GUILayout.Width(120));
            previewSize = EditorGUILayout.Vector2Field("", previewSize, GUILayout.Width(120));
            if (previewSize != _target.gridSize && GUILayout.Button("apply", GUILayout.Width(60)))
            {
                _target.gridSize = previewSize;
                _target.OnGridSizeChange();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        void DisplayGrid()
        {
            EditorGUILayout.BeginVertical("box");
            _scrollView = EditorGUILayout.BeginScrollView(_scrollView, GUILayout.Height(60));

            for (var y = 0; y < _target.gridSize.y; y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (var x = 0; x < _target.gridSize.x; x++)
                {
                    int index = PuzzleMath.GtoI(x, y, _target.gridSize);

                    GUI.color = currentCell == index ? Color.blue : _target.playGrid[index].content.Count > 0 ? Color.white : Color.gray;

                    if (GUILayout.Button("" + x + " " + y))
                    {
                        currentCell = index;
                    }
                }
                EditorGUILayout.EndHorizontal();

            }

            EditorGUILayout.EndScrollView();

            GUI.color = Color.white;
            if (GUILayout.Button("Refresh grid"))
            {
                _target.OnGridSizeChange();
            }
            EditorGUILayout.EndVertical();
        }

        void DisplayElements()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUI.color = Color.gray;
            EditorGUILayout.BeginVertical("box");
            GUI.color = Color.white;
            showElements = EditorGUILayout.Foldout(showElements, "Elements");

            if (showElements)
            {
                for (var i = 0; i < _target.elements.Count; i++)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical("box");

                    PuzzleObjectDesc obj = _target.elements[i];

                    EditorGUILayout.LabelField("--------------------");
                    obj.type = EditorGUILayout.TextField("key Name", obj.type);
                    obj.colorTag = EditorGUILayout.ColorField("color tag", obj.colorTag);
                    obj.canBePushedBy = EditorGUILayout.MaskField("canBePushedBy", obj.canBePushedBy, options);
                    obj.canBlock = EditorGUILayout.MaskField("canBlock", obj.canBlock, options);
                    obj.interactiveByInput = EditorGUILayout.Toggle("InteractiveByTouch", obj.interactiveByInput);

                    if (obj.interactiveByInput)
                        obj.snapToCell = EditorGUILayout.Toggle("snaping to cell", obj.snapToCell);

                    obj.speed = Mathf.Clamp(EditorGUILayout.FloatField("transition speed", obj.speed), 0, 10);

                    obj.isLogical = EditorGUILayout.Toggle("is Logical", obj.isLogical);

                    obj.isTrigger = EditorGUILayout.Toggle("isTrigger", obj.isTrigger);

                    if (obj.isTrigger)
                    {
                        EditorGUILayout.HelpBox("Trigger will be fire when all objects Tagged are on the trigger's cell", MessageType.Info);
                        obj.triggerWith = EditorGUILayout.MaskField("Trigger With", obj.triggerWith, options);
                        obj.victoryLayer = EditorGUILayout.IntField("Victory Layer : ", obj.victoryLayer);
                    }

                    if (!obj.isLogical)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("elements.Array.data[" + i + "].element"));

                        if (obj.feedbacks == null || obj.feedbacks.Length != 5)
                            obj.feedbacks = new PuzzleObjectFeedback[5];

                        EditorGUILayout.LabelField("Feedback :", EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;

                        for (int id = 0; id < obj.feedbacks.Length; id++)
                        {
                            //Debug.Log(obj.canBlock + " " + obj.element.name);
                            if ((!obj.isTrigger && id == 2) || (obj.canBlock == 0 && id == 3))
                                continue;

                            DisplayFeedbackOnElementDesc(obj, id);
                        }
                        EditorGUI.indentLevel--;
                    }

                    _target.elements[i] = obj;

                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.LabelField("--------------------");

                if (GUILayout.Button("Add Element"))
                {
                    if (_target.elements == null)
                        _target.elements = new List<PuzzleObjectDesc>();

                    _target.elements.Add(new PuzzleObjectDesc());
                }
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Remove Element") && EditorUtility.DisplayDialog("Delete this Element ?", "", "Yes", "Nope"))
                {
                    if (_target.elements != null && _target.elements.Count > 0)
                        _target.elements.RemoveAt(_target.elements.Count - 1);

                    _target.OnGridSizeChange();
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        void DisplayActions()
        {
            EditorGUILayout.Space();
            GUI.color = Color.gray;
            EditorGUILayout.BeginVertical("box");
            GUI.color = Color.white;
            EditorGUILayout.LabelField("Actions Available", new GUIStyle() { fontSize = 15 });

            if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(20)))
                _target.actions.Add(new PuzzleAction());

            for (int i = 0; i < _target.actions.Count; i++)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)) &&
                    EditorUtility.DisplayDialog("Delete this action ?", "", "Yes", "Nope"))
                {
                    _target.actions.RemoveAt(i++);
                    continue;
                }


                EditorGUILayout.LabelField("Unique ID", GUILayout.Width(Screen.width / 6));
                _target.actions[i].uniqueID = i;
                EditorGUILayout.LabelField("" + _target.actions[i].uniqueID, GUILayout.Width(Screen.width / 6));
                EditorGUILayout.LabelField("Name", GUILayout.Width(Screen.width / 6));
                _target.actions[i].keyName = EditorGUILayout.TextField(_target.actions[i].keyName, GUILayout.Width(Screen.width / 6));
                EditorGUILayout.EndHorizontal();
                _target.actions[i].angleMove = Mathf.Clamp(EditorGUILayout.FloatField("Angle Move", _target.actions[i].angleMove, GUILayout.Width(Screen.width / 2)), 0, 360);
                _target.actions[i].subjectsFlag = EditorGUILayout.MaskField("Subjects", _target.actions[i].subjectsFlag, options, GUILayout.Width(Screen.width / 2));
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();

        }

        float offsetRadius = 3;
        private float curveAngle = 0;

        void DisplayDispositionMode()
        {
            EditorGUILayout.BeginVertical("box");

            if (GUILayout.Button("Mode Grid"))
            {
                Undo.RecordObject(target, "Puzzle system modification");
                _target.SetGridMode();
            }

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Offset", GUILayout.Width(Screen.width / 6f));
            offsetRadius = EditorGUILayout.FloatField(offsetRadius, GUILayout.Width(Screen.width / 10f));
            EditorGUILayout.LabelField("curving", GUILayout.Width(Screen.width / 6f));
            curveAngle = Mathf.Clamp(EditorGUILayout.FloatField(curveAngle, GUILayout.Width(Screen.width / 10f)), -180, 180);


            if (GUILayout.Button("Mode Circle"))
            {

                Undo.RecordObject(target, "Puzzle system modification");
                _target.SetCircleMode(offsetRadius, curveAngle);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        bool TestNecessities(PuzzleSystem _target)
        {
            if (_target.playGrid == null || _target.playGrid.Length <= 0)
            {
                _target.gridSize = PuzzleSystem.minGridSize;
                _target.OnGridSizeChange();
                return false;
            }
            return true;
        }

        void DisplayFeedbackOnElementDesc(PuzzleObjectDesc pod, int id)
        {
            string[] nameFeed = new string[5] { "Idle", "Move", "Trigger", "Block", "Validate" };
            List<string> listAnim = new List<string>() { "NONE" };
            foreach (AnimationState animClip in pod.element.GetComponent<Animation>())
                listAnim.Add(animClip.name);


            EditorGUILayout.LabelField(nameFeed[id] + " :");
            EditorGUILayout.BeginHorizontal();
            EditorGUI.indentLevel++;

            pod.feedbacks[id].idAnime = EditorGUILayout.Popup("Animation name ", pod.feedbacks[id].idAnime, listAnim.ToArray());
            pod.feedbacks[id].animationClip = pod.element.GetComponent<Animation>().GetClip(listAnim[pod.feedbacks[id].idAnime]);

            if (pod.feedbacks[id].idAnime == 0)
                pod.feedbacks[id].animationClip = null;

            pod.feedbacks[id].audioClip = EditorGUILayout.ObjectField("", pod.feedbacks[id].audioClip, typeof(AudioClip), true, GUILayout.Width(150)) as AudioClip;

            EditorGUI.indentLevel--;
            EditorGUILayout.EndHorizontal();
        }
    }
}