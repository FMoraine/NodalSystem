// Machinika Museum Prototype v.01
// © Littlefield Studio
// Writted by Rémi Carreira - 2015
//
// InputsManager.cs

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using NodalInteractiveCreator.Objects;
using NodalInteractiveCreator.Controllers;
using NodalInteractiveCreator.Viewpoints;
using NodalInteractiveCreator.HUD;

namespace NodalInteractiveCreator.Managers
{
    public class InputsManager : MonoBehaviour
    {
        public static bool _isTouch = false;
        public static Transform cameraFrom = null;

        public GraphicRaycaster CanvasRaycaster = null;

        public float AxisSensitivity = 0.1f;
        public float DoubleTapDelay = 0.3f;
        public float DoubleTapMaxDistance = 30.0f;
        public float PinchOutDistance = 1f;
        [Range(0.5f, 1)] public float PinchInDistance = 1f;
        public float _smoothPinch = 5f;

        [SerializeField]
        private Text _debugHit = null;

        private Camera currentCamera = null;
        private CameraController currentCameraController = null;

        private List<InputContainer> inputsBegan = new List<InputContainer>();
        private List<InputContainer> inputsMoved = new List<InputContainer>();
        private List<InputContainer> inputsEnded = new List<InputContainer>();
        private int nbTouch = 0;

        private Vector2 lastPositionOneTouch = Vector2.zero;
        private float lastTimeOneTouch = 0.0f;

        private bool zoomOut = false;
        public bool ZoomOut { set { zoomOut = value; } }
        private bool zoomIn = false;
        public bool ZoomIn { set { zoomIn = value; } }

        private Vector3 targetPinch = new Vector3();
        private ViewPoint targetVP = null;

        private float initialPinchLength = 0.0f;
        private float currentPinchLength = 0.0f;

        private PointerEventData canvasPointer = new PointerEventData(null);
        private List<RaycastResult> canvasResults = new List<RaycastResult>();

        private CamerasManager camerasMgr = null;

        public bool LockInput { get; set; }
        public bool LockPinch { get; set; }
        public bool LockCanvas { get; set; }
        public bool LockViewpoint { get; set; }
        public bool LockInteractivity { get; set; }

        public float CurrentPinchTime { get; set; }

        public CameraController CurrentCameraController { get { return currentCameraController; } }

        void Start()
        {
            _isTouch = false;

            camerasMgr = GameManager.GetCamerasManager();
            cameraFrom = camerasMgr.GetCurrentCamera().transform;
        }

        void Update()
        {
            if (null != camerasMgr)
            {
                currentCamera = camerasMgr.GetCurrentCamera();
                currentCameraController = currentCamera.GetComponent<CameraController>();
            }

            if (zoomOut || zoomIn)
                return;

            if (!Inventory.InspectSystem.IsInspecting())
                if (((LockInput || LockInteractivity || LockCanvas) && !CurrentCameraController._isPinched) || InfoBox._infoBoxEnabled)
                {
                    inputsMoved.ForEach(delegate (InputContainer ic)
                    {
                        if (ic != null)
                        {
                            if (ic.ObjectTouched != null)
                            {
                                if (ic.ObjectTouchedType == InputContainer.ObjectType.INTERACTIVE && ic.ObjectTouched.GetComponent<InteractiveObject>() != null)
                                    ic.ObjectTouched.GetComponent<InteractiveObject>().DeselectObject();
                                else if (ic.ObjectTouchedType == InputContainer.ObjectType.CANVAS && ic.ObjectTouched.GetComponent<InteractiveCanvas>() != null)
                                {
                                    if (ic.ObjectTouched.GetComponent<InteractiveCanvas>().IsSelected())
                                        ic.ObjectTouched.GetComponent<InteractiveCanvas>().Deselect();
                                }
                            }
                        }
                    });

                    inputsBegan.Clear();
                    inputsMoved.Clear();
                    inputsEnded.Clear();
                    nbTouch = 0;
                }

            if (!LockInput)
            {
                if (0 < Input.touchCount)
                {
                    HandleMobileInputs();
                }
                #region Editor
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                else
                {
                    HandlePCInputs();
                }
#endif
                #endregion

                UpdateTouchs();
                UpdateInputsBeganList();
                UpdateInputsEndedList();
            }
            else
            {
                if (nbTouch != 0)
                {
                    inputsBegan.Clear();
                    inputsMoved.Clear();
                    inputsEnded.Clear();
                    nbTouch = 0;
                }
            }
        }

