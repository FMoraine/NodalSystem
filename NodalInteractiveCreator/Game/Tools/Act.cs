using System.Collections.Generic;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using UnityEngine;
using NodalInteractiveCreator.Managers;
using NodalInteractiveCreator.Controllers;
using NodalInteractiveCreator.Viewpoints;
using NodalInteractiveCreator.HUD;
using NodalInteractiveCreator.Objects.Puzzle;
using NodalInteractiveCreator.Endoscop;

namespace NodalInteractiveCreator.Objects
{
    public abstract class Act
    {
        [SerializeField]
        [Versioning("actionsInteractives")]
        public List<ActionInteractive> _listActionInteractive = new List<ActionInteractive>();
        [SerializeField]
        [Versioning(VERSIONING_TAG.VIEWPOINT_ACTION)]
        public List<ActionViewpoint> _listActionViewpoint = new List<ActionViewpoint>();
        [SerializeField]
        [Versioning(VERSIONING_TAG.ANIMATION_ACTION)]
        public List<ActionAnimation> _listActionAnimation = new List<ActionAnimation>();
        [SerializeField]
        [Versioning(VERSIONING_TAG.AUDIO_ACTION)]
        public List<ActionAudio> _listActionAudio = new List<ActionAudio>();

        [System.Serializable]
        public class Action
        {
            public int _idAction = 0;
            public bool _isActionInteractive = false;
            public bool _isActionViewpoint = false;
            public bool _isActionAnimation = false;
            public bool _isActionAudio = false;
            public bool _playAudio = false;

            [System.Serializable]
            public class GET_VALUE
            {
                public int _idEvent;
                public int _idAction;
                public bool _enabled;
                public bool _value;
            }

            [System.Serializable]
            public struct ActiveAct
            {
                public InteractiveObject objToActive;
                public int idAction;
                public int idCheck;
                public int idMode;
                public float delay;

                public List<string> GetModeNames()
                {
                    List<string> listModeNames = new List<string>(2) { "Anime", "Time" };

                    return listModeNames;
                }
            }

            internal static bool GetValueAllow(ref List<GET_VALUE> list_GET_VALUE_STORY, ref List<bool> listCurrentStoryValue)
            {
                GetValueStory(ref list_GET_VALUE_STORY, ref listCurrentStoryValue);

                foreach (GET_VALUE getValue in list_GET_VALUE_STORY)
                {
                    if (getValue._enabled)
                        if (getValue._value != listCurrentStoryValue[list_GET_VALUE_STORY.IndexOf(getValue)])
                            return false;
                }
                return true;
            }

            internal static void GetValueStory(ref List<GET_VALUE> list_GET_VALUE_STORY, ref List<bool> listCurrentStoryValue)
            {
                Machine myMachine = GameManager.GetMachine();
                Machine.EVENTS myEvent;
                string myAction;

                listCurrentStoryValue.Clear();

                foreach (GET_VALUE getValue in list_GET_VALUE_STORY)
                {
                    myEvent = myMachine.Events[getValue._idEvent]._event;
                    myAction = myMachine.Events[getValue._idEvent]._listActions[getValue._idAction]._name;
                    listCurrentStoryValue.Add(myMachine.GetConditionValue(myEvent, myAction));
                }
            }

            //internal static void WaitActiveAction(ActiveAct activeAct)
            //{
            //    if (activeAct.idMode == 0)
            //    {
            //        if (activeAct.idAction == 0)
            //            activeAct.objToActive.StartCoroutine(activeAct.objToActive.WaitActiveAction(activeAct.objToActive._actionFinal));
            //        else if (activeAct.idAction == 1)
            //            activeAct.objToActive.StartCoroutine(activeAct.objToActive.WaitActiveAction(activeAct.objToActive._actionStart));
            //        else
            //        {
                        
