using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodalInteractiveCreator.Controllers;
using NodalInteractiveCreator.HUD;

namespace NodalInteractiveCreator.Managers
{
    public class GameState : MonoBehaviour
    {
        public enum STATE
        {
            NONE = 0,
            INTRO,
            GAMEPLAY,
            OUTRO,
        }

        public STATE currentState = STATE.NONE;
        public Cutscene _myCutscene = null;

        private void Awake()
        {
            if (null == _myCutscene)
                _myCutscene = FindObjectOfType<Cutscene>();
        }

        void Update()
        {
            if (STATE.NONE == currentState)
            {
                if (!GameManager.GetInputsManager().LockInput)
                    GameManager.GetInputsManager().LockInput = true;
            }
            else if (STATE.INTRO == currentState)
            {
                UpdateINTRO();
                if (!GameManager.GetInputsManager().LockInput)
                    GameManager.GetInputsManager().LockInput = true;
            }
            else if (STATE.GAMEPLAY == currentState)
            {
                UpdateGameplay();
                if (GameManager.GetInputsManager().LockInput)
                    GameManager.GetInputsManager().LockInput = false;
            }
            else if (STATE.OUTRO == currentState)
            {
                UpdateOUTRO();
                if (!GameManager.GetInputsManager().LockInput)
                    GameManager.GetInputsManager().LockInput = true;
            }
        }

        protected virtual void UpdateINTRO()
        {
            if (_myCutscene.CurrentCutscene != Cutscene.CUTSCENE.INTRO)
            {
                _myCutscene.PlayCutscene();
            }
        }

        protected virtual void UpdateGameplay()
        {
            if (_myCutscene.CurrentCutscene == Cutscene.CUTSCENE.IN_GAME)
            {
                _myCutscene.PlayCutscene();
                currentState = STATE.NONE;
            }
            else if (GameManager.GetCamerasManager().GetCurrentCamera() == GameManager.GetCamerasManager().CutsceneCamera)
            {
                _myCutscene._cutsceneAnimator.enabled = false;
                GameManager.GetCamerasManager().ChangeCamera(CamerasManager.IndexCamera.MAIN_CAMERA);
            }
        }

        protected virtual void UpdateOUTRO()
        {
            if (_myCutscene.CurrentCutscene != Cutscene.CUTSCENE.OUTRO)
            {
                _myCutscene.PlayCutscene();
            }
        }

        public void Fade(float time)
        {
            if (_myCutscene._screenFade != null)
                _myCutscene.Fade(time);
        }

        public void OpenBox(int id)
        {
            string sentence;

            if (Application.systemLanguage == SystemLanguage.French)
                sentence = GameManager.GetSentenceTranslate().GetAt(id).SENTENCE_FR;
            else
                sentence = GameManager.GetSentenceTranslate().GetAt(id).SENTENCE_EN;

            HUDManager.GetInfoBox().DisplayInfo(sentence);
        }

        public void LoadScene()
        {
            GameManager.Instance.LoadScene(_myCutscene._nameSceneToLoad);
        }
    }
}