using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodalInteractiveCreator.Managers;
using NodalInteractiveCreator.Viewpoints;
using NodalInteractiveCreator.HUD;

namespace NodalInteractiveCreator.Controllers
{
    public class Cutscene : MonoBehaviour
    {
        [System.Serializable]
        public class CheckpointCutscene
        {
            public string _nameCuts;
            public ViewPoint _vpFinalCuts;
            public Vector3 _posInitCuts = new Vector3();
            public Vector3 _rotInitCuts = new Vector3();
            public bool _switchToCutsceneCamera = false;
            //Variable before a Cutscene
            public bool _moveTo, _spawnTo;
            public float _durationMoveTo;
            public bool _fadeToCutscene, _fadeWithMoveTo;
            public float _durationFadeTo;

            //Variable atfer a Cutscene
            public bool _moveToEnd, _spawnToEnd;
            public float _durationMoveToEnd;
            public bool _fadeToCutsceneEnd, _fadeWithMoveToEnd;
            public float _durationFadeToEnd;

            public CheckpointCutscene()
            {
                _nameCuts = "";
                _vpFinalCuts = null;
                _posInitCuts = new Vector3(0, 0, 0);
                _rotInitCuts = new Vector3(0, 0, 0);
                _moveTo = true;
                _durationMoveTo = 1f;
            }

            public CheckpointCutscene(string name, ViewPoint vp, Vector3 pos, Vector3 rot)
            {
                _nameCuts = name;
                _vpFinalCuts = vp;
                _posInitCuts = pos;
                _rotInitCuts = rot;
            }
        }

        public enum CUTSCENE
        {
            NONE,
            INTRO,
            IN_GAME,
            OUTRO
        }

        public static string _myCutsceneName = null;
        public static bool _cutsceneIsPlaying = false;

        public Animator _cutsceneAnimator = null;
        public GameObject _screenFade;
        public GameObject _screenValidate;
        public string _nameSceneToLoad;
        public List<CheckpointCutscene> _pointCutscene = new List<CheckpointCutscene>();

        public bool _showToolsHUD = true;
        public bool _showItemsHUD = true;

        private Camera _cutsceneCamera;

        private CUTSCENE _currentCutscene = CUTSCENE.NONE;
        public CUTSCENE CurrentCutscene { get { return _currentCutscene; } set { _currentCutscene = value; } }

        private void Awake()
        {
            _myCutsceneName = string.Empty;
            _cutsceneIsPlaying = false;
        }

        private void Start()
        {
            if (null == _cutsceneCamera)
                _cutsceneCamera = GetComponent<Camera>();

            if (null == _cutsceneAnimator)
                _cutsceneAnimator = gameObject.GetComponent<Animator>();

            _cutsceneCamera.GetComponent<CameraController>().DefaultViewpoint = GameManager.GetCamerasManager().MainCamera.GetComponent<CameraController>().DefaultViewpoint;
        }

        public void PlayCutscene()
        {
            if (null != HUDManager.GetItemHUD())
                HUDManager.GetItemHUD().UnshowItems();

            if (null != HUDManager.GetToolsHUD())
                HUDManager.GetToolsHUD().UnshowTools();

            if (GameManager.GetGameState().currentState == GameState.STATE.INTRO)
            {
                _currentCutscene = CUTSCENE.INTRO;
                _myCutsceneName = CurrentCutscene.ToString();

                StartCoroutine(AnimeIsPlayingINTRO());
                _cutsceneAnimator.Play(_myCutsceneName);

                return;
            }
            else if (GameManager.GetGameState().currentState == GameState.STATE.GAMEPLAY)
            {
                _currentCutscene = CUTSCENE.NONE;
            }
            else if (GameManager.GetGameState().currentState == GameState.STATE.OUTRO)
            {
                _currentCutscene = CUTSCENE.OUTRO;
                _myCutsceneName = CurrentCutscene.ToString();
            }

            int idPointCuts = -1;
            foreach (CheckpointCutscene cpCuts in _pointCutscene)
                if (cpCuts._nameCuts == _myCutsceneName)
                    idPointCuts = _pointCutscene.IndexOf(cpCuts);

            if (null != _pointCutscene[idPointCuts]._vpFinalCuts)
                _cutsceneCamera.GetComponent<CameraController>().DefaultViewpoint = _pointCutscene[idPointCuts]._vpFinalCuts;
            else
                _cutsceneCamera.GetComponent<CameraController>().DefaultViewpoint = GameManager.GetCamerasManager().MainCamera.GetComponent<CameraController>().DefaultViewpoint;

            if (_pointCutscene[idPointCuts]._switchToCutsceneCamera)
            {
                if (_pointCutscene[idPointCuts]._moveTo)
                    StartCoroutine(MoveTo(idPointCuts));
                else if (_pointCutscene[idPointCuts]._spawnTo)
                    StartCoroutine(SpawnTo(idPointCuts));
            }
            else
            {
                StartCoroutine(AnimeIsPlaying(idPointCuts));
                _cutsceneAnimator.Play(_myCutsceneName, -1, 0);
            }
        }

