using UnityEngine;
using System.Collections;
using NodalInteractiveCreator.Objects;

namespace NodalInteractiveCreator.Screwdriver
{
    public class ScrewdriverFormSwitch : RotatableObject
    {
        public GameObject FirstHeadWheels = null;
        public GameObject SecondHeadWheels = null;

        private Quaternion baseRotation = Quaternion.identity;

        private bool snap = false;
        private Quaternion startRotation = Quaternion.identity;
        private Quaternion finalRotation = Quaternion.identity;
        private float snapTime = 0.0f;
        private float snapDuration = 0.20f;

        protected override void Awake()
        {
            base.Awake();

            baseRotation = _objectTransform.localRotation;

            if (null != _objectCollider)
            {
                _objectCollider.enabled = true;
            }

            EnableFirstHeadWheels(true);
            EnableSecondHeadWheels(false);
        }

        protected void Update()
        {
            if (true == snap)
            {
                snapTime += Time.deltaTime;

                _objectTransform.localRotation = Quaternion.Lerp(startRotation, finalRotation, snapTime / snapDuration);

                if (snapTime >= snapDuration)
                {
                    snap = false;
                }
            }
        }

        public override void DeselectObject()
        {
            base.DeselectObject();

            snap = true;
            snapTime = 0.0f;
            startRotation = _objectTransform.localRotation;

            Vector3 forwardA = baseRotation * Vector3.right;
            Vector3 forwardB = _objectTransform.localRotation * Vector3.right;

            float angleA = Mathf.Atan2(forwardA.x, forwardA.y) * Mathf.Rad2Deg;
            float angleB = Mathf.Atan2(forwardB.x, forwardB.y) * Mathf.Rad2Deg;

            float realAngle = Mathf.DeltaAngle(angleB, angleA);

            if (realAngle > (minAngle * 0.5f))
            {
                finalRotation = Quaternion.AngleAxis(0.0f, Vector3.forward);

                EnableFirstHeadWheels(true);
                EnableSecondHeadWheels(false);
            }
            else
            {
                finalRotation = Quaternion.AngleAxis(minAngle, Vector3.forward);

                EnableFirstHeadWheels(false);
                EnableSecondHeadWheels(true);
            }
        }

        private void EnableFirstHeadWheels(bool Enable)
        {
            if (null != FirstHeadWheels)
            {
                FirstHeadWheels.SetActive(Enable);
            }
        }

        private void EnableSecondHeadWheels(bool Enable)
        {
            if (null != SecondHeadWheels)
            {
                SecondHeadWheels.SetActive(Enable);
            }
        }
    }
}
