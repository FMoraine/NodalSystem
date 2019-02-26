using UnityEngine;
using System.Collections;
using Machinika.Screwdriver;

namespace NodalInteractiveCreator.HUD
{
    public class ScrewdriverImage : DraggableImage
    {
        private Ray ray;
        private RaycastHit raycastHit;
        private Screw screw = null;

        protected override void OnMovement(Vector3 InputPosition)
        {
            base.OnMovement(InputPosition);

            if (true == IsDragging() && null != mainCamera)
            {
                ray = mainCamera.ScreenPointToRay(InputPosition);

                if (true == Physics.Raycast(ray, out raycastHit))
                {
                    screw = raycastHit.collider.GetComponent<Screw>();
                }
                else
                {
                    screw = null;
                }

                if (null != currentDropSlot)
                {
                    if (null != screw)
                    {
                        currentDropSlot.ChangeAlpha(1.0f);
                    }
                    else
                    {
                        currentDropSlot.ChangeAlpha(0.5f);
                    }
                }
            }
        }

        protected override void OnDrag()
        {
            base.OnDrag();

            screw = null;
            currentImage.enabled = false;

            if (null != currentDropSlot)
            {
                currentDropSlot.ChangeAlpha(0.5f);
            }
        }

        protected override void OnDeselect()
        {
            base.OnDeselect();

            currentImage.enabled = true;

            if (null != currentDropSlot)
            {
                currentDropSlot.ChangeAlpha(0.0f);
            }
            if (null != screw)
            {
                screw.ExecuteTrigger();
            }
        }
    }
}