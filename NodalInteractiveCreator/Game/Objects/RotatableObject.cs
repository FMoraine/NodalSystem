// Machinika Museum
// © Littlefield Studio
// Writted by Rémi Carreira - 2015
//
// Edited by Franck-Olivier FILIN - 2017
//
// RotatableObject.cs

using System;
using System.Collections;
using System.Collections.Generic;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using NodalInteractiveCreator.Managers;
using NodalInteractiveCreator.Tools;
using UnityEngine;

#if UNITY_EDITOR

#endif

namespace NodalInteractiveCreator.Objects
{
    public class RotatableObject : InteractiveObject
    {
        public bool ShowCheckRotation = true;
        public bool ShowSpecificRotation = false;
        public bool ShowMinRotation = true;
        public bool ShowMaxRotation = true;
        public bool Swipe = false;
        [Versioning("swipeH")]
        public bool HorizontalSwipe = false;
        public bool CheckRotation = false;
        public bool SpecificRotation = false;
        public bool LimitRotation = false;
        public bool SnapToSpecificRotation = false;
        public bool SnapToMinRotation = false;
        public bool SnapToMaxRotation = false;
        public bool SnapToCheckRotation = false;
        public bool AntiHoraire = false;
        public bool _backToStart = false;
        public bool _activeActionBackToStart = false;

        public Axis AxisToRotate = Axis.RIGHT;
        public float RotationSpeed = 1.0f;
        public float PrecisionMin = 1.0f;
        public float PrecisionMax = 1.0f;
        public float SnapDuration = 1.0f;
        public int NbCheck = 0;

        public float SpecificAngle { get { return specificAngle; } set { specificAngle = value; } }
        public float MinAngle { get { return minAngle; } set { minAngle = value; } }
        public float MaxAngle { get { return maxAngle; } set { maxAngle = value; } }
        public List<Check> ListCheck { get { return listCheck; } set { listCheck = value; } }

        public AudioClip _audioClipStart = null;
        public AudioClip _audioClipFinal = null;

        [SerializeField]
        protected float specificAngle = 0.0f;
        [SerializeField]
        protected float minAngle = 0.0f;
        [SerializeField]
        protected float maxAngle = 0.0f;
        [SerializeField]
        protected List<Check> listCheck = new List<Check>();

        private Vector3 objectScreenPosition = Vector3.zero;
        private Vector3 cameraAngles = Vector3.zero;
        private Vector3 startVector = Vector3.zero;
        private Vector3 currentVector = Vector3.zero;
        private Vector3 crossProduct = Vector3.zero;

        private float lastAngle = 0.0f;
        private float currentAngle = 0.0f;
        private float differenceAngle = 0.0f;

        private Vector3 lastInputPosition = Vector3.zero;
        private Vector3 currentInputPosition = Vector3.zero;
        private Vector3 deltaPosition = Vector3.zero;

        private float realObjectAngle = 0.0f;
        private Vector3 rotationAxis = Vector3.zero;

        private Quaternion _initRot = Quaternion.identity;

        protected SpecificRotations SpecificRotationsComponent = null;

        protected override void Awake()
        {
            base.Awake();

            _initRot = _objectTransform.rotation;

            if (_objectAudio != null)
                _objectAudio.loop = true;

            SpecificRotationsComponent = GetComponent<SpecificRotations>();
        }

        private void DetermineRotationAxis()
        {
            if (Axis.RIGHT == AxisToRotate)
                rotationAxis = this.transform.right;
            else if (Axis.UP == AxisToRotate)
                rotationAxis = this.transform.up;
            else if (Axis.FORWARD == AxisToRotate)
                rotationAxis = this.transform.forward;

            rotationAxis.Normalize();
        }

        public override void SelectObject(Camera Camera, Vector3 InputPosition)
        {
            base.SelectObject(Camera, InputPosition);

            if (!_interactable)
                return;

            if (_objectAudio != null)
                _objectAudio.enabled = true;

            DetermineRotationAxis();

            if (null != _mainCamera)
            {
                objectScreenPosition = _mainCamera.WorldToScreenPoint(_objectTransform.position);
                cameraAngles = _mainCamera.transform.rotation.eulerAngles;
            }

            startVector = InputPosition - objectScreenPosition;
            lastAngle = 0.0f;
            currentAngle = 0.0f;

            lastInputPosition = InputPosition;
            currentInputPosition = InputPosition;

            StopAllCoroutines();
        }

