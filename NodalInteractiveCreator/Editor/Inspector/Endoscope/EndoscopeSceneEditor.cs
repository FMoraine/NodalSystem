using System.Collections.Generic;
using NodalInteractiveCreator.Editor.Objects;
using NodalInteractiveCreator.Endoscop;
using NodalInteractiveCreator.Objects.Puzzle;
using UnityEditor;
using UnityEngine;

namespace NodalInteractiveCreator.Editor.Endoscope
{
    [CustomEditor(typeof(EndoscopeScene))]
    public class EndoscopeSceneEditor : InteractiveObjectEditor
    {
        private EndoscopeScene subject;
        private Transform subjT;

        private EndoscopeEditMode editMode;

        enum EndoscopeEditMode
        {
            PATH_EDIT,
            POSITIONS,
            BEZIER
        }
        
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("versionUtility"));

            EditorGUI.BeginChangeCheck();
            subject = (EndoscopeScene)target;
            subjT = subject.transform;

            serializedObject.Update();
            if(!subject.endoController)
                DrawInitialize();
            else
            {
                DrawTools();
                EditorGUILayout.Space();
                DrawNecessities();

                EditorGUILayout.Space();

                if (subject.specs.path != null)
                    DrawSpecs();
            }

            if (EditorGUI.EndChangeCheck())
            {
                PrefabUtility.DisconnectPrefabInstance(target);
            }

