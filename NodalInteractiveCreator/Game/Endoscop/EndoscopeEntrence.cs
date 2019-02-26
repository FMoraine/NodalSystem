using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using NodalInteractiveCreator.Managers;
using NodalInteractiveCreator.HUD;
using NodalInteractiveCreator.Objects;

namespace NodalInteractiveCreator.Endoscop
{
    [RequireComponent(typeof(Collider))]
    public class EndoscopeEntrence : InteractiveObject
    {
        public EndoscopeScene linkedScene;
        public Transform gameObjectNoise;
        public static Action<EndoscopeScene> OnEntrenceActivated;

        [SerializeField] private Animator EndoscopeAnimator;
        public int animIndex = 0;

        private static bool isActive = false;

        private Vector3 startPos;
        private Vector2 noiseProgress;
        public float noisePower = 1f;
        public float noiseSpeed = 2f;
        protected override void Awake()
        {
            base.Awake();
            isActive = false;

            enabled = true;
            if (!gameObjectNoise)
                gameObjectNoise = transform;
            linkedScene.OnClose += OnExit;
            startPos = gameObjectNoise.localPosition;
        }

        public override void SelectObject(Camera Camera, Vector3 InputPosition)
        {
            base.SelectObject(Camera, InputPosition);
            OnEntrenceActivated.Invoke(linkedScene);
        }

        void StartFirstMove()
        {
            linkedScene.StartEntrence();
        }

        void OnEntrenceEnd()
        {
            OnEntrenceActivated.Invoke(linkedScene);
        }

        void OnExitEnd()
        {
            isActive = false;
        }

        void Update()
        {
            if (!linkedScene || !linkedScene.isActive)
                return;

            noiseProgress.x += Time.deltaTime * noiseSpeed * linkedScene.endoController.currentAxis;
            noiseProgress.y += Time.deltaTime * noiseSpeed * linkedScene.endoController.currentAxis;

            float sampleX = Mathf.PerlinNoise(noiseProgress.x, 0) * noisePower - noisePower / 2;
            float sampleY = Mathf.PerlinNoise(0, noiseProgress.y) * noisePower - noisePower / 2;
            gameObjectNoise.localPosition = startPos + new Vector3(sampleX, sampleY, 0);
        }

        void OnExit()
        {
            GameManager.GetInputsManager().LockPinch = false;
            HUDManager.GetItemHUD().ShowItems();

            EndoscopeAnimator.SetTrigger("Close_" + animIndex);
        }

        public void OnEntrance()
        {
            GameManager.GetInputsManager().LockPinch = true;
            HUDManager.GetItemHUD().UnshowItems();

            if (!linkedScene || isActive)
                return;

            if (OnEntrenceActivated == null)
            {
                Debug.LogError("There is no EndoscopePad on scene, action avorted");
                return;
            }

            EndoscopeAnimator.SetTrigger("Open_" + animIndex);
            isActive = true;
        }

        public static bool IsActive()
        {
            return isActive;
        }
    }
}