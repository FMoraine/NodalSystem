using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace NodalInteractiveCreator.Screwdriver
{
    public class ScrewdriverHead : MonoBehaviour
    {
        [System.Serializable]
        public class SubHead
        {
            public Image Image = null;
            public GameObject Pivot = null;

            public AnimationCurve RotationCurves = null;
            public AnimationCurve TranslationCurves = null;
        }

        /*
         * 
         */

        public List<Sprite> Forms = new List<Sprite>();
        public List<SubHead> SubHeads = new List<SubHead>();

        private int formIndex = 0;
        private float rotationValue = 0.0f;
        private float translationValue = 0.0f;

        private void Start()
        {
            UpdateImages();
        }

        public void ChangeForm(bool NextForm)
        {
            UpdateIndex(NextForm);

            UpdateImages();
        }

        private void UpdateIndex(bool NextForm)
        {
            formIndex = (true == NextForm) ? (formIndex + 1) : (formIndex - 1);
            if (formIndex >= Forms.Count)
            {
                formIndex = 0;
            }
            else if (formIndex < 0)
            {
                formIndex = Forms.Count - 1;
            }
        }

        private void UpdateImages()
        {
            if (formIndex >= 0 && formIndex < Forms.Count)
            {
                Sprite currentSprite = Forms[formIndex];

                foreach (SubHead head in SubHeads)
                {
                    if (null != head && null != head.Image)
                    {
                        head.Image.sprite = currentSprite;
                    }
                }
            }
        }

        public void Rotate(float Step)
        {
            rotationValue += Step;

            foreach (SubHead head in SubHeads)
            {
                if (null != head && null != head.Pivot)
                {
                    float angle = Mathf.Sign(rotationValue) * head.RotationCurves.Evaluate(Mathf.Abs(rotationValue));

                    head.Pivot.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }
            }
        }

        public void Translate(float Step)
        {
            translationValue += Step;

            foreach (SubHead head in SubHeads)
            {
                if (null != head && null != head.Image)
                {
                    float YPosition = Mathf.Sign(translationValue) * head.TranslationCurves.Evaluate(Mathf.Abs(translationValue));

                    head.Image.transform.localPosition = new Vector3(0.0f, YPosition, 0.0f);
                }
            }
        }

        public int GetFormIndex()
        {
            return formIndex;
        }
    }


    /*
    *
    */

#if UNITY_EDITOR

    [CustomEditor(typeof(ScrewdriverHead))]
    public class ScrewdriverHeadEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying)
            {
                ChangeFormButtons();

                RotateButtons();

                TranslateButtons();
            }
        }

        private void ChangeFormButtons()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Prev Form"))
            {
                ScrewdriverHead head = target as ScrewdriverHead;
                if (null != head)
                {
                    head.ChangeForm(false);
                }
            }
            if (GUILayout.Button("Next Form"))
            {
                ScrewdriverHead head = target as ScrewdriverHead;
                if (null != head)
                {
                    head.ChangeForm(true);
                }
            }

            GUILayout.EndHorizontal();
        }

        private void RotateButtons()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Rotation -90"))
            {
                ScrewdriverHead head = target as ScrewdriverHead;
                if (null != head)
                {
                    head.Rotate(-90.0f);
                }
            }
            if (GUILayout.Button("Rotation +90"))
            {
                ScrewdriverHead head = target as ScrewdriverHead;
                if (null != head)
                {
                    head.Rotate(90.0f);
                }
            }

            GUILayout.EndHorizontal();
        }

        private void TranslateButtons()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Translation -90"))
            {
                ScrewdriverHead head = target as ScrewdriverHead;
                if (null != head)
                {
                    head.Translate(-90.0f);
                }
            }
            if (GUILayout.Button("Translation +90"))
            {
                ScrewdriverHead head = target as ScrewdriverHead;
                if (null != head)
                {
                    head.Translate(90.0f);
                }
            }

            GUILayout.EndHorizontal();
        }
    }
#endif
}