            //            //if (activeAct.objToActive is RotatableObject)
            //            //{
            //            //    RotatableObject myRotObj = activeAct.objToActive as RotatableObject;
            //            //    myRotObj.StartCoroutine(myRotObj.WaitActiveAction(myRotObj.ListCheck[activeAct.idCheck].ActionCheck));
            //            //}
            //            //else if (activeAct.objToActive is TranslatableObject)
            //            //{
            //            //    TranslatableObject myTransObj = activeAct.objToActive as TranslatableObject;
            //            //    myTransObj.StartCoroutine(myTransObj.WaitActiveAction(myTransObj.ListCheck[activeAct.idCheck].ActionCheck));
            //            //}
            //        }
            //    }
            //    else
            //    {
            //        if (activeAct.idAction == 0)
            //            activeAct.objToActive.StartCoroutine(activeAct.objToActive.WaitActiveAction(activeAct.objToActive._actionFinal, activeAct.delay));
            //        else if (activeAct.idAction == 1)
            //            activeAct.objToActive.StartCoroutine(activeAct.objToActive.WaitActiveAction(activeAct.objToActive._actionStart, activeAct.delay));
            //        else
            //        {
            //            //if (activeAct.objToActive is Objects.RotatableObject)
            //            //{
            //            //    Objects.RotatableObject myRotObj = activeAct.objToActive as Objects.RotatableObject;
            //            //    myRotObj.StartCoroutine(myRotObj.WaitActiveAction(myRotObj.ListCheck[activeAct.idCheck].ActionCheck, activeAct.delay));
            //            //}
            //            //else if (activeAct.objToActive is Objects.TranslatableObject)
            //            //{
            //            //    Objects.TranslatableObject myTransObj = activeAct.objToActive as Objects.TranslatableObject;
            //            //    myTransObj.StartCoroutine(myTransObj.WaitActiveAction(myTransObj.ListCheck[activeAct.idCheck].ActionCheck, activeAct.delay));
            //            //}
            //        }
            //    }
            //}

            internal static bool GetTestValue(ref List<GET_VALUE> list_GET_VALUE_STORY, ref bool enabled_TEST_INTERACTIVITY_VALUE, ref List<bool> listCurrentStoryValue, ref bool currentStoryValue, ref bool valueForTEST_INTERACTIVITY_VALUE)
            {
                if (list_GET_VALUE_STORY.Exists(x => x._enabled == true) && enabled_TEST_INTERACTIVITY_VALUE)
                {
                    if (GetValueAllow(ref list_GET_VALUE_STORY, ref listCurrentStoryValue) && currentStoryValue == valueForTEST_INTERACTIVITY_VALUE)
                        return true;
                }
                else if (list_GET_VALUE_STORY.Exists(x => x._enabled == true))
                {
                    if (GetValueAllow(ref list_GET_VALUE_STORY, ref listCurrentStoryValue))
                        return true;
                }
                else if (enabled_TEST_INTERACTIVITY_VALUE)
                {
                    if (currentStoryValue == valueForTEST_INTERACTIVITY_VALUE)
                        return true;
                }
                else
                    return true;

                return false;
            }
        }

        [System.Serializable]
        public class ActionInteractive
        {
            public enum ACTIONS_INTERACTIVE
            {
                NONE = 0,
                GET_VALUE_STORY = 1, //Done
                SET_VALUE_STORY = 2, //Done
                LOCK_INTERACTIVE = 3, //Done
                UNLOCK_INTERACTIVE = 4, //Done
                ACTIVE_PUSH = 5, //Done
                CHANGE_INTERACTIVITY = 6, //Done
                CHANGE_TO_PICKABLE = 7, //''
                CHANGE_TO_PUSH = 8, //''
                CHANGE_TO_ROTATE = 9, // ''
                CHANGE_TO_TARGET = 10, //''
                CHANGE_TO_TRANSLATE = 11, //''
                BLOCK_OR_UNBLOCK_CHECK = 12, //Done
                BLOCK_ALL_CHECKS = 13, //Done
                UNBLOCK_ALL_CHECKS = 14, //Done
                TEST_INTERACTIVITY_VALUE = 15, //Review
                OPEN_DIALOGBOX = 16, //Done
                CHANGE_MATERIAL = 17, //Wip
                REPLACE_MATERIAL = 18, //Review
                SET_ACTIVE_GAMEOBJECT = 19, //Done
                ACTIVE_ACTION = 20, //Done
                DISABLE_ENABLE_BACK_TO_START = 21, //Done
                SET_MATERIAL_CONFIG = 22, //To Do
                ROTATE_OBJECT = 24, //Review
                ENDOSCOPE_ACTIVATE = 26, //Done
                BLOCK_UNBLOCK_ITEM_TARGET = 27, //Done
                SHOW_UNSHOW_TUTO = 28, //Done
                PUZZLE_ACTION = 29 //Done
            }

            [SerializeField]
            private InteractiveObject _objTarget = null;
            public InteractiveObject ObjTarget { get { return _objTarget; } set { _objTarget = value; } }
            [SerializeField]
            public ACTIONS_INTERACTIVE _action = ACTIONS_INTERACTIVE.NONE;

            public float _timeForWait;
            public bool _activeInteraction = true;

            public static bool _currentStoryValue = false;
            private List<bool> _listCurrentStoryValue = new List<bool>();

            //Variable for SET_VALUE_STORY
            public bool _valueForSET_VALUE_STORY = false;
            public int _idEventForSET_VALUE_STORY = 0;
            public int _idActionForSET_VALUE_STORY = 0;