        public override void MoveObject(Vector3 InputPosition)
        {
            base.MoveObject(InputPosition);

            if (!_interactable)
                return;

            Vector3 lastRot = _objectTransform.localEulerAngles;

            if (false == Swipe)
                RotateBehaviourWithRotationGesture(InputPosition);
            else
                RotateBehaviourWithSwipe(InputPosition);

            AudioVolume(lastRot);
            ChangeMaterial();

            if (_backToStart)
                CheckAngleCondition();

            //act_Progress.Invoke(realObjectAngle);
        }

        public override void DeselectObject()
        {
            base.DeselectObject();

            if (!_interactable)
                return;

            StartCoroutine(FadeOutVolumeAudio());

            if (_backToStart)
            {
                if (_activeActionBackToStart)
                {
                    if (null != act_Start)
                        act_Start.Invoke();
                }

                StartCoroutine(RotateObject(-realObjectAngle, SnapDuration, null, 0, null));

                if (null != _objectAudio)
                {
                    _objectAudio.loop = true;
                    _objectAudio.enabled = true;
                }
            }
            else
                CheckAngleCondition();
        }

        public void AutoRotate(float angle)
        {
            StartCoroutine(Auto(angle));
        }

        private IEnumerator Auto(float from)
        {
            float timer = 0;
            do
            {
                realObjectAngle = Mathf.Lerp(realObjectAngle, from, timer / SnapDuration);
                _objectTransform.rotation = Quaternion.AngleAxis(realObjectAngle, rotationAxis) * _objectTransform.rotation;

                ChangeMaterial();

                if (_objectAudio != null && Mathf.Round(from) != 0)
                    _objectAudio.volume = _volume;

                timer += Time.deltaTime;

                yield return null;
            }
            while (timer / SnapDuration < 1);
            StopAllCoroutines();
        }

        private void ChangeMaterial()
        {
            float gap = realObjectAngle > 0 ? realObjectAngle/MaxAngle : realObjectAngle/MinAngle;

            MaterialConfig.ChangeMaterial(ref gap, ref _materials);
        }

        #region RotateBehaviourWithRotationGesture

        private void RotateBehaviourWithRotationGesture(Vector3 InputPosition)
        {
            currentVector = InputPosition - objectScreenPosition;
            crossProduct = Vector3.Cross(startVector, currentVector);
            lastAngle = currentAngle;
            currentAngle = Vector3.Angle(startVector, currentVector);
            differenceAngle = currentAngle - lastAngle;

            float angleStep = DetermineAngleStepInRotationGesture();

            angleStep = ComputeRealAngleStep(angleStep);

            _objectTransform.rotation = Quaternion.AngleAxis(angleStep, rotationAxis) * _objectTransform.rotation;
        }

        private float DetermineAngleStepInRotationGesture()
        {
            float angleStep = differenceAngle * RotationSpeed;

            if (Axis.RIGHT == AxisToRotate)
            {
                if (crossProduct.z > 0.0f)
                    angleStep *= -1.0f;
                if (cameraAngles.y >= 0.0f && cameraAngles.y <= 180.0f)
                    angleStep *= -1.0f;
            }
            else if (Axis.UP == AxisToRotate)
            {
                if (crossProduct.z <= 0.0f)
                    angleStep *= -1.0f;
                if (cameraAngles.x >= 0.0f && cameraAngles.x <= 180.0f)
                    angleStep *= -1.0f;
            }
            else
            {
                if (crossProduct.z > 0.0f)
                    angleStep *= -1.0f;
                if (cameraAngles.y >= -90.0f && cameraAngles.y <= 90.0f)
                    angleStep *= -1.0f;
            }

            return angleStep;
        }

        #endregion

        #region RotateBehaviourWithSwipe

        private void RotateBehaviourWithSwipe(Vector3 InputPosition)
        {
            lastInputPosition = currentInputPosition;
            currentInputPosition = InputPosition;
            deltaPosition = currentInputPosition - lastInputPosition;

            float angleStep = DetermineAngleStepInSwipe();

            angleStep = ComputeRealAngleStep(angleStep);

            _objectTransform.rotation = Quaternion.AngleAxis(angleStep, rotationAxis) * _objectTransform.rotation;
        }

        private float DetermineAngleStepInSwipe()
        {
            float angleStep = 0.0f;

            if (Axis.RIGHT == AxisToRotate)
            {
                angleStep = (false == HorizontalSwipe) ? -(deltaPosition.y * RotationSpeed) : (deltaPosition.x * RotationSpeed);
            }
            else if (Axis.UP == AxisToRotate)
            {
                angleStep = (false == HorizontalSwipe) ? (deltaPosition.y * RotationSpeed) : -(deltaPosition.x * RotationSpeed);
            }
            else if (Axis.FORWARD == AxisToRotate)
            {
                angleStep = (false == HorizontalSwipe) ? (deltaPosition.y * RotationSpeed) : -(deltaPosition.x * RotationSpeed);
            }

            return angleStep;
        }

