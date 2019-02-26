using UnityEngine;
using System.Collections;

namespace NodalInteractiveCreator.HUD
{
    public class InteractiveCanvas : MonoBehaviour
    {
        protected bool selected = false;
        protected bool onCanvas = false;
        protected bool alwaysOnCanvas = false;

        protected Camera mainCamera = null;
        protected Transform canvasTransform = null;

        protected float selectedTime = 0.0f;
        protected float deselectedTime = 0.0f;

        protected virtual void Awake()
        {
            canvasTransform = this.transform;
        }

        public void Select(Camera MainCamera, Vector3 InputPosition)
        {
            selected = true;
            alwaysOnCanvas = true;
            mainCamera = MainCamera;

            selectedTime = Time.timeSinceLevelLoad;
            deselectedTime = 0.0f;

            OnSelect(InputPosition);
        }

        public void EnterCanvas()
        {
            onCanvas = true;
        }

        public void ExitCanvas()
        {
            onCanvas = false;
            alwaysOnCanvas = false;
        }

        public void Move(Vector3 InputPosition)
        {
            OnMovement(InputPosition);
        }

        public void Deselect()
        {
            selected = false;
            deselectedTime = Time.timeSinceLevelLoad;

            OnDeselect();
        }

        protected virtual void OnSelect(Vector3 InputPosition)
        {
        }

        protected virtual void OnMovement(Vector3 InputPosition)
        {
        }

        protected virtual void OnDeselect()
        {
        }

        public bool IsSelected()
        {
            return selected;
        }

        public bool IsOnCanvas()
        {
            return onCanvas;
        }
    }
}