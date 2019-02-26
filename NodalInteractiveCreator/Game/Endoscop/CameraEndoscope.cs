using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NodalInteractiveCreator.Objects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NodalInteractiveCreator.Endoscop
{
    public class CameraEndoscope : MonoBehaviour
    {
        public static bool _onMove;

        protected EndoscopeData data;

        protected int pathPosition = 0;

        protected Transform t;
        private int pathLenght;
        private float maxDist;
        private float currentDist;
        private float noiseProgress;
        private Quaternion decal;
        private Vector3 forwardPath;
        [HideInInspector]
        public Vector2 axisLook;

        private Vector2 _axisSpeedLook;
        [HideInInspector]
        public float axisMovement;
        public float currentAxis = 0;

        private bool asFirstStep = false;
        public float angleMaxSpeed = 45;
        public AnimationCurve angleSpeedCurve = AnimationCurve.Linear(0, 0, 1, 1);

        protected Dictionary<EndoscopeMovement, Action> modeToAction;

        void Awake()
        {
            t = transform;
            modeToAction = new Dictionary<EndoscopeMovement, Action>()
            {
                {EndoscopeMovement.LINEAR, LinearMovement},
                {EndoscopeMovement.SNAPPING, SnapingMovement},
            };
        }

        public void Initialise(EndoscopeData pData)
        {
            Debug.Log("OnMove");
            _onMove = true;

            data = pData;
            pathLenght = data.path.Count;

            for (int i = 0; i < pathLenght; i++)
            {
                PathPoint p = data.path[i];
                p.callCount = p.callMax;
                data.path[i] = p;
            }
            SetToCheckPoint(0, 0, false);
            t.rotation = Quaternion.LookRotation(forwardPath, Vector3.up);
        }

        public void Disactivate()
        {

        }

        void Update()
        {
            modeToAction[data.movementMode].Invoke();

            noiseProgress += -axisMovement * data.noiseSpeed * Time.deltaTime;

            _axisSpeedLook.x = Mathf.Clamp(_axisSpeedLook.x + axisLook.x / 100 * data.rotationSpeed, -1f, 1f);
            _axisSpeedLook.y = Mathf.Clamp(_axisSpeedLook.y + axisLook.y / 100 * data.rotationSpeed, -1f, 1f);
            RotateOverAxis(_axisSpeedLook.magnitude);
        }

        void LinearMovement()
        {
            currentAxis = -axisMovement;
            MoveAlong(-axisMovement * data.speed * Time.deltaTime);
        }

        private bool canSwitch = true;

        void SnapingMovement()
        {

            if (Mathf.Abs(axisMovement) > data.snapDelta && currentAxis != Mathf.Sign(axisMovement) && canSwitch)
            {
                Debug.Log("OnMove");
                //_onMove = true;

                currentAxis = Mathf.Sign(axisMovement);
            }

            if (asFirstStep && currentAxis == 1f && pathPosition == 0)
            {

                currentAxis = 0;
            }

            MoveAlong(-currentAxis * data.speed * Time.deltaTime);
        }

        public IEnumerator StartMove()
        {
            asFirstStep = true;
            yield return new WaitForSeconds(0.2f);
            currentAxis = -1f;
        }

        void MoveAlong(float moveQuantity)
        {
            currentDist += moveQuantity;

            if (currentDist <= maxDist && currentDist >= 0)
            {
                float coef = currentDist / maxDist;
                Vector3 noise3D = ((Vector3.right * (Mathf.PerlinNoise(noiseProgress, 0) - 0.5f)) + (Vector3.up * (Mathf.PerlinNoise(0, noiseProgress) - 0.5f))) *
                                data.noisePower;

                noise3D -= noise3D / 2;

                Vector3 to = EndoscopeMath.LerpAlongBezier(data.path[pathPosition].curve, data.path[pathPosition + 1].curve, coef);

                t.localPosition = to;

                if (coef + 0.05f <= 1)
                {
                    Vector3 next = EndoscopeMath.LerpAlongBezier(data.path[pathPosition].curve, data.path[pathPosition + 1].curve,
                            coef + 0.05f);
                    forwardPath = (t.parent.rotation * (next - (to + noise3D))).normalized;
                }
            }
            else
            {
                Debug.Log("EndMove");
                _onMove = false;

                var next = pathPosition + (currentDist < 0 ? -1 : 1);

                if (next >= 0 && next < pathLenght - 1)
                    SetToCheckPoint(next, currentDist < 0 ? currentDist : currentDist - maxDist, currentDist < 0);
                else
                {
                    if (currentDist > maxDist || currentDist < 0)
                    {
                        ActivateActionsOnPoint(currentDist > maxDist ? next : 0);
                    }

                    currentDist = Mathf.Clamp(currentDist, 0, maxDist);
                }
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, transform.position + forwardPath * 5);
        }

        void RotateOverAxis(float intensity)
        {
            Quaternion origin = Quaternion.LookRotation(forwardPath, Vector3.up);
            Quaternion to =
                origin * Quaternion.AngleAxis(data.maxAngleSpread * intensity / 2, new Vector3(_axisSpeedLook.y, -_axisSpeedLook.x).normalized);

            float degreeBetween = Quaternion.Angle(transform.rotation, to);
            float degreeFromTo = Quaternion.Angle(origin, transform.rotation);
            t.rotation = Quaternion.Lerp(transform.rotation, to, 1);
        }

        void SetToCheckPoint(int checkPPos, float dist, bool reverse)
        {
            pathPosition = Mathf.Clamp(checkPPos, 0, pathLenght - 2);

            maxDist = EndoscopeMath.GetLengthOfBezier(data.path[pathPosition].curve, data.path[pathPosition + 1].curve, 20);
            currentDist = reverse ? maxDist : 0;

            ActivateActionsOnPoint(reverse ? pathPosition + 1 : pathPosition);
            MoveAlong(dist);
        }

        void ActivateActionsOnPoint(int point)
        {
            if (point < 0 || point >= data.path.Count)
                return;

            PathPoint p = data.path[point];

            if (p.isCheckPoint)
                currentAxis = 0;

            p.actions.Invoke();

            data.path[point] = p;
        }
    }
}
