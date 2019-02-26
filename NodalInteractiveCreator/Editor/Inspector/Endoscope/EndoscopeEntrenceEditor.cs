using NodalInteractiveCreator.Endoscop;
using UnityEditor;

namespace NodalInteractiveCreator.Editor.Objects
{
    [CustomEditor(typeof(EndoscopeEntrence))]
    public class EndoscopeEntrenceEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EndoscopeEntrence t = target as EndoscopeEntrence;

            EditorGUILayout.BeginVertical("box");
            t.animIndex = EditorGUILayout.IntField("anim index", t.animIndex);

            serializedObject.Update();
            EditorGUILayout.ObjectField(serializedObject.FindProperty("linkedScene"));


            EditorGUILayout.ObjectField(serializedObject.FindProperty("gameObjectNoise"));
            t.noiseSpeed = EditorGUILayout.FloatField("Noise Speed", t.noiseSpeed);
            t.noisePower = EditorGUILayout.FloatField("Noise Power", t.noisePower);
            EditorGUILayout.ObjectField(serializedObject.FindProperty("EndoscopeAnimator"));
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);

            EditorGUILayout.EndVertical();
        }
    }
}
