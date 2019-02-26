using System;
using UnityEngine;

namespace NodalInteractiveCreator.Objects.Puzzle
{
    //[RequireComponent(typeof(Collider))]
    public class InteractivePuzzleElement : TouchableElement
    {
        public System.Action<Vector2, Vector2> onMove;
        public System.Action<float> onMoveComplete;
        protected Vector3 startPos;
        protected Vector3 lastPos;

        public bool IsTouching { get; protected set; }
        private bool display = false;
        [HideInInspector]
        public Vector3 targetPos;

        public float smoothT = 0.08f;

        private Vector3 velocityMove;

        public override void SelectObject(Camera Camera, Vector3 InputPosition)
        {
            base.SelectObject(Camera, InputPosition);
            startPos = InputPosition;
            display = true;
            IsTouching = true;
        }

        public override void DeselectObject()
        {
            base.DeselectObject();
            display = false;
            IsTouching = false;
            Vector3 vec = startPos - lastPos;

            float angle = Mathf.Atan2(Vector3.up.y - vec.y, Vector3.up.x - vec.x) * Mathf.Rad2Deg - 90;

            if (angle < 0)
                angle = Mathf.Abs(angle);
            else
                angle = 360 - angle;


            if (onMoveComplete != null)
                onMoveComplete.Invoke(angle);
        }

        public void ResetStartPoint()
        {
            startPos = lastPos;
        }

        public override void MoveObject(Vector3 InputPosition)
        {

            base.MoveObject(InputPosition);
            lastPos = InputPosition;
        }

        void Update()
        {
            if (onMove != null && display)
                onMove.Invoke(lastPos, startPos);

            if (_interactable)
                transform.localPosition =
                Vector3.SmoothDamp(transform.localPosition, targetPos, ref velocityMove, smoothT);
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying || !display)
                return;

            startPos.z = 5;
            lastPos.z = 5;

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(Camera.current.ScreenToWorldPoint(startPos), 0.1f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(Camera.current.ScreenToWorldPoint(lastPos), 0.1f);
            Gizmos.color = Color.white;
        }

        public bool TestCollisionsWithOther(Vector3 pos)
        {
            Collider[] l = Physics.OverlapBox(pos, _objectCollider.bounds.extents, transform.rotation);

            if (l.Length > 0)
            {
                foreach (var collider in l)
                {
                    if (collider != _objectCollider && collider.GetComponent<InteractivePuzzleElement>() != null)
                        return true;
                }
            }

            return false;
        }

        private void OnDestroy()
        {
            onMove = null;
        }

        [Serializable]
        public class Tester
        {
            public Material mat;
            public AnimationCurve curve;
            public GameObject g;
        }
    }
}