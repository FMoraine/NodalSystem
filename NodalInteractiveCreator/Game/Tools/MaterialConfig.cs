// Machinika Museum Prototype v.01
// © Littlefield Studio
// Writted by Franck-Olivier FILIN - 2017
//
// MaterialConfig.cs

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MaterialConfig
{
    public MeshRenderer _meshRenderer = null;
    public Material _material = null;
    public string  _propertyName = string.Empty;
    public Gradient _gradient = new Gradient();
    public Texture _texture = null;
    public Vector2 _tiling = new Vector2(1, 1);
    public Vector2 _offset = new Vector2(0, 0);
    public float _value = 0;

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

                    if (matConf._meshRenderer.sharedMaterial.HasProperty("_Color"))
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

    public static void SetGradientWithCurrentColorMaterial(List<MaterialConfig> materialConfigs)
    {
        foreach (MaterialConfig matConf in materialConfigs)
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
}

