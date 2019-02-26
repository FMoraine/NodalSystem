using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes
{
    class StyleHelper
    {
        public static Dictionary<ColorStyle, Color> enumToColor = new Dictionary<ColorStyle, Color>()
        {
            {ColorStyle.RED, Color.red},
            {ColorStyle.BLUE, Color.blue},
            {ColorStyle.CYAN, Color.cyan},
            {ColorStyle.GREEN, Color.green},
            {ColorStyle.WHITE, Color.white},
            {ColorStyle.MAGENTA, Color.magenta},
            {ColorStyle.YELLOW, Color.yellow},
        };
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class NodeConnectStyleAttribute : Attribute
    {
        
        public Color color = Color.white;
        public string tag = ".";
        public string desc = "";

        public NodeConnectStyleAttribute(string tag, ColorStyle color , string description )
        {
            this.tag = tag;
            this.color = StyleHelper.enumToColor[color];
            desc = description;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeStyleAttribute : Attribute
    {
        public string name = "NO_NAME";
        public string path = "";

        public NodeStyleAttribute(string name)
        {
            this.name = name;
        }

        public NodeStyleAttribute(string name, string path)
        {
            this.name = name;
            this.path = path;
        }

        public NodeStyleAttribute(string name, PathCategory category, string subPath)
        {
            this.name = name;
            this.path = string.IsNullOrEmpty(subPath) ? category.ToString() : category.ToString() + "/" + subPath;
        }
    }

    public enum ColorStyle
    {
        RED,
        BLUE,
        CYAN,
        GREEN,
        WHITE,
        MAGENTA,
        YELLOW,
    }

    public enum PathCategory
    {
        ACTIONS,
        MONO,
        LOGICS,
    }
}
