// Machinika Museum Prototype v.01
// © Littlefield Studio
// Writted by Rémi Carreira - 2015
//
// FadeImage.cs

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace NodalInteractiveCreator.HUD
{
    public class FadeImage : MonoBehaviour
    {
        public Image ImageToFade = null;
        public float FadeInTime = 1.0f;
        public float FadeOutTime = 1.0f;

        private bool play = false;
        private bool fadeIn = false;
        private float time = 0.0f;

        private Color inColor = Color.black;
        private Color outColor = Color.black;

        void Update()
        {
            if (true == play && null != ImageToFade)
            {
                UpdateColors();
                UpdateFade();

                time -= Time.deltaTime;
            }
        }

        void UpdateColors()
        {
            inColor = ImageToFade.color;
            inColor.a = 0.0f;

            outColor = ImageToFade.color;
            outColor.a = 1.0f;
        }

        void UpdateFade()
        {
            if (true == fadeIn)
            {
                UpdateFadeIn();

                if (ImageToFade.color.a <= 0.0f)
                    play = false;
            }
            else
            {
                UpdateFadeOut();

                if (ImageToFade.color.a >= 1.0f)
                    play = false;
            }
        }

        void UpdateFadeIn()
        {
            ImageToFade.color = Color.Lerp(outColor, inColor, (FadeInTime - time) / FadeInTime);
        }

        void UpdateFadeOut()
        {
            ImageToFade.color = Color.Lerp(inColor, outColor, (FadeOutTime - time) / FadeOutTime);
        }

        public void PlayFade(bool In)
        {
            play = true;
            fadeIn = In;

            if (true == fadeIn)
                time = FadeInTime;
            else
                time = FadeOutTime;
        }

        public bool IsPlaying()
        {
            return play;
        }
    }
}