using UnityEngine;
using System.Collections;
using NodalInteractiveCreator.Objects;

namespace NodalInteractiveCreator.Screwdriver
{
    public class ScrewdriverRotateWheel : RotatableObject
    {
        public ScrewdriverHead Head = null;
        public byte ToothsNumber = 0;

        private Quaternion defaultQuaternion = Quaternion.identity;
        private Quaternion lastRotation = Quaternion.identity;

        private float toothAngle = 0.0f;

        private bool clampToTooth = false;
        private Quaternion startRotation = Quaternion.identity;
        private Quaternion finishRotation = Quaternion.identity;

        private float clampTime = 0.0f;
        private float clampDuration = 0.25f;

        private int currentToothIndex = 0;

        private Quaternion localRotationSinceSelect = Quaternion.identity;

        protected override void Awake()
        {
            base.Awake();

            defaultQuaternion = _objectTransform.localRotation;
            lastRotation = _objectTransform.localRotation;

            toothAngle = 360 / ToothsNumber;

            if (null != _objectCollider)
            {
                _objectCollider.enabled = true;
            }
        }

        protected void Update()
        {
            if (true == clampToTooth)
            {
                clampTime += Time.deltaTime;


                if (clampTime >= clampDuration)
                {
                    clampToTooth = false;

                    _objectTransform.localRotation = finishRotation;
                }
                else
                {
                    _objectTransform.localRotation = Quaternion.Lerp(startRotation, finishRotation, clampTime / clampDuration);
                }

                Vector3 crossProduct = Vector3.Cross(lastRotation * Vector3.up, _objectTransform.localRotation * Vector3.up);
                float angle = Quaternion.Angle(lastRotation, _objectTransform.localRotation);

                lastRotation = _objectTransform.localRotation;

                if (crossProduct.z != 0.0f && null != Head)
                {
                    if (crossProduct.z > 0.0f)
                    {
                        Head.Rotate(angle);
                    }
                    else
                    {
                        Head.Rotate(-angle);
                    }
                }
            }
        }

        public override void SelectObject(Camera Camera, Vector3 InputPosition)
        {
            base.SelectObject(Camera, InputPosition);

            localRotationSinceSelect = _objectTransform.localRotation;
        }

        public override void MoveObject(Vector3 InputPosition)
        {
            base.MoveObject(InputPosition);

            Vector3 crossProduct = Vector3.Cross(lastRotation * Vector3.up, _objectTransform.localRotation * Vector3.up);
            float angle = Quaternion.Angle(lastRotation, _objectTransform.localRotation);

            lastRotation = _objectTransform.localRotation;

            if (crossProduct.z != 0.0f && null != Head)
            {
                if (crossProduct.z > 0.0f)
                {
                    Head.Rotate(angle);
                }
                else
                {
                    Head.Rotate(-angle);
                }
            }

            DetermineToothIndex();
        }

        public override void DeselectObject()
        {
            base.DeselectObject();

            DetermineToothIndex();

            float indexAngle = currentToothIndex * toothAngle;

            clampToTooth = true;
            clampTime = 0.0f;
            startRotation = _objectTransform.localRotation;
            finishRotation = Quaternion.AngleAxis(indexAngle, Vector3.forward);
        }

        void DetermineToothIndex()
        {
            float angle = Quaternion.Angle(defaultQuaternion, _objectTransform.localRotation);
            Vector3 crossProduct = Vector3.Cross(defaultQuaternion * Vector3.up, _objectTransform.localRotation * Vector3.up);

            if (crossProduct.z < 0.0f)
            {
                angle = 360.0f - angle;
            }

            currentToothIndex = (int)((angle / toothAngle) + 0.5f);

            if (currentToothIndex == ToothsNumber)
            {
                currentToothIndex = 0;
            }
        }

        void FakeVelocity()
        {
            float angle = Quaternion.Angle(localRotationSinceSelect, _objectTransform.localRotation);
            Vector3 crossProduct = Vector3.Cross(localRotationSinceSelect * Vector3.up, _objectTransform.localRotation * Vector3.up);

            if (angle > 15.0f)
            {
                currentToothIndex += (crossProduct.z > 0.0f) ? 1 : -1;

                if (currentToothIndex == ToothsNumber)
                {
                    currentToothIndex = 0;
                }
                else if (currentToothIndex < 0)
                {
                    currentToothIndex = ToothsNumber - 1;
                }
            }
        }

        public int GetCurrentToothIndex()
        {
            return currentToothIndex;
        }
    }
}