// Machinika Museum
// © Littlefield Studio
// Writted by Rémi Carreira - 22/02/2016
//
// LerpImageOpacity.cs

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace NodalInteractiveCreator.HUD
{
    public class LerpImageOpacity : MonoBehaviour
    {
        public IEnumerator Lerp(Image SrcImage, float StartOpacity, float FinalOpacity, float Duration)
        {
            if (null != SrcImage)
            {
                if (StartOpacity >= 0f && StartOpacity <= 1f && FinalOpacity >= 0f && FinalOpacity <= 1f && Duration >= 0f)
                {
                    float timer = 0.0f;
                    Color color = SrcImage.color;

                    while (timer <= Duration)
                    {
                        color.a = Mathf.Lerp(StartOpacity, FinalOpacity, timer / Duration);
                        SrcImage.color = color;
                        yield return null;
                        timer += Time.deltaTime;
                    }

                    color.a = FinalOpacity;
                    SrcImage.color = color;
                }
            }
        }
    }
}