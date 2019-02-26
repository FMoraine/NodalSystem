using UnityEngine;
using System.Collections;
using NodalInteractiveCreator.Objects;

namespace NodalInteractiveCreator.Screwdriver
{
    public class ScrewdriverFormWheel : RotatableObject
    {
        public ScrewdriverHead Head = null;
        public byte NumberOfFaces = 0;

        private byte currentFace = 0;
        private float angleByFace = 0.0f;

        private Quaternion tmpLocalRotation = Quaternion.identity;

        protected override void Awake()
        {
            base.Awake();

            tmpLocalRotation = _objectTransform.localRotation;
            _objectCollider.enabled = true;

            if (0 != NumberOfFaces)
            {
                angleByFace = (360.0f / (float)NumberOfFaces);
            }
        }

        public override void MoveObject(Vector3 InputPosition)
        {
            base.MoveObject(InputPosition);

            float lastFace = currentFace;

            DetermineCurrentFace();

            if (null != Head)
            {
                if (lastFace == 3 && currentFace == 0)
                {
                    Head.ChangeForm(true);
                }
                else if (lastFace == 0 && currentFace == 3)
                {
                    Head.ChangeForm(false);
                }
                else if (lastFace < currentFace)
                {
                    Head.ChangeForm(true);
                }
                else if (lastFace > currentFace)
                {
                    Head.ChangeForm(false);
                }
            }
        }

        public override void DeselectObject()
        {
            base.DeselectObject();

            DetermineCurrentFace();

            if (true == gameObject.activeSelf)
            {
                StartCoroutine(SnapToFace());
            }
        }

        private void DetermineCurrentFace()
        {
            Vector3 rightA = tmpLocalRotation * Vector3.right;
            Vector3 rightB = _objectTransform.localRotation * Vector3.right;

            float angleA = Mathf.Atan2(rightA.x, rightA.y) * Mathf.Rad2Deg;
            float angleB = Mathf.Atan2(rightB.x, rightB.y) * Mathf.Rad2Deg;

            float angle = Mathf.DeltaAngle(angleB, angleA);

            if (angle < 0.0f)
            {
                angle += 360.0f;
            }

            currentFace = (byte)((angle / angleByFace) + 0.5f);

            if (currentFace == NumberOfFaces)
            {
                currentFace = 0;
            }
        }

        private IEnumerator SnapToFace()
        {
            Quaternion startRotation = _objectTransform.localRotation;
            Quaternion finishRotation = Quaternion.AngleAxis((currentFace * angleByFace), Vector3.forward);

            float timer = 0;
            float duration = 0.2f;

            while (timer < duration)
            {
                _objectTransform.localRotation = Quaternion.Lerp(startRotation, finishRotation, timer / duration);

                yield return null;

                timer += Time.deltaTime;
            }

            _objectTransform.localRotation = finishRotation;
        }

        public byte GetCurrentFace()
        {
            return currentFace;
        }
    }
}