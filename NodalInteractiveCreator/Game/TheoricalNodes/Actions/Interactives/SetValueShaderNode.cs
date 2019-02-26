using System;
using System.Collections;
using System.Collections.Generic;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Game.TheoricalNodes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using NodalInteractiveCreator.HUD;
using NodalInteractiveCreator.Objects;
using NodalInteractiveCreator.Tools;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools
{

    [Serializable]
    [NodeStyle("SetValueShader", PathCategory.ACTIONS, "Interactives")]
    public class SetValueShaderNode : ActionNode
    {
        public SerializerObject<GameObject> _go = new SerializerObject<GameObject>();
        public SerializerObject<MeshRenderer> _meshRenderer = new SerializerObject<MeshRenderer>();
        public int _idMat = 0;
        public string _propertyName = string.Empty;
        public Color _color = Color.clear;
        public Texture _texture = null;
        public Vector2 _tiling = new Vector2(1, 1);
        public Vector2 _offset = new Vector2(0, 0);
        public float _value = 0;

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void ActiveNode()
        {
            base.ActiveNode();

            if (null != _meshRenderer.Subject && _meshRenderer.Subject.sharedMaterials[_idMat].HasProperty(_propertyName))
            {
                //MaterialPropertyBlock block = new MaterialPropertyBlock();
                //_meshRenderer.Subject.GetPropertyBlock(block);

                if (_color != Color.clear)
                    _meshRenderer.Subject.materials[_idMat].SetColor(_propertyName, _color);

                if (null != _texture)
                    _meshRenderer.Subject.materials[_idMat].SetTexture(_propertyName, _texture);

                if (_tiling != new Vector2(1, 1))
                    _meshRenderer.Subject.sharedMaterials[_idMat].SetTextureScale(_propertyName, _tiling);

                if (_offset != new Vector2(0, 0))
                    _meshRenderer.Subject.sharedMaterials[_idMat].SetTextureOffset(_propertyName, _offset);

                if (_value != 0)
                    _meshRenderer.Subject.materials[_idMat].SetFloat(_propertyName, _value);

                //_meshRenderer.Subject.SetPropertyBlock(block);
            }
        }
    }
}