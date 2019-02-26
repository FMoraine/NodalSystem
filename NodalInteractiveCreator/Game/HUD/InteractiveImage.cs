using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace NodalInteractiveCreator.HUD
{
    [RequireComponent(typeof(Image))]
    public class InteractiveImage : InteractiveCanvas
    {
        protected Image currentImage = null;

        protected override void Awake()
        {
            base.Awake();

            currentImage = GetComponent<Image>();
        }
    }
}