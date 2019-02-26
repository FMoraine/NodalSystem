using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace NodalInteractiveCreator.HUD
{
    public class BasicTextDisplayPattern : MonoBehaviour
    {
        public float ElapsedTime = 0.1f;

        //Number letter display
        public int _speed = 1;

        private Text uiText = null;
        private bool playing = false;
        private string sentence = "";
        private int letterIndex = 0;
        private float timer = 0.0f;

        /// <summary>
        /// Start pattern
        /// </summary>
        /// <param name="UI">Text where pattern is executed</param>
        /// <param name="Sentence">String to display</param>
        /// <returns>Return true if can execute this pattern</returns>
        public bool ExecutePattern(Text UI, string Sentence)
        {
            if (null != UI)
            {
                uiText = UI;
                uiText.text = "";
                sentence = Sentence;
                playing = true;
                letterIndex = 0;
                timer = 0.0f;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if the pattern is playing
        /// </summary>
        /// <returns></returns>
        public bool IsPlaying()
        {
            return playing;
        }

        private void LateUpdate()
        {
            if (true == playing)
            {
                timer -= Time.deltaTime;

                if (timer <= 0.0f)
                {
                    if (letterIndex < sentence.Length)
                    {
                        for (int i = 0; i < _speed; i++)
                        {
                            if (letterIndex + i < sentence.Length)
                                uiText.text += sentence[letterIndex + i];
                        }

                        timer += ElapsedTime;
                        letterIndex += 1 * _speed;
                    }
                    else
                    {
                        playing = false;
                    }
                }
            }
        }
    }
}