        #region HandleInputs
        void HandleMobileInputs()
        {
            for (int index = 0; index < Input.touchCount; ++index)
            {
                Touch touch = Input.GetTouch(index);
                if (TouchPhase.Began == touch.phase)
                    InputBegan(InputContainer.InputType.MOBILE, touch.fingerId, touch.position);
                else if (TouchPhase.Moved == touch.phase)
                    InputMoved(InputContainer.InputType.MOBILE, touch.fingerId, touch.position);
                else if (TouchPhase.Ended == touch.phase || TouchPhase.Canceled == touch.phase)
                    InputEnded(InputContainer.InputType.MOBILE, touch.fingerId, touch.position);
            }
        }

        void HandlePCInputs()
        {
            for (int indexButton = 0; indexButton < 3; ++indexButton)
            {
                if (Input.GetMouseButtonDown(indexButton))
                    InputBegan(InputContainer.InputType.PC, indexButton, Input.mousePosition);
                else if (Input.GetMouseButton(indexButton))
                    InputMoved(InputContainer.InputType.PC, indexButton, Input.mousePosition);
                else if (Input.GetMouseButtonUp(indexButton))
                    InputEnded(InputContainer.InputType.PC, indexButton, Input.mousePosition);
            }
        }
        #endregion

        #region Update Touchs
        void UpdateTouchs()
        {
            nbTouch = inputsBegan.Count + inputsMoved.Count + inputsEnded.Count;

            if (nbTouch != 2 && inputsMoved.Count != 2 && CurrentCameraController._isPinched && !LockPinch)
            {
                CurrentCameraController.ZoomPinchOver();
            }

            if (nbTouch > 0)
            {
                if (nbTouch == 1)
                    SingleTouch();
                else
                    MultiTouch();

                _isTouch = true;
            }
            else if (nbTouch < 0)
            {
                _isTouch = false;
                nbTouch = 0;
                inputsBegan.Clear();
                inputsMoved.Clear();
                inputsEnded.Clear();
            }
        }

        void SingleTouch()
        {
            CurrentCameraController._isPinched = false;

            if (0 < inputsBegan.Count && null != inputsBegan[0])
            {
                if (_debugHit != null)
                {
                    if (RaycastGameObject(inputsBegan[0]))
                        _debugHit.text = inputsBegan[0].ObjectTouched.name;
                    else
                        _debugHit.text = "";
                }

                if (HavingDoubleTap(inputsBegan[0].CurrentPosition) && RaycastViewPoint(inputsBegan[0]))
                    SelectViewpoint(inputsBegan[0]);
                else if (true == RaycastCanvas(inputsBegan[0]))
                    SelectCanvas(inputsBegan[0]);
                else if (true == RaycastObject(inputsBegan[0]))
                    SelectObject(inputsBegan[0]);
                else
                    HUDManager.GetInfoBox().HideInfo();
            }
            else if (0 < inputsMoved.Count && null != inputsMoved[0])
            {
                if (null != inputsMoved[0].ObjectTouched && inputsMoved[0].ObjectTouchedType != InputContainer.ObjectType.NONE)
                {
                    if (InputContainer.ObjectType.INTERACTIVE == inputsMoved[0].ObjectTouchedType)
                        UpdateObjectMovement(inputsMoved[0]);
                    else
                        UpdateCanvas(inputsMoved[0]);
                }
                else
                    UpdateCameraMovement(inputsMoved[0].DeltaPosition);
            }
            else if (0 < inputsEnded.Count && null != inputsEnded[0])
            {
                lastPositionOneTouch = inputsEnded[0].CurrentPosition;
                lastTimeOneTouch = Time.realtimeSinceStartup;
                _isTouch = false;

                if (null != inputsEnded[0].ObjectTouched)
                {
                    if (InputContainer.ObjectType.INTERACTIVE == inputsEnded[0].ObjectTouchedType)
                        DeselectObject(inputsEnded[0]);
                    else if (InputContainer.ObjectType.CANVAS == inputsEnded[0].ObjectTouchedType)
                        DeselectCanvas(inputsEnded[0]);
                    else
                        UpdateCameraMovement(inputsEnded[0].DeltaPosition);
                }
                else if (InputContainer.InputType.PC == inputsEnded[0].Type && 1 == inputsEnded[0].Index && !LockPinch)
                {
                    if (null != currentCameraController)
                    {
                        currentCameraController.ChangeModeToZoomOut();
                    }
                }
                else
                {
                    UpdateCameraMovement(inputsEnded[0].DeltaPosition);
                }
            }
        }

