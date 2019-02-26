using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NodalInteractiveCreator.Endoscop;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NodalInteractiveCreator.HUD
{
    [RequireComponent(typeof(Animation))]
    public class ToolsHUD : MonoBehaviour
    {
        public bool ShowOnStart = false;
        public AnimationClip EnterClip = null;
        public AnimationClip ExitClip = null;

        private Animation selfAnimation = null;

        private void Awake()
        {
            selfAnimation = GetComponent<Animation>();

            if (null != EnterClip)
            {
                selfAnimation.AddClip(EnterClip, "Enter");
            }
            if (null != ExitClip)
            {
                selfAnimation.AddClip(ExitClip, "Exit");
            }
        }

        private void Start()
        {
            if (false == ShowOnStart)
            {
                gameObject.SetActive(false);
            }
            else if (null != selfAnimation.GetClip("Enter"))
            {
                ShowTools();
            }
        }

        public void ShowTools()
        {
            if (false == gameObject.activeSelf && false == selfAnimation.isPlaying && null != selfAnimation.GetClip("Enter"))
            {
                foreach (Image i in GetComponents<Image>())
                    i.raycastTarget = true;

                gameObject.SetActive(true);
                selfAnimation.Play("Enter");
            }
        }

        public void UnshowTools()
        {
            if (true == gameObject.activeSelf && false == selfAnimation.isPlaying && null != selfAnimation.GetClip("Exit"))
            {
                foreach (Image i in GetComponents<Image>())
                    i.raycastTarget = false;

                selfAnimation.Play("Exit");
                StartCoroutine(WaitSecondsBeforeDisable(selfAnimation.GetClip("Exit").length));
            }
        }

        private IEnumerator WaitSecondsBeforeDisable(float Seconds)
        {
            yield return new WaitForSeconds(Seconds);

            gameObject.SetActive(false);
        }

        public bool IsToolsShown()
        {
            return gameObject.activeSelf;
        }

        public bool IsToolsPlayingAnimation()
        {
            return selfAnimation.isPlaying;
        }

        public void EndoscopeClicked()
        {
            if (EndoscopePad.current)
                EndoscopePad.current.Close();
        }
    }

    /*
     * 
     */

#if UNITY_EDITOR

    [CustomEditor(typeof(ToolsHUD))]
    public class ToolsHUDEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (true == Application.isPlaying)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Show"))
                {
                    ToolsHUD hud = target as ToolsHUD;
                    if (null != hud)
                    {
                        hud.ShowTools();
                    }
                }
                if (GUILayout.Button("Unshow"))
                {
                    ToolsHUD hud = target as ToolsHUD;
                    if (null != hud)
                    {
                        hud.UnshowTools();
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
    }
#endif
}