        #endregion

        private float ComputeRealAngleStep(float AngleStep)
        {
            realObjectAngle += AngleStep;

            ClampAngleAt360Degree();

            if (true == LimitRotation)
            {
                if (realObjectAngle < minAngle)
                {
                    AngleStep -= realObjectAngle - minAngle;
                    realObjectAngle = minAngle;
                }
                else if (realObjectAngle > maxAngle)
                {
                    AngleStep -= realObjectAngle - maxAngle;
                    realObjectAngle = maxAngle;
                }
            }

            if (true == CheckRotation)
            {
                if (AntiHoraire)
                    AngleStep = -AngleStep;
            }
            return AngleStep;
        }

        private void ClampAngleAt360Degree()
        {
            if (LimitRotation || _backToStart)
            {
                if (realObjectAngle < -360)
                    realObjectAngle += 360;
                else if (realObjectAngle > 360)
                    realObjectAngle -= 360;
            }
            else
            {
                if (realObjectAngle < 0)
                    realObjectAngle += 360;
                else if (realObjectAngle > 360)
                    realObjectAngle -= 360;
            }
        }

        void CheckAngleCondition()
        {
            if (true == LimitRotation)
                CheckLimitsCondition();

            if (true == CheckRotation)
                CheckChecksCondition();

            //if (true == SpecificRotation)
            //    CheckSpecificCondition();
        }

        private void CheckLimitsCondition()
        {
            StartCoroutine(FadeOutVolumeAudio());

            if (maxAngle - realObjectAngle <= PrecisionMax)
            {
                if (false == IsSelected() && true == SnapToMaxRotation && maxAngle - realObjectAngle >= 1)
                {
                    StartCoroutine(RotateObject((maxAngle - realObjectAngle), SnapDuration, act_Final, 0, _audioClipFinal));
                }
                else
                {
                    if (null != act_Final)
                        act_Final.Invoke();

                    StartCoroutine(PlayAudioToValidate(_audioClipFinal));
                }
            }
            else if ((realObjectAngle - minAngle <= PrecisionMin))
            {
                if (false == IsSelected() && true == SnapToMinRotation && realObjectAngle - minAngle >= 1)
                {
                    StartCoroutine(RotateObject((minAngle - realObjectAngle), SnapDuration, act_Start, 0, _audioClipStart));
                }
                else
                {
                    if (null != act_Start)
                        act_Start.Invoke();

                    StartCoroutine(PlayAudioToValidate(_audioClipStart));
                }
            }
        }

        private void CheckChecksCondition()
        {
            foreach (Check check in ListCheck)
            {
                if (realObjectAngle >= check.Angle - check.LimitSnapMin && realObjectAngle <= check.Angle + check.LimitSnapMax)
                {
                    if (false == IsSelected() && true == SnapToCheckRotation && (realObjectAngle - check.Angle >= 1 || realObjectAngle - check.Angle <= -1))
                    {
                        StartCoroutine(RotateObjectForCheck((check.Angle - realObjectAngle), SnapDuration, act_Check, check.Id, _audioClipDefault));
                    }
                    else
                    {
                        if (null != act_Check)
                            act_Check.Invoke(check.Id);
                    }

                    if (!check.AudioPlayed)
                        StartCoroutine(check.PlayAudioToValidate(_objectAudio, _volume));
                }
                else if (realObjectAngle >= -check.Angle - check.LimitSnapMin && realObjectAngle <= -check.Angle + check.LimitSnapMax)
                {
                    if (false == IsSelected() && true == SnapToCheckRotation && (realObjectAngle - check.Angle >= 1 || realObjectAngle - check.Angle <= -1))
                    {
                        StartCoroutine(RotateObjectForCheck((check.Angle - realObjectAngle), SnapDuration, act_Check, check.Id, _audioClipDefault));
                    }
                    else
                    {
                        if (null != act_Check)
                            act_Check.Invoke(check.Id);
                    }

                    if (!check.AudioPlayed)
                        StartCoroutine(check.PlayAudioToValidate(_objectAudio, _volume));
                }
                else if (_backToStart)
                {
                    if (null != act_Start)
                        act_Start.Invoke();
                    StartCoroutine(PlayAudioToValidate(_audioClipStart));
                }
            }
        }

