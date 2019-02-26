using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodalInteractiveCreator.Managers;

namespace NodalInteractiveCreator.Objects.Puzzle
{
    [Serializable]
    public struct PuzzleObjectFeedback
    {
        public int idAnime;
        public AnimationClip animationClip;
        public AudioClip audioClip;
    }

    [Serializable]
    public struct PuzzleObjectDesc
    {
        public string type;

        public bool canMove;
        public bool interactiveByInput;
        public bool snapToCell;
        public int canBePushedBy;
        public int canBlock;
        public float speed;
        public bool isLogical;
        public int victoryLayer;

        public bool isTrigger;
        public int triggerWith;

        public Color colorTag;
        public InteractivePuzzleElement element;

        public PuzzleObjectFeedback[] feedbacks;

        public static bool operator ==(PuzzleObjectDesc x, PuzzleObjectDesc y)
        {
            return x.type == y.type;
        }

        public static bool operator !=(PuzzleObjectDesc x, PuzzleObjectDesc y)
        {
            return x.type != y.type;
        }

        public override bool Equals(object obj)
        {
            return type != ((PuzzleObjectDesc)obj).type;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [Serializable]
    public class PuzzleObject
    {
        private static float sensibility = 0.08f;
        private static float marginSecure = 0.5f;
        public int IDdesc;

        public PuzzleCell cell { get; set; }
        public PuzzleObjectDesc desc;
        public InteractivePuzzleElement interactive;
        public Action<PuzzleObject> OnCompleteAction;
        bool isInMovement;

        protected ConnexionPath current;
        private Transform parent;
        private bool locked = false;
        private float currentProgress;
        private Vector3 targetPos;

        public PuzzleObject(int pIDDesc)
        {
            isInMovement = false;
            IDdesc = pIDDesc;
        }

        public void AssociateInteractive(InteractivePuzzleElement pInteractive)
        {
            interactive = pInteractive;
            interactive._interactable = desc.interactiveByInput;
            interactive.onMoveComplete += OnMoveComplete;
            interactive.onMove += OnMove;

            parent = pInteractive.transform.parent;
            interactive.targetPos = cell.localPosition;
        }

        public void OnMoveComplete(float angle)
        {
            if (currentProgress > 0 && !isInMovement && desc.snapToCell)
            {
                if (currentProgress < 0.5f)
                    interactive.StartCoroutine(LerpOnPath(currentProgress, 0, current, cell.Orientation));
                else
                {
                    interactive.ResetStartPoint();
                    interactive.StartCoroutine(LerpOnPath(currentProgress, 1, current, cell.Orientation));
                    ChangeCell(current.destCell);
                }
            }

            if (OnCompleteAction != null)
                OnCompleteAction.Invoke(this);
        }

        void OnMove(Vector2 screenPoint, Vector2 startPos)
        {
            if (!GetCamera() || !desc.interactiveByInput || isInMovement)
                return;

            if (!locked && current == null)
            {
                SelectGoodPath(screenPoint, startPos);


            }
            else if (current != null)
            {
                locked = true;
                MoveAlongPath(screenPoint);
            }
        }

        Camera GetCamera()
        {
            Camera cam = null;

            if (GameManager.Instance && GameManager.Instance.CamerasMgr)
                cam = GameManager.Instance.CamerasMgr.GetCurrentCamera();

            if (cam == null)
                cam = Camera.current;

            return cam;
        }

        private Vector3 vel;

        void MoveAlongPath(Vector2 screenPoint)
        {
            ConnexionPath path = current;
            BezierCurve b = path.bezierPath;

            Camera cam = GetCamera();

            if (cam == null)
                return;

            Vector3 p1 = cam.WorldToScreenPoint(parent.position + parent.rotation * path.bezierPath.point1);
            Vector3 p2 =
                cam.WorldToScreenPoint(parent.position + parent.rotation * path.bezierPath.curvePoint1);
            Vector3 p3 =
                cam.WorldToScreenPoint(parent.position + parent.rotation * path.bezierPath.curvePoint2);
            Vector3 p4 = cam.WorldToScreenPoint(parent.position + parent.rotation * path.bezierPath.point2);


            double pos = PuzzleMath.getClosestPointToCubicBezier(screenPoint.x, screenPoint.y, 25, 10, p1.x, p1.y, p2.x,
                p2.y, p3.x, p3.y, p4.x, p4.y);

            pos *= 100;
            pos = Mathf.Round((float)pos) / 100;

            Vector3 posToGo = PuzzleMath.Casteljau(b.point1, b.curvePoint1, b.curvePoint2, b.point2, (float)pos);

            Vector3 worldPostoGo = parent.position + parent.rotation * posToGo;

            if (!interactive.TestCollisionsWithOther(worldPostoGo))
            {
                currentProgress = (float)pos;
                if (pos < 0.01f)
                {
                    pos = 0;
                    locked = false;
                    current = null;
                }
                else if (pos > 1 - sensibility)
                {
                    ChangeCell(current.destCell);
                    interactive.ResetStartPoint();
                    pos = 1;
                }

                interactive.targetPos = PuzzleMath.Casteljau(b.point1, b.curvePoint1, b.curvePoint2, b.point2, (float)pos);
            }
        }

        void SelectGoodPath(Vector2 screenPoint, Vector2 startPos)
        {
            Camera cam = GetCamera();
            Vector2 cellCenter = cam.WorldToScreenPoint(parent.position + parent.rotation * cell.localPosition);

            current = null;
            float selectedAngle = 360;

            foreach (var path in cell.paths)
            {
                BezierCurve b = path.bezierPath;
                Vector3 posStart = PuzzleMath.Casteljau(b.point1, b.curvePoint1, b.curvePoint2, b.point2, (float)sensibility);
                Vector2 p1 = cam.WorldToScreenPoint(parent.position + parent.rotation * posStart);

                float angleDelta = Vector2.Angle(cellCenter - p1, startPos - screenPoint);

                if (angleDelta < selectedAngle && angleDelta < path.angleSpread)
                {
                    selectedAngle = angleDelta;
                    current = path;

                }
            }
        }

        public bool TryMove(float angle, PuzzleObject origin = null)
        {
            if (isInMovement)
                return false;

            if (origin == null)
                origin = this;


            foreach (var path in cell.paths)
            {
                if (PuzzleMath.IsInAngleSpread(angle, path.angleMedian, path.angleSpread))
                {
                    return MoveAt(path, angle, origin);
                }
            }

            return false;
        }

        public bool MoveAt(ConnexionPath path, float angle, PuzzleObject origin)
        {
            List<PuzzleObject> l = path.destCell.content;

            for (int i = l.Count - 1; i >= 0; i--)
            {
                if (PuzzleMath.IDIsInMask(IDdesc, l[i].desc.canBlock))
                {
                    if (PuzzleMath.IDIsInMask(IDdesc, l[i].desc.canBePushedBy) && origin != l[i])
                    {
                        if (!l[i].TryMove(angle, origin))
                        {
                            //Debug.Log("Play Feedback Block : " + interactive.name);
                            PlayFeedback(3);

                            if (interactive != null)
                                interactive.StartCoroutine(WaitToPlayFeedbackIdle());
                            return false;
                        }
                    }
                    else
                    {
                        //Debug.Log("Play Feedback Block : " + interactive.name);
                        PlayFeedback(3);

                        if (interactive != null)
                            interactive.StartCoroutine(WaitToPlayFeedbackIdle());
                        return false;
                    }
                }
            }

            PuzzleCell precedent = cell;
            ChangeCell(path.destCell);

            if (interactive)
                interactive.StartCoroutine(LerpOnPath(0f, 1f, path, precedent.Orientation));

            return true;
        }

        void ChangeCell(PuzzleCell next)
        {
            cell.content.Remove(this);
            cell = next;
            cell.content.Add(this);

            currentProgress = 0;
            current = null;
            locked = false;

            if (OnCompleteAction != null && (!interactive || !interactive.IsTouching || !desc.interactiveByInput))
                OnCompleteAction.Invoke(this);
        }

        public bool IsValid()
        {
            if (!desc.isTrigger)
                return true;

            int numToValid = PuzzleMath.FlagToIntA(desc.triggerWith).Count;
            int count = 0;
            foreach (var obj in cell.content)
            {
                int layer = (int)Mathf.Pow(2, obj.IDdesc);

                if (layer == (desc.triggerWith & layer))
                {
                    count++;

                    //Debug.Log("Play Feedback Trigger : " + interactive.name);
                    PlayFeedback(2);
                    if (interactive != null)
                        interactive.StartCoroutine(WaitToPlayFeedbackIdle());
                }
            }

            return count >= numToValid;
        }

        IEnumerator LerpOnPath(float start, float to, ConnexionPath path, Quaternion baseQuaternion)
        {
            //Debug.Log("Play Feedback Move : " + interactive.name);
            PlayFeedback(1);

            isInMovement = true;

            float elapsedT = 0;

            float relativeSpeed = desc.speed * Mathf.Abs(start - to);
            BezierCurve b = path.bezierPath;

            while (elapsedT <= relativeSpeed)
            {
                float relativeProgress = Mathf.Lerp(start, to, elapsedT / desc.speed);
                interactive.transform.localPosition = PuzzleMath.Casteljau(b.point1, b.curvePoint1, b.curvePoint2, b.point2, relativeProgress);
                interactive.transform.localRotation = Quaternion.Lerp(baseQuaternion, path.destCell.Orientation, relativeProgress);

                elapsedT += Time.deltaTime;
                yield return null;
            }


            interactive.transform.localPosition =
                PuzzleMath.Casteljau(b.point1, b.curvePoint1, b.curvePoint2, b.point2, to);
            interactive.transform.localRotation = Quaternion.Lerp(baseQuaternion, path.destCell.Orientation, to);
            interactive.targetPos = interactive.transform.localPosition;

            isInMovement = false;

            //Debug.Log("Play Feedback Idle : " + interactive.name);
            PlayFeedback(0);
        }

        public void PlayFeedback(int idFeed)
        {
            if (interactive != null)
            {
                if (interactive.GetComponent<AudioSource>() != null)
                    interactive.GetComponent<AudioSource>().clip = desc.feedbacks[idFeed].audioClip;

                if (interactive.GetComponent<Animation>() != null)
                {
                    interactive.GetComponent<Animation>().clip = desc.feedbacks[idFeed].animationClip;
                    interactive.GetComponent<Animation>().Play();
                }
            }
        }

        private IEnumerator WaitToPlayFeedbackIdle()
        {
            yield return new WaitWhile(() => interactive.GetComponent<Animation>().isPlaying);
            PlayFeedback(0);
        }
    }
}