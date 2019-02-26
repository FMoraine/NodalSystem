using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NodalInteractiveCreator.Endoscop
{
    public class EndoscopePad : MonoBehaviour
    {
        public static EndoscopePad current;

        [SerializeField] private Animator animController;
        [SerializeField] private GameObject padGFXRoot;
        [SerializeField] private int animIndex = 0;
        [SerializeField] private Joystick3D moveJoystick;
        [SerializeField] private Joystick3D cameraRotateJoystick;

        private EndoscopeScene displayScene;

        void Awake()
        {
            if (EndoscopeEntrence.OnEntrenceActivated != null)
                EndoscopeEntrence.OnEntrenceActivated = null;

            EndoscopeEntrence.OnEntrenceActivated += Open;
            padGFXRoot.SetActive(false);
        }

        void Open(EndoscopeScene scene)
        {
            current = this;
            displayScene = scene;

            animController.Play("Open_" + animIndex, 0);
            padGFXRoot.SetActive(true);
            displayScene.Activate(true);

        }

        public void Close()
        {
            if (current == this)
                current = null;
            animController.Play("Close_" + animIndex, 0);
            EndoscopeEntrence.OnEntrenceActivated -= Open;
        }

        void Update()
        {
            if (displayScene)
            {
                displayScene.ControlCameraMovement(moveJoystick.inputValue.y);
                displayScene.ControlCameraOrientation(cameraRotateJoystick.inputValue);
            }
        }

        public void OnEndClose()
        {
            displayScene.Activate(false);
            padGFXRoot.SetActive(false);
            EndoscopeEntrence.OnEntrenceActivated += Open;

        }

        public void OnEndOpen()
        {

        }
    }
}