        void MultiTouch()
        {
            foreach (InputContainer input in inputsBegan)
            {
                if (null != input)
                {
                    if (true == RaycastCanvas(input) && null != input.ObjectTouched)
                    {
                        bool res = false;

                        inputsMoved.ForEach(delegate (InputContainer inp)
                        {
                            if (inp.ObjectTouchedType != InputContainer.ObjectType.CANVAS)
                                res = true;
                            else
                            {
                                res = false;
                                return;
                            }
                        });

                        if (res)
                            SelectCanvas(input);
                    }
                    else if (true == RaycastObject(input))
                    {
                        if (input.ObjectTouched.GetComponent<TouchableElement>().IsSelected())
                        {
                            inputsBegan.Remove(input);
                            break;
                        }

                        bool res = false;

                        inputsMoved.ForEach(delegate (InputContainer inp)
                        {
                            if (inp.ObjectTouched != input.ObjectTouched)
                                res = true;
                            else
                            {
                                res = false;
                                return;
                            }
                        });

                        if (res)
                            SelectObject(input);
                    }
                }
            }

            foreach (InputContainer input in inputsMoved)
            {
                if (null != input && null != input.ObjectTouched)
                {
                    if (InputContainer.ObjectType.INTERACTIVE == input.ObjectTouchedType)
                        UpdateObjectMovement(input);
                    else if (InputContainer.ObjectType.CANVAS == input.ObjectTouchedType)
                        UpdateCanvas(input);
                }
            }

            foreach (InputContainer input in inputsEnded)
            {
                if (null != input && null != input.ObjectTouched)
                {
                    if (InputContainer.ObjectType.INTERACTIVE == input.ObjectTouchedType)
                        DeselectObject(input);
                    else if (InputContainer.ObjectType.CANVAS == input.ObjectTouchedType)
                        DeselectCanvas(input);
                }
            }

            if (!zoomOut && !zoomIn && nbTouch == 2 && inputsMoved.Count == 2 && !LockPinch && CurrentCameraController.GetCameraMode() != CameraController.Mode.PINCH_OVER)
            {
                if (inputsMoved[0].ObjectTouched == null || inputsMoved[1].ObjectTouched == null || inputsMoved[0].ObjectTouched == inputsMoved[1].ObjectTouched)
                {
                    inputsMoved.ForEach(delegate (InputContainer inp)
                    {
                        if (inp != null)
                            if (inp.ObjectTouched != null)
                            {
                                if (inp.ObjectTouched.GetComponent<InteractiveObject>() != null)
                                    if (!(inp.ObjectTouched.GetComponent<InteractiveObject>() is ItemTarget))
                                        DeselectObject(inp);

                                if (inp.ObjectTouched.GetComponent<InteractiveCanvas>() != null)
                                    DeselectCanvas(inp);
                            }
                    });

                    CurrentCameraController._isPinched = true;
                    LockInteractivity = true;
                    LockCanvas = true;

                    currentPinchLength = Vector2.Distance(inputsMoved[0].CurrentPosition, inputsMoved[1].CurrentPosition) * AxisSensitivity;
                    currentPinchLength = Mathf.Abs(currentPinchLength);

                    if (initialPinchLength > currentPinchLength)
                        UpdatePinchOut();
                    else
                        UpdatePinchIn();
                }
            }
            else if (nbTouch > 2 && inputsMoved.Count > 2 && CurrentCameraController.GetCameraMode() == CameraController.Mode.PINCH_OVER)
            {
                CurrentCameraController._isPinched = false;
                initialPinchLength = Vector2.Distance(inputsMoved[0].CurrentPosition, inputsMoved[1].CurrentPosition) * AxisSensitivity;
                initialPinchLength = Mathf.Abs(initialPinchLength);
            }
        }

