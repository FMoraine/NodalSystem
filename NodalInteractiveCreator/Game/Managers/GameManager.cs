using NodalInteractiveCreator.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NodalInteractiveCreator.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance = null;

        public GameState GameState = null;
        public CamerasManager CamerasMgr = null;
        public InputsManager InputsMgr = null;
        public Machine Machine = null;
        public SentenceTranslate SentenceTranslate = null;

        public GameObject _loadingScreen;
        public GameObject _screenFade;
        public GameObject _audioFade;

        void Awake()
        {
            Instance = this;

            if (null == GameState)
                GameState = FindObjectOfType<GameState>();

            if (null == CamerasMgr)
                CamerasMgr = FindObjectOfType<CamerasManager>();

            if (null == InputsMgr)
            {
                if (null != FindObjectOfType<InputsManager>())
                    InputsMgr = FindObjectOfType<InputsManager>();
            }

            if (null == Machine)
                Machine = FindObjectOfType<Machine>();

            if (null == SentenceTranslate)
                SentenceTranslate = FindObjectOfType<SentenceTranslate>();

            if (!_screenFade.activeSelf)
                _screenFade.SetActive(true);
        }

        void Start()
        {
            if (null != GameState)
            {
                GameState.currentState = GameState.STATE.INTRO;
            }
            StartCoroutine(FadeInterScene(true));
        }

        public static GameState GetGameState()
        {
            if (null != Instance)
            {
                return Instance.GameState;
            }
            return null;
        }

        public static CamerasManager GetCamerasManager()
        {
            if (null != Instance)
            {
                return Instance.CamerasMgr;
            }
            return null;
        }

        public static InputsManager GetInputsManager()
        {
            if (null != Instance)
            {
                return Instance.InputsMgr;
            }
            return null;
        }

        public static Machine GetMachine()
        {
            if (null != Instance)
            {
                return Instance.Machine;
            }
            return null;
        }

        public static SentenceTranslate GetSentenceTranslate()
        {
            if (null != Instance)
            {
                return Instance.SentenceTranslate;
            }
            return null;
        }

        public void LoadScene(string nameScene)
        {
            foreach (Button b in FindObjectsOfType<Button>())
                b.enabled = false;

            StartCoroutine(LoadingAsyncScene(nameScene));
        }

        private IEnumerator FadeInterScene(bool fadeIn)
        {
            AudioSource myAudio = _audioFade.GetComponent<AudioSource>();
            Animation myAnime = _screenFade.GetComponent<Animation>();
            string nameAnimeClip = myAnime.clip.name;

            if (fadeIn)
            {
                if (myAnime != null)
                {
                    myAnime[nameAnimeClip].speed = 1;
                    myAnime[nameAnimeClip].time = 0;
                    myAnime.Play();
                }

                if (myAudio != null)
                {
                    myAudio.volume = 0;
                    while (myAudio.volume < .9f)
                    {
                        myAudio.volume = myAnime[nameAnimeClip].normalizedTime;
                        yield return null;
                    }
                    myAudio.volume = 1;
                }
            }
            else
            {
                if (myAnime != null)
                {
                    myAnime[nameAnimeClip].speed = -1;
                    myAnime[nameAnimeClip].time = 1;
                    myAnime.Play();
                }

                if (myAudio != null)
                {
                    while (myAudio.volume > 0.1f)
                    {
                        myAudio.volume = myAnime[nameAnimeClip].normalizedTime;
                        yield return null;
                    }
                    myAudio.volume = 0;
                }
            }
        }

        private IEnumerator LoadingAsyncScene(string name)
        {
            yield return StartCoroutine(FadeInterScene(false));

            AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
            asyncLoad.allowSceneActivation = false;

            if (_loadingScreen != null)
            {
                _audioFade.GetComponent<AudioSource>().clip = null;
                StartCoroutine(FadeInterScene(true));

                _loadingScreen.SetActive(true);
                yield return new WaitForSeconds(2f);

                StartCoroutine(FadeInterScene(false));
            }

            yield return new WaitWhile(() => asyncLoad.progress < .9f);

            while (!asyncLoad.isDone)
            {
                if ((_screenFade != null && !_screenFade.GetComponent<Animation>().isPlaying))
                {
                    asyncLoad.allowSceneActivation = true;
                }
                yield return null;
            }
        }
    }
}