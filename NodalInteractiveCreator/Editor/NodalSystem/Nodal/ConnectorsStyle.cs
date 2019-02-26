using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.Nodal
{
    public class ConnectorsStyle
    {
        public Color color = Color.white;
        public string name = ".";
        public string desc = "";

        public static ConnectorsStyle GetStyleOfField(FieldInfo f)
        {
            ConnectorsStyle s = new ConnectorsStyle();

            object[] a = f.GetCustomAttributes(typeof(NodeConnectStyleAttribute), true);

            for (int i = 0; i < a.Length; i++)
            {
                NodeConnectStyleAttribute attribute = (NodeConnectStyleAttribute) a[i];

                s.color = attribute.color;
                s.desc = attribute.desc;
                s.name = attribute.tag;

                return s;
            }

            return new ConnectorsStyle();
        }
    }

    public class NodeStyle
    {
        public string name = "";
    }
}