            //Variable for GET_VALUE_STORY
            public int _idEventForGET_VALUE_STORY = 0;
            public int _idActionForGET_VALUE_STORY = 0;
            public List<Action.GET_VALUE> _list_GET_VALUE_STORY = new List<Action.GET_VALUE>();

            //Variable for TEST_INTERACTIVITY_VALUE
            public bool _enabled_TEST_INTERACTIVITY_VALUE = false;
            public bool _valueForTEST_INTERACTIVITY_VALUE = false;
            public int _idEventForTEST_INTERACTIVITY_VALUE = 0;
            public int _idActionForTEST_INTERACTIVITY_VALUE = 0;
            public List<Object> _objToCheckForTEST_INTERACTIVITY_VALUE = new List<Object>();

            //Variable for REPLACE_MATERIAL
            public int _idCurrentMat;
            public Material _newMat;

            //Variable for SET_ACTIVE_GAMEOBJECT
            public GameObject _goToActive;
            public bool _valueForSET_ACTIVE_GAMEOBJECT;

            //Variable for OPEN_DIALOGBOX
            public int _idMessageBox;
            public bool _oneShot;
            public bool _finalBox;
            public bool _onClick;

            //Variable for SHOW_UNSHOW_TUTO
            public bool _showTuto = false;
            public Tuto.TUTO_STEP _step;

            //Other Variable
            public bool _enableObj = true;
            public bool _valueForBLOCK_UNBLOCK_ITEM_TARGET = false;
            public bool _valueForDISABLE_ENABLE_BACK_TO_START = false;
            public int _idAction;
            public int _idAngleForROTATE_OBJECT = 0;
            public GameObject _goToCHANGE_INTERACTIVITY;
            public List<bool> _valuesForBLOCK_OR_UNBLOCK_CHECK = new List<bool>();
            public List<MaterialConfig> _materialConfigTargets = new List<MaterialConfig>();

            //Puzzle System variable
            public int _actionID = 0;