        //private void CheckSpecificCondition()
        //{
        //    if (null != SpecificRotationsComponent)
        //    {
        //        if (true == SpecificRotationsComponent.CheckSpecificAngle(realObjectAngle))
        //        {
        //            if (false == IsSelected() && true == SpecificRotationsComponent.Snap)
        //            {
        //                StopAllCoroutines();
        //                StartCoroutine(RotateObject(SpecificRotationsComponent.GetDeltaAngle(), SpecificRotationsComponent.SnapDuration));
        //            }
        //            OnSpecificRotation();
        //        }
        //    }
        //}

        //protected virtual void OnSpecificRotation()
        //{

        //}

        //public override void BlockOrUnblockAllChecks(bool res)
        //{
        //    base.BlockOrUnblockAllChecks(res);

        //    foreach (Check check in ListCheck)
        //        check.Block = res;
        //}

        //public override void BlockOrUnblockChecks(List<bool> listRes)
        //{
        //    base.BlockOrUnblockChecks(listRes);

        //    foreach (Check check in ListCheck)
        //        check.Block = listRes[check.Id];
        //}

        public void AudioVolume(Vector3 lastRot)
        {
            if (_objectAudio != null)
            {
                if (maxAngle - realObjectAngle <= PrecisionMax)
                    StartCoroutine(PlayAudioToValidate(_audioClipStart));
                else if (realObjectAngle - minAngle <= PrecisionMin)
                    StartCoroutine(PlayAudioToValidate(_audioClipFinal));
                else
                {
                    bool res = false;
                    foreach (Check check in ListCheck)
                    {
                        if (realObjectAngle >= check.Angle - check.LimitSnapMin && realObjectAngle <= check.Angle + check.LimitSnapMax)
                        {
                            res = true;
                            StartCoroutine(check.PlayAudioToValidate(_objectAudio, _volume));
                        }
                        else if (realObjectAngle >= -check.Angle - check.LimitSnapMin && realObjectAngle <= -check.Angle + check.LimitSnapMax)
                        {
                            res = true;
                            StartCoroutine(check.PlayAudioToValidate(_objectAudio, _volume));
                        }
                        else
                        {
                            check.AudioPlayed = false;
                        }
                    }

                    if (res)
                        return;
                    else
                    {
                        if (_objectAudio.clip != _audioClipDefault && !_objectAudio.isPlaying)
                        {
                            _audioToValidatePlayed = false;
                            _objectAudio.clip = _audioClipDefault;
                            _objectAudio.loop = true;
                            _objectAudio.enabled = true;
                        }

                        if (!_objectAudio.isPlaying)
                            _objectAudio.Play();

                        if (lastRot == _objectTransform.eulerAngles)
                        {
                            float velocity = 0;
                            _objectAudio.volume = Mathf.SmoothDamp(_objectAudio.volume, 0, ref velocity, .05f);
                        }
                        else
                        {
                            float gap = Vector3.Distance(lastRot, _objectTransform.eulerAngles);
                            _objectAudio.volume = Mathf.SmoothDamp(_objectAudio.volume, 1f, ref gap, .05f) * _volume;
                        }
                    }
                }
            }
        }

        //private IEnumerator RotateObject(float AngleStep, float Duration)
        //{
        //    float timer = 0.0f;
        //    float lastAngleStep = 0.0f;
        //    float currentAngleStep = 0.0f;

        //    do
        //    {
        //        lastAngleStep = currentAngleStep;
        //        //currentAngleStep = Mathf.Lerp(0.0f, AngleStep, timer / Duration);
        //        currentAngleStep = Mathf.SmoothStep(0.0f, AngleStep, timer / Duration);
        //        float realAngleStep = ComputeRealAngleStep(currentAngleStep - lastAngleStep);

        //        _objectTransform.rotation = Quaternion.AngleAxis(realAngleStep, rotationAxis) * _objectTransform.rotation;

        //        if (_objectAudio != null && Mathf.Round(AngleStep) != 0)
        //            _objectAudio.volume = _volume;

        //        timer += Time.deltaTime;

        //        yield return null;
        //    }
        //    while (timer < Duration);

        //    if (_backToStart)
        //        _objectTransform.rotation = _initRot;

        //    StartCoroutine(FadeOutVolumeAudio());
        //}

