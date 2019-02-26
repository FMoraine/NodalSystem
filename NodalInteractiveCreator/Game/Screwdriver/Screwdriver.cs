// Machinika Museum Prototype v.01
// © Littlefield Studio
// Writted by Rémi Carreira - 2016
//
// Screwdriver.cs


using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace NodalInteractiveCreator.Screwdriver
{
    public class Screwdriver : MonoBehaviour
    {
        [System.Serializable]
        public class Wheels
        {
            public ScrewdriverFormWheel FormWheel = null;
            public ScrewdriverRotateWheel RotateWheel = null;
            public ScrewdriverTranslateWheel TranslateWheel = null;
        }

        public Wheels FirstHeadWheels = null;
        public Wheels SecondHeadWheels = null;

        private static Screwdriver Instance = null;

        private Transform selfTransform = null;

        private void Awake()
        {
            Screwdriver.Instance = this;

            selfTransform = this.transform;

            gameObject.SetActive(false);
        }

        public bool CheckScrewdriver(Screw Screw)
        {
            if (null != Screw && null != FirstHeadWheels && null != SecondHeadWheels)
            {
                return (true == CheckForm(Screw.FirstForm, FirstHeadWheels) && true == CheckForm(Screw.SecondForm, SecondHeadWheels));
            }
            return false;
        }

        private bool CheckForm(Screw.Form Form, Wheels Wheels)
        {
            if (null != Form && null != Wheels)
            {
                if (null != Wheels.FormWheel && Wheels.FormWheel.GetCurrentFace() == Form.FormIndex)
                {
                    if (null != Wheels.TranslateWheel && Wheels.TranslateWheel.GetCurrentToothIndex() == Form.TranslateIndex)
                    {
                        if (null != Wheels.RotateWheel && Wheels.RotateWheel.GetCurrentToothIndex() == Form.RotateIndex)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void SetPosition(Vector3 Position)
        {
            selfTransform.position = Position;
        }

        public void LookAt(Vector3 Target)
        {
            selfTransform.LookAt(Target);
        }

        public void Move(Vector3 From, Vector3 To)
        {
            if (true == gameObject.activeSelf)
            {
                StartCoroutine(LerpMove(From, To));
            }
        }

        private IEnumerator LerpMove(Vector3 From, Vector3 To)
        {
            float timer = 0;

            while (timer < 1.0f)
            {
                timer += Time.deltaTime;

                selfTransform.position = Vector3.Lerp(From, To, timer / 1.0f);

                yield return null;
            }
        }

        public static Screwdriver GetInstance()
        {
            return Screwdriver.Instance;
        }
    }
}