        void UpdatePinchOut()
        {
            if ((initialPinchLength - currentPinchLength) > (initialPinchLength * PinchOutDistance)) //ZOOM OUT VIEWPOINT
            {
                if (currentCameraController.ChangeModeToZoomOut())
                {
                    zoomOut = true;
                    inputsBegan.Clear();
                    inputsMoved.Clear();
                    inputsEnded.Clear();
                }
            }
        }

        void UpdatePinchIn()
        {
            RaycastHit hit;

            Vector2 touchZeroPrevPos = inputsMoved[0].CurrentPosition - inputsMoved[0].DeltaPosition;
            Vector2 touchOnePrevPos = inputsMoved[1].CurrentPosition - inputsMoved[1].DeltaPosition;
            Vector2 prevTouchDelta = Vector2.Lerp(touchZeroPrevPos, touchOnePrevPos, 0.5f);

            if (Raycast(prevTouchDelta, out hit))
            {
                if (hit.collider.GetComponent<ViewPoint>() != null)
                    targetVP = hit.collider.GetComponent<ViewPoint>();
            }
            else
                targetVP = null;

            float distance = Vector3.Distance(CurrentCameraController.GetCurrentViewpoint().TargetPosition, CurrentCameraController.GetCurrentViewpoint().CameraPosition);

            if (distance > 20)
                targetPinch = Vector3.Lerp(cameraFrom.position, currentCamera.ScreenPointToRay(prevTouchDelta).GetPoint(1000) * distance, .01f / distance);
            else if (distance > 10)
                targetPinch = Vector3.Lerp(cameraFrom.position, currentCamera.ScreenPointToRay(prevTouchDelta).GetPoint(1000) * distance, .005f / distance);
            else
                targetPinch = Vector3.Lerp(cameraFrom.position, currentCamera.ScreenPointToRay(prevTouchDelta).GetPoint(1000) * distance, .001f / distance);


            float t = Mathf.Abs((currentPinchLength - initialPinchLength) * AxisSensitivity);
            t = Mathf.SmoothStep(0.0f, 1.0f, t / _smoothPinch);

            CurrentCameraController.ZoomPinch(cameraFrom, targetPinch, t);

            if (t > .75f && targetVP != null) //ZOOM TO VIEWPOINT TARGET
            {
                if (currentCameraController.ChangeModeToZoomTo(targetVP))
                {
                    zoomIn = true;
                    inputsBegan.Clear();
                    inputsMoved.Clear();
                    inputsEnded.Clear();
                }
            }
        }

        void SelectViewpoint(InputContainer Input)
        {
            if (LockViewpoint || InfoBox._infoBoxEnabled || zoomIn)
                return;

            ViewPoint viewPoint = Input.ObjectTouched.GetComponent<ViewPoint>();
            if (null != viewPoint && null != currentCameraController)
            {
                currentCameraController.ChangeModeToZoomTo(viewPoint);
                zoomIn = true;
                inputsBegan.Clear();
                inputsMoved.Clear();
                inputsEnded.Clear();
            }
        }

        void SelectCanvas(InputContainer input)
        {
            if (LockCanvas && !InfoBox._infoBoxEnabled)
                return;

            if (null != input && null != input.ObjectTouched)
            {
                InteractiveCanvas interactiveCanvas = input.ObjectTouched.GetComponent<InteractiveCanvas>();
                if (null != interactiveCanvas)
                {
                    interactiveCanvas.Select(currentCamera, input.CurrentPosition);
                }
            }
        }

        void UpdateCanvas(InputContainer input)
        {
            if (LockCanvas && !InfoBox._infoBoxEnabled)
                return;

            if (null != input && null != input.ObjectTouched)
            {
                InteractiveCanvas interactiveCanvas = input.ObjectTouched.GetComponent<InteractiveCanvas>();
                if (null != interactiveCanvas && interactiveCanvas.IsSelected())
                {
                    if (!interactiveCanvas.IsSelected())
                        return;

                    if (input.DeltaPosition.x != 0.0f || input.DeltaPosition.y != 0.0f)
                    {
                        if (true == RaycastCanvas(input.CurrentPosition) && input.ObjectTouched == canvasResults[0].gameObject)
                            interactiveCanvas.EnterCanvas();
                        else
                            interactiveCanvas.ExitCanvas();
                    }
                    interactiveCanvas.Move(input.CurrentPosition);
                }
            }
        }

