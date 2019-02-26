using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using NodalInteractiveCreator.Managers;
using NodalInteractiveCreator.Controllers;
using NodalInteractiveCreator.Inventory;
using NodalInteractiveCreator.Viewpoints;
using NodalInteractiveCreator.Objects;
using NodalInteractiveCreator.Endoscop;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NodalInteractiveCreator.HUD
{
    [RequireComponent(typeof(Animation))]
    public class Tuto : MonoBehaviour
    {

        public enum TUTO_STEP
        {
            NONE,
            TUTO_START,
            MOVE_CAMERA,
            GO_INTO_VIEWPOINT,
            TRANSLATABLE,
            TAKE_LETTER,
            GO_INTO_INVENTORY,
            BACK_TO_INVENTORY,
            ROTATABLE,
            PUSHABLE,
            BACK_TO_VIEWPOINT,
            TUTO_END,
            TUTO_ENDOSCOPE_START,
            TUTO_ENDOSCOPE_MOVE,
            TUTO_ENDOSCOPE_END,
        }

        public static Text _textTuto;
        public static bool _isTuto;
        public static TUTO_STEP _step = TUTO_STEP.NONE;

        [Header("Init")]
        public bool ShowOnStart = false;
        public SentenceTranslate _mySentenceTranslateTuto;

        [Header("Animation")]
        public AnimationClip _panelClip = null;
        public AnimationClip _textClip = null;

        [Header("Step: GO_INTO_VIEWPOINT")]
        public ViewPoint _vpGO_INTO_VIEWPOINT = null;
        public GameObject _arrowSwipe = null;

        [Header("Step: TRANSLATABLE")]
        public Machine.EVENTS _eventStepTRANSLATABLE;
        public string _actionStepTRANSLATABLE;

        [Header("Step: TAKE_LETTER")]
        public Machine.EVENTS _eventStepTAKE_LETTER;
        public string _actionStepTAKE_LETTER;
        public GameObject _itemLetter;

        [Header("Step: ROTATABLE")]
        public ViewPoint _vpROTATABLE = null;
        public Machine.EVENTS _eventStepROTATABLE;
        public string _actionStepROTATABLE;
        public GameObject _arrowRotate = null;

        [Header("Step: PUSHABLE")]
        public ViewPoint _vpPUSHABLE = null;
        public InteractiveObject _objPUSHABLE;

        [Header("Step: BACK_TO_VIEWPOINT")]
        public ViewPoint _vpBACK_TO_VIEWPOINT = null;

        [Header("Step: TUTO_ENDOSCOPE")]
        public GameObject _goTUTO_ENDOSCOPE = null;

        [Header("Step: TUTO_ENDOSCOPE_END")]
        public Machine.EVENTS _eventStepTUTO_ENDOSCOPE_END;
        public string _actionStepTUTO_ENDOSCOPE_END;
        public ItemSlot _endoscopeTools = null;
        public ViewPoint _vpTUTO_ENDOSCOPE_END = null;

        private Animation selfAnimation = null;
        private bool _displayTxt;
        private bool _displayPanel;

        private void Awake()
        {
            selfAnimation = GetComponent<Animation>();
            if (null == _mySentenceTranslateTuto)
                _mySentenceTranslateTuto = GetComponent<SentenceTranslate>();

            if (null != _panelClip)
            {
                selfAnimation.AddClip(_panelClip, "Enter");
                selfAnimation.AddClip(_panelClip, "Exit");
            }
            if (null != _textClip)
            {
                selfAnimation.AddClip(_textClip, "Texte");
            }

            if (ShowOnStart)
                _isTuto = true;

            _step = TUTO_STEP.NONE;
        }

        private void Start()
        {
            _mySentenceTranslateTuto.Load(_mySentenceTranslateTuto._file);
            _textTuto = GetComponentInChildren<Text>();

            if (false == ShowOnStart)
            {
                selfAnimation["Enter"].time = 0;
                gameObject.SetActive(false);
            }
            else
            {
                StartCoroutine(TutoStep(TUTO_STEP.TUTO_START));
            }
        }

        public void ShowPanel()
        {
            if (false == selfAnimation.isPlaying && null != selfAnimation.GetClip("Enter") && !_displayPanel)
            {
                _isTuto = true;

                _displayPanel = true;
                selfAnimation["Enter"].time = 0;
                selfAnimation.Play("Enter");
                gameObject.SetActive(true);
            }
        }

        public void UnshowPanel()
        {
            if (true == gameObject.activeSelf && false == selfAnimation.isPlaying && null != selfAnimation.GetClip("Exit") && _displayPanel)
            {
                _isTuto = false;

                _displayPanel = false;
                selfAnimation["Exit"].time = 1;
                selfAnimation["Exit"].speed = -1;
                selfAnimation.Play("Exit");
                StartCoroutine(WaitSecondsBeforDisable(selfAnimation.GetClip("Exit").length));
            }
        }

        public bool IsPanelShown()
        {
            return gameObject.activeSelf;
        }

        public bool IsPanelPlayingAnimation()
        {
            return selfAnimation.isPlaying;
        }

        private IEnumerator WaitSecondsBeforDisable(float Seconds)
        {
            yield return new WaitForSeconds(Seconds);
            gameObject.SetActive(false);
        }

        public IEnumerator TutoStep(TUTO_STEP step)
        {
            switch (step)
            {
                case TUTO_STEP.NONE:
                    break;

                case TUTO_STEP.TUTO_START:
                    _step = TUTO_STEP.TUTO_START;

                    ViewPointController.LockAllVp();

                    yield return new WaitUntil(() => GameManager.GetGameState().currentState == GameState.STATE.GAMEPLAY);

                    ShowPanel();

                    if (Application.systemLanguage == SystemLanguage.French)
                        _mySentenceTranslateTuto.OpenBox(_mySentenceTranslateTuto.Find_KEY("BOX_" + _step.ToString()).SENTENCE_FR);
                    else
                        _mySentenceTranslateTuto.OpenBox(_mySentenceTranslateTuto.Find_KEY("BOX_" + _step.ToString()).SENTENCE_EN);

                    yield return new WaitForSecondsRealtime(1f);

                    StartCoroutine(TutorialSentence());

                    while (!_displayTxt)
                        yield return null;

                    yield return new WaitWhile(() => InfoBox._infoBoxEnabled);

                    goto case TUTO_STEP.MOVE_CAMERA;


                case TUTO_STEP.MOVE_CAMERA:
                    _step = TUTO_STEP.MOVE_CAMERA;

                    StartCoroutine(TutorialSentence());

                    while (!_displayTxt)
                        yield return null;

                    Vector3 initTransformMainCamera = GameManager.GetCamerasManager().MainCamera.transform.position;

                    yield return new WaitWhile(() => GameManager.GetCamerasManager().MainCamera.transform.position == initTransformMainCamera);
                    yield return new WaitForSecondsRealtime(2f);

                    goto case TUTO_STEP.GO_INTO_VIEWPOINT;


                case TUTO_STEP.GO_INTO_VIEWPOINT:
                    _step = TUTO_STEP.GO_INTO_VIEWPOINT;

                    StartCoroutine(TutorialSentence());

                    while (!_displayTxt)
                        yield return null;

                    _vpGO_INTO_VIEWPOINT.UnlockViewpoint();

                    yield return new WaitWhile(() => _vpGO_INTO_VIEWPOINT != GameManager.GetCamerasManager().MainCamera.GetComponent<CameraController>().GetCurrentViewpoint());

                    goto case TUTO_STEP.TRANSLATABLE;


                case TUTO_STEP.TRANSLATABLE:
                    _step = TUTO_STEP.TRANSLATABLE;

                    GameManager.GetInputsManager().LockPinch = true;

                    StartCoroutine(TutorialSentence());

                    while (!_displayTxt)
                        yield return null;

                    int idEvent = GameManager.GetMachine().Events.FindIndex(x => x._event == _eventStepTRANSLATABLE);
                    int idAction = GameManager.GetMachine().Events[idEvent]._listActions.FindIndex(x => x._name == _actionStepTRANSLATABLE);

                    _arrowSwipe.SetActive(true);

                    yield return new WaitWhile(() => !GameManager.GetMachine().Events[idEvent]._listActions[idAction]._value);

                    _arrowSwipe.SetActive(false);

                    goto case TUTO_STEP.TAKE_LETTER;


                case TUTO_STEP.TAKE_LETTER:
                    _step = TUTO_STEP.TAKE_LETTER;

                    StartCoroutine(TutorialSentence());

                    while (!_displayTxt)
                        yield return null;

                    idEvent = GameManager.GetMachine().Events.FindIndex(x => x._event == _eventStepTAKE_LETTER);
                    idAction = GameManager.GetMachine().Events[idEvent]._listActions.FindIndex(x => x._name == _actionStepTAKE_LETTER);

                    _itemLetter.GetComponent<Animation>().wrapMode = WrapMode.Loop;

                    yield return new WaitWhile(() => !GameManager.GetMachine().Events[idEvent]._listActions[idAction]._value);

                    //GameManager.GetCamerasManager().GetCurrentCamera().GetComponent<CameraController>().ChangeModeToZoomOutTo(GameManager.GetCamerasManager().GetCurrentCamera().GetComponent<CameraController>().DefaultViewpoint);

                    goto case TUTO_STEP.GO_INTO_INVENTORY;


                case TUTO_STEP.GO_INTO_INVENTORY:
                    _step = TUTO_STEP.GO_INTO_INVENTORY;

                    StartCoroutine(TutorialSentence());

                    while (!_displayTxt)
                        yield return null;

                    yield return new WaitWhile(() => GameManager.GetCamerasManager().GetCurrentCamera() != GameManager.GetCamerasManager().InventoryCamera);

                    _itemLetter.GetComponent<Animation>().wrapMode = WrapMode.Once;

                    goto case TUTO_STEP.BACK_TO_INVENTORY;


                case TUTO_STEP.BACK_TO_INVENTORY:

                    //_step = TUTO_STEP.NONE;

                    //StartCoroutine(TutorialSentence());

                    //yield return new WaitForSecondsRealtime(2.5f);

                    _step = TUTO_STEP.BACK_TO_INVENTORY;

                    StartCoroutine(TutorialSentence());

                    while (!_displayTxt)
                        yield return null;

                    yield return new WaitWhile(() => GameManager.GetCamerasManager().GetCurrentCamera() != GameManager.GetCamerasManager().MainCamera);

                    if (Application.systemLanguage == SystemLanguage.French)
                        _mySentenceTranslateTuto.OpenBox(_mySentenceTranslateTuto.Find_KEY("BOX_" + _step.ToString()).SENTENCE_FR);
                    else
                        _mySentenceTranslateTuto.OpenBox(_mySentenceTranslateTuto.Find_KEY("BOX_" + _step.ToString()).SENTENCE_EN);

                    yield return new WaitWhile(() => InfoBox._infoBoxEnabled);

                    GameManager.GetCamerasManager().GetCurrentCamera().GetComponent<CameraController>().ChangeModeToZoomOutTo(_vpROTATABLE);

                    goto case TUTO_STEP.ROTATABLE;


                case TUTO_STEP.ROTATABLE:
                    _step = TUTO_STEP.ROTATABLE;

                    GameManager.GetInputsManager().LockPinch = true;

                    StartCoroutine(TutorialSentence());

                    while (!_displayTxt)
                        yield return null;

                    idEvent = GameManager.GetMachine().Events.FindIndex(x => x._event == _eventStepROTATABLE);
                    idAction = GameManager.GetMachine().Events[idEvent]._listActions.FindIndex(x => x._name == _actionStepROTATABLE);

                    _arrowRotate.SetActive(true);

                    while (!GameManager.GetMachine().Events[idEvent]._listActions[idAction]._value)
                    {
                        if (!GameManager.GetInputsManager().LockPinch)
                            GameManager.GetInputsManager().LockPinch = true;

                        yield return null;
                    }

                    _arrowRotate.SetActive(false);
                    UnshowPanel();

                    break; //goto case TUTO_STEP.PUSHABLE;


                case TUTO_STEP.PUSHABLE:
                    _step = TUTO_STEP.PUSHABLE;

                    yield return new WaitForSecondsRealtime(.25F);

                    GameManager.GetCamerasManager().GetCurrentCamera().GetComponent<CameraController>().ChangeModeToZoomOutTo(_vpPUSHABLE);

                    StartCoroutine(TutorialSentence());

                    while (!_displayTxt)
                        yield return null;

                    while (!_objPUSHABLE.IsSelected())
                    {
                        if (!GameManager.GetInputsManager().LockPinch)
                            GameManager.GetInputsManager().LockPinch = true;

                        yield return null;
                    }

                    yield return new WaitForSecondsRealtime(.5f);

                    if (Application.systemLanguage == SystemLanguage.French)
                        _mySentenceTranslateTuto.OpenBox(_mySentenceTranslateTuto.Find_KEY("BOX_" + _step.ToString()).SENTENCE_FR);
                    else
                        _mySentenceTranslateTuto.OpenBox(_mySentenceTranslateTuto.Find_KEY("BOX_" + _step.ToString()).SENTENCE_EN);

                    yield return new WaitWhile(() => InfoBox._infoBoxEnabled);

                    goto case TUTO_STEP.BACK_TO_VIEWPOINT;


                case TUTO_STEP.BACK_TO_VIEWPOINT:
                    _step = TUTO_STEP.BACK_TO_VIEWPOINT;

                    StartCoroutine(TutorialSentence());

                    while (!_displayTxt)
                        yield return null;

                    GameManager.GetInputsManager().LockPinch = false;

                    yield return new WaitWhile(() => _vpBACK_TO_VIEWPOINT != GameManager.GetCamerasManager().MainCamera.GetComponent<CameraController>().GetCurrentViewpoint());

                    goto case TUTO_STEP.TUTO_END;


                case TUTO_STEP.TUTO_END:
                    _step = TUTO_STEP.TUTO_END;

                    StartCoroutine(TutorialSentence());

                    while (!_displayTxt)
                        yield return null;

                    if (Application.systemLanguage == SystemLanguage.French)
                        _mySentenceTranslateTuto.OpenBox(_mySentenceTranslateTuto.Find_KEY("BOX_" + _step.ToString()).SENTENCE_FR);
                    else
                        _mySentenceTranslateTuto.OpenBox(_mySentenceTranslateTuto.Find_KEY("BOX_" + _step.ToString()).SENTENCE_EN);

                    yield return new WaitWhile(() => InfoBox._infoBoxEnabled);

                    ViewPointController.UnlockAllVp();

                    _vpGO_INTO_VIEWPOINT.LockViewpoint();

                    UnshowPanel();

                    break;


                case TUTO_STEP.TUTO_ENDOSCOPE_START:
                    _step = TUTO_STEP.TUTO_ENDOSCOPE_START;

                    if (Application.systemLanguage == SystemLanguage.French)
                        _mySentenceTranslateTuto.OpenBox(_mySentenceTranslateTuto.Find_KEY("BOX_" + _step.ToString()).SENTENCE_FR);
                    else
                        _mySentenceTranslateTuto.OpenBox(_mySentenceTranslateTuto.Find_KEY("BOX_" + _step.ToString()).SENTENCE_EN);

                    yield return new WaitForSecondsRealtime(1f);

                    _goTUTO_ENDOSCOPE.SetActive(true);

                    yield return new WaitWhile(() => InfoBox._infoBoxEnabled);

                    GameManager.GetInputsManager().LockPinch = true;

                    ShowPanel();

                    StartCoroutine(TutorialSentence());

                    while (!_displayTxt)
                        yield return null;

                    yield return new WaitUntil(() => EndoscopeEntrence.IsActive());

                    _endoscopeTools.GetComponent<Button>().interactable = false;

                    goto case TUTO_STEP.TUTO_ENDOSCOPE_MOVE;


                case TUTO_STEP.TUTO_ENDOSCOPE_MOVE:
                    _step = TUTO_STEP.TUTO_ENDOSCOPE_MOVE;

                    yield return new WaitForSecondsRealtime(3f);

                    StartCoroutine(TutorialSentence());

                    while (!_displayTxt)
                        yield return null;

                    idEvent = GameManager.GetMachine().Events.FindIndex(x => x._event == _eventStepTUTO_ENDOSCOPE_END);
                    idAction = GameManager.GetMachine().Events[idEvent]._listActions.FindIndex(x => x._name == _actionStepTUTO_ENDOSCOPE_END);

                    yield return new WaitWhile(() => !GameManager.GetMachine().Events[idEvent]._listActions[idAction]._value);

                    goto case TUTO_STEP.TUTO_ENDOSCOPE_END;


                case TUTO_STEP.TUTO_ENDOSCOPE_END:
                    _step = TUTO_STEP.TUTO_ENDOSCOPE_END;

                    yield return new WaitForSecondsRealtime(2f);

                    if (Application.systemLanguage == SystemLanguage.French)
                        _mySentenceTranslateTuto.OpenBox(_mySentenceTranslateTuto.Find_KEY("BOX_" + _step.ToString()).SENTENCE_FR);
                    else
                        _mySentenceTranslateTuto.OpenBox(_mySentenceTranslateTuto.Find_KEY("BOX_" + _step.ToString()).SENTENCE_EN);

                    yield return new WaitWhile(() => InfoBox._infoBoxEnabled);

                    GameManager.GetInputsManager().LockPinch = true;

                    StartCoroutine(TutorialSentence());

                    while (!_displayTxt)
                        yield return null;

                    _endoscopeTools.GetComponent<Button>().interactable = true;

                    yield return new WaitUntil(() => _endoscopeTools.IsSelected());

                    GameManager.GetCamerasManager().MainCamera.GetComponent<CameraController>().ChangeModeToZoomOutTo(_vpTUTO_ENDOSCOPE_END);
                    GameManager.GetInputsManager().LockPinch = false;

                    UnshowPanel();

                    break;
            }
        }

        private IEnumerator TutorialSentence()
        {
            _displayTxt = false;

            selfAnimation["Texte"].time = 1;
            selfAnimation["Texte"].speed = -1;
            selfAnimation.PlayQueued("Texte");

            while (selfAnimation.isPlaying)
                yield return null;

            if (Application.systemLanguage == SystemLanguage.French)
                _textTuto.text = _mySentenceTranslateTuto.Find_KEY(_step.ToString()).SENTENCE_FR;
            else
                _textTuto.text = _mySentenceTranslateTuto.Find_KEY(_step.ToString()).SENTENCE_EN;

            selfAnimation["Texte"].time = 0;
            selfAnimation["Texte"].speed = 1;
            selfAnimation.Play("Texte");

            while (selfAnimation.isPlaying)
                yield return null;

            _displayTxt = true;
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(Tuto))]
    public class TutoEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (true == Application.isPlaying)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Show"))
                {
                    Tuto hud = target as Tuto;
                    if (null != hud)
                    {
                        hud.ShowPanel();
                    }
                }
                if (GUILayout.Button("Unshow"))
                {
                    Tuto hud = target as Tuto;
                    if (null != hud)
                    {
                        hud.UnshowPanel();
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
    }
#endif
}