using UnityEngine;
using UnityEditor;
using NodalInteractiveCreator.Objects.Puzzle;

namespace NodalInteractiveCreator.Editor.Puzzle
{
    public partial class PuzzleSystemEditor
    {
        void DisplayCell(int index)
        {
            if (index >= _target.playGrid.Length || index < 0)
                return;

            Vector2 gridPos = PuzzleMath.ItoG(index, _target.gridSize);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("--------------------");

            EditorGUILayout.LabelField("Selected Cell : " + gridPos.x + " " + gridPos.y);

            PuzzleCell cell = _target.playGrid[index];
            int newFlag = 0;

            if (options.Length > 0)
                newFlag = EditorGUILayout.MaskField("Elements inside :", cell.flag, options);

            cell.localPosition = EditorGUILayout.Vector3Field("Local Pos", cell.localPosition);

            if (newFlag != cell.flag)
            {
                cell.content = PuzzleMath.FlagToObjects(newFlag, _target.elements.Count);
                cell.flag = newFlag;
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            DisplayCellConnections(cell);
            _target.playGrid[index] = cell;

            EditorGUILayout.LabelField("--------------------");
            EditorGUILayout.EndVertical();


        }

        void DisplayCellConnections(PuzzleCell cell)
        {
            for (int i = cell.paths.Count - 1; i >= 0; i--)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    cell.paths.RemoveAt(i);
                    return;
                }


                EditorGUILayout.LabelField("Connected to : " + cell.paths[i].gridPos.x + " : " + cell.paths[i].gridPos.y, GUILayout.Width(Screen.width / 3));
                cell.paths[i].angleSpread = Mathf.Clamp(EditorGUILayout.FloatField(cell.paths[i].angleSpread, GUILayout.Width(Screen.width / 8)), 10, 360);

                cell.paths[i].angleMedian = EditorGUILayout.Slider(cell.paths[i].angleMedian, 0, 359, GUILayout.Width(Screen.width / 8));


                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();

        }

        void DisplayGUIConnectionEditor()
        {
            Handles.BeginGUI();
            Camera c = Camera.current;
            if (!c)
                return;

            int size = _target.playGrid.Length;
            for (var i = 0; i < size; i++)
            {
                Vector3 screenPos = c.WorldToScreenPoint(_target.transform.position + _target.transform.rotation * _target.playGrid[i].localPosition);

                if (i == currentCell)
                {
                    GUI.color = new Color(0, 0, 1, 0.8f);
                    GUI.Box(new Rect(screenPos.x, Screen.height - screenPos.y - 40, 60, 20), "Target");


                }
                else
                {
                    PuzzleCell cell = _target.playGrid[currentCell];
                    int connectionIndex = -1;

                    for (var b = 0; b < cell.paths.Count; b++)
                        if (PuzzleMath.GtoI(cell.paths[b].gridPos, _target.gridSize) == i)
                            connectionIndex = b;

                    GUI.color = connectionIndex > -1 ? new Color(1, 0, 0, 0.8f) : new Color(0, 1, 0, 0.8f);

                    if (GUI.Button(new Rect(screenPos.x, Screen.height - screenPos.y - 40, 20, 20), connectionIndex > -1 ? "-" : "+"))
                    {
                        Undo.RecordObject(target, "Puzzle system modification");

                        if (connectionIndex > -1)
                            cell.paths.RemoveAt(connectionIndex);
                        else
                            cell.AddConnection(0, 70, PuzzleMath.ItoG(i, _target.gridSize), new BezierCurve(Vector3.up, Vector3.down));
                    }
                    GUI.color = Color.white;
                    if (GUI.Button(new Rect(screenPos.x - 30, Screen.height - screenPos.y - 40, 20, 20), "O"))
                    {
                        Undo.RecordObject(target, "Puzzle system modification");
                        currentCell = i;
                        EditorUtility.SetDirty(target);
                    }


                }
            }


            Handles.EndGUI();
        }