        void DeselectCanvas(InputContainer input)
        {
            if (LockCanvas && !InfoBox._infoBoxEnabled)
                return;

            if (null != input && null != input.ObjectTouched)
            {
                InteractiveCanvas interactiveCanvas = input.ObjectTouched.GetComponent<InteractiveCanvas>();
                if (interactiveCanvas is InfoBox || (null != interactiveCanvas && interactiveCanvas.IsSelected()))
                    interactiveCanvas.Deselect();
            }
        }

        void SelectObject(InputContainer input)
        {
            if (LockInteractivity)
                return;

            if (null != input && null != input.ObjectTouched)
            {
                TouchableElement interactiveObject = null;

                if (input.ObjectTouched.GetComponents<TouchableElement>().Length > 1)
                {
                    foreach (TouchableElement t in input.ObjectTouched.GetComponents<TouchableElement>())
                    {
                        interactiveObject = t.OnActiveScript ? t : null;

                        if (interactiveObject != null)
                            break;
                    }
                }
                else
                    interactiveObject = input.ObjectTouched.GetComponent<TouchableElement>();

                if (null == interactiveObject || (interactiveObject is ItemTarget && LockPinch))
                    return;

                if (interactiveObject._interactable)
                    interactiveObject.SelectObject(currentCamera, input.CurrentPosition);
            }
        }

        void DeselectObject(InputContainer input)
        {
            if (LockInteractivity)
                return;

            if (null != input && null != input.ObjectTouched)
            {
                TouchableElement interactiveObject = null;

                if (input.ObjectTouched.GetComponents<TouchableElement>().Length > 1)
                {
                    foreach (TouchableElement t in input.ObjectTouched.GetComponents<TouchableElement>())
                    {
                        interactiveObject = t.OnActiveScript ? t : null;

                        if (interactiveObject != null)
                            break;
                    }
                }
                else
                    interactiveObject = input.ObjectTouched.GetComponent<TouchableElement>();

                if (null == interactiveObject || !interactiveObject.IsSelected())
                    return;

                interactiveObject.DeselectObject();

                UpdateCameraMovement(input.DeltaPosition);
            }
        }

        void UpdateObjectMovement(InputContainer input)
        {
            if (LockInteractivity)
                return;

            if (null != input && null != input.ObjectTouched && null != currentCameraController)
            {
                TouchableElement interactiveObject = null;

                if (input.ObjectTouched.GetComponents<TouchableElement>().Length > 1)
                {
                    foreach (TouchableElement t in input.ObjectTouched.GetComponents<TouchableElement>())
                    {
                        interactiveObject = t.OnActiveScript ? t : null;

                        if (interactiveObject != null)
                            break;
                    }
                }
                else
                    interactiveObject = input.ObjectTouched.GetComponent<TouchableElement>();

                if (null == interactiveObject || !interactiveObject.IsSelected())
                    return;

                if (interactiveObject.GetType() != typeof(ItemTarget))
                    interactiveObject.MoveObject(input.CurrentPosition);
                else
                    UpdateCameraMovement(input.DeltaPosition);
            }
        }

        void UpdateCameraMovement(Vector2 DeltaPosition)
        {
            if (null != currentCameraController)
            {
                DeltaPosition *= AxisSensitivity;
                currentCameraController.MoveCamera(DeltaPosition);
            }
        }

        bool HavingDoubleTap(Vector2 Position)
        {
            float elapsed = Time.realtimeSinceStartup - lastTimeOneTouch;
            if (elapsed <= DoubleTapDelay)
            {
                if (DoubleTapMaxDistance > Vector2.Distance(Position, lastPositionOneTouch))
                    return true;
            }
            return false;
        }

        public IEnumerator WaitToUnlock(float delay)
        {
            yield return new WaitForSeconds(delay);
            LockInteractivity = false;
            LockCanvas = false;
            LockViewpoint = false;
            LockPinch = false;
        }

        #endregion

        #region UpdateInputsLists
        void UpdateInputsBeganList()
        {
            if (0 != inputsBegan.Count)
            {
                foreach (InputContainer container in inputsBegan)
                {
                    inputsMoved.Add(container);
                }
                inputsBegan.Clear();

                if (nbTouch == 2 && inputsMoved.Count == 2 && !zoomIn && !zoomOut)
                    UpdateInitialPinchLength();
            }
        }

