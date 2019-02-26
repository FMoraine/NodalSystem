// Machinika Museum Prototype v.01
// © Littlefield Studio
// Writted by Rémi Carreira - 2015 - 2016
//
// Edited by Franck-Olivier FILIN - 2017
//
// InteractiveObject.cs

using System;
using System.Collections;
using System.Collections.Generic;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using NodalInteractiveCreator.Controllers;
using NodalInteractiveCreator.Managers;
using NodalInteractiveCreator.Tools;
using UnityEngine;

namespace NodalInteractiveCreator.Objects
{
    [RequireComponent(typeof(Collider))]
    public class InteractiveObject : TouchableElement
    {
        #region Variables
        public Action act_Start;
        public Action act_Final;
        public Action<int> act_Check;
        public Action<float> act_Progress;
        public Action act_Touch;

        public List<MaterialConfig> _materials = new List<MaterialConfig>();
        #endregion

        protected override void Awake()
        {
            base.Awake();
        }

        public void SetActiveObj(GameObject goToActive, bool value)
        {
            goToActive.SetActive(value);

            if (null != goToActive.GetComponent<InteractiveObject>())
                goToActive.GetComponent<InteractiveObject>().enabled = true;
        }

        public static void ChangeMaterial(ref float gap, ref List<MaterialConfig> listMatConf)
        {
            foreach (MaterialConfig matConf in listMatConf)
            {
                if (null != matConf._meshRenderer)
                {
                    if (matConf._meshRenderer.sharedMaterial.HasProperty(matConf._propertyName))
                    {
                        MaterialPropertyBlock block = new MaterialPropertyBlock();

                        matConf._meshRenderer.GetPropertyBlock(block);

                        if (matConf._material.HasProperty("_Color"))
                            block.SetColor("_Color", matConf._gradient.Evaluate(gap));

                        if (matConf._material.HasProperty("_RendererColor"))
                            block.SetColor("_RendererColor", matConf._gradient.Evaluate(gap));

                        if (null != matConf._texture)
                            block.SetTexture(matConf._propertyName, matConf._texture);

                        if (matConf._tiling != new Vector2(1, 1))
                        {
                            float tilingX = Mathf.SmoothStep(matConf._meshRenderer.sharedMaterial.mainTextureScale.x, matConf._tiling.x, gap);
                            float tilingY = Mathf.SmoothStep(matConf._meshRenderer.sharedMaterial.mainTextureScale.y, matConf._tiling.y, gap);

                            matConf._meshRenderer.sharedMaterial.SetTextureScale(matConf._propertyName, new Vector2(tilingX, tilingY));
                        }

                        if (matConf._offset != new Vector2(0, 0))
                        {
                            float offsetX = Mathf.SmoothStep(matConf._meshRenderer.sharedMaterial.mainTextureOffset.x, matConf._offset.x, gap);
                            float offsetY = Mathf.SmoothStep(matConf._meshRenderer.sharedMaterial.mainTextureOffset.y, matConf._offset.y, gap);

                            matConf._meshRenderer.sharedMaterial.SetTextureOffset(matConf._propertyName, new Vector2(offsetX, offsetY));
                        }

                        if (matConf._value != 0)
                        {
                            float value = Mathf.SmoothStep(matConf._meshRenderer.sharedMaterial.GetFloat(matConf._propertyName), matConf._value, gap);

                            block.SetFloat(matConf._propertyName, value);
                        }

                        matConf._meshRenderer.SetPropertyBlock(block);
                    }
                }
            }
        }

        public void LoadWaitForActiveInteractivity(System.Action method, float delay)
        {
            StartCoroutine(WaitForActiveInteractivity(method, delay));
        }

        protected void SetGradientWithCurrentColorMaterial()
        {
            foreach (MaterialConfig matConf in _materials)
            {
                if (null != matConf._meshRenderer)
                {
                    if (matConf._material.HasProperty("_Color"))
                    {
                        GradientColorKey[] gck = new GradientColorKey[matConf._gradient.colorKeys.Length];

                        for (int i = 0; i < gck.Length; i++)
                        {
                            if (i == 0)
                                gck[i].color = matConf._meshRenderer.materials[0].GetColor("_Color");
                            else
                                gck[i].color = matConf._gradient.colorKeys[i].color;

                            gck[i].time = matConf._gradient.colorKeys[i].time;
                        }

                        matConf._gradient.SetKeys(gck, matConf._gradient.alphaKeys);
                    }
                }
            }
        }

        protected void InitAudioForBackToStart(Check check)
        {
            check.AudioPlayed = false;
            _objectAudio.loop = true;
            _objectAudio.enabled = true;
        }

        public IEnumerator WaitActiveAction(Act act)
        {
            yield return new WaitWhile(() => Cutscene._cutsceneIsPlaying == true);

            if (GetComponent<Animation>() != null)
                yield return new WaitWhile(() => GetComponent<Animation>().isPlaying == true);

            if (GameManager.GetInputsManager().LockInteractivity)
                GameManager.GetInputsManager().LockInteractivity = false;

            if (GameManager.GetInputsManager().LockPinch)
                GameManager.GetInputsManager().LockPinch = false;

            StopCoroutine(WaitActiveAction(act));
        }

        public IEnumerator WaitActiveAction(Act act, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (GameManager.GetInputsManager().LockInteractivity)
                GameManager.GetInputsManager().LockInteractivity = false;

            if (GameManager.GetInputsManager().LockPinch)
                GameManager.GetInputsManager().LockPinch = false;

            StopCoroutine(WaitActiveAction(act, delay));
        }

        public IEnumerator WaitForActiveInteractivity(System.Action method, float delay)
        {
            yield return new WaitForSeconds(delay);

            method.Invoke();
            StopCoroutine(WaitForActiveInteractivity(method, delay));
        }
    }
}
