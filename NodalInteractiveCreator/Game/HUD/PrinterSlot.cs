// Machinika Museum
// © Littlefield Studio
// Writted by Rémi Carreira - 22/02/2016
//
// PrinterSlot.cs

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using NodalInteractiveCreator.Inventory;
using UnityEngine.EventSystems;

namespace NodalInteractiveCreator.HUD
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(LerpImageOpacity))]
    [RequireComponent(typeof(LerpImageFilling))]
    public class PrinterSlot : ToolSlot
    {
        public enum Mode : byte
        {
            Normal = 0,
            Print,
            Scan
        }

        [Header("Duration")]
        public float ScanDuration = 2.5f;
        public float PrintDuration = 2.5f;
        public float TranslateDuration = 0.75f;

        [Header("Image")]
        public Image PrintImage = null;
        public Image ScanImage = null;
        public GameObject HolderFeedback = null;

        public Dictionary<int, byte> itemsDuplicated = new Dictionary<int, byte>();

        private Mode mode = Mode.Normal;
        private LerpImageOpacity lerpOpacity = null;
        private LerpImageFilling lerpFilling = null;
        private Vector3 originePos = Vector3.zero;
        private ItemSlot itemSlot;

        private static bool _once = false;

        protected override void Awake()
        {
            base.Awake();
            lerpOpacity = GetComponent<LerpImageOpacity>();
            lerpFilling = GetComponent<LerpImageFilling>();
        }

        protected override void Start()
        {
            base.Start();
            SetMode(Mode.Normal);
        }

        protected override void OnSelect(Vector3 InputPosition)
        {
            if (!_once)
            {
                if (Application.systemLanguage == SystemLanguage.French)
                    DisplayMessage("Je peux maintenant me servir de l'imprimante pour dupliquer certains objets !");
                else
                    DisplayMessage("English_Sentence");

                _once = true;
            }
            else
                base.OnSelect(InputPosition);
        }

        protected override void OnDrag()
        {
            base.OnDrag();
        }

        protected override void OnMovement(Vector3 InputPosition)
        {
            base.OnMovement(InputPosition);

            if (true == IsDragging() && null != mainCamera)
            {
                RaycastCanvas(InputPosition);

                foreach (RaycastResult result in hits)
                {
                    itemSlot = result.gameObject.GetComponent<ItemSlot>();
                    if (null != itemSlot)
                    {
                        currentDropSlot.ChangeAlpha(1.0f);
                        currentDropSlot.ChangeScale(2.5F);
                        break;
                    }
                    else
                    {
                        currentDropSlot.ChangeAlpha(0.5f);
                        currentDropSlot.ChangeScale(2.5F);
                    }
                }
            }
        }

        protected override void OnDeselect()
        {
            base.OnDeselect();

            if (null != itemSlot)
            {
                DuplicateItem(itemSlot.currentItemId);
            }
        }

        public bool DuplicateItem(int ItemId)
        {
            if (false == gameObject.activeSelf || null == Inventory.Inventory.Instance)
                return false;

            if (!_once) _once = true;

            if (Mode.Normal != mode)
            {
                if (Mode.Scan == mode)
                {
                    if (Application.systemLanguage == SystemLanguage.French)
                        DisplayMessage("L'imprimante est déjà en train de scanner...");
                    else
                        DisplayMessage("English_Sentence");

                }
                else
                {
                    if (Application.systemLanguage == SystemLanguage.French)
                        DisplayMessage("L'imprimante est déjà en train d'imprimer...");
                    else
                        DisplayMessage("English_Sentence");
                }
                return false;
            }

            ItemData item = Inventory.Inventory.Instance.GetItem(ItemId);
            if (null == item || 1 == item._numberMax || !item._printable)
            {
                if (Application.systemLanguage == SystemLanguage.French)
                    DisplayMessage("Cet objet ne peut pas être dupliqué.");
                else
                    DisplayMessage("English_Sentence");
                return false;
            }

            if (itemsDuplicated.ContainsKey(ItemId))
            {
                if (item._numberMax == itemsDuplicated[ItemId] + 1)
                {
                    if (Application.systemLanguage == SystemLanguage.French)
                        DisplayMessage("Cet objet à déjà été dupliqué.");
                    else
                        DisplayMessage("English_Sentence");
                    return false;
                }
                else
                    itemsDuplicated[ItemId] += 1;
            }
            else
            {
                itemsDuplicated.Add(ItemId, 1);
            }

            ItemSlot[] items = FindObjectsOfType<ItemSlot>();

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].currentItemId == ItemId)
                {
                    originePos = items[i].gameObject.GetComponent<RectTransform>().position;
                    HolderFeedback.GetComponent<Image>().sprite = items[i].GetComponent<Image>().sprite;
                }
            }

            StartCoroutine(DuplicateItemCoroutine(ItemId));
            return true;
        }

        private IEnumerator DuplicateItemCoroutine(int ItemId)
        {
            SetMode(Mode.Scan);
            yield return StartCoroutine(AnimateScanImage());

            SetMode(Mode.Print);
            yield return StartCoroutine(AnimatePrintImage());

            SetMode(Mode.Normal);
            yield return StartCoroutine(TranslateDuplicataToOrignal());

            if (null != Inventory.Inventory.Instance)
            {
                Inventory.Inventory.Instance.AddItem(ItemId);
            }
        }

        private IEnumerator AnimateScanImage()
        {
            float LerpStep = ScanDuration / 6f;

            for (byte count = 0; count < 3; ++count)
            {
                //Lerp 0 -> 1
                yield return StartCoroutine(lerpOpacity.Lerp(ScanImage, 0.0f, 1.0f, LerpStep));

                //Lerp 1 -> 0
                yield return StartCoroutine(lerpOpacity.Lerp(ScanImage, 1.0f, 0.0f, LerpStep));
            }
        }

        private IEnumerator AnimatePrintImage()
        {
            yield return StartCoroutine(lerpFilling.LerpVertically(PrintImage, 0f, 1f, PrintDuration));
        }

        private IEnumerator TranslateDuplicataToOrignal()
        {
            float time = 0;
            Vector3 printerPos = GetComponent<RectTransform>().position;

            HolderFeedback.SetActive(true);

            while (time < 1)
            {
                HolderFeedback.GetComponent<RectTransform>().position = Vector2.Lerp(printerPos, originePos, time);
                time += Time.deltaTime * TranslateDuration;

                yield return null;
            }

            HolderFeedback.SetActive(false);
        }

        private void DisplayMessage(string Message)
        {
            if (0 != Message.Length)
            {
                InfoBox Box = HUDManager.GetInfoBox();
                if (null != Box)
                {
                    Box.DisplayInfo(Message);
                }
            }
        }

        private void SetMode(Mode NewMode)
        {
            mode = NewMode;

            if (null != PrintImage)
            {
                PrintImage.gameObject.SetActive(Mode.Print == mode);
            }
            if (null != ScanImage)
            {
                ScanImage.gameObject.SetActive(Mode.Scan == mode);
            }
        }

        public bool IsPrinting()
        {
            return (Mode.Print == mode);
        }

        public bool IsScanning()
        {
            return (Mode.Scan == mode);
        }
    }
}