            public void PlayActionInteractive(InteractiveObject obj, ACTIONS_INTERACTIVE action)
            {
                if (!_activeInteraction || !ObjTarget.gameObject.activeSelf)
                    return;

                Machine.EVENTS myEvent;
                string myAction;

                switch (action)
                {
                    case ACTIONS_INTERACTIVE.TEST_INTERACTIVITY_VALUE:
                        //myEvent = obj.LinkToMachine.Events[_idEventForTEST_INTERACTIVITY_VALUE]._event;
                        //myAction = obj.LinkToMachine.Events[_idEventForTEST_INTERACTIVITY_VALUE]._listActions[_idActionForTEST_INTERACTIVITY_VALUE]._name;

                        foreach (InteractiveObject o in _objToCheckForTEST_INTERACTIVITY_VALUE)
                        {
                            //if (o._actionCurrentCheck != _idActionForTEST_INTERACTIVITY_VALUE)
                            //{
                            //    _currentStoryValue = obj.LinkToMachine.GetConditionValue(myEvent, myAction);
                            //    return;
                            //}
                            //_currentStoryValue = !obj.LinkToMachine.GetConditionValue(myEvent, myAction);
                        }
                        break;

                    case ACTIONS_INTERACTIVE.GET_VALUE_STORY:
                        Action.GetValueStory(ref _list_GET_VALUE_STORY, ref _listCurrentStoryValue);
                        break;

                    case ACTIONS_INTERACTIVE.SET_VALUE_STORY:
                        if (Action.GetTestValue(ref _list_GET_VALUE_STORY, ref _enabled_TEST_INTERACTIVITY_VALUE, ref _listCurrentStoryValue, ref _currentStoryValue, ref _valueForTEST_INTERACTIVITY_VALUE))
                        {
                            //myEvent = obj.LinkToMachine.Events[_idEventForSET_VALUE_STORY]._event;
                            //myAction = obj.LinkToMachine.Events[_idEventForSET_VALUE_STORY]._listActions[_idActionForSET_VALUE_STORY]._name;

                            //obj.LinkToMachine.SetConditionValue(myEvent, myAction, _valueForSET_VALUE_STORY);
                        }
                        break;

                    case ACTIONS_INTERACTIVE.SET_MATERIAL_CONFIG:
                        if (Action.GetTestValue(ref _list_GET_VALUE_STORY, ref _enabled_TEST_INTERACTIVITY_VALUE, ref _listCurrentStoryValue, ref _currentStoryValue, ref _valueForTEST_INTERACTIVITY_VALUE))
                        {
                            obj._materials.Clear();
                            obj._materials.AddRange(_materialConfigTargets);
                            //obj.LoadWaitForActiveInteractivity(() => obj._materials = _materialConfigTargets, _timeForWait);
                        }
                        break;

                    case ACTIONS_INTERACTIVE.DISABLE_ENABLE_BACK_TO_START:
                        if (Action.GetTestValue(ref _list_GET_VALUE_STORY, ref _enabled_TEST_INTERACTIVITY_VALUE, ref _listCurrentStoryValue, ref _currentStoryValue, ref _valueForTEST_INTERACTIVITY_VALUE))
                        {
                            //obj.LoadWaitForActiveInteractivity(() => obj.DisableEnableBackToStart(_valueForDISABLE_ENABLE_BACK_TO_START), _timeForWait);
                        }
                        break;

                    case ACTIONS_INTERACTIVE.SHOW_UNSHOW_TUTO:
                        if (Action.GetTestValue(ref _list_GET_VALUE_STORY, ref _enabled_TEST_INTERACTIVITY_VALUE, ref _listCurrentStoryValue, ref _currentStoryValue, ref _valueForTEST_INTERACTIVITY_VALUE))
                        {
                            Tuto mytuto = HUDManager.GetTuto();

                            if (_showTuto)
                            {
                                if (!mytuto.gameObject.activeSelf)
                                    mytuto.gameObject.SetActive(true);

                                mytuto.StartCoroutine(mytuto.TutoStep(_step));
                            }
                            else
                            {
                                mytuto.UnshowPanel();
                            }
                        }
                        break;

                    case ACTIONS_INTERACTIVE.ACTIVE_ACTION:
                        if (Action.GetTestValue(ref _list_GET_VALUE_STORY, ref _enabled_TEST_INTERACTIVITY_VALUE, ref _listCurrentStoryValue, ref _currentStoryValue, ref _valueForTEST_INTERACTIVITY_VALUE))
                        {
                            //if (_idAction == 0)
                            //    obj.LoadWaitForActiveInteractivity(() => obj.ActiveAction(obj._actionFinal), _timeForWait);
                            //else if (_idAction == 1)
                            //    obj.LoadWaitForActiveInteractivity(() => obj.ActiveAction(obj._actionStart), _timeForWait);
                        }
                        break;

                    case ACTIONS_INTERACTIVE.SET_ACTIVE_GAMEOBJECT:
                        if (Action.GetTestValue(ref _list_GET_VALUE_STORY, ref _enabled_TEST_INTERACTIVITY_VALUE, ref _listCurrentStoryValue, ref _currentStoryValue, ref _valueForTEST_INTERACTIVITY_VALUE))
                        {
                            //obj.LoadWaitForActiveInteractivity(() => obj.SetActiveObj(_goToActive, _valueForSET_ACTIVE_GAMEOBJECT), _timeForWait);
                        }
                        break;

                    //WiP
                    case ACTIONS_INTERACTIVE.REPLACE_MATERIAL:
                        if (Action.GetTestValue(ref _list_GET_VALUE_STORY, ref _enabled_TEST_INTERACTIVITY_VALUE, ref _listCurrentStoryValue, ref _currentStoryValue, ref _valueForTEST_INTERACTIVITY_VALUE))
                        {
                            //obj.LoadWaitForActiveInteractivity(() => obj.GetComponent<MeshRenderer>().sharedMaterials[_idCurrentMat] = _newMat, _timeForWait);
                            //obj.GetComponent<MeshRenderer>().sharedMaterials[_idCurrentMat] = _newMat;
                        }

                        //if (_list_GET_VALUE_STORY.Exists(x => x._enabled == true) && _enabled_TEST_INTERACTIVITY_VALUE)
                        //{
                        //    if (Action.GetValueAllow(ref _list_GET_VALUE_STORY, ref _listCurrentStoryValue) && _currentStoryValue == _valueForTEST_INTERACTIVITY_VALUE)
                        //        obj.GetComponent<MeshRenderer>().sharedMaterials[_idCurrentMat] = _newMat;
                        //}
                        //else if (_list_GET_VALUE_STORY.Exists(x => x._enabled == true))
                        //{
                        //    if (Action.GetValueAllow(ref _list_GET_VALUE_STORY, ref _listCurrentStoryValue))
                        //        obj.GetComponent<MeshRenderer>().sharedMaterials[_idCurrentMat] = _newMat; 
                        //}
                        //else if (_enabled_TEST_INTERACTIVITY_VALUE)
                        //{
                        //    if (_currentStoryValue == _valueForTEST_INTERACTIVITY_VALUE)
                        //        obj.GetComponent<MeshRenderer>().sharedMaterials[_idCurrentMat] = _newMat;
                        //}
                        //else
                        //{
                        //    Debug.Log(obj.GetComponent<Renderer>().sharedMaterials[_idCurrentMat]);

                        //    foreach (Material mat in obj.GetComponent<MeshRenderer>().sharedMaterials)
                        //    {
                        //        if (obj.GetComponent<MeshRenderer>().sharedMaterials[_idCurrentMat].name == mat.name)
                        //            obj.GetComponent<MeshRenderer>().sharedMaterial = _newMat;
                        //    }
                        //}
                        break;

                    case ACTIONS_INTERACTIVE.CHANGE_MATERIAL:
                        if (Action.GetTestValue(ref _list_GET_VALUE_STORY, ref _enabled_TEST_INTERACTIVITY_VALUE, ref _listCurrentStoryValue, ref _currentStoryValue, ref _valueForTEST_INTERACTIVITY_VALUE))
                        {
                            float gap = 1;
                            //obj.LoadWaitForActiveInteractivity(() => ObjTarget.ChangeMaterial(ref gap, ref _materialConfigTargets), _timeForWait);
                        }
                        break;

                    case ACTIONS_INTERACTIVE.OPEN_DIALOGBOX:
                        if (Action.GetTestValue(ref _list_GET_VALUE_STORY, ref _enabled_TEST_INTERACTIVITY_VALUE, ref _listCurrentStoryValue, ref _currentStoryValue, ref _valueForTEST_INTERACTIVITY_VALUE))
                        {
                            //if (_finalBox)
                            //    InfoBox._finalInfoBox = true;
                            //if (_oneShot)
                            //    _activeInteraction = false;

                            //if (obj is ItemTrigger && _onClick)
                            //{
                            //    ItemTrigger it = obj as ItemTrigger;

                            //    if (it._openBoxOnClick)
                            //        obj.LoadWaitForActiveInteractivity(() => obj.OpenBox(_idMessageBox), _timeForWait);
                            //}
                            //else
                                //obj.LoadWaitForActiveInteractivity(() => obj.OpenBox(_idMessageBox), _timeForWait);
                        }
                        break;

                    case ACTIONS_INTERACTIVE.LOCK_INTERACTIVE:
                        if (Action.GetTestValue(ref _list_GET_VALUE_STORY, ref _enabled_TEST_INTERACTIVITY_VALUE, ref _listCurrentStoryValue, ref _currentStoryValue, ref _valueForTEST_INTERACTIVITY_VALUE))
                        {
                            //obj.LoadWaitForActiveInteractivity(() => obj.LockObject(), _timeForWait);
                        }
                        break;

                    case ACTIONS_INTERACTIVE.UNLOCK_INTERACTIVE:
                        if (Action.GetTestValue(ref _list_GET_VALUE_STORY, ref _enabled_TEST_INTERACTIVITY_VALUE, ref _listCurrentStoryValue, ref _currentStoryValue, ref _valueForTEST_INTERACTIVITY_VALUE))
                        {
                            //obj.LoadWaitForActiveInteractivity(() => obj.UnlockObject(_enableObj), _timeForWait);
                        }
                        break;


                    case ACTIONS_INTERACTIVE.ACTIVE_PUSH:
                        if (Action.GetTestValue(ref _list_GET_VALUE_STORY, ref _enabled_TEST_INTERACTIVITY_VALUE, ref _listCurrentStoryValue, ref _currentStoryValue, ref _valueForTEST_INTERACTIVITY_VALUE))
                        {
                            //obj.LoadWaitForActiveInteractivity(() => obj.ActivePush(obj), _timeForWait);
                        }
                        break;

                    case ACTIONS_INTERACTIVE.CHANGE_INTERACTIVITY:
                        if (Action.GetTestValue(ref _list_GET_VALUE_STORY, ref _enabled_TEST_INTERACTIVITY_VALUE, ref _listCurrentStoryValue, ref _currentStoryValue, ref _valueForTEST_INTERACTIVITY_VALUE))
                        {
                            //obj.LoadWaitForActiveInteractivity(() => obj.ChangeInteractivity(_goToCHANGE_INTERACTIVITY), _timeForWait);
                        }
                        break;

                    case ACTIONS_INTERACTIVE.CHANGE_TO_PICKABLE:
                        break;

                    case ACTIONS_INTERACTIVE.CHANGE_TO_PUSH:
                        break;

                    case ACTIONS_INTERACTIVE.CHANGE_TO_ROTATE:
                        break;

                    case ACTIONS_INTERACTIVE.CHANGE_TO_TARGET:
                        break;

                    case ACTIONS_INTERACTIVE.CHANGE_TO_TRANSLATE:
                        break;

                    case ACTIONS_INTERACTIVE.BLOCK_OR_UNBLOCK_CHECK:
                        if (Action.GetTestValue(ref _list_GET_VALUE_STORY, ref _enabled_TEST_INTERACTIVITY_VALUE, ref _listCurrentStoryValue, ref _currentStoryValue, ref _valueForTEST_INTERACTIVITY_VALUE))
                        {
                            //obj.LoadWaitForActiveInteractivity(() => obj.BlockOrUnblockChecks(_valuesForBLOCK_OR_UNBLOCK_CHECK), _timeForWait);
                        }
                        break;

                    case ACTIONS_INTERACTIVE.BLOCK_ALL_CHECKS:
                        if (Action.GetTestValue(ref _list_GET_VALUE_STORY, ref _enabled_TEST_INTERACTIVITY_VALUE, ref _listCurrentStoryValue, ref _currentStoryValue, ref _valueForTEST_INTERACTIVITY_VALUE))
                        {
                            //obj.LoadWaitForActiveInteractivity(() => obj.BlockOrUnblockAllChecks(true), _timeForWait);
                        }
                        break;

                    case ACTIONS_INTERACTIVE.UNBLOCK_ALL_CHECKS:
                        if (Action.GetTestValue(ref _list_GET_VALUE_STORY, ref _enabled_TEST_INTERACTIVITY_VALUE, ref _listCurrentStoryValue, ref _currentStoryValue, ref _valueForTEST_INTERACTIVITY_VALUE))
                        {
                           //obj.LoadWaitForActiveInteractivity(() => obj.BlockOrUnblockAllChecks(false), _timeForWait);
                        }
                        break;

                    case ACTIONS_INTERACTIVE.PUZZLE_ACTION:
                        if (obj is PuzzleSystem)
                        {
                            PuzzleSystem ps = obj as PuzzleSystem;
                            ps.StartAction(_actionID);
                        }
                        break;

                    case ACTIONS_INTERACTIVE.ROTATE_OBJECT:
                        //if (obj is Objects.RotatableObject)
                        //{
                        //    Objects.RotatableObject ro = obj as Objects.RotatableObject;
                        //    obj.LoadWaitForActiveInteractivity(() => ro.AutoRotate(ro.ListCheck[_idAngleForROTATE_OBJECT].Angle), _timeForWait);
                        //}
                        break;

                    case ACTIONS_INTERACTIVE.ENDOSCOPE_ACTIVATE:
                        if (obj is EndoscopeEntrence)
                        {
                            EndoscopeEntrence ee = obj as EndoscopeEntrence;
                            ee.OnEntrance();
                        }
                        break;

                    case ACTIONS_INTERACTIVE.BLOCK_UNBLOCK_ITEM_TARGET:
                        //if (obj is ItemTrigger)
                        //{
                        //    ItemTrigger it = obj as ItemTrigger;

                        //    if (_valueForBLOCK_UNBLOCK_ITEM_TARGET)
                        //        obj.LoadWaitForActiveInteractivity(() => it._blockEnabled = true, _timeForWait);
                        //    else
                        //        obj.LoadWaitForActiveInteractivity(() => it._blockEnabled = false, _timeForWait);
                        //}
                        break;

                    default:
                        Debug.Log(obj.name + " : Action " + action.ToString() + " doesn't existed !");
                        break;
                }
            }
        }

