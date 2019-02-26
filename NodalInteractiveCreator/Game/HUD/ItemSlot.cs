// Machinika Museum Prototype v.01
// © Littlefield Studio
// Writted by Rémi Carreira - 2015
//
// ItemSlot.cs

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using NodalInteractiveCreator.Objects;
using NodalInteractiveCreator.Endoscop;
using NodalInteractiveCreator.HUD;

namespace NodalInteractiveCreator.Inventory
{
    public class ItemSlot : DraggableImage
    {
        private static float InspectTime = 0.3f;

        public Image DigitImage = null;

        public int currentItemId = -1;
        public int nbItem = -1;

        public bool alwaysAvailable = false;
        public bool workWithOtherTools = true;
        public bool canbeUseAsEndoscope = false;
        public bool asInspectorPannel = true;

        private Ray ray;
        private RaycastHit raycastHit;
        private ItemTarget trigger = null;
        private PrinterSlot printer = null;

        private PointerEventData pointerEventData = null;
        private List<RaycastResult> hits = new List<RaycastResult>();

        void Start()
        {
            pointerEventData = new PointerEventData(EventSystem.current);

            if (canbeUseAsEndoscope)
                SetItemId(-2);

            UpdateItemImage();
            UpdateDigitImage();
        }

        void UpdateItemImage()
        {
            if (alwaysAvailable)
                return;

            currentImage.sprite = null;
            currentImage.gameObject.SetActive(false);

            if (-1 != currentItemId)
            {
                if (null != Inventory.Instance && null != Inventory.Instance.ItemDatabase)
                {
                    ItemData data = Inventory.Instance.ItemDatabase.FindItem(currentItemId);
                    if (null != data)
                    {
                        currentImage.sprite = data._iconInventory;
                        currentImage.gameObject.SetActive(true);

                        currentImage.enabled = true;
                    }
                }
            }
        }

        void UpdateDigitImage()
        {
            if (alwaysAvailable)
                return;

            if (null != DigitImage)
            {
                DigitImage.sprite = null;
                DigitImage.gameObject.SetActive(false);

                if (1 < nbItem)
                {
                    if (null != Inventory.Instance && null != Inventory.Instance.DigitDatabase)
                    {
                        DigitData data = Inventory.Instance.DigitDatabase.FindDigit(nbItem);
                        if (null != data)
                        {
                            DigitImage.sprite = data._icon;
                            DigitImage.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }

        protected override void OnDrag()
        {
            if (EndoscopeEntrence.IsActive() && canbeUseAsEndoscope)
                return;

            base.OnDrag();

            currentImage.enabled = (1 < nbItem);

            --nbItem;
            UpdateDigitImage();

            currentDropSlot.ChangeAlpha(0.5f);
            currentDropSlot.ChangeScale(2.5f);
        }

        protected override void OnSelect(Vector3 InputPosition)
        {
            base.OnSelect(InputPosition);

            trigger = null;
            printer = null;
        }

        protected override void OnMovement(Vector3 InputPosition)
        {
            base.OnMovement(InputPosition);

            if (true == IsDragging() && null != mainCamera)
            {
                trigger = null;
                printer = null;

                RaycastCanvas(InputPosition);

                if (null == printer)
                {
                    RaycastObjects(InputPosition);
                    if (null == trigger)
                    {
                        currentDropSlot.ChangeAlpha(0.5f);
                        currentDropSlot.ChangeScale(2.5F);
                    }
                }
            }
        }

        private void RaycastCanvas(Vector3 InputPosition)
        {
            if (!workWithOtherTools)
                return;

            GraphicRaycaster raycaster = HUDManager.GetGraphicRaycaster();
            if (null != raycaster)
            {
                hits.Clear();
                pointerEventData.position = new Vector2(InputPosition.x, InputPosition.y);
                raycaster.Raycast(pointerEventData, hits);

                foreach (RaycastResult result in hits)
                {
                    printer = result.gameObject.GetComponent<PrinterSlot>();
                    if (null != printer)
                    {
                        currentDropSlot.ChangeAlpha(1.0f);
                        currentDropSlot.ChangeScale(2.5F);
                        break;
                    }
                }
            }
        }

        private void RaycastObjects(Vector3 InputPosition)
        {
            ray = mainCamera.ScreenPointToRay(InputPosition);

            if (true == Physics.Raycast(ray, out raycastHit))
            {
                trigger = raycastHit.collider.GetComponent<ItemTarget>();

                if (null != trigger)
                {
                    if (trigger.ItemId == currentItemId || (trigger._activeEndoscope && currentItemId == -2) || (trigger._activeScrewdriver && currentItemId == -1))
                    {
                        currentDropSlot.ChangeAlpha(1.0f);
                        currentDropSlot.ChangeScale(2.5F);
                    }
                }
            }
        }

        protected override void OnDeselect()
        {
            if (true == IsDragging())
            {
                currentImage.enabled = true;

                ++nbItem;
                UpdateDigitImage();

                if (null != printer)
                    printer.DuplicateItem(currentItemId);
                else if (null != trigger)
                {
                    if (trigger.ItemId == currentItemId)
                    {
                        trigger.ExecuteTrigger();

                        if (null != GetComponent<Animation>() && nbItem <= 0)
                            GetComponent<Animation>().enabled = true;
                    }
                    else if (trigger._activeEndoscope && currentItemId == -2)
                        trigger.ExecuteEndoscope();
                    else if (trigger._activeScrewdriver && currentItemId == -1)
                        trigger.ExecuteScrewdriver();
                }
            }

            if ((deselectedTime - selectedTime) < InspectTime && asInspectorPannel)
            {
                if (null != Inventory.Instance && null != Inventory.Instance.InspectSystem)
                {
                    if (true == InspectSystem.IsInspecting() && currentItemId == Inventory.Instance.InspectSystem.ItemInspecting())
                        Inventory.Instance.InspectSystem.CloseItemInspection();
                    else
                        Inventory.Instance.InspectSystem.LaunchItemInspection(currentItemId);
                }
            }

            base.OnDeselect();
        }

        public void SetItemId(int ItemId)
        {
            currentItemId = ItemId;

            UpdateItemImage();
        }

        public void SetNbItem(int Number)
        {
            nbItem = Number;

            UpdateDigitImage();
        }

        public int GetItemId()
        {
            return currentItemId;
        }

        public int GetNbItem()
        {
            return nbItem;
        }

        public bool IsUsed()
        {
            return (-1 != currentItemId && -1 != nbItem);
        }
    }
}