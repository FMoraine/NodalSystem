// Machinika Museum Prototype v.01
// © Littlefield Studio
// Writted by Rémi Carreira - 2015
//
// CameraController.cs

using NodalInteractiveCreator.HUD;
using NodalInteractiveCreator.Managers;
using NodalInteractiveCreator.Screwdriver;
using NodalInteractiveCreator.Viewpoints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodalInteractiveCreator.Controllers
{
    public class CameraController : MonoBehaviour
    {
        public enum Mode
        {
            NONE = 0,
            FIX_SCREW,
            TURN_TABLE,
            ZOOM_TO,
            ZOOM_TO_SCREW,
            ZOOM_OUT,
            PINCH_OVER
        }

        public delegate void PositionReached();
        public event PositionReached OnPositionReached;

        public ViewPoint DefaultViewpoint = null;
        public Vector2 RotationSpeed = Vector2.one;
        public float ZoomToDuration = 1.0f;
        public float ZoomOutDuration = 0.80f;
        public float ZoomDistanceRatio = 0.25f;
        public float _smoothSpeed = 5f;

        public bool _isPinched;

        private Mode cameraMode = Mode.NONE;
        private Transform cameraTransform = null;

        private ViewPoint currentViewpoint = null;
        private Vector3 movementAngle = Vector3.zero;
        private Vector3 viewpointAngle = Vector3.zero;
        private Quaternion viewpointRotation = Quaternion.identity;
        private float viewpointDistance = 0.0f;

        private Vector3 startPosition = Vector3.zero;
        private Vector3 finishPosition = Vector3.zero;
        private Quaternion startRotation = Quaternion.identity;
        private Quaternion finishRotation = Quaternion.identity;
        private float startFOV = 0.0f;
        private float finishFOV = 0.0f;
        private float transitionDuration = 0.0f;
        private float elapsedTime = 0.0f;

        private List<SavePoint> savedPoints = new List<SavePoint>();
        private SavePoint lastSavedPoint = null;

        private Screw currentScrew = null;

        public void ChangeModeToNone()
        {
            cameraMode = Mode.NONE;
        }

        public Vector3 GetAngleMovement()
        {
            return viewpointAngle + movementAngle;
        }

        public Vector3 GetLocalMovement()
        {
            return movementAngle;
        }

        public bool ChangeModeToTurnTable(ViewPoint NewViewpoint)
        {
            if (null != NewViewpoint)
            {
                if (null != currentViewpoint)
                {
                    currentViewpoint.UnusedViewpoint();
                }

                cameraMode = Mode.TURN_TABLE;
                currentViewpoint = NewViewpoint;

                movementAngle = Vector3.zero;
                viewpointRotation = currentViewpoint.GetCameraRotation();
                viewpointAngle = viewpointRotation.eulerAngles;
                viewpointDistance = currentViewpoint.GetDistance();

                cameraTransform.position = currentViewpoint.CameraPosition;
                cameraTransform.rotation = Quaternion.Euler(viewpointAngle);

                currentViewpoint.UseViewpoint();
                return true;
            }
            return false;
        }

        public bool ChangeModeToZoomTo(ViewPoint NewViewpoint)
        {
            if (null != NewViewpoint)
            {
                StopAllCoroutines();

                if (null != currentViewpoint)
                {
                    AddSavedPoint();
                    currentViewpoint.UnusedViewpoint();
                }

                if (_isPinched)
                {
                    GameManager.GetInputsManager().LockInteractivity = false;
                    GameManager.GetInputsManager().LockCanvas = false;
                    _isPinched = false;
                }
                else
                    GameManager.GetInputsManager().LockInput = true;

                cameraMode = Mode.ZOOM_TO;
                currentViewpoint = NewViewpoint;

                movementAngle = Vector3.zero;
                viewpointRotation = currentViewpoint.GetCameraRotation();
                viewpointAngle = viewpointRotation.eulerAngles;
                viewpointDistance = currentViewpoint.GetDistance();

                startPosition = cameraTransform.position;
                startRotation = cameraTransform.rotation;
                transitionDuration = Vector3.Distance(cameraTransform.position, NewViewpoint.CameraPosition) * ZoomDistanceRatio + ZoomToDuration;
                elapsedTime = 0.0f;

                Camera selfCamera = GetComponent<Camera>();
                if (null != selfCamera)
                {
                    startFOV = selfCamera.fieldOfView;
                    finishFOV = currentViewpoint.FieldOfView;
                }
                return true;
            }
            return false;
        }

        public bool ChangeModeToZoomScrew(Screw Source, Vector3 CameraPosition, Quaternion CameraRotation)
        {
            if (null != currentViewpoint)
            {
                AddSavedPoint();
                currentViewpoint.UnusedViewpoint();
            }

            if (_isPinched)
            {
                GameManager.GetInputsManager().LockInteractivity = false;
                _isPinched = false;
            }
            else
                GameManager.GetInputsManager().LockInput = true;


            currentScrew = Source;
            cameraMode = Mode.ZOOM_TO_SCREW;
            currentViewpoint = null;

            startPosition = cameraTransform.position;
            startRotation = cameraTransform.rotation;
            finishPosition = CameraPosition;
            finishRotation = CameraRotation;
            elapsedTime = 0.0f;

            Camera selfCamera = GetComponent<Camera>();
            if (null != selfCamera)
            {
                startFOV = selfCamera.fieldOfView;
                finishFOV = 45.0f;
            }
            return true;
        }

        public bool ChangeModeToZoomOut()
        {
            if (null != lastSavedPoint && cameraMode != Mode.ZOOM_OUT && DefaultViewpoint != currentViewpoint)
            {
                StopAllCoroutines();

                if (null != currentViewpoint)
                {
                    if (currentViewpoint == lastSavedPoint.ViewPoint)
                        RemoveSavedPoint();

                    currentViewpoint.UnusedViewpoint();
                }
                if (Mode.FIX_SCREW == cameraMode)
                {
                    if (null != currentScrew)
                    {
                        currentScrew.QuitScrew();
                    }
                }

                if (_isPinched)
                    _isPinched = false;
                else
                    GameManager.GetInputsManager().LockInput = true;


                cameraMode = Mode.ZOOM_OUT;
                currentViewpoint = lastSavedPoint.ViewPoint;

                movementAngle = Vector3.zero;
                viewpointRotation = currentViewpoint.GetCameraRotation();
                viewpointAngle = viewpointRotation.eulerAngles;
                viewpointDistance = currentViewpoint.GetDistance();

                startPosition = cameraTransform.position;
                startRotation = cameraTransform.rotation;
                transitionDuration = Vector3.Distance(cameraTransform.position, lastSavedPoint.Position) * ZoomDistanceRatio + ZoomOutDuration;
                elapsedTime = 0.0f;

                Camera selfCamera = GetComponent<Camera>();
                if (null != selfCamera)
                {
                    startFOV = selfCamera.fieldOfView;
                    finishFOV = currentViewpoint.FieldOfView;
                }
                return true;
            }
            return false;
        }

        public bool ChangeModeToZoomOutTo(ViewPoint NewViewpoint)
        {
            savedPoints.Clear();
            AddSavedPoint();
            lastSavedPoint.ViewPoint = DefaultViewpoint;
            lastSavedPoint.Position = DefaultViewpoint.CameraPosition;
            lastSavedPoint.Rotation = DefaultViewpoint.GetCameraRotation();

            if (null != lastSavedPoint && cameraMode != Mode.ZOOM_TO)
            {
                StopAllCoroutines();

                if (null != currentViewpoint)
                {
                    //if (currentViewpoint == lastSavedPoint.ViewPoint)
                    //	RemoveSavedPoint();
                    currentViewpoint.UnusedViewpoint();
                }
                if (Mode.FIX_SCREW == cameraMode)
                {
                    if (null != currentScrew)
                    {
                        currentScrew.QuitScrew();
                    }
                }

                if (_isPinched)
                {
                    GameManager.GetInputsManager().LockInteractivity = false;
                    GameManager.GetInputsManager().LockCanvas = false;
                    _isPinched = false;
                }
                else
                    GameManager.GetInputsManager().LockInput = true;

                cameraMode = Mode.ZOOM_TO;
                currentViewpoint = NewViewpoint;

                movementAngle = Vector3.zero;
                viewpointRotation = currentViewpoint.GetCameraRotation();
                viewpointAngle = viewpointRotation.eulerAngles;
                viewpointDistance = currentViewpoint.GetDistance();

                startPosition = cameraTransform.position;
                startRotation = cameraTransform.rotation;
                transitionDuration = Vector3.Distance(cameraTransform.position, NewViewpoint.CameraPosition) * ZoomDistanceRatio + ZoomOutDuration;
                elapsedTime = 0.0f;

                Camera selfCamera = GetComponent<Camera>();
                if (null != selfCamera)
                {
                    startFOV = selfCamera.fieldOfView;
                    finishFOV = currentViewpoint.FieldOfView;
                }
                return true;
            }
            return false;
        }

        public void SpwanTo(ViewPoint NewViewpoint)
        {
            StopAllCoroutines();

            if (null != currentViewpoint)
            {
                currentViewpoint.UnusedViewpoint();
            }
            if (Mode.FIX_SCREW == cameraMode)
            {
                if (null != currentScrew)
                {
                    currentScrew.QuitScrew();
                }
            }

            if (lastSavedPoint != null)
            {
                savedPoints.Clear();

                SavePoint newSave = new SavePoint(DefaultViewpoint, DefaultViewpoint.CameraPosition, DefaultViewpoint.GetCameraRotation());
                lastSavedPoint = newSave;
                savedPoints.Add(newSave);
            }

            currentViewpoint = NewViewpoint;

            movementAngle = Vector3.zero;
            viewpointRotation = currentViewpoint.GetCameraRotation();
            viewpointAngle = viewpointRotation.eulerAngles;
            viewpointDistance = currentViewpoint.GetDistance();

            cameraTransform.position = currentViewpoint.CameraPosition;
            cameraTransform.rotation = currentViewpoint.GetCameraRotation();

            transitionDuration = 0;
            elapsedTime = 0.0f;

            Camera selfCamera = GetComponent<Camera>();
            if (null != selfCamera)
                selfCamera.fieldOfView = currentViewpoint.FieldOfView;

            cameraMode = Mode.TURN_TABLE;
            currentViewpoint.UseViewpoint();
        }

        public bool MoveToViewpointCameraPosInit()
        {
            if (cameraMode != Mode.ZOOM_TO)
            {
                StopAllCoroutines();

                cameraMode = Mode.ZOOM_TO;

                currentViewpoint = DefaultViewpoint;
                movementAngle = Vector3.zero;
                viewpointRotation = currentViewpoint.GetCameraRotation();
                viewpointAngle = viewpointRotation.eulerAngles;
                viewpointDistance = currentViewpoint.GetDistance();

                startPosition = currentViewpoint.CameraPosition;
                startRotation = currentViewpoint.GetCameraRotation();
                transitionDuration = Vector3.Distance(startPosition, currentViewpoint.CameraPosition) * ZoomDistanceRatio + ZoomOutDuration;
                elapsedTime = 0.0f;

                Camera selfCamera = GetComponent<Camera>();
                if (null != selfCamera)
                {
                    startFOV = selfCamera.fieldOfView;
                    finishFOV = currentViewpoint.FieldOfView;
                }
                return true;
            }
            return false;
        }

        public void MoveCamera(Vector2 InputDelta)
        {
            StopAllCoroutines();

            if (Mode.TURN_TABLE == cameraMode && null != currentViewpoint && !InfoBox._infoBoxEnabled)
            {
                movementAngle.x -= InputDelta.y * RotationSpeed.x;
                movementAngle.y += InputDelta.x * RotationSpeed.y;

                Quaternion rotStart = cameraTransform.rotation;
                ClampMovementAngle();
                Quaternion rotEnd = Quaternion.Euler(viewpointAngle.x + movementAngle.x, viewpointAngle.y + movementAngle.y, viewpointAngle.z);

                if (InputsManager._isTouch)
                {
                    ClampMovementAngle();
                    //cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, currentViewpoint.TargetPosition - (cameraTransform.rotation * Vector3.forward * viewpointDistance) , ref velo, 1F);
                    //cameraTransform.rotation = Quaternion.Euler(viewpointAngle.x + movementAngle.x, viewpointAngle.y + movementAngle.y, viewpointAngle.z);

                    cameraTransform.rotation = Quaternion.Lerp(rotStart, rotEnd, Time.deltaTime * _smoothSpeed);
                    cameraTransform.position = currentViewpoint.TargetPosition - (cameraTransform.rotation * Vector3.forward * viewpointDistance);
                }
                else
                    StartCoroutine(SmoothMoveCamera(rotEnd));
            }
        }

        public void ZoomPinch(Transform from, Vector3 to, float time)
        {
            StopAllCoroutines();

            if (GameManager.GetCamerasManager().GetCurrentCamera() != GameManager.GetCamerasManager().InventoryCamera)
            {
                cameraTransform.position = Vector3.Lerp(from.position, to, time);
                //cameraTransform.rotation = Quaternion.Lerp(from.rotation, look, time*.25f);
                GetComponent<Camera>().fieldOfView = Mathf.Lerp(currentViewpoint.FieldOfView, currentViewpoint.FieldOfView - 10, time);
            }
            else
            {
                GetComponent<Camera>().fieldOfView = Mathf.Lerp(currentViewpoint.FieldOfView, currentViewpoint.FieldOfView - 10, time);
            }
        }

        public bool ZoomPinchOver()
        {
            if (cameraMode != Mode.PINCH_OVER)
            {
                //StopAllCoroutines();

                if (_isPinched)
                {
                    GameManager.GetInputsManager().LockInteractivity = false;
                    GameManager.GetInputsManager().LockCanvas = false;
                    _isPinched = false;
                }

                cameraMode = Mode.PINCH_OVER;

                startPosition = cameraTransform.position;
                startRotation = cameraTransform.rotation;
                transitionDuration = Vector3.Distance(startPosition, InputsManager.cameraFrom.position) * ZoomDistanceRatio + ZoomOutDuration;
                elapsedTime = 0;

                Camera selfCamera = GetComponent<Camera>();
                if (null != selfCamera)
                {
                    startFOV = selfCamera.fieldOfView;
                    finishFOV = currentViewpoint.FieldOfView;
                }
                return true;
            }
            return false;
        }

        public IEnumerator SmoothMoveCamera(Quaternion end)
        {
            while (Quaternion.Angle(cameraTransform.rotation, end) > .1)
            {
                cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, end, Time.deltaTime * _smoothSpeed);
                cameraTransform.position = currentViewpoint.TargetPosition - (cameraTransform.forward * viewpointDistance);

                ClampMovementAngle();

                yield return null;
            }
        }

        public Mode GetCameraMode()
        {
            return cameraMode;
        }

        public ViewPoint GetCurrentViewpoint()
        {
            return currentViewpoint;
        }

        private void Awake()
        {
            cameraTransform = this.transform;
        }

        private void Start()
        {
            ChangeModeToTurnTable(DefaultViewpoint);
        }

        private void Update()
        {
            if (Mode.ZOOM_TO == cameraMode)
            {
                UpdateZoomTo();
            }
            else if (Mode.ZOOM_TO_SCREW == cameraMode)
            {
                UpdateZoomToScrew();
            }
            else if (Mode.ZOOM_OUT == cameraMode)
            {
                UpdateZoomOut();
            }
            else if (Mode.PINCH_OVER == cameraMode)
            {
                UpdatePinch();
            }
        }

        private void UpdatePinch()
        {
            if (null != currentViewpoint)
            {
                elapsedTime += Time.deltaTime;
                float time = Mathf.SmoothStep(0.0f, 1.0f, elapsedTime / transitionDuration);
                //GameManager.GetInputsManager().CurrentPinchTime = time;

                cameraTransform.position = Vector3.Lerp(startPosition, InputsManager.cameraFrom.position, time);
                cameraTransform.rotation = Quaternion.Lerp(startRotation, InputsManager.cameraFrom.rotation, time);

                Camera selfCamera = GetComponent<Camera>();
                if (null != selfCamera)
                {
                    selfCamera.fieldOfView = Mathf.Lerp(startFOV, finishFOV, elapsedTime / transitionDuration);
                }

                if (time >= 1)
                {
                    if (GameManager.GetGameState().currentState != GameState.STATE.GAMEPLAY)
                        GameManager.GetGameState().currentState = GameState.STATE.GAMEPLAY;

                    cameraMode = Mode.TURN_TABLE;
                }
            }
        }

        private void UpdateZoomTo()
        {
            if (null != currentViewpoint)
            {
                elapsedTime += Time.deltaTime;

                float time = Mathf.SmoothStep(0.0f, 1.0f, elapsedTime / transitionDuration);

                cameraTransform.position = Vector3.Slerp(startPosition, currentViewpoint.CameraPosition, time);
                cameraTransform.rotation = Quaternion.Slerp(startRotation, currentViewpoint.GetCameraRotation(), time);

                Camera selfCamera = GetComponent<Camera>();
                if (null != selfCamera)
                {
                    selfCamera.fieldOfView = Mathf.Lerp(startFOV, finishFOV, elapsedTime / transitionDuration);
                }

                if (time >= 1)
                {
                    if (GameManager.GetGameState().currentState != GameState.STATE.GAMEPLAY)
                        GameManager.GetGameState().currentState = GameState.STATE.GAMEPLAY;

                    cameraMode = Mode.TURN_TABLE;
                    currentViewpoint.UseViewpoint();

                    GameManager.GetInputsManager().LockInput = false;
                    GameManager.GetInputsManager().ZoomIn = false;
                }
            }
        }

        private void UpdateZoomToScrew()
        {
            elapsedTime += Time.deltaTime;

            cameraTransform.position = Vector3.Lerp(startPosition, finishPosition, elapsedTime / 1.0f);
            cameraTransform.rotation = Quaternion.Lerp(startRotation, finishRotation, elapsedTime / 1.0f);

            Camera selfCamera = GetComponent<Camera>();
            if (null != selfCamera)
            {
                selfCamera.fieldOfView = Mathf.Lerp(startFOV, finishFOV, elapsedTime / transitionDuration);
            }

            if (elapsedTime >= 1.0f)
            {
                GameManager.GetInputsManager().LockInput = false;

                cameraMode = Mode.FIX_SCREW;

                OnPositionReached();
            }
        }

        private void UpdateZoomOut()
        {
            if (null != lastSavedPoint)
            {
                elapsedTime += Time.deltaTime;

                float time = Mathf.SmoothStep(0.0f, 1.0f, elapsedTime / transitionDuration);

                cameraTransform.position = Vector3.Slerp(startPosition, lastSavedPoint.Position, time);
                cameraTransform.rotation = Quaternion.Slerp(startRotation, lastSavedPoint.Rotation, time);

                Camera selfCamera = GetComponent<Camera>();
                if (null != selfCamera)
                {
                    selfCamera.fieldOfView = Mathf.Lerp(startFOV, finishFOV, elapsedTime / transitionDuration);
                }

                if (time >= 1)
                {
                    cameraMode = Mode.TURN_TABLE;
                    movementAngle = Angles.ClampAngleIn360Degree(lastSavedPoint.Rotation.eulerAngles - viewpointRotation.eulerAngles);

                    movementAngle.z = 0.0f;

                    currentViewpoint.UseViewpoint();
                    RemoveSavedPoint();

                    GameManager.GetInputsManager().LockInput = false;
                    GameManager.GetInputsManager().ZoomOut = false;

                    if (!_isPinched)
                    {
                        GameManager.GetInputsManager().LockInteractivity = false;
                        GameManager.GetInputsManager().LockCanvas = false;
                    }

                }
            }
        }

        private void ClampMovementAngle()
        {
            movementAngle = Angles.ClampAngleIn360Degree(movementAngle);

            if (movementAngle.x <= 180.0f && movementAngle.x > currentViewpoint.MaxXAngle)
            {
                movementAngle.x = currentViewpoint.MaxXAngle;
            }
            else if (movementAngle.x > 180.0f && movementAngle.x - 360.0f < currentViewpoint.MinXAngle)
            {
                movementAngle.x = currentViewpoint.MinXAngle;
            }
            if (movementAngle.y <= 180.0f && movementAngle.y > currentViewpoint.MaxYAngle)
            {
                movementAngle.y = currentViewpoint.MaxYAngle;
            }
            else if (movementAngle.y > 180.0f && movementAngle.y - 360.0f < currentViewpoint.MinYAngle)
            {
                movementAngle.y = currentViewpoint.MinYAngle;
            }
        }

        private void AddSavedPoint()
        {
            SavePoint newPoint;

            if (_isPinched)
                newPoint = new SavePoint(currentViewpoint, InputsManager.cameraFrom.position, InputsManager.cameraFrom.rotation);
            else
                newPoint = new SavePoint(currentViewpoint, cameraTransform.position, cameraTransform.rotation);

            if (null != newPoint)
            {
                lastSavedPoint = newPoint;
                savedPoints.Add(newPoint);
            }
        }

        private void RemoveSavedPoint()
        {
            savedPoints.Remove(lastSavedPoint);

            if (savedPoints.Count > 0)
            {
                lastSavedPoint = savedPoints[savedPoints.Count - 1];
            }
            else
            {
                lastSavedPoint = null;
            }
        }
    }
}