        [System.Serializable]
        public class ActionViewpoint
        {
            public enum ACTIONS_VIEWPOINT
            {
                NONE,
                GET_VALUE_STORY,
                LOCK_VIEWPOINT,
                UNLOCK_VIEWPOINT,
                MOVE_TO
            }

            [SerializeField]
            private ViewPoint _vpTarget = null;
            public ViewPoint VpTarget { get { return _vpTarget; } set { _vpTarget = value; } }
            [SerializeField]
            public ACTIONS_VIEWPOINT _action = ACTIONS_VIEWPOINT.NONE;

            public bool _activeInteraction = true;

            public static bool _currentStoryValue = false;
            private List<bool> _listCurrentStoryValue = new List<bool>();
            public List<Action.GET_VALUE> _list_GET_VALUE_STORY = new List<Action.GET_VALUE>();

            public int _idEventForGET_VALUE_STORY = 0;
            public int _idActionForGET_VALUE_STORY = 0;

            public void PlayActionViewpoint(ViewPoint vp, ACTIONS_VIEWPOINT action)
            {
                if (!_activeInteraction)
                    return;

                switch (action)
                {
                    case ACTIONS_VIEWPOINT.GET_VALUE_STORY:
                        Action.GetValueStory(ref _list_GET_VALUE_STORY, ref _listCurrentStoryValue);
                        break;

                    case ACTIONS_VIEWPOINT.LOCK_VIEWPOINT:
                        if (_list_GET_VALUE_STORY.Exists(x => x._enabled == true))
                        {
                            if (Action.GetValueAllow(ref _list_GET_VALUE_STORY, ref _listCurrentStoryValue))
                                vp.LockViewpoint();
                        }
                        else
                            vp.LockViewpoint();
                        break;

                    case ACTIONS_VIEWPOINT.UNLOCK_VIEWPOINT:
                        if (_list_GET_VALUE_STORY.Exists(x => x._enabled == true))
                        {
                            if (Action.GetValueAllow(ref _list_GET_VALUE_STORY, ref _listCurrentStoryValue))
                                vp.UnlockViewpoint();
                        }
                        else
                            vp.UnlockViewpoint();
                        break;

                    case ACTIONS_VIEWPOINT.MOVE_TO:
                        if (_list_GET_VALUE_STORY.Exists(x => x._enabled == true))
                        {
                            if (Action.GetValueAllow(ref _list_GET_VALUE_STORY, ref _listCurrentStoryValue))
                                GameManager.GetCamerasManager().MainCamera.GetComponent<CameraController>().ChangeModeToZoomOutTo(vp);
                        }
                        else
                            GameManager.GetCamerasManager().MainCamera.GetComponent<CameraController>().ChangeModeToZoomOutTo(vp);
                        break;

                    default:
                        Debug.Log("Action " + action.ToString() + " doesn't existed !");
                        break;
                }
            }
        }

