 using System;
using System.Collections.Generic;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.Nodal.GUIConnector
{
    public class BoxConnector
    {
        public static int linksSize = 20;
        public static int offsetBetweenLinks = 3;

        public ConnectorsStyle style = new ConnectorsStyle();
        public bool selected = false;

        public Rect bounds;

        private List<LayerConnector> layerConnectors = new List<LayerConnector>() ;

        public Action<BoxConnector, NodalLayer> OnClicked;

        public Rect GetLayerBound(NodalLayer layer)
        {
            foreach (var l in layerConnectors)
            {
                if (l.layer == layer)
                    return l.bounds;
            }

            return Rect.zero;
        }

        public virtual float Draw(Rect parent, float decal)
        {
            bounds = GetBounds(parent);
            bounds.y += decal;
            layerConnectors = GetBoxes();
            float height = 0;  

            GUI.color = Color.gray;
            GUI.Box(new Rect(bounds.x-2, bounds.y -2 , bounds.width+4, (linksSize + offsetBetweenLinks) * layerConnectors.Count+2), "");
            GUI.color = selected ? Color.gray : style.color;
            

            for (int i = 0; i < layerConnectors.Count; i++)
            {
                if (layerConnectors[i].DrawAndGetClicked(bounds))
                {
                    Click(layerConnectors[i].layer);
                }

                bounds.y += (linksSize + offsetBetweenLinks);
                height += (linksSize + offsetBetweenLinks);
            }

            GUI.color = Color.white;
            return height;

        }

        protected void Click(NodalLayer layer)
        {
            if(OnClicked != null)
                OnClicked.Invoke(this , layer);
        }

        public void Select(bool state)
        {

            selected = state;
        }

        public virtual Rect GetBounds(Rect parent)
        {
            return new Rect(parent.x - linksSize, parent.y, linksSize, linksSize);
        }

        public virtual int GetSide()
        {
            return -1;
        }

        protected virtual List<LayerConnector> GetBoxes()
        {
            return new List<LayerConnector>();
        }
    }
}