        private IEnumerator MoveTo(int id)
        {
            //FADE IN DURING MOVE TO
            if (_pointCutscene[id]._fadeToCutscene && _pointCutscene[id]._fadeWithMoveTo)
                Fade(-_pointCutscene[id]._durationFadeTo);

            float timer = 0;
            Vector3 startPos = GameManager.GetCamerasManager().MainCamera.transform.position;
            Vector3 startRot = GameManager.GetCamerasManager().MainCamera.transform.eulerAngles;

            GameManager.GetInputsManager().CurrentCameraController.StopAllCoroutines();

            float speed = Vector3.Distance(startPos, _pointCutscene[id]._posInitCuts) / _pointCutscene[id]._durationMoveTo;

            while (timer < _pointCutscene[id]._durationMoveTo)
            {
                GameManager.GetCamerasManager().MainCamera.transform.position = Vector3.Lerp(startPos, _pointCutscene[id]._posInitCuts, timer / _pointCutscene[id]._durationMoveTo);
                GameManager.GetCamerasManager().MainCamera.transform.rotation = Quaternion.Lerp(Quaternion.Euler(startRot), Quaternion.Euler(_pointCutscene[id]._rotInitCuts), timer / _pointCutscene[id]._durationMoveTo);

                timer += (Time.deltaTime * speed);
                yield return null;
            }

            _cutsceneCamera.transform.position = new Vector3(_pointCutscene[id]._posInitCuts.x, _pointCutscene[id]._posInitCuts.y, _pointCutscene[id]._posInitCuts.z);
            _cutsceneCamera.transform.eulerAngles = new Vector3(_pointCutscene[id]._rotInitCuts.x, _pointCutscene[id]._rotInitCuts.y, _pointCutscene[id]._rotInitCuts.z);

            //FADE IN AFTER MOVE TO
            if (_pointCutscene[id]._fadeToCutscene && !_pointCutscene[id]._fadeWithMoveTo)
            {
                Fade(-_pointCutscene[id]._durationFadeTo);
                yield return new WaitForSeconds(_pointCutscene[id]._durationFadeTo);
            }

            _cutsceneAnimator.Play(_myCutsceneName, -1, 0);
            StartCoroutine(AnimeIsPlaying(id));
        }

        private IEnumerator SpawnTo(int id)
        {
            //FADE IN BEFORE SPAWN TO
            if (_pointCutscene[id]._fadeToCutscene)
            {
                Fade(-_pointCutscene[id]._durationFadeTo);
                yield return new WaitForSeconds(_pointCutscene[id]._durationFadeTo);
            }

            _cutsceneCamera.transform.position = new Vector3(_pointCutscene[id]._posInitCuts.x, _pointCutscene[id]._posInitCuts.y, _pointCutscene[id]._posInitCuts.z);
            _cutsceneCamera.transform.eulerAngles = new Vector3(_pointCutscene[id]._rotInitCuts.x, _pointCutscene[id]._rotInitCuts.y, _pointCutscene[id]._rotInitCuts.z);

            StartCoroutine(AnimeIsPlaying(id));
            _cutsceneAnimator.Play(_myCutsceneName, -1, 0);
        }

        private IEnumerator AnimeIsPlayingINTRO()
        {
            _cutsceneAnimator.enabled = true;

            if (GameManager.GetCamerasManager().GetCurrentCamera() != GameManager.GetCamerasManager().CutsceneCamera)
            {
                GameManager.GetCamerasManager().ChangeCamera(CamerasManager.IndexCamera.CUTSCENE_CAMERA);
                GameManager.GetCamerasManager().MainCamera.GetComponent<CameraController>().SpwanTo(_cutsceneCamera.GetComponent<CameraController>().DefaultViewpoint);
            }

            float _timer = 0;

            while (_timer < _cutsceneAnimator.GetCurrentAnimatorStateInfo(0).length)
            {
                _timer += Time.deltaTime * _cutsceneAnimator.GetCurrentAnimatorStateInfo(0).speed;
                yield return null;
            }

            _cutsceneCamera.GetComponent<CameraController>().ChangeModeToZoomTo(_cutsceneCamera.GetComponent<CameraController>().DefaultViewpoint);
            GameManager.GetCamerasManager().MainCamera.fieldOfView = _cutsceneCamera.GetComponent<CameraController>().DefaultViewpoint.FieldOfView;

            SetShowHUD();
        }

