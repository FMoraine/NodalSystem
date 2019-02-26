using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.Nodal;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.Nodal.GUIConnector;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.Settings;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using UnityEditor;
// using UnityEditor.Android;
using UnityEditor.Graphs;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem
{
    public class NodalController
    {
        public NodalInteractionSystem target;
        public InputUser userinputs = new InputUser();
        public List<DraggableNode> selected = new List<DraggableNode>();
        public NodePanel panel;
        protected Vector2 precedentMousePos;

        public EmitterConnector From
        {
            get { return _from; }
            set
            {
                if(_from != null)
                    _from.Select(false);

                _from = value;

                if(_from!=null)
                    _from.Select(true);
            }
        }

        public ReceiverConnector To
        {
            get { return _to; }
            set
            {
                if (_to != null)
                   _to.Select(false);

                _to = value;

                if(_to !=null)
                    _to.Select(true);
                
            }
        }

        protected NodalLayer layerFrom;
        protected EmitterConnector _from;
        protected NodalLayer layerTo;
        protected ReceiverConnector _to;

        void Connect()
        {
            target.Connect(_from.nodelink.guid , _to
                .nodelink.guid, layerFrom , layerTo);


            UnSelectLinks();
            panel.GenerateLink();
            NodalEditor.MarkSceneDirty();
        }

        void Unselect()
        {
            foreach (var node in selected)
            {
                node.SetSelected(false);
            }
            selected.Clear();

          
        }

        void UnSelectLinks()
        {
            From = null;
            To = null;
        }

        public void OnBoxClicked(BoxConnector b, NodalLayer layer)
        {

            if (b is EmitterConnector)
            {
                From = (EmitterConnector)b;
                layerFrom = layer;
            }
            else if (b is ReceiverConnector && From != null)
            {
                To = (ReceiverConnector)b;
                layerTo = layer;
                Connect();

            }
        }

        private int refreshCount = 3;
        private int count = 0;
        protected bool beginDrag = false;
        
        public void Loop()
        {

          
             
            Event e = Event.current;

            if (DragAndDrop.objectReferences.Length > 0 && DetectWindow(e) == null)
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            switch (e.type)
            {
                case EventType.KeyUp:
                {
                    if (Event.current.keyCode == (KeyCode.Space))
                        userinputs.space = false;
                    else if (Event.current.keyCode == (KeyCode.LeftAlt))
                    {
                        DrawableContent.dragAvailable = true;
                        userinputs.alt = false;
                    }
                    else if (Event.current.keyCode == (KeyCode.LeftControl))
                        userinputs.maj = userinputs.ctrl = false;
                    else if (Event.current.keyCode == (KeyCode.Delete))
                        DeleteSelected();
                    else if (Event.current.keyCode == (KeyCode.D) && userinputs.ctrl)
                        Clone();
                      
                        break;
                }

                case EventType.KeyDown:
                {
                    if (Event.current.keyCode == (KeyCode.Space))
                        userinputs.space = true;
                    else if (Event.current.keyCode == (KeyCode.LeftAlt))
                    {
                        DrawableContent.dragAvailable = false;
                     
                        userinputs.alt = true;
                    }
                      
                    else if (Event.current.keyCode == (KeyCode.LeftControl))
                    {
                        userinputs.maj = userinputs.ctrl = true;
                    }
                     

                        break;
                }
                case EventType.MouseDown:
                {
                    if(!NodalEditor.addUI.GetBounds().Contains(e.mousePosition) && e.button == 0)
                        Click(e);

                    beginDrag = true;
                    break;
                }
                case EventType.MouseDrag:
                {
                    if (userinputs.alt)
                    {
                        if (beginDrag)
                        {
                            precedentMousePos = e.mousePosition;
                            beginDrag = false;
                        }
                        Drag(e);
                    }
                         
                    break;
                }
                case EventType.ContextClick:
                {
                    RightClick(e);
                    break;
                }
                case EventType.DragPerform:
                {
                    EndDrag(e);
                   
                    break;
                }
            }
        }

        void Clone()
        {
            foreach (var s in selected)
            {
                AddPanel.AddTheorical(s.node.Subject.GetType(), target, panel, s.Bounds.position - (Vector2.left + Vector2.down) * 150);
            }
        }

        void EndDrag(Event e)
        {
            if(DragAndDrop.objectReferences.Length == 0 || DetectWindow(e) != null)
                return;
            NodalEditor.addUI.OnObjectDrop(DragAndDrop.objectReferences[0]);
        }

        void RightClick(Event e)
        {
            if (selected.Count > 0)
            {
                Unselect();
            }
            UnSelectLinks();

           NodalEditor.addUI.center = e.mousePosition;
           NodalEditor.addUI.Open(!NodalEditor.addUI.IsOpen);
            
            NodalEditor.instance.Repaint();
        }

        void Click(Event e)
        {
           
            NodalEditor.addUI.Open(false);
            FindSelectedWindow(e);

          
            NodalEditor.instance.Repaint();
        }

        void Drag(Event e)
        {

            panel.interactions.offset += (e.mousePosition - precedentMousePos) * NICStettings.Settings.preferences.dragSpeed;
            NodalEditor.instance.Repaint();
            precedentMousePos = e.mousePosition;

        }  

        void DeleteSelected()
        {
            foreach (var node in selected)
            {
                NodalEditor.instance.DeleteNode(node.node);
            }
            NodalEditor.instance.Repaint();
            NodalEditor.MarkSceneDirty();
        }

        void FindSelectedWindow(Event e)
        {
            DraggableNode n = DetectWindow(e);

            if (n == null && !userinputs.maj )
            {
                Unselect();
                return;
            }
             

            if (n != null && !n.selected && !selected.Contains(n) && !userinputs.alt)
            {
                if (!userinputs.maj)
                    Unselect();

                n.SetSelected(true);
                selected.Add(n);

                NodalEditor.instance.Repaint();
            }
        }

        DraggableNode DetectWindow(Event e)
        {
            foreach (var n in panel.nodes)
            {
                if ( n.offsetBounds.Contains(e.mousePosition))
                {
                    return n;
                } 
            }

            return null;
        }
    }

   

    public class InputUser
    {
        public bool alt = false;
        public bool space = false;
        public bool maj = false;
        public bool ctrl = false;

    }
}