        [System.Serializable]
        public class ActionAnimation
        {
            public enum ACTIONS_ANIMATION
            {
                NONE,
                GET_VALUE_STORY,
                PLAY_ANIMATION,
                PLAY_CUTSCENE,
                STOP_ANIMATION,
                TEST_INTERACTIVITY_VALUE
            }

            [SerializeField]
            private Animation _animeTarget = null;
            public Animation AnimeTarget { get { return _animeTarget; } set { _animeTarget = value; } }
            [SerializeField]
            public ACTIONS_ANIMATION _action = ACTIONS_ANIMATION.NONE;

            public float _timeForWait;
            public bool _activeInteraction = true;

            //Variable for PLAY_ANIMATION
            public AnimationClip _animeClip;
            public int _idAnim = 0;
            public bool _reverse = false;
            public bool _replay = false;
            public bool _lockPinch = false;
            public bool _lockInteractivity = false;
            public bool _activeAct = false;
            public List<Action.ActiveAct> _activeActions = new List<Action.ActiveAct>();

            public Animator _gameAnimator;
            public int _idState = 0;
            public InteractiveObject _myObj = null;

            public string _cutsceneName = null;

            public static bool _currentStoryValue = false;
            private List<bool> _listCurrentStoryValue = new List<bool>();
            public List<Action.GET_VALUE> _list_GET_VALUE_STORY = new List<Action.GET_VALUE>();

