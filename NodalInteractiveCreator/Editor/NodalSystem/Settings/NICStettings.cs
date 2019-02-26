using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using UnityEditor;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.Settings
{
    public class NICStettings : EditorWindow
    {
        public static NICSettingsData Settings
        {
            get
            {
                if (_settings == null)
                    _settings = GetRsc();

                return _settings;
            }
        }

        protected static NICSettingsData _settings;
        protected const string settingsPath = "NIC/Settings/";
        protected const string settingsFileName = "settings.json";

        protected static string GlobalPath
        {
            get
            {
                return Path.Combine(Application.dataPath, settingsPath) + settingsFileName;
            }
            
        }

        [MenuItem("Window/Nodal Editor Settings")]
        public static void ShowWindow()
        {
            GetWindow(typeof(NICStettings));
        }

        public static NICSettingsData GetRsc()
        {
            if (!File.Exists(GlobalPath))
                CreateSettingsFile();

            var reader = new StreamReader(GlobalPath, Encoding.Default, true);

            string json = reader.ReadToEnd();
            json = Serializer.SearchForGUIDLinks(json);
            NICSettingsData v = JsonUtility.FromJson<NICSettingsData>(json);
            reader.Close();


            return v;
        }

        protected static void CreateSettingsFile()
        {
            File.WriteAllText(GlobalPath, JsonUtility.ToJson(new NICSettingsData()));
            AssetDatabase.Refresh();
        }

        void OnEnable()
        {

        }

        private Vector2 scroll;
        void OnGUI()
        {
            SerializedSettings s = ScriptableObject.CreateInstance<SerializedSettings>();
            s.data = Settings;

            SerializedObject o = new SerializedObject(s);
            
            o.Update();

            scroll = GUILayout.BeginScrollView(scroll);

            if(GUILayout.Button("Save"))Save();
            GUILayout.Space(20);

            EditorGUI.BeginChangeCheck();
            GUI.color = Color.gray;
            EditorGUILayout.BeginVertical("box");
            GUI.color = Color.white;
            EditorGUILayout.PropertyField(o.FindProperty("data.windowStyle"), true);
            EditorGUILayout.EndVertical();

            GUILayout.Space(20);
            GUI.color = Color.gray;
            EditorGUILayout.BeginVertical("box");
            GUI.color = Color.white;
            EditorGUILayout.PropertyField(o.FindProperty("data.nodeEditorStyle"), true);
            EditorGUILayout.EndVertical();

            GUILayout.Space(20);
            GUI.color = Color.gray;
            EditorGUILayout.BeginVertical("box");
            GUI.color = Color.white;
            EditorGUILayout.PropertyField(o.FindProperty("data.addButtonStyle"), true);
            EditorGUILayout.EndVertical();

            GUILayout.Space(20);
            GUI.color = Color.gray;
            EditorGUILayout.BeginVertical("box");
            GUI.color = Color.white;
            EditorGUILayout.PropertyField(o.FindProperty("data.addCategoryButtonStyle"), true);
            EditorGUILayout.EndVertical();

            GUILayout.Space(20);
            GUI.color = Color.gray;
            EditorGUILayout.BeginVertical("box");
            GUI.color = Color.white;

            EditorGUILayout.PropertyField(o.FindProperty("data.background"), true);
            EditorGUILayout.EndVertical();

            GUILayout.Space(20);
            GUI.color = Color.gray;
            EditorGUILayout.BeginVertical("box");
            GUI.color = Color.white;
            EditorGUILayout.PropertyField(o.FindProperty("data.preferences"), true);
            EditorGUILayout.EndVertical();

            GUILayout.Space(20);
            GUI.color = Color.gray;
            EditorGUILayout.BeginVertical("box");
            GUI.color = Color.white;
            EditorGUILayout.PropertyField(o.FindProperty("data.nodeFieldStyle"), true);
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())Save();
            GUILayout.EndScrollView();
            o.ApplyModifiedProperties();
            
            _settings = s.data;
        }
        protected void Save()
        {
            string json = JsonUtility.ToJson(Settings);
            json = Serializer.SearchForLinks(json);
            File.WriteAllText(GlobalPath, json);
        }


    }


    class SerializedSettings : ScriptableObject
    {
        public NICSettingsData data;
    }

    [Serializable]
    public class NICSettingsData
    {
        public GUIStyle windowStyle = new GUIStyle();
        public GUIStyle nodeEditorStyle = new GUIStyle();
        public GUIStyle addButtonStyle = new GUIStyle();
        public GUIStyle addCategoryButtonStyle = new GUIStyle();
        public GUIStyle nodeFieldStyle = new GUIStyle();
        public Texture2D background;
        public NICPreferences preferences = new NICPreferences();


    }
    [Serializable]
    public class NICPreferences
    {
        public float dragSpeed = 3;
        public Color colorLink = Color.white;
    }
}