        private IEnumerator AnimeIsPlaying(int id)
        {
            _cutsceneAnimator.enabled = true;

            if (_pointCutscene[id]._switchToCutsceneCamera)
            {
                if (GameManager.GetCamerasManager().GetCurrentCamera() != GameManager.GetCamerasManager().CutsceneCamera)
                {
                    GameManager.GetCamerasManager().ChangeCamera(CamerasManager.IndexCamera.CUTSCENE_CAMERA);
                    GameManager.GetCamerasManager().MainCamera.GetComponent<CameraController>().SpwanTo(_cutsceneCamera.GetComponent<CameraController>().DefaultViewpoint);
                }
            }

            float _timer = 0;

            while (_timer < _cutsceneAnimator.GetCurrentAnimatorStateInfo(0).length)
            {
                _timer += Time.deltaTime * _cutsceneAnimator.GetCurrentAnimatorStateInfo(0).speed;
                yield return null;
            }

            if (GameManager.GetGameState().currentState != GameState.STATE.OUTRO)
            {
                _cutsceneIsPlaying = false;

                if (!_pointCutscene[id]._switchToCutsceneCamera)
                {
                    if (null != _pointCutscene[id]._vpFinalCuts)
                        GameManager.GetCamerasManager().MainCamera.GetComponent<CameraController>().ChangeModeToZoomTo(_pointCutscene[id]._vpFinalCuts);
                    else if (GameManager.GetGameState().currentState != GameState.STATE.GAMEPLAY)
                        GameManager.GetGameState().currentState = GameState.STATE.GAMEPLAY;
                }
                else
                {
                    if (_pointCutscene[id]._spawnToEnd)
                    {
                        _cutsceneCamera.GetComponent<CameraController>().SpwanTo(_cutsceneCamera.GetComponent<CameraController>().DefaultViewpoint);

                        if (_pointCutscene[id]._fadeToCutsceneEnd && !_pointCutscene[id]._fadeWithMoveToEnd) //FADE OUT AFTER SPAWN TO
                            Fade(_pointCutscene[id]._durationFadeToEnd);

                        if (GameManager.GetGameState().currentState != GameState.STATE.GAMEPLAY)
                            GameManager.GetGameState().currentState = GameState.STATE.GAMEPLAY;
                    }
                    else if (_pointCutscene[id]._moveToEnd)
                    {
                        if (_pointCutscene[id]._fadeToCutsceneEnd && !_pointCutscene[id]._fadeWithMoveToEnd) //FADE OUT BEFORE MOVE TO
                        {
                            Fade(_pointCutscene[id]._durationFadeToEnd);
                            yield return new WaitForSeconds(_pointCutscene[id]._durationFadeToEnd);
                        }
                        else if (_pointCutscene[id]._fadeToCutsceneEnd && _pointCutscene[id]._fadeWithMoveToEnd) //FADE OUT DURING MOVE TO
                            Fade(_pointCutscene[id]._durationFadeToEnd);

                        _cutsceneCamera.GetComponent<CameraController>().ChangeModeToZoomTo(_cutsceneCamera.GetComponent<CameraController>().DefaultViewpoint);
                    }
                }
                SetShowHUD();
            }
            else
            {
                if (_screenValidate != null)
                {
                    _screenValidate.SetActive(true);
                    GameManager.Instance._audioFade = _screenValidate;
                }
            }
        }

        public void Fade(float time)
        {
            if (time == 0)
                return;

            _screenFade.GetComponent<Animation>().Stop();
            _screenFade.GetComponent<Animation>()["FadeScreen"].normalizedTime = 0;
            _screenFade.GetComponent<Animation>()["FadeScreen"].normalizedSpeed = 1;

            if (time < 0)
                _screenFade.GetComponent<Animation>()["FadeScreen"].normalizedTime = 1;
            else
                _screenFade.GetComponent<Animation>()["FadeScreen"].normalizedTime = 0;

            _screenFade.GetComponent<Animation>()["FadeScreen"].speed = _screenFade.GetComponent<Animation>()["FadeScreen"].speed / time;
            _screenFade.GetComponent<Animation>().Play("FadeScreen");
        }

        private void SetShowHUD()
        {
            if (null != HUDManager.GetItemHUD() && _showItemsHUD)
                HUDManager.GetItemHUD().ShowItems();

            if (null != HUDManager.GetToolsHUD() && _showToolsHUD)
                HUDManager.GetToolsHUD().ShowTools();
        }
    }
}