        private IEnumerator RotateObject(float AngleStep, float Duration, Action act, int idLayer, AudioClip audio)
        {
            float timer = 0.0f;
            float lastAngleStep = 0.0f;
            float currentAngleStep = 0.0f;

            do
            {
                lastAngleStep = currentAngleStep;
                currentAngleStep = Mathf.Lerp(0.0f, AngleStep, timer / Duration);
                float realAngleStep = ComputeRealAngleStep(currentAngleStep - lastAngleStep);

                _objectTransform.rotation = Quaternion.AngleAxis(realAngleStep, rotationAxis) * _objectTransform.rotation;

                ChangeMaterial();

                timer += Time.deltaTime;

                if (_objectAudio != null && Mathf.Round(AngleStep) != 0)
                    _objectAudio.volume = _volume;

                yield return null;
            }
            while (timer <= Duration);

            if (_backToStart)
                _objectTransform.rotation = _initRot;

            if (act != null)
                act.Invoke();

            StartCoroutine(PlayAudioToValidate(audio));
        }

        private IEnumerator RotateObjectForCheck(float AngleStep, float Duration, Action<int> act, int idLayer, AudioClip audio)
        {
            float timer = 0.0f;
            float lastAngleStep = 0.0f;
            float currentAngleStep = 0.0f;

            do
            {
                lastAngleStep = currentAngleStep;
                currentAngleStep = Mathf.Lerp(0.0f, AngleStep, timer / Duration);
                float realAngleStep = ComputeRealAngleStep(currentAngleStep - lastAngleStep);

                _objectTransform.rotation = Quaternion.AngleAxis(realAngleStep, rotationAxis) * _objectTransform.rotation;

                ChangeMaterial();

                timer += Time.deltaTime;

                if (_objectAudio != null && Mathf.Round(AngleStep) != 0)
                    _objectAudio.volume = _volume;

                yield return null;
            }
            while (timer <= Duration);

            if (_backToStart)
                _objectTransform.rotation = _initRot;

            if (act != null)
                act.Invoke(idLayer);

            StartCoroutine(PlayAudioToValidate(audio));
        }

        #region Gizmo
#if UNITY_EDITOR

        private Mesh objectMesh = null;
        private Quaternion defaultRotation = Quaternion.identity;
        private Vector3 defaultAxis = Vector3.zero;

        void OnDrawGizmosSelected()
        {
            defaultRotation = GetDefaultRotation();
            defaultAxis = GetDefaultAxis();

            if (null == objectMesh)
            {
                objectMesh = GetMesh();
            }
            if (null != objectMesh)
            {
                if (true == SpecificRotation && true == ShowSpecificRotation)
                    DrawMeshAtSpecificRotation();

                if (true == LimitRotation)
                {
                    if (true == ShowMinRotation)
                        DrawMeshAtMinRotation();
                    if (true == ShowMaxRotation)
                        DrawMeshAtMaxRotation();
                }

                if (true == CheckRotation && true == ShowCheckRotation)
                {
                    foreach (Check check in ListCheck)
                        DrawMeshAtCheckRotation(check.Angle);
                }
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

        private Quaternion GetDefaultRotation()
        {
            //if (true == Application.isPlaying)
            //{
            //    return (this.transform.rotation * Quaternion.AngleAxis(-realObjectAngle, defaultAxis));
            //}
            return this.transform.rotation;
        }

        private Vector3 GetDefaultAxis()
        {
            if (Axis.RIGHT == AxisToRotate)
                return transform.right;
            else if (Axis.UP == AxisToRotate)
                return transform.up;

            return this.transform.forward;
        }

        private void DrawMeshAtSpecificRotation()
        {
            Gizmos.color = Color.green;

            Gizmos.DrawWireMesh(objectMesh, this.transform.position, Quaternion.AngleAxis(specificAngle, defaultAxis) * defaultRotation, this.transform.lossyScale);

            Gizmos.color = Color.white;
        }

        private void DrawMeshAtMinRotation()
        {
            Gizmos.color = Color.green;

            Gizmos.DrawWireMesh(objectMesh, this.transform.position, Quaternion.AngleAxis(minAngle, defaultAxis) * defaultRotation, this.transform.lossyScale);

            Gizmos.color = Color.white;
        }

        private void DrawMeshAtMaxRotation()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireMesh(objectMesh, this.transform.position, Quaternion.AngleAxis(maxAngle, defaultAxis) * defaultRotation, this.transform.lossyScale);

            Gizmos.color = Color.white;
        }

        private void DrawMeshAtCheckRotation(float checkAngle)
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawWireMesh(objectMesh, this.transform.position, Quaternion.AngleAxis(checkAngle, defaultAxis) * defaultRotation, this.transform.lossyScale);

            Gizmos.color = Color.white;
        }
#endif
        #endregion
    }

}
