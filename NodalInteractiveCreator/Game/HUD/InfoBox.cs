// Machinika Museum Prototype v.01
// © Littlefield Studio
// Writted by Rémi Carreira - 2016
//
// InfoBox.cs

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NodalInteractiveCreator.Managers;
using NodalInteractiveCreator.Inventory;

namespace NodalInteractiveCreator.HUD
{
    public class InfoBox : InteractiveCanvas
    {
        public Image InfoBackground = null;
        public Text InfoText = null;
        public BasicTextDisplayPattern DisplayPattern = null;
        public float DurationTolerenceClose = .5f;
        public AnimationClip AnimeDB = null;

        public static bool _infoBoxEnabled = false;
        public static bool _closeAllow = false;
        public static bool _changeAnchor = false;
        public static bool _finalInfoBox;

        protected override void Awake()
        {
            _infoBoxEnabled = false;
            _closeAllow = false;
            _changeAnchor = false;
            _finalInfoBox = false;
        }

        protected override void OnSelect(Vector3 InputPosition)
        {
            base.OnSelect(InputPosition);
        }

        protected override void OnDeselect()
        {
            base.OnDeselect();
            HideInfo();
        }

        public void ChangeAnchorAtBottom()
        {
            if (_changeAnchor)
            {
                GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 148.5f);
                GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
                GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0);

                _changeAnchor = false;
            }
            else
            {
                GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
                GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            }
        }

        /// <summary>
        /// Enable the info background and display the sentence
        /// </summary>
        /// <param name="Sentence">String to display</param>
        /// <returns>Return true if can display the info box</returns>
        public bool DisplayInfo(string Sentence)
        {
            if (null != InfoBackground && null != InfoText)
            {
                if (null != DisplayPattern && true == DisplayPattern.ExecutePattern(InfoText, Sentence) && !_infoBoxEnabled)
                {
                    if (!InspectSystem.IsInspecting())
                    {
                        GameManager.GetInputsManager().LockPinch = true;
                        GameManager.GetInputsManager().LockCanvas = true;
                        GameManager.GetInputsManager().LockViewpoint = true;
                        GameManager.GetInputsManager().LockInteractivity = true;
                    }

                    ChangeAnchorAtBottom();

                    InfoBackground.enabled = true;
                    InfoBackground.gameObject.SetActive(true);
                    InfoText.enabled = true;
                    InfoText.gameObject.SetActive(true);

                    StartCoroutine(WaitToAvailableCloseBox());
                    return true;
                }
                else
                {
                    Debug.LogWarning("Display Text failed", this.gameObject);
                }
            }
            else
            {
                Debug.LogWarning("Information Background or Text is null", this.gameObject);
            }
            return false;
        }

        /// <summary>
        /// Hide Information Background and Text
        /// </summary>
        /// <param name="TextDisplay">If true, hide only of the text is displayed</param>
        /// <returns>Return true if the info background and text are hidden</returns>
        public bool HideInfo()
        {
            if (null != InfoBackground && null != InfoText)
            {
                if (null == DisplayPattern || (_closeAllow && _infoBoxEnabled))
                {
                    GameManager.GetInputsManager().LockPinch = false;
                    GameManager.GetInputsManager().LockCanvas = false;
                    GameManager.GetInputsManager().LockViewpoint = false;
                    GameManager.GetInputsManager().LockInteractivity = false;

                    if (_finalInfoBox)
                    {
                        GameManager.GetGameState().currentState = GameState.STATE.OUTRO;
                        _finalInfoBox = false;
                    }

                    StartCoroutine(CloseBox());
                    return true;
                }
            }
            else
            {
                Debug.LogWarning("Information Background or Text is null", this.gameObject);
            }
            return false;
        }

        private IEnumerator CloseBox()
        {
            _closeAllow = false;

            GetComponent<Animation>()[AnimeDB.name].time = AnimeDB.length;
            GetComponent<Animation>()[AnimeDB.name].speed = -1;
            GetComponent<Animation>().Play(AnimeDB.name);

            yield return new WaitWhile(() => GetComponent<Animation>().isPlaying);

            GetComponent<Animation>()[AnimeDB.name].time = 0;
            GetComponent<Animation>()[AnimeDB.name].speed = 1;

            InfoBackground.enabled = false;
            InfoBackground.gameObject.SetActive(false);
            InfoText.enabled = false;
            InfoText.gameObject.SetActive(false);

            _infoBoxEnabled = false;
        }

        private IEnumerator WaitToAvailableCloseBox()
        {
            _infoBoxEnabled = true;
            _closeAllow = false;

            yield return new WaitWhile(() => DisplayPattern.IsPlaying());

            yield return new WaitForSecondsRealtime(DurationTolerenceClose);

            _closeAllow = true;
        }
    }
}