        void OnSceneGUI()
        {
            if (Application.isPlaying)
                return;

            _target = (PuzzleSystem)target;
            if (!TestNecessities(_target) || options == null)
                return;

            if (editConnection)
                DisplayGUIConnectionEditor();

            DisplayCellConnections();

            if (currentCell >= _target.playGrid.Length || currentCell < 0)
                currentCell = 0;

            DisplayBezierHandles(_target.playGrid[currentCell]);
        }



        void DisplayCellConnections()
        {
            int size = _target.gridSize.x * _target.gridSize.y;

            for (int i = 0; i < size; i++)
            {
                PuzzleCell cell = _target.playGrid[i];
                Vector3 o = _target.t.position;

                Vector3 worldPos = _target.t.rotation * cell.localPosition + o;

                if (editPosition)
                {
                    Vector3 pos = Quaternion.Inverse(_target.t.rotation) *
                                  (Handles.PositionHandle(worldPos, cell.worldOrientation) - o);

                    if (pos != cell.localPosition)
                    {
                        Undo.RecordObject(target, "Puzzle system modification");
                        cell.localPosition = pos;
                    }
                }

                foreach (var connection in cell.paths)
                {
                    BezierCurve b = connection.bezierPath;
                    int index = PuzzleMath.GtoI(connection.gridPos, _target.gridSize);

                    if (index >= size || index < 0)
                        continue;

                    Vector3 curve2 = _target.t.rotation * _target.playGrid[index].Orientation * b.curvePoint2;

                    Vector3 pos1 = cell.worldPos;
                    Vector3 pos2 = _target.playGrid[index].worldPos;

                    if (Application.isPlaying)
                        Handles.DrawBezier(o + _target.t.rotation * b.point1, o + _target.t.rotation * b.point2, o + _target.t.rotation * b.curvePoint1, o + _target.t.rotation * b.curvePoint2, Color.red, null, 2f);
                    else
                        Handles.DrawBezier(pos1, pos2, pos1 + _target.t.rotation * b.curvePoint1, pos2 + curve2, Color.red, null, 2f);
                }
            }
        }


        void DisplayBezierHandles(PuzzleCell cell)
        {
            if (PuzzleSystem.displayConnectors)
            {
                Vector3 o = _target.t.position;

                for (int j = 0; j < cell.paths.Count; j++)
                {
                    BezierCurve b = cell.paths[j].bezierPath;
                    Vector3 worldPos = _target.t.rotation * (cell.localPosition + b.curvePoint1) + o;
                    Vector3 worldPosCell = _target.t.rotation * cell.localPosition + o;
                    Vector3 pos = Quaternion.Inverse(_target.t.rotation) *
                                  (Handles.PositionHandle(worldPos, Quaternion.identity) - o) - cell.localPosition;

                    Handles.DrawDottedLine(worldPosCell, worldPos, 0.2f);

                    if (pos != b.curvePoint1)
                    {
                        Undo.RecordObject(target, "Puzzle system modification");
                        b.curvePoint1 = pos;
                    }

                    int index = PuzzleMath.GtoI(cell.paths[j].gridPos, _target.gridSize);

                    if (index < 0 || index >= _target.gridSize.x * _target.gridSize.y)
                        continue;

                    PuzzleCell otherCell = _target.playGrid[index];

                    worldPos = _target.t.rotation * (otherCell.localPosition + otherCell.Orientation * b.curvePoint2) + o;
                    worldPosCell = _target.t.rotation * otherCell.localPosition + o;
                    pos = Quaternion.Inverse(otherCell.Orientation) * (Quaternion.Inverse(_target.t.rotation) *
                                  (Handles.PositionHandle(worldPos, Quaternion.identity) - o) - otherCell.localPosition);

                    Handles.DrawDottedLine(worldPosCell, worldPos, 0.2f);

                    if (pos != b.curvePoint2)
                    {
                        Undo.RecordObject(target, "Puzzle system modification");
                        b.curvePoint2 = pos;
                    }

                    cell.paths[j].bezierPath = b;
                }

            }
        }
    }
}