            serializedObject.ApplyModifiedProperties();
        }

        void DrawTools()
        {
            EditorGUILayout.BeginHorizontal("box");
            GUI.color = editMode == EndoscopeEditMode.PATH_EDIT ? Color.white : Color.gray;
            if (GUILayout.Button("Edit path points", GUILayout.Width(Screen.width / 3) , GUILayout.Height(80)))
                editMode = EndoscopeEditMode.PATH_EDIT;
            GUI.color = editMode == EndoscopeEditMode.POSITIONS ? Color.white : Color.gray;
            if (GUILayout.Button("Edit position", GUILayout.Width(Screen.width / 3), GUILayout.Height(80)))
                editMode = EndoscopeEditMode.POSITIONS;
            GUI.color = editMode == EndoscopeEditMode.BEZIER ? Color.white : Color.gray;
            if (GUILayout.Button("Edit bezier", GUILayout.Width(Screen.width / 3), GUILayout.Height(80)))
                editMode = EndoscopeEditMode.BEZIER;

            GUI.color = Color.white;
            
            EditorGUILayout.EndHorizontal();
        }

        void DrawNecessities()
        {
            serializedObject.Update();
            GUI.color = Color.gray;
            EditorGUILayout.BeginVertical("box");
            GUI.color = Color.white;
            EditorGUILayout.ObjectField(serializedObject.FindProperty("renderText"));
            EditorGUILayout.ObjectField(serializedObject.FindProperty("skyBoxMat"));
            subject.specs.movementMode = (EndoscopeMovement)EditorGUILayout.EnumPopup("Movement Mode :" , subject.specs.movementMode);

            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }

        void DrawInitialize()
        {
            EditorGUILayout.BeginVertical("box");
            subject.sceneSize = EditorGUILayout.FloatField("Skybox Size", subject.sceneSize);
            if (GUILayout.Button("Set up scene"))
                subject.SetUpEnvironment(subject.sceneSize);
            EditorGUILayout.EndVertical();
        }

        void DrawSpecs()
        {
            GUI.color = Color.gray;
            EditorGUILayout.BeginVertical("box");
            GUI.color = Color.white;
            List<PathPoint> pathLocal = subject.specs.path;
            EditorGUILayout.LabelField("Camera Specs");
            subject.specs.speed = EditorGUILayout.FloatField("speed progress", subject.specs.speed);
            subject.specs.noisePower = EditorGUILayout.FloatField("noise power", subject.specs.noisePower);
            subject.specs.noiseSpeed = EditorGUILayout.FloatField("noise speed", subject.specs.noiseSpeed);
            subject.specs.rotationSpeed = EditorGUILayout.FloatField("rotation speed", subject.specs.rotationSpeed);
            subject.specs.maxAngleSpread = EditorGUILayout.FloatField("max angle spread", subject.specs.maxAngleSpread);
            if (subject.specs.movementMode == EndoscopeMovement.SNAPPING)
                subject.specs.snapDelta = EditorGUILayout.Slider("Delta snap : " , subject.specs.snapDelta, 0.05f, 0.7f);


            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Path points");
            if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(20)))
                pathLocal.Add(new PathPoint(){curve = new BezierCurve(Vector3.one, Vector3.forward , Vector3.zero, Vector3.down)});

            EditorGUILayout.EndHorizontal();

            for (int i = pathLocal.Count-1; i >=0; i--)
            {
                EditorGUILayout.BeginHorizontal("box");
                if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    pathLocal.RemoveAt(i);
                    continue;
                }
                EditorGUILayout.LabelField("Path point " + i, GUILayout.Width(100));

                //if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(20)))
                //{
                //    pathLocal[i].actions.Add(new InteractiveObject());
                //    return;
                //}

                EditorGUILayout.BeginVertical();

                if(subject.specs.movementMode == EndoscopeMovement.SNAPPING)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("specs.path.Array.data[" + i + "].isCheckPoint"), new GUIContent("Check point ?"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("specs.path.Array.data[" + i + "].callMax"), new GUIContent("max call"));
                //for (int j = pathLocal[i].actions.Count - 1; j >= 0; j--)
                //{
                //    EditorGUILayout.BeginHorizontal("box");
                //    if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20)))
                //    {
                //        pathLocal[i].actions.RemoveAt(j);
                //        return;
                //    }

                //    EditorGUILayout.BeginVertical();
                //    EditorGUILayout.PropertyField(serializedObject.FindProperty("specs.path.Array.data[" + i + "].actions.Array.data[" + j + "]"));

                //    if(null != pathLocal[i].actions[j])
                //        ShowOnInspectorListObjToActivePath(pathLocal[i].actions[j]);
                //    EditorGUILayout.EndVertical();

                //    //EditorGUILayout.PropertyField(serializedObject.FindProperty("specs.path.Array.data[" + i + "].actions.Array.data[" + j + "]"),true);

                //    EditorGUILayout.EndHorizontal();
                //}
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        void OnSceneGUI()
        {
            subject = (EndoscopeScene)target;
            subjT = subject.transform;

            if (!subject.endoController)
                return;

            List<PathPoint> pathLocal = subject.specs.path;

            subject.sceneSize = Handles.RadiusHandle(subjT.rotation , subjT.position , subject.sceneSize);

            for (int i = pathLocal.Count-1; i >= 0 ; i--)
            {
                PathPoint bc = pathLocal[i];

                if(editMode == EndoscopeEditMode.POSITIONS)
                    bc.curve.point1 = Quaternion.Inverse(subjT.rotation) * (Handles.PositionHandle(subjT.position + subjT.rotation * bc.curve.point1, subject.transform.rotation) - subjT.position);

                Handles.color = Color.red;
                Vector3 start = ToWorld(bc.curve.point1);
                if (i < pathLocal.Count-1)
                {
                  
                    Vector3 to = ToWorld(pathLocal[i + 1].curve.point1);
                    Handles.DrawBezier(start, to, start + subjT.rotation * bc.curve.curvePoint1, to - subjT.rotation * pathLocal[i + 1].curve.curvePoint1, Color.red, null, 5f);
                }
             
                Handles.color = Color.white;

                if (editMode == EndoscopeEditMode.BEZIER)
                {
                    Handles.color = Color.green;
                    Handles.DrawLine(start , ToWorld(bc.curve.point1 + bc.curve.curvePoint1));
                    Handles.color = Color.white;
                    bc.curve.curvePoint1 = Quaternion.Inverse(subjT.rotation) * (Handles.PositionHandle(ToWorld(bc.curve.point1 + bc.curve.curvePoint1), Quaternion.identity) - subjT.position) - bc.curve.point1;
                }
               
                pathLocal[i] = bc;

                if(editMode == EndoscopeEditMode.PATH_EDIT)
                    DisplayGUI(i);
            } 
        }

        void DisplayGUI(int indexPath)
        {
            Handles.BeginGUI();
            Vector3 screenPoint = Camera.current.WorldToScreenPoint(ToWorld(subject.specs.path[indexPath].curve.point1));
            screenPoint.y = Screen.height-screenPoint.y;

            Rect rect = new Rect(screenPoint, new Vector2(20, 20));
            Rect rect2 = new Rect(screenPoint.x + 30 , screenPoint.y , 20 , 20);

            if (GUI.Button(rect, "X"))
            {
                subject.specs.path.RemoveAt(indexPath);
            }
            GUI.TextArea(rect2, "" + indexPath);
            Handles.EndGUI();
        }

        Vector3 ToWorld(Vector3 localPos)
        {
            return subjT.position + subjT.rotation * localPos;
        }
    }
}
