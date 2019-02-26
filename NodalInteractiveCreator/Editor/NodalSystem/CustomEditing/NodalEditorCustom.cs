using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.Settings;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.CustomEditing
{
    public class NodalEditorCustom
    {
        public GUIStyle myGUI = new GUIStyle();
        protected const float width = 100;
        protected const float elementHeight = 15;
        protected Rect bounds = new Rect(0 , 0 , width, 25);
        public INodal Subject;
        protected List<FieldInfo> editableFields;

        public NodalEditorCustom(Type t)
        {
            editableFields = NodalTools.GetFieldsByAttribute<NodalField>(t);
            myGUI = NICStettings.Settings.nodeFieldStyle;
        }

        public virtual void OnGUI()
        {
            for (int i = 0; i < editableFields.Count; i++)
            {
                FieldInfo f = editableFields[i];
                SimpleEditorDrawer(f);
            }
        }

        public virtual Rect GetCustomBounds()
        {
            bounds.width = width;
            bounds.height = 0;
            bounds.height = editableFields.Count * elementHeight * 2 + 5;
            return bounds;
        }

        public void SerializeObjectEditorDrawer(SerializedObject s, FieldInfo f)
        {
            s.Update();

            SerializedProperty sp = s.FindProperty(f.Name);
            if (sp == null)
                return;

            EditorGUILayout.LabelField(new GUIContent(f.Name, f.Name), myGUI, GUILayout.Height(elementHeight));
            EditorGUILayout.PropertyField(sp, new GUIContent("",f.Name), true, GUILayout.Width(width - 10), GUILayout.Height(elementHeight));

            s.ApplyModifiedProperties();
        }

        void SimpleEditorDrawer(FieldInfo f)
        {
            EditorGUILayout.LabelField(new GUIContent(f.Name, f.Name), myGUI, GUILayout.Height(elementHeight));

            if (f.FieldType == typeof(float))
                f.SetValue(Subject, EditorGUILayout.FloatField((float)f.GetValue(Subject), GUILayout.Height(elementHeight)));

            else if (f.FieldType == typeof(bool))
                f.SetValue(Subject, EditorGUILayout.Toggle((bool)f.GetValue(Subject), GUILayout.Height(elementHeight)));

            else if (f.FieldType == typeof(string))
                f.SetValue(Subject, EditorGUILayout.TextField((string)f.GetValue(Subject), GUILayout.Height(elementHeight)));

            else if (f.FieldType == typeof(int))
                f.SetValue(Subject, EditorGUILayout.IntField((int)f.GetValue(Subject), GUILayout.Height(elementHeight)));

            else if (typeof(Enum).IsAssignableFrom(f.FieldType))
                f.SetValue(Subject, EditorGUILayout.EnumPopup((Enum)f.GetValue(Subject), GUILayout.Height(elementHeight)));

            else if (typeof(Object).IsAssignableFrom(f.FieldType))
                f.SetValue(Subject, EditorGUILayout.ObjectField((Object)f.GetValue(Subject), f.FieldType, true, GUILayout.Height(elementHeight)));

            else if (typeof(SerializerObject).IsAssignableFrom(f.FieldType))
            {
                SerializerObject s = (SerializerObject)f.GetValue(Subject);

                if (s == null)
                    s = (SerializerObject)Activator.CreateInstance(f.FieldType);

                s.Assign(EditorGUILayout.ObjectField(s.GetSubject(), s.GetSubjectType(),
                    true, GUILayout.Height(elementHeight)));

                f.SetValue(Subject, s);
            }
        }
    }
}
