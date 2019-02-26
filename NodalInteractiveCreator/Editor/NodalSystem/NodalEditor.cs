using System.Collections.Generic;
using System.ComponentModel.Design;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts;
using UnityEditor;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem
{
    public partial class NodalEditor : EditorWindow
    {
        [MenuItem("Window/NodalEditor")]
        public static void ShowWindow()
        {
            GetWindow(typeof(NodalEditor));
        }

        public static NodalEditor instance;

        public static NodalInteractionSystem Target
        {
            get {
                if (instance)
                    return instance._target;
                else
                {
                    return null;
                }
            }
            set
            {
                if (instance)
                    instance._target = value;
            }
        }

        private NodalInteractionSystem _target;  
        public static bool guidView = false;

        public static AddPanel addUI = new AddPanel();
        private readonly NodePanel nodeUI = new NodePanel();

        void OnEnable()
        {
            instance = this;
            Init();
            EditorApplication.playmodeStateChanged += StateChange;
        }

        void StateChange()
        {
            if(EditorApplication.isPlaying == EditorApplication.isPlayingOrWillChangePlaymode && Target != null)
                Refresh();

        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            if(instance)
                instance.Refresh();

        }
          
        void OnGUI()
        {
            if (Target == null)
            {
                _target = DrawSelectTarget();
                if (Target)
                {
                    if (Target.IsAnyEmptyNodes())
                        Target.RemoveEmptyNodes();

                    Rebind();
                }
                else
                    return;
            }

            nodeUI.target = Target;
            addUI.target = Target;


            nodeUI.InputDetect();
            DrawWindow();

            nodeUI.DrawContent();
            addUI.DrawContent();
            nodeUI.DrawUIOverlay();
        }

        private Vector2 pos;

        private static  int btnPerRaw = 4; 
        private static float marginSize = 40;
        private static float marginHeight = 100;

        NodalInteractionSystem DrawSelectTarget() {   

            NodalInteractionSystem target = null;

            NodalInteractionSystem[] array = GameObject.FindObjectsOfType<NodalInteractionSystem>();

            int rawWithMargin = btnPerRaw + 2;
            float size = Screen.width / rawWithMargin;
            Rect area = new Rect(Screen.width / rawWithMargin, marginHeight, Screen.width - (Screen.width / rawWithMargin) *2, Screen.height - marginHeight * 2);
            Rect globalArea = new Rect(area.x , area.y , area.width- marginSize, size +  size * Mathf.FloorToInt((float)array.Length / btnPerRaw));

            GUI.color = Color.black;
            GUI.Box(area , "");
            GUI.color = Color.white;

            Rect btnSize = new Rect(area.x, area.y, size - marginSize, size- marginSize);

            pos = GUI.BeginScrollView(area, pos, globalArea, false, true);
            for (int i = 0; i < array.Length; i++)
            {
                btnSize.position = new Vector2(area.x + size * (i % btnPerRaw) + marginSize/2, area.y + size * Mathf.FloorToInt((float)i/ btnPerRaw) + marginSize/2);

                if (GUI.Button(btnSize, array[i].name))
                {
                    target = array[i];
                 
                }
            }

            GUI.EndScrollView();
               

            return target;
        }

        void DrawWindow()
        {
            BeginWindows();
            nodeUI.DrawWindows();
            EndWindows();
        }

     
    }
}
