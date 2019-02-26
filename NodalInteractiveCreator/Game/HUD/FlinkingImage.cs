// Machinika Museum Prototype v.01
// © Littlefield Studio
// Writted by Rémi Carreira - 2015
//
// FlinkingImage.cs

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace NodalInteractiveCreator.HUD
{
    public class FlinkingImage : MonoBehaviour
    {
        public Image ImageToFlink = null;
        public float FlinkingTime = 0.5f;

        private bool play = false;
        private float time = 0.0f;

        void Update()
        {
            if (true == play)
            {
                if (time <= 0.0f && null != ImageToFlink)
                {
                    ImageToFlink.enabled = !ImageToFlink.enabled;
                    time = FlinkingTime;
                }

                time -= Time.deltaTime;
            }
        }

        public void Play()
        {
            play = true;
            time = FlinkingTime;
        }

        public void Stop(bool display)
        {
            play = false;

            if (null != ImageToFlink)
                ImageToFlink.enabled = display;
        }

        public bool IsPlaying()
        {
            return play;
        }
    }
}