            public int _idEventForGET_VALUE_STORY = 0;
            public int _idActionForGET_VALUE_STORY = 0;

            //Variable for TEST_INTERACTIVITY_VALUE
            public bool _enabled_TEST_INTERACTIVITY_VALUE = false;
            public bool _valueForTEST_INTERACTIVITY_VALUE = false;
            public int _idEventForTEST_INTERACTIVITY_VALUE = 0;
            public int _idActionForTEST_INTERACTIVITY_VALUE = 0;
            public List<Object> _objToCheckForTEST_INTERACTIVITY_VALUE = new List<Object>();

            public void PlayActionAnimation(Animation anime, ACTIONS_ANIMATION action)
            {
                if (!_activeInteraction)
                    return;

                Machine.EVENTS myEvent;
                string myAction;

                switch (action)
                {
                    case ACTIONS_ANIMATION.GET_VALUE_STORY:
                        Action.GetValueStory(ref _list_GET_VALUE_STORY, ref _listCurrentStoryValue);
                        break;

                    case ACTIONS_ANIMATION.TEST_INTERACTIVITY_VALUE:
                        myEvent = GameManager.GetMachine().Events[_idEventForTEST_INTERACTIVITY_VALUE]._event;
                        myAction = GameManager.GetMachine().Events[_idEventForTEST_INTERACTIVITY_VALUE]._listActions[_idActionForTEST_INTERACTIVITY_VALUE]._name;

                        //foreach (InteractiveObject o in _objToCheckForTEST_INTERACTIVITY_VALUE)
                        //{
                        //    if (o._actionCurrentCheck != _idActionForTEST_INTERACTIVITY_VALUE)
                        //    {
                        //        _currentStoryValue = GameManager.GetMachine().GetConditionValue(myEvent, myAction);
                        //        return;
                        //    }
                        //    _currentStoryValue = !GameManager.GetMachine().GetConditionValue(myEvent, myAction);
                        //}
                        break;

                    case ACTIONS_ANIMATION.PLAY_ANIMATION:
                        if (Action.GetTestValue(ref _list_GET_VALUE_STORY, ref _enabled_TEST_INTERACTIVITY_VALUE, ref _listCurrentStoryValue, ref _currentStoryValue, ref _valueForTEST_INTERACTIVITY_VALUE))
                        {
                            if (_lockInteractivity)
                                GameManager.GetInputsManager().LockInteractivity = true;
                            if (_lockPinch)
                                GameManager.GetInputsManager().LockPinch = true;

                            //_myObj.LoadWaitForActiveInteractivity(() => _myObj.ActionPlayAnimation(anime, _animeClip, _replay, _reverse), _timeForWait);

                            //if (_activeAct)
                            //    foreach (Action.ActiveAct activeAct in _activeActions)
                            //        Action.WaitActiveAction(activeAct);
                            //else if (_lockInteractivity || _lockPinch)
                            //    GameManager.GetInputsManager().StartCoroutine(GameManager.GetInputsManager().WaitToUnlock(_animeClip.length));
                        }
                        break;

                    case ACTIONS_ANIMATION.STOP_ANIMATION:
                        //if (Action.GetTestValue(ref _list_GET_VALUE_STORY, ref _enabled_TEST_INTERACTIVITY_VALUE, ref _listCurrentStoryValue, ref _currentStoryValue, ref _valueForTEST_INTERACTIVITY_VALUE))
                            //_myObj.StartCoroutine(_myObj.WaitForActiveInteractivity(() => anime.Stop(), _timeForWait));
                        break;

                    case ACTIONS_ANIMATION.PLAY_CUTSCENE:
                        if (Action.GetTestValue(ref _list_GET_VALUE_STORY, ref _enabled_TEST_INTERACTIVITY_VALUE, ref _listCurrentStoryValue, ref _currentStoryValue, ref _valueForTEST_INTERACTIVITY_VALUE))
                        {
                            //_myObj.LoadWaitForActiveInteractivity(() => _myObj.ActionPlayCutscene(_cutsceneName), _timeForWait);

                            //if (_activeAct)
                            //    foreach (Action.ActiveAct activeAct in _activeActions)
                            //        Action.WaitActiveAction(activeAct);
                        }
                        break;

                    default:
                        Debug.Log("Action " + action.ToString() + " doesn't existed !");
                        break;
                }
            }
        }

