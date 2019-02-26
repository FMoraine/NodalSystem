using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.Nodal.GUIConnector
{
    public class LayerConnector
    {
        public NodalLayer layer;
        public Rect bounds;
        public bool selected = false;
        public ConnectorsStyle defaultStyle;

        public LayerConnector(NodalLayer n)
        {
            layer = n;
        }

        public bool DrawAndGetClicked(Rect bounds)
        {
            this.bounds = bounds;

            bool asPersonalizedStyle = layer.color != Color.clear;

            GUI.color = selected ? Color.gray : asPersonalizedStyle? layer.color : defaultStyle.color;
            bool result = GUI.Button(bounds, new GUIContent(asPersonalizedStyle ? layer.content : defaultStyle.name,asPersonalizedStyle ? layer.desc : defaultStyle.desc));
            GUI.color = Color.white;

            return result;
        }
    }
}