        void UpdateInitialPinchLength()
        {
            if (CurrentCameraController.GetCameraMode() != CameraController.Mode.PINCH_OVER)
            {
                CurrentPinchTime = 0;
                cameraFrom.position = currentCamera.transform.position;
                cameraFrom.rotation = currentCamera.transform.rotation;
            }

            targetVP = null;
            initialPinchLength = Vector2.Distance(inputsMoved[0].CurrentPosition, inputsMoved[1].CurrentPosition) * AxisSensitivity;
            initialPinchLength = Mathf.Abs(initialPinchLength);
        }

        void UpdateInputsEndedList()
        {
            if (0 != inputsEnded.Count)
                inputsEnded.Clear();
        }
        #endregion

        #region InputPhase
        void InputBegan(InputContainer.InputType Type, int InputIndex, Vector2 Position)
        {
            InputContainer newContainer = new InputContainer(Position);
            if (null != newContainer)
            {
                newContainer.Type = Type;
                newContainer.Index = InputIndex;

                inputsBegan.Add(newContainer);
            }
        }

        void InputMoved(InputContainer.InputType Type, int InputIndex, Vector2 Position)
        {
            InputContainer input = GetInputMoved(Type, InputIndex);
            if (null != input)
                input.SetCurrentPosition(Position);
        }

        void InputEnded(InputContainer.InputType Type, int InputIndex, Vector2 Position)
        {
            InputContainer input = GetInputMoved(Type, InputIndex);
            if (null != input)
            {
                input.SetCurrentPosition(Position);
                inputsEnded.Add(input);
                inputsMoved.Remove(input);
            }
        }

        InputContainer GetInputMoved(InputContainer.InputType Type, int InputIndex)
        {
            foreach (InputContainer input in inputsMoved)
            {
                if (input.Type == Type && input.Index == InputIndex)
                    return input;
            }
            return null;
        }
        #endregion

        #region Raycast
        bool RaycastViewPoint(InputContainer input)
        {
            RaycastHit hit;
            if (true == Raycast(input.CurrentPosition, out hit))
            {
                if (null != hit.collider.gameObject.GetComponent<ViewPoint>())
                {
                    input.ObjectTouched = hit.collider.gameObject;
                    input.ObjectTouchedType = InputContainer.ObjectType.VIEWPOINT;
                    return true;
                }
            }
            return false;
        }

        bool RaycastCanvas(InputContainer input)
        {
            if (null != CanvasRaycaster)
            {
                canvasResults.Clear();

                canvasPointer.position = input.CurrentPosition;

                CanvasRaycaster.Raycast(canvasPointer, canvasResults);

                if (0 != canvasResults.Count)
                {
                    input.ObjectTouched = canvasResults[0].gameObject;
                    input.ObjectTouchedType = InputContainer.ObjectType.CANVAS;
                    return true;
                }
            }
            return false;
        }

        bool RaycastCanvas(Vector2 Position)
        {
            if (null != CanvasRaycaster)
            {
                canvasResults.Clear();

                canvasPointer.position = Position;

                CanvasRaycaster.Raycast(canvasPointer, canvasResults);

                return (0 != canvasResults.Count);
            }
            return false;
        }

        bool RaycastObject(InputContainer input)
        {
            RaycastHit hit;
            if (true == Raycast(input.CurrentPosition, out hit))
            {
                if (null != hit.collider.gameObject.GetComponent<TouchableElement>())
                {
                    input.ObjectTouched = hit.collider.gameObject;
                    input.ObjectTouchedType = InputContainer.ObjectType.INTERACTIVE;
                    return true;
                }
            }
            return false;
        }

        bool RaycastGameObject(InputContainer input)
        {
            RaycastHit hit;
            if (true == Raycast(input.CurrentPosition, out hit))
            {
                if (null != hit.collider.gameObject)
                {
                    input.ObjectTouched = hit.collider.gameObject;
                    input.ObjectTouchedType = InputContainer.ObjectType.NONE;
                    return true;
                }
            }
            return false;
        }

        bool Raycast(Vector2 Position, out RaycastHit hit)
        {
            if (null != currentCamera)
            {
                return Physics.Raycast(currentCamera.ScreenPointToRay(Position), out hit);
            }

            hit = new RaycastHit();
            return false;
        }
        #endregion

    }
}