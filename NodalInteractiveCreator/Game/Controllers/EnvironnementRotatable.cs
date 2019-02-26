using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using NodalInteractiveCreator.Managers;
using NodalInteractiveCreator.Controllers;
using NodalInteractiveCreator.Viewpoints;

namespace NodalInteractiveCreator.Objects
{
    public class EnvironnementRotatable : MonoBehaviour
    {
        private Vector3 axisRight;
        public ViewPoint linkedViewPoint;
        private Vector3 up;

        public float smoothT = 0.07f;
        public AnimationCurve curvingY = AnimationCurve.Linear(0, 0, 360, 0);
        public AnimationCurve curvingX = AnimationCurve.Linear(0, -90, 0, 90);
        private Quaternion startQuat;
        private Transform target;
        void Awake()
        {
            startQuat = transform.localRotation;
            target = linkedViewPoint.transform;
        }

        private float yVel;
        private float xVel;
        private float ZVel;
        public void Update()
        {
            if (GameManager.Instance.InputsMgr == null ||
                GameManager.Instance.InputsMgr.CurrentCameraController == null || !linkedViewPoint || !linkedViewPoint.enabled)
                return;

            CameraController camController = GameManager.Instance.InputsMgr.CurrentCameraController;

            Vector3 lookAxis = (target.position - camController.transform.position).normalized;
            axisRight = Vector3.Cross(lookAxis, Vector3.up);

            Debug.DrawLine(camController.transform.position, target.position);
            Debug.DrawLine(camController.transform.position, camController.transform.position + axisRight * 10);

            Vector3 rawAngle = camController.transform.rotation.eulerAngles;

            float xAngle = curvingY.Evaluate(rawAngle.x);
            float yAngle = curvingX.Evaluate(rawAngle.y);

            Quaternion r = Quaternion.AngleAxis(yAngle, Vector3.up);
            Quaternion u = Quaternion.AngleAxis(xAngle, axisRight);
            Vector3 finalEuler = (u * r * startQuat).eulerAngles;

            float xFinal = Mathf.SmoothDampAngle(transform.rotation.eulerAngles.x, finalEuler.x, ref xVel, smoothT);
            float yFinal = Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, finalEuler.y, ref yVel, smoothT);
            float zFinal = Mathf.SmoothDampAngle(transform.rotation.eulerAngles.z, finalEuler.z, ref ZVel, smoothT);
            transform.localRotation = Quaternion.Euler(xFinal, yFinal, zFinal);
        }



        Vector2 GetMinMaxOfCurve(AnimationCurve curve)
        {
            if (curve.keys.Length == 0)
                return Vector2.zero;
            return new Vector2(curve.keys[0].value, curve.keys[curve.length - 1].value);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(transform.position, transform.position + axisRight * 10);
        }
    }
}