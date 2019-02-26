using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.Nodal.GUIConnector
{

    public class BoxConnector<TNode> : BoxConnector where TNode : NodeConnector
    {
        public TNode nodelink;

        protected int side = -1;
        protected GUIStyle labelType;
        protected int labelWidth = 50;

        public BoxConnector(TNode link) : base()
        {
            side = GetSide();
            nodelink = link;
            labelType = new GUIStyle(); 
            labelType.normal.textColor = Color.white;
            labelType.alignment = side > 0 ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight;
        }

        string GetNameOfGenericParams()
        {
            Type[] t = nodelink.GetType().GetGenericArguments();

            return t.Length == 0 ? "" : t[0].Name;
        }

        public override float Draw(Rect parent, float decal)
        {
            if (nodelink.MainLayer.hide)
                return 0;

            GUI.Label(new Rect(bounds.x + (side > 0 ? linksSize : -labelWidth), bounds.y, labelWidth, linksSize), GetNameOfGenericParams(), labelType);

            if (NodalEditor.guidView)
                GUI.TextField(new Rect(bounds.position.x + bounds.width, bounds.position.y, 300, linksSize), nodelink.guid);

            return base.Draw(parent, decal);
        }

        protected override List<LayerConnector> GetBoxes()
        {
            List<LayerConnector> r = new List<LayerConnector>();

            for (int i = 0; i < nodelink.LayerCount; i++)
            {
                LayerConnector l = new LayerConnector(nodelink.GetLayerAt(i));
                l.defaultStyle = style;
                l.selected = selected;
                r.Add(l);
            }

            if (nodelink.LayerCount == 0)
            {
                LayerConnector l = new LayerConnector(nodelink.MainLayer);
                l.defaultStyle = style;
                l.selected = selected;
                r.Add(l);
            }
            return r;
        }
    }


    public class EmitterConnector : BoxConnector<NodeEmitter>
    {
        public EmitterConnector(NodeEmitter link) : base(link)
        {
        }

        public override Rect GetBounds(Rect parent)
        {
            return new Rect(parent.x + parent.width, parent.y, linksSize, linksSize);
        }


        public override int GetSide()
        {
            return 1;
        }


    }


    public class ReceiverConnector : BoxConnector<NodeReceiver>
    {
        public ReceiverConnector(NodeReceiver link) : base(link)
        {

        }


        public override Rect GetBounds(Rect parent)
        {
            return new Rect(parent.x - linksSize, parent.y, linksSize, linksSize);
        }


        public override int GetSide()
        {
            return -1;
        }
    }
}
