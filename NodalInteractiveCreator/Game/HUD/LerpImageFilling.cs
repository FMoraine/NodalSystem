using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace NodalInteractiveCreator.HUD
{
    public class LerpImageFilling : MonoBehaviour
    {
        public IEnumerator LerpVertically(Image SrcImage, float StartValue, float FinalValue, float Duration)
        {
            if (null != SrcImage)
            {
                if (StartValue >= 0f && StartValue <= 1f && FinalValue >= 0f && FinalValue <= 1f && Duration >= 0f)
                {
                    SrcImage.type = Image.Type.Filled;
                    SrcImage.fillMethod = Image.FillMethod.Vertical;

                    float timer = 0.0f;

                    while (timer <= Duration)
                    {
                        SrcImage.fillAmount = Mathf.Lerp(StartValue, FinalValue, timer / Duration);
                        yield return null;
                        timer += Time.deltaTime;
                    }
                }
            }
        }
    }
}