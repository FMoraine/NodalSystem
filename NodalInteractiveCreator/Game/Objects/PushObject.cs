// Machinika Museum
// © Littlefield Studio
// Writted by Franck-Olivier FILIN - 2017
//
// PushObject.cs

using System.Collections;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using UnityEngine;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NodalInteractiveCreator.Objects
{
    public class PushObject : InteractiveObject
    {
        public bool _showDebug = true;
        public Axis _axisTranslate;
        public Axis _axisRotate;
        public float _offsetPos = 0;
        public float _offsetRot = 0;
        public float _duration = 0;
        public bool _twoStep = false;
        public bool _onPress = false;

        private Vector3 _currentAxisTranslate = new Vector3();
        private Vector3 _currentAxisRotate = new Vector3();
        private Vector3 _startPos = new Vector3();
        private Vector3 _endPos = new Vector3();
        private Quaternion _startRot = Quaternion.identity;
        private Quaternion _endRot = Quaternion.identity;
        private bool _isInversed;
        private bool _isMoved;

        protected override void Awake()
        {
            base.Awake();
        }

        public override void SelectObject(Camera Camera, Vector3 InputPosition)
        {
            base.SelectObject(Camera, InputPosition);

            if (_isMoved)
                return;

            if (_objectAudio != null)
                _objectAudio.enabled = true;

            //if (!_twoStep)
            //SetGradientWithCurrentColorMaterial();

            ActivePush();
        }

        public override void DeselectObject()
        {
            base.DeselectObject();
        }

        public void ActivePush()
        {
            InitStartEnd();

            StartCoroutine(ProcessMove());
        }

        private void ChangeMaterial(Vector3 initPos, Quaternion initRot)
        {
            float gap = 0;

            float gapPos = Vector3.Distance(_objectTransform.position, initPos) / _offsetPos;
            float gapRot = Quaternion.Angle(_objectTransform.rotation, initRot) / _offsetRot;

            if(float.IsNaN(gapPos) || gapPos < gapRot)
                gap = Mathf.Abs(gapRot);
            else
                gap = Mathf.Abs(gapPos);

            MaterialConfig.ChangeMaterial(ref gap, ref _materials);
        }

        private void InitStartEnd()
        {
            DetermineAxis();

            _startPos = _isInversed ? _objectTransform.position + (_currentAxisTranslate * -_offsetPos) : _objectTransform.position;
            _startRot = _isInversed ? Quaternion.AngleAxis(-_offsetRot, _currentAxisRotate) * _objectTransform.rotation : _objectTransform.rotation;

            _endPos = _isInversed ? _objectTransform.position : _objectTransform.position + (_currentAxisTranslate * _offsetPos);
            _endRot = _isInversed ? _objectTransform.rotation : Quaternion.AngleAxis(_offsetRot, _currentAxisRotate) * _objectTransform.rotation;
        }

        private void DetermineAxis()
        {
            if (Axis.RIGHT == _axisTranslate)
                _currentAxisTranslate = _objectTransform.right;
            else if (Axis.UP == _axisTranslate)
                _currentAxisTranslate = _objectTransform.up;
            else
                _currentAxisTranslate = _objectTransform.forward;

            if (Axis.RIGHT == _axisRotate)
                _currentAxisRotate = _objectTransform.right;
            else if (Axis.UP == _axisRotate)
                _currentAxisRotate = _objectTransform.up;
            else
                _currentAxisRotate = _objectTransform.forward;
        }

        private IEnumerator ProcessMove()
        {
            _isMoved = true;
            yield return MoveObj(_startPos, _endPos, _startRot, _endRot, _isInversed);

            if (_twoStep)
            {
                _isInversed = !_isInversed;
                _isMoved = false;

                if(act_Final != null)
                    act_Final.Invoke();

                StopAllCoroutines();
            }
            else if (IsSelected() && _onPress)
            {
                if(act_Final != null)
                    act_Final.Invoke();

                yield return new WaitWhile(() => IsSelected());
            }
            yield return MoveObj(_startPos, _endPos, _startRot, _endRot, !_isInversed);

            if (act_Start != null)
                act_Start.Invoke();

            _isMoved = false;
        }

        private IEnumerator MoveObj(Vector3 posFrom, Vector3 posTo, Quaternion rotFrom, Quaternion rotTo, bool isInverse)
        {
            float endTimer = !isInverse ? _duration : 0;
            float timer = isInverse ? _duration : 0;

            while (isInverse ? timer > endTimer : timer < _duration)
            {

                //if (act_Progress != null)
                //    act_Progress.Invoke(timer / _duration);
                ChangeMaterial(posTo, rotTo);

                _objectTransform.position = Vector3.Lerp(posFrom, posTo, timer / _duration);
                _objectTransform.rotation = Quaternion.Lerp(rotFrom, rotTo, timer / _duration);
                timer += isInverse ? -Time.deltaTime : Time.deltaTime;
                yield return null;
            }


            //if (act_Progress != null)
            //    act_Progress.Invoke(endTimer / _duration);
            ChangeMaterial(posTo, rotTo);

            _objectTransform.position = Vector3.Lerp(posFrom, posTo, endTimer / _duration);
            _objectTransform.rotation = Quaternion.Lerp(rotFrom, rotTo, endTimer / _duration);
        }

        #region Gizmo
#if UNITY_EDITOR
        private Mesh objectMesh = null;
        private Vector3 defaultPosition = new Vector3();
        private Vector3 GetDefaultPosition { get { return this.transform.position; } }
        private Quaternion defaultRotation = Quaternion.identity;
        private Quaternion GetDefaultRotation { get { return this.transform.rotation; } }
        private Vector3 defaultAxisTranslate = Vector3.zero;
        private Vector3 defaultAxisRotate = Vector3.zero;

        void OnDrawGizmosSelected()
        {
            if (true == _showDebug && true == Selection.Contains(this.gameObject))
            {
                defaultPosition = GetDefaultPosition;
                defaultRotation = GetDefaultRotation;
                GetDefaultAxis();

                if (null == objectMesh)
                    objectMesh = GetComponent<MeshFilter>().sharedMesh;

                DrawMeshInitTransform();
                DrawMeshAtMaxTransform();
            }
        }

        private void GetDefaultAxis()
        {
            if (Axis.RIGHT == _axisTranslate)
                defaultAxisTranslate = transform.right;
            else if (Axis.UP == _axisTranslate)
                defaultAxisTranslate = transform.up;
            else
                defaultAxisTranslate = transform.forward;

            if (Axis.RIGHT == _axisRotate)
                defaultAxisRotate = transform.right;
            else if (Axis.UP == _axisRotate)
                defaultAxisRotate = transform.up;
            else
                defaultAxisRotate = transform.forward;
        }

        private void DrawMeshInitTransform()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireMesh(objectMesh, defaultPosition, defaultRotation, this.transform.lossyScale);
        }

        private void DrawMeshAtMaxTransform()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireMesh(objectMesh, defaultPosition + (_offsetPos * defaultAxisTranslate), Quaternion.AngleAxis(_offsetRot, defaultAxisRotate) * defaultRotation, this.transform.lossyScale);
        }

#endif
        #endregion
    }
}

