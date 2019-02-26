using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace NodalInteractiveCreator.HUD
{
    [RequireComponent(typeof(Image))]
    public class DropSlot : MonoBehaviour
    {
        private Image currentImage = null;
        private RectTransform currentRect = null;

        void Awake()
        {
            currentImage = GetComponent<Image>();
            currentRect = GetComponent<RectTransform>();

            DraggableImage.currentDropSlot = this;

            gameObject.SetActive(false);
        }

        public void SetSlotImage(Sprite Icon)
        {
            if (null != currentImage)
            {
                currentImage.sprite = Icon;

                gameObject.SetActive((null != currentImage.sprite));
            }
        }

        public void SetSlotPosition(Vector3 Position)
        {
            if (null != currentRect)
            {
                currentRect.position = Position;
            }
        }

        public void MoveSlot(Vector3 Movement)
        {
            if (null != currentRect)
            {
                currentRect.position += Movement;
            }
        }

        public void ChangeAlpha(float Alpha)
        {
            if (null != currentImage)
            {
                Color currentColor = currentImage.color;
                currentColor.a = Alpha;
                currentImage.color = currentColor;
            }
        }

        public void ChangeScale(float size)
        {
            if (null != currentImage)
            {
                currentRect.localScale = new Vector3(size, size, size);
            }
        }
    }
}