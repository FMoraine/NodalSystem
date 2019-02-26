// Machinika Museum
// © Littlefield Studio
// Writted by Franck-Olivier FILIN - 2017
//
// TranslatableObject.cs

using System.Collections;
using System.Collections.Generic;
using NodalInteractiveCreator.Managers;
using NodalInteractiveCreator.Tools;
using UnityEngine;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NodalInteractiveCreator.Objects
{
    public class TranslatableObject : InteractiveObject
    {
        public static bool _isBacked = false;

        //Public Variables
        public bool ShowMinTranslation = true;
        public bool ShowMaxTranslation = true;
        public bool ShowCheckTranslation = true;
        public bool CheckTranslation = false;
        public bool SnapOnMinPosition = false;
        public bool SnapOnMaxPosition = false;
        public bool SnapOnCheckPosition = false;
        public bool ActiveStartOnMove;
        public bool ActiveFinalOnMove;
        public bool _backToStart = false;
        public bool _activeActionBackToStart = false;
        public float SnapDuration = 1.0f;

        public Axis AxisToTranslate = Axis.RIGHT;
        public float MinNumberOfUnit = 0.0f;
        public float MaxNumberOfUnit = 0.0f;

        public float PrecisionMin = 0f;
        public float PrecisionMax = 0f;
        public int NbCheck = 0;

        public AudioClip _audioClipStart = null;
        public AudioClip _audioClipFinal = null;

        [SerializeField]
        protected List<Check> listCheck = new List<Check>();
        public List<Check> ListCheck { get { return listCheck; } set { listCheck = value; } }

        //Private Variables
        private Vector3 _initPos = new Vector3();

        private Vector3 positionOnSelect = Vector3.zero;
        private Vector3 positionOnDeselect = Vector3.zero;

        private Vector3 clampedPosition = Vector3.zero;
        private Vector3 minPosition = Vector3.zero;
        private Vector3 maxPosition = Vector3.zero;
        private List<Vector3> checkPosition = new List<Vector3>();

        private Vector3 selectOffset = Vector3.zero;
        private Vector3 movementOffset = Vector3.zero;

        private Plane planeToRaycast;
        private Ray rayForRaycast;
        private Vector3 raycastPosition = Vector3.zero;
        private Vector3 raycastDirection = Vector3.zero;
        private float raycastDistance = 0.0f;

        private Vector3 currentAxis = Vector3.zero;

        Vector3 minCheckPos = Vector3.zero;
        Vector3 maxCheckPos = Vector3.zero;

        protected override void Awake()
        {
            base.Awake();

            _isBacked = false;
            _initPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

            if (_objectAudio != null)
                _objectAudio.loop = true;
        }

        public override void SelectObject(Camera Camera, Vector3 InputPosition)
        {
            if (IsSelected() || (_backToStart && _objectTransform.position != _initPos))
                return;

            base.SelectObject(Camera, InputPosition);

            if (_objectAudio != null)
                _objectAudio.enabled = true;

            DetermineAxis();
            CreatePlaneToRaycast();

            minPosition = _objectTransform.position + (currentAxis * MinNumberOfUnit) - movementOffset;
            maxPosition = _objectTransform.position + (currentAxis * MaxNumberOfUnit) - movementOffset;

            //InitActionBackToStart();

            if (CheckTranslation)
                InitClampPositionForCheck();

            positionOnSelect = _objectTransform.position;
            rayForRaycast = _mainCamera.ScreenPointToRay(InputPosition);

            if (planeToRaycast.Raycast(rayForRaycast, out raycastDistance))
            {
                raycastPosition = rayForRaycast.GetPoint(raycastDistance);
                selectOffset = raycastPosition - _objectTransform.position;
            }
        }

        public override void MoveObject(Vector3 InputPosition)
        {
            base.MoveObject(InputPosition);

            rayForRaycast = _mainCamera.ScreenPointToRay(InputPosition);

            if (planeToRaycast.Raycast(rayForRaycast, out raycastDistance))
            {
                Vector3 lastPos = _objectTransform.position;

                raycastPosition = rayForRaycast.GetPoint(raycastDistance);
                raycastDirection = raycastPosition - _objectTransform.position;

                _objectTransform.position += Vector3.Project(raycastDirection, currentAxis) - Vector3.Project(selectOffset, currentAxis);

                ClampPosition();
                CheckDistanceToMinPosition();
                CheckDistanceToMaxPosition();

                if (CheckTranslation)
                {
                    ClampPositionForCheck();
                    foreach (Check check in ListCheck)
                        CheckDistanceToCheckPosition(check.Id);
                }

                ChangeMaterial();

                AudioVolume(lastPos);

                //if (Vector3.Distance(_objectTransform.position, minCheckPos) == 0 || Vector3.Distance(_objectTransform.position, maxCheckPos) == 0)
                //    StartCoroutine(FadeOutVolumeAudio());
            }
        }

        public void MoveObjectProcuration(float value)
        {
            Vector3 lastPos = _objectTransform.position;

            _objectTransform.position += value * currentAxis;

            ClampPosition();

            if (CheckTranslation)
            {
                ClampPositionForCheck();

                foreach (Check check in ListCheck)
                    CheckDistanceToCheckPosition(check.Id);
            }

            AudioVolume(lastPos);

            ChangeMaterial();

            if (_backToStart)
            {
                _isBacked = true;

                if (_activeActionBackToStart)
                {
                    CheckDistanceToMinPosition();
                    CheckDistanceToMaxPosition();
                }
            }
        }

        public override void DeselectObject()
        {
            base.DeselectObject();

            if (ItemTarget._isTrigged)
            {
                ItemTarget._isTrigged = false;
                return;
            }

            positionOnDeselect = _objectTransform.position;
            movementOffset += positionOnDeselect - positionOnSelect;

            StartCoroutine(FadeOutVolumeAudio());

            if (_backToStart)
            {
                _isBacked = true;
                CheckActionBackToStart();
            }
            else
            {
                CheckDistanceToMinPosition();
                CheckDistanceToMaxPosition();

                foreach (Check check in ListCheck)
                    CheckDistanceToCheckPosition(check.Id);
            }

            checkPosition.Clear();
        }

        private void DetermineAxis()
        {
            if (Axis.RIGHT == AxisToTranslate)
                currentAxis = _objectTransform.right;
            else if (Axis.UP == AxisToTranslate)
                currentAxis = _objectTransform.up;
            else
                currentAxis = _objectTransform.forward;

            currentAxis.Normalize();
        }

        void CreatePlaneToRaycast()
        {
            if (null != _mainCamera)
            {
                planeToRaycast = new Plane(_mainCamera.transform.forward, _objectTransform.position);
            }
        }

        #region Clamp Position
        void ClampPosition()
        {
            clampedPosition = _objectTransform.position;

            if (minPosition.x < maxPosition.x)
                clampedPosition.x = Mathf.Clamp(clampedPosition.x, minPosition.x, maxPosition.x);
            else
                clampedPosition.x = Mathf.Clamp(clampedPosition.x, maxPosition.x, minPosition.x);

            if (minPosition.y < maxPosition.y)
                clampedPosition.y = Mathf.Clamp(clampedPosition.y, minPosition.y, maxPosition.y);
            else
                clampedPosition.y = Mathf.Clamp(clampedPosition.y, maxPosition.y, minPosition.y);

            if (minPosition.z < maxPosition.z)
                clampedPosition.z = Mathf.Clamp(clampedPosition.z, minPosition.z, maxPosition.z);
            else
                clampedPosition.z = Mathf.Clamp(clampedPosition.z, maxPosition.z, minPosition.z);

            _objectTransform.position = clampedPosition;
        }

        void InitClampPositionForCheck()
        {
            bool res = false;
            minCheckPos = Vector3.zero;
            maxCheckPos = Vector3.zero;
            foreach (Check check in ListCheck)
            {
                checkPosition.Add(_objectTransform.position + (currentAxis * check.Distance) - movementOffset);

                if (res)
                    continue;

                if (check.Block && checkPosition[check.Id].x < _objectTransform.position.x)
                    minCheckPos = checkPosition[check.Id];
                else if (check.Block && checkPosition[check.Id].x > _objectTransform.position.x)
                {
                    maxCheckPos = checkPosition[check.Id];
                    res = true;
                }

                if (check.Block && checkPosition[check.Id].y < _objectTransform.position.y)
                    minCheckPos = checkPosition[check.Id];
                else if (check.Block && checkPosition[check.Id].y > _objectTransform.position.y)
                {
                    maxCheckPos = checkPosition[check.Id];
                    res = true;
                }

                if (check.Block && checkPosition[check.Id].z < _objectTransform.position.z)
                    minCheckPos = checkPosition[check.Id];
                else if (check.Block && checkPosition[check.Id].z > _objectTransform.position.z)
                {
                    maxCheckPos = checkPosition[check.Id];
                    res = true;
                }
            }

            if (minCheckPos == Vector3.zero)
                minCheckPos = minPosition;

            if (maxCheckPos == Vector3.zero)
                maxCheckPos = maxPosition;
        }

        void ClampPositionForCheck()
        {
            clampedPosition = _objectTransform.position;

            if (minCheckPos.x < maxCheckPos.x)
                clampedPosition.x = Mathf.Clamp(clampedPosition.x, minCheckPos.x, maxCheckPos.x);
            else
                clampedPosition.x = Mathf.Clamp(clampedPosition.x, maxCheckPos.x, minCheckPos.x);

            if (minCheckPos.y < maxCheckPos.y)
                clampedPosition.y = Mathf.Clamp(clampedPosition.y, minCheckPos.y, maxCheckPos.y);
            else
                clampedPosition.y = Mathf.Clamp(clampedPosition.y, maxCheckPos.y, minCheckPos.y);

            if (minCheckPos.z < maxCheckPos.z)
                clampedPosition.z = Mathf.Clamp(clampedPosition.z, minCheckPos.z, maxCheckPos.z);
            else
                clampedPosition.z = Mathf.Clamp(clampedPosition.z, maxCheckPos.z, minCheckPos.z);

            _objectTransform.position = clampedPosition;
        }
        #endregion

        private void ChangeMaterial()
        {
            float gap = Vector3.Distance(_objectTransform.position, minPosition) / Vector3.Distance(minPosition, maxPosition);

            MaterialConfig.ChangeMaterial(ref gap, ref _materials);
        }

        private void CheckActionBackToStart()
        {
            if (Mathf.Abs(Vector3.Distance(_objectTransform.position, minPosition)) <= PrecisionMin)
            {
                if (null != act_Start && _activeActionBackToStart)
                    act_Start.Invoke();

                StartCoroutine(PlayAudioToValidate(_audioClipStart));
            }
            else if (Mathf.Abs(Vector3.Distance(_objectTransform.position, maxPosition)) <= PrecisionMax)
            {
                if (null != act_Final && _activeActionBackToStart)
                    act_Final.Invoke();

                StartCoroutine(PlayAudioToValidate(_audioClipFinal));
            }

            StartCoroutine(SnapToPosition(SnapDuration, _initPos, null, null));
        }
        

        bool CheckDistanceToMinPosition()
        {
            float distance = Vector3.Distance(_objectTransform.position, minPosition);

            if (distance < 0.0f)
                distance *= -1.0f;

            if (distance <= PrecisionMin)
            {
                if (true == SnapOnMinPosition && !IsSelected())
                {
                    StartCoroutine(FadeOutVolumeAudio());
                    StartCoroutine(SnapToPosition(SnapDuration, minPosition, act_Start, _audioClipStart));
                }
                else
                {
                    if ((IsSelected() && ActiveStartOnMove) || (!IsSelected() && !ActiveStartOnMove))
                        if (null != act_Start)
                            act_Start.Invoke();

                    StartCoroutine(PlayAudioToValidate(_audioClipStart));
                }
                return true;
            }
            return false;
        }

        bool CheckDistanceToMaxPosition()
        {
            float distance = Vector3.Distance(_objectTransform.position, maxPosition);

            if (distance < 0.0f)
                distance *= -1.0f;

            if (distance <= PrecisionMax)
            {
                if (true == SnapOnMaxPosition && !IsSelected() /*distance >= 0.1F*/)
                {
                    StartCoroutine(FadeOutVolumeAudio());
                    StartCoroutine(SnapToPosition(SnapDuration, maxPosition, act_Final, _audioClipFinal));
                }
                else
                {
                    if ((IsSelected() && ActiveFinalOnMove) || (!IsSelected() && !ActiveFinalOnMove))
                        if (null != act_Final)
                            act_Final.Invoke();

                    StartCoroutine(PlayAudioToValidate(_audioClipFinal));
                }
                return true;
            }
            return false;
        }

        bool CheckDistanceToCheckPosition(int idCheck)
        {
            float distance = Vector3.Distance(_objectTransform.position, checkPosition[idCheck]);

            bool testMin = _objectTransform.position == Vector3.Min(_objectTransform.position, checkPosition[idCheck]);
            bool testMax = _objectTransform.position == Vector3.Max(_objectTransform.position, checkPosition[idCheck]);

            if ((/*testMin &&*/ distance <= ListCheck[idCheck].LimitSnapMin) || (/*testMax &&*/ distance <= ListCheck[idCheck].LimitSnapMax))
            {
                if (true == SnapOnCheckPosition && !IsSelected())
                {
                    StartCoroutine(FadeOutVolumeAudio());
                    StartCoroutine(SnapToPosition(SnapDuration, checkPosition[idCheck], act_Check, idCheck, _audioClipStart));
                }
                else
                {
                    if ((IsSelected() && ListCheck[idCheck].ActiveCheckOnMove) || (!IsSelected() && !ListCheck[idCheck].ActiveCheckOnMove))
                        if (null != act_Check)
                            act_Check.Invoke(idCheck);

                    StartCoroutine(PlayAudioToValidate(_audioClipStart));
                }
                return true;
            }
            return false;
        }

        public IEnumerator SnapToPosition(float Duration, Vector3 Position, Action act, AudioClip audio)
        {
            float timer = 0;
            Vector3 startPosition = _objectTransform.position;

            while (!IsSelected() && timer < Duration)
            {
                _objectTransform.position = Vector3.Lerp(startPosition, Position, timer / Duration);
                timer += Time.deltaTime;

                //ChangeMaterial();

                yield return null;
            }

            _objectTransform.position = Position;
            movementOffset += Position - startPosition;

            if (null != act)
                act.Invoke();

            StartCoroutine(PlayAudioToValidate(audio));
        }

        public IEnumerator SnapToPosition(float Duration, Vector3 Position, Action<int> act, int idLayer, AudioClip audio)
        {
            float timer = 0;
            Vector3 startPosition = _objectTransform.position;

            while (!IsSelected() && timer < Duration)
            {
                _objectTransform.position = Vector3.Lerp(startPosition, Position, timer / Duration);
                timer += Time.deltaTime;

                ChangeMaterial();

                yield return null;
            }

            _objectTransform.position = Position;
            movementOffset += Position - startPosition;

            if (null != act)
                act.Invoke(idLayer);

            StartCoroutine(PlayAudioToValidate(audio));
        }

        private void AudioVolume(Vector3 lastPos)
        {
            if (_objectAudio != null)
            {
                if (Vector3.Distance(_objectTransform.position, maxPosition) <= PrecisionMax && _audioClipStart != null)
                {
                    StartCoroutine(PlayAudioToValidate(_audioClipStart));
                }
                else if (Vector3.Distance(_objectTransform.position, minPosition) <= PrecisionMin && _audioClipFinal != null)
                {
                    StartCoroutine(PlayAudioToValidate(_audioClipFinal));
                }
                else
                {
                    foreach (Check check in ListCheck)
                    {
                        float distance = Vector3.Distance(_objectTransform.position, checkPosition[check.Id]);

                        bool testMin = _objectTransform.position == Vector3.Min(_objectTransform.position, checkPosition[check.Id]);
                        bool testMax = _objectTransform.position == Vector3.Max(_objectTransform.position, checkPosition[check.Id]);

                        if (((testMin && distance <= ListCheck[check.Id].LimitSnapMin) || (testMax && distance <= ListCheck[check.Id].LimitSnapMax)) && check.Block)
                        {
                            StartCoroutine(check.PlayAudioToValidate(_objectAudio, _volume));
                            return;
                        }
                        else
                        {
                            check.AudioPlayed = false;
                        }
                    }

                    if (_objectAudio.clip != _audioClipDefault)
                    {
                        _audioToValidatePlayed = false;
                        _objectAudio.clip = _audioClipDefault;
                        _objectAudio.loop = true;
                        _objectAudio.enabled = true;
                        _objectAudio.Play();
                    }

                    if (lastPos == _objectTransform.position)
                    {
                        float velocity = 0;
                        _objectAudio.volume = Mathf.SmoothDamp(_objectAudio.volume, 0, ref velocity, .05f);
                    }
                    else
                    {
                        float gap = Vector3.Distance(lastPos, _objectTransform.position);
                        _objectAudio.volume = Mathf.SmoothDamp(_objectAudio.volume, 1, ref gap, .05f) * _volume;
                    }
                }
            }
        }

        #region Gizmo
#if UNITY_EDITOR

        private Mesh objectMesh = null;
        private Vector3 defaultTranslation = new Vector3();
        private Vector3 defaultAxis = Vector3.zero;

        void OnDrawGizmosSelected()
        {
            defaultTranslation = GetDefaultTranslation();
            defaultAxis = GetDefaultAxis();

            if (true == Selection.Contains(this.gameObject))
            {
                if (null == objectMesh)
                {
                    objectMesh = GetMesh();
                }

                if (null != objectMesh)
                {
                    Gizmos.DrawLine(defaultTranslation + (defaultAxis * MinNumberOfUnit) - movementOffset, defaultTranslation + (defaultAxis * MaxNumberOfUnit) - movementOffset);

                    if (true == ShowMinTranslation)
                        DrawMeshAtMinTranslation();

                    if (true == ShowMaxTranslation)
                        DrawMeshAtMaxTranslation();

                    if (true == CheckTranslation && true == ShowCheckTranslation)
                    {
                        foreach (Check check in ListCheck)
                            DrawMeshAtCheckTranslation(check.Distance);
                    }
                }
            }
        }

        void OnValidate()
        {
            if (SnapDuration < 0.0f)
            {
                SnapDuration = 0.0f;
            }
            if (MinNumberOfUnit > 0.0f)
            {
                MinNumberOfUnit = 0.0f;
            }
            if (MaxNumberOfUnit < 0.0f)
            {
                MaxNumberOfUnit = 0.0f;
            }
        }

        private Mesh GetMesh()
        {
            MeshFilter filter = GetComponent<MeshFilter>();
            if (null != filter)
            {
                return filter.sharedMesh;
            }
            return null;
        }

        private Vector3 GetDefaultTranslation()
        {
            if (true == Application.isPlaying)
            {
                return (this.transform.position);
            }
            return this.transform.position;
        }

        private Vector3 GetDefaultAxis()
        {
            //Vector3 axis = Vector3.zero;

            if (Axis.RIGHT == AxisToTranslate)
                return transform.right;
            else if (Axis.UP == AxisToTranslate)
                return transform.up;

            return transform.forward;
        }

        private void DrawMeshAtMinTranslation()
        {
            Gizmos.color = Color.green;

            Gizmos.DrawWireMesh(objectMesh, defaultTranslation + (defaultAxis * MinNumberOfUnit) - movementOffset, this.transform.rotation, this.transform.lossyScale);

            Gizmos.color = Color.white;
        }

        private void DrawMeshAtMaxTranslation()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireMesh(objectMesh, defaultTranslation + (defaultAxis * MaxNumberOfUnit) - movementOffset, this.transform.rotation, this.transform.lossyScale);

            Gizmos.color = Color.white;
        }

        private void DrawMeshAtCheckTranslation(float CheckDistance)
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawWireMesh(objectMesh, defaultTranslation + (defaultAxis * CheckDistance) - movementOffset, this.transform.rotation, this.transform.lossyScale);

            Gizmos.color = Color.white;
        }
#endif
        #endregion
    }

}