        [System.Serializable]
        public class ActionAudio
        {
            public enum ACTIONS_AUDIO
            {
                NONE,
                GET_VALUE_STORY,
                PLAY_OUT_OF_VIEWPOINT,
                CHANGE_AUDIOCLIP
            }

            [SerializeField]
            private AudioSource _audioSourceTarget = null;
            public AudioSource AudioSourceTarget { get { return _audioSourceTarget; } set { _audioSourceTarget = value; } }
            [SerializeField]
            public ACTIONS_AUDIO _action = ACTIONS_AUDIO.NONE;

            public bool _activeInteraction = true;

            public AudioClip _myAudioClip;
            public float _volume = 1;

            public static bool _currentStoryValue = false;
            private List<bool> _listCurrentStoryValue = new List<bool>();
            public List<Action.GET_VALUE> _list_GET_VALUE_STORY = new List<Action.GET_VALUE>();

            public int _idEventForGET_VALUE_STORY = 0;
            public int _idActionForGET_VALUE_STORY = 0;

            public void PlayActionAudio(AudioSource audioSource, ACTIONS_AUDIO action)
            {
                if (!_activeInteraction)
                    return;

                switch (action)
                {
                    case ACTIONS_AUDIO.GET_VALUE_STORY:
                        Action.GetValueStory(ref _list_GET_VALUE_STORY, ref _listCurrentStoryValue);
                        break;

                    case ACTIONS_AUDIO.PLAY_OUT_OF_VIEWPOINT:

                        break;

                    case ACTIONS_AUDIO.CHANGE_AUDIOCLIP:
                        if (_list_GET_VALUE_STORY.Exists(x => x._enabled == true))
                        {
                            if (Action.GetValueAllow(ref _list_GET_VALUE_STORY, ref _listCurrentStoryValue))
                            {
                                if (audioSource.clip != _myAudioClip)
                                {
                                    audioSource.enabled = false;
                                    audioSource.volume = _volume;
                                    audioSource.clip = _myAudioClip;
                                }
                            }
                        }
                        else
                        {
                            if (audioSource.clip != _myAudioClip)
                            {
                                audioSource.enabled = false;
                                audioSource.volume = _volume;
                                audioSource.clip = _myAudioClip;
                            }
                        }
                        break;

                    default:
                        Debug.Log("Action " + action.ToString() + " doesn't existed !");
                        break;
                }
            }
        }
    }

}