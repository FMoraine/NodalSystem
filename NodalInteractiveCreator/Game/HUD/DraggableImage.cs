using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace NodalInteractiveCreator.HUD
{
    public class DraggableImage : InteractiveImage
    {
        public static DropSlot currentDropSlot = null;

        private static Vector3 universalDragOffset = new Vector3(0.0f, 0.0f, 0.0f);
        private static float universalDragTime = 0.075f;
        private float currentDragTime = 0.0f;
        private bool dragging = false;

        private Vector3 lastInputPosition = Vector3.zero;
        private Vector3 currentInputPosition = Vector3.zero;
        private Vector3 inputOffset = Vector3.zero;

        protected virtual void Update()
        {
            if (true == selected && false == dragging && true == alwaysOnCanvas && !Inventory.InspectSystem.IsInspecting())
            {
                if (currentDragTime > universalDragTime)
                {
                    dragging = true;
                    OnDrag();
                }

                currentDragTime += Time.deltaTime;
            }
        }

        protected virtual void OnDrag()
        {
            if (null != currentDropSlot)
            {
                currentDropSlot.SetSlotImage(currentImage.sprite);

                inputOffset = currentInputPosition - canvasTransform.position;

                currentDropSlot.SetSlotPosition(canvasTransform.position + inputOffset + universalDragOffset);
            }
        }

        protected override void OnSelect(Vector3 InputPosition)
        {
            dragging = false;
            currentDragTime = 0.0f;

            lastInputPosition = InputPosition;
            currentInputPosition = lastInputPosition;
            inputOffset = Vector3.zero;
        }

        protected override void OnMovement(Vector3 InputPosition)
        {
            if (true == dragging && null != currentDropSlot)
            {
                lastInputPosition = currentInputPosition;
                currentInputPosition = InputPosition;

                currentDropSlot.MoveSlot(currentInputPosition - lastInputPosition);
            }
        }

        protected override void OnDeselect()
        {
            if (true == dragging && null != currentDropSlot)
            {
                currentDropSlot.SetSlotImage(null);
                dragging = false;
            }
        }

        public bool IsDragging()
        {
            return dragging;
        }
    }
}