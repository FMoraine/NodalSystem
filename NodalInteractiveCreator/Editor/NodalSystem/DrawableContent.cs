using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.Settings;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem
{
    public class DrawableContent
    {
        public static bool dragAvailable = true;
        public string windowName = "";
        public int sortingID = -1;
        public bool selected = false;
        public Color selectedColor = Color.yellow;
        public Rect offsetBounds;
        public Rect Bounds
        {
            get
            {

                return _bounds;
            }
        }

        protected Rect _bounds = new Rect(0 , 0 , 50 , 50);

        protected bool draggable = false;


        public DrawableContent(int id)
        {
         
            sortingID = id;
        }

        public virtual void DrawContentOverride()
        {

        }


        public void SetSelected(bool state)
        {
            if (selected == state)
                return;

            selected = state;
        }

        public void DrawWindow(Vector2 offset)
        {
            _bounds.x += offset.x; 
            _bounds.y += offset.y;
            _bounds.size = GetSize();
            offsetBounds = _bounds;
            
            GUI.color = selected ? selectedColor : Color.white;

            if(draggable && dragAvailable)
                _bounds = GUI.Window(sortingID, offsetBounds, DrawContent , windowName, NICStettings.Settings.windowStyle);
            else
                GUI.Window(sortingID, offsetBounds, DrawContent, windowName, NICStettings.Settings.windowStyle);

            _bounds.x -= offset.x;
            _bounds.y -= offset.y;
            GUI.color = Color.white;
        }

        protected virtual Vector2 GetSize()
        {
            return _bounds.size;
        }

        protected virtual void DrawContent(int id)
        {
            if (draggable)
                GUI.DragWindow();
        }
    }
}
