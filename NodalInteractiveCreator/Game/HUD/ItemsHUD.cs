using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NodalInteractiveCreator.HUD
{
    [RequireComponent(typeof(Animation))]
    public class ItemsHUD : MonoBehaviour
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
                ShowItems();
            }
        }

        public void ShowItems()
        {
            if (false == gameObject.activeSelf && false == selfAnimation.isPlaying && null != selfAnimation.GetClip("Enter"))
            {
                foreach (Image i in GetComponents<Image>())
                    i.raycastTarget = true;

                gameObject.SetActive(true);
                selfAnimation.Play("Enter");
            }
        }

        public void UnshowItems()
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

        public bool IsItemsShown()
        {
            return gameObject.activeSelf;
        }

        public bool IsItemsPlayingAnimation()
        {
            return selfAnimation.isPlaying;
        }
    }

    /*
     *
     */

    #if UNITY_EDITOR

    [CustomEditor(typeof(ItemsHUD))]
    public class ItemsHUDEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (true == Application.isPlaying)
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Show"))
                {
                    ItemsHUD hud = target as ItemsHUD;
                    if (null != hud)
                    {
                        hud.ShowItems();
                    }
                }
                if (GUILayout.Button("Unshow"))
                {
                    ItemsHUD hud = target as ItemsHUD;
                    if (null != hud)
                    {
                        hud.UnshowItems();
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
    }
#endif
}