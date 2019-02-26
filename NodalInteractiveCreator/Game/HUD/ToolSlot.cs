using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using NodalInteractiveCreator.HUD;

namespace NodalInteractiveCreator.Inventory
{
    public class ToolSlot : DraggableImage
    {
        protected Ray ray;
        protected RaycastHit raycastHit;

        protected PointerEventData pointerEventData = null;
        protected List<RaycastResult> hits = new List<RaycastResult>();

        private int nbItem = 1;

        protected override void Awake()
        {
            base.Awake();
        }

        protected virtual void Start()
        {
            pointerEventData = new PointerEventData(EventSystem.current);
        }

        protected override void OnSelect(Vector3 InputPosition)
        {
            base.OnSelect(InputPosition);
        }

        protected override void OnDrag()
        {
            base.OnDrag();

            currentImage.enabled = (1 < nbItem);

            --nbItem;

            currentDropSlot.ChangeAlpha(0.5f);
            currentDropSlot.ChangeScale(2.5f);
        }


        protected override void OnMovement(Vector3 InputPosition)
        {
            base.OnMovement(InputPosition);
        }

        protected override void OnDeselect()
        {
            if (true == IsDragging())
            {
                currentImage.enabled = true;
                ++nbItem;
            }

            base.OnDeselect();
        }

        protected void RaycastCanvas(Vector3 InputPosition)
        {
            GraphicRaycaster raycaster = HUDManager.GetGraphicRaycaster();
            if (null != raycaster)
            {
                hits.Clear();
                pointerEventData.position = new Vector2(InputPosition.x, InputPosition.y);
                raycaster.Raycast(pointerEventData, hits);
            }
        }
    }
}