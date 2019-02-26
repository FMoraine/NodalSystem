using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.Settings;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Game.TheoricalNodes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using NodalInteractiveCreator.Objects;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem
{
    public class AddPanel : Panel
    {
        private static int buttonCategoriesH = 25;
        private static int buttonCategoriesSpace = 10;
        private static int marginCatBox = 5;
        private static int scrollerSizeMax = 150;
        private static int buttonCategoriesW = 200;
        private Rect bounds;
        public NodePanel nodePanel;

        private static readonly List<NodePath> nodeClasses = new List<NodePath>();
        private static readonly List<MonoToNode> monoNodeClasses = new List<MonoToNode>();
        private static readonly Dictionary<string , NodePart> pathMap = new Dictionary<string, NodePart>();

        protected bool isOpen = false;
        public bool IsOpen
        {
            get { return isOpen; }

        }
        private static GUIStyle scrollBar = new GUIStyle();
        protected bool drawCat = false;
        protected Vector2 boxSize;
        protected string currentPath = "";
        public Vector2 center;
   
        [InitializeOnLoadMethod]
        protected static void InitializeMonoClasses()
        {
            nodeClasses.Clear();
            monoNodeClasses.Clear();
            pathMap.Clear();
            pathMap.Add("" , new NodePart(""));
            var type = typeof(TheoricalNode);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p)  && !p.IsAbstract && p != typeof(TheoricalNode) && !p.IsGenericType);

            foreach (var t in types)
            {
                NodeStyleAttribute style = NodalTools.GetStyleOfType(t);
                NodePath n = new NodePath(style.path, t, style.name);
                nodeClasses.Add(n);

                AddToPart(style.path , n);
            }

            foreach (var p in pathMap)
            {
                List<NodePart> n = new List<NodePart>();
                foreach (var t in pathMap)
                {
                    if (t.Key.StartsWith(p.Key) && t.Key != p.Key)
                    {
                        string finalPath = p.Key == "" ? t.Key : t.Key.Remove(0, p.Key.Length+1);

                        if(finalPath.IndexOf("/") == -1)
                            n.Add(t.Value); 
                       
                    }
                     
                }
                p.Value.Parts.AddRange(n);
            }

            type = typeof(MonoBehaviourNode<>);
            types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p =>  NodalTools.IsSubclassOfRawGeneric(typeof(MonoBehaviourNode<>) , p) && p != typeof(MonoBehaviourNode<>) && !p.IsAbstract  && !p.IsGenericType);
            
            foreach (var t in types)
               monoNodeClasses.Add(new MonoToNode(GetMonoBTypeHeritance(t).GetGenericArguments()[0] , t));
            
            scrollBar.normal.textColor = Color.red;
        }

        static void AddToPart(string path, NodePath node)
        {
            if (pathMap.ContainsKey(path))
                pathMap[path].Nodes.Add(node);

            else
            {
                GeneratePath(path);
                pathMap[path].Nodes.Add(node);
            }
        }

        static void GeneratePath(string path)
        {
            if(pathMap.ContainsKey(path))return;

            pathMap.Add(path , new NodePart(path));
            
            int i = Mathf.Max(path.LastIndexOf("/") , 0);
            path = path.Remove(i ,  path.Length - i);
            GeneratePath(path);
        }


        static Type GetMonoBTypeHeritance(Type from)
        {
            return from.BaseType.IsGenericType &&
                   from.BaseType.GetGenericTypeDefinition() == typeof(MonoBehaviourNode<>)
                ? from.BaseType
                : GetMonoBTypeHeritance(from.BaseType);
        }

        public override void InitNecessities()
        {
            bounds.x = 0;
            bounds.y = 0;
            bounds.width = 200;
            base.InitNecessities();
        }

        public void DrawContent()
        {
            bounds.height = Screen.height/3;

            if(!isOpen)
                return;

            bounds.position = center;
            GUI.Box(bounds, "" );

            if (currentPath != "" && GUI.Button(new Rect(bounds.x + buttonCategoriesW- buttonCategoriesH, bounds.y ,  buttonCategoriesH, buttonCategoriesH), new GUIContent("◄", "Back")))
            {
                int i = Mathf.Max(currentPath.LastIndexOf("/"), 0);
                currentPath = currentPath.Remove(i, currentPath.Length - i);
            }

            DrawCategories();
        }

        public void Open(bool state)
        {

            isOpen = state;
         
        }

        Vector2 pos = Vector2.zero;
        Vector2 p = Vector2.zero;
        void DrawCategories()
        {
            NodePart cat = pathMap[currentPath];

            Rect btn = new Rect(bounds.x , bounds.y , buttonCategoriesW, buttonCategoriesH);
            GUI.Label(btn, currentPath);
            btn.y += buttonCategoriesH;

            bounds.width += 15;
            float dynamicHeight = (cat.Nodes.Count + cat.Parts.Count +  2) * buttonCategoriesH;
       
            pos = GUI.BeginScrollView(bounds, pos, new Rect(bounds.x, bounds.y, bounds.width-15, dynamicHeight), false, true);
            bounds.width -= 15;
            foreach (var c in cat.Parts)
            {
                if (GUI.Button(btn, c.name + " ►", NICStettings.Settings.addCategoryButtonStyle))
                {
                    currentPath = c.path;
                }

                btn.y += buttonCategoriesH;
            }
            
            foreach (var c in cat.Nodes)
            {
                if (GUI.Button(btn, c.name, NICStettings.Settings.addButtonStyle))
                {
                    isOpen = false;
                    AddTheorical(c.nodeType, target, nodePanel,new Vector2(center.x  - nodePanel.interactions.offset.x , center.y - nodePanel.interactions.offset.y));
                    currentPath = "";
                }
                btn.y += buttonCategoriesH;
            }

            GUI.EndScrollView();
        }

        void SetScrollBarStyle()
        {
            /*
            GUI.skin.verticalScrollbarUpButton.fixedWidth = 0;
            GUI.skin.verticalScrollbarDownButton.fixedWidth = 0;
            GUI.skin.verticalScrollbar.fixedWidth = 5;
            GUI.skin.verticalScrollbarThumb.fixedWidth = 5;
            */
            
        }

        public Rect GetBounds()
        {
            return bounds;
        }

    
        public static INodal AddTheorical(Type classes , NodalInteractionSystem target , NodePanel nodePanel, Vector2 pos)
        {
            INodal t = (INodal)Activator.CreateInstance(classes);
            target.AddNodeBahviour(t);
            nodePanel.AddNode(t, target.nodes.Count - 1, new Vector2(pos.x, pos.y));

            NodalEditor.MarkSceneDirty();
            NodalEditor.instance.Repaint();
            return t;
        }

        public void OnObjectDrop(Object g)
        {
            if (g is GameObject)
            {
                var scripts = ((GameObject) g).GetComponents<MonoBehaviour>();

                foreach (var script in scripts)
                {
                    foreach (var m in monoNodeClasses)
                    {
                        if (script.GetType() == m.mono)
                        {
                            ((MonoBehaviourNode)AddTheorical(m.nodal , target , nodePanel , center)).SetSubject(script);
                        }
                    }   
                }
            }
        }
    }


    class MonoToNode
    {
        public MonoToNode(Type pMono, Type pNodal)
        {
            mono = pMono;
            nodal = pNodal;
        }

        public Type mono;
        public Type nodal;
    }

    struct NodePath
    {
        public NodePath(string path, Type nodeType, string name)
        {
            this.name = name;
            this.path = path;
            this.nodeType = nodeType;
        }

        public string path;
        public string name;
        public Type nodeType; 
    }


    struct NodePart
    {
        public NodePart(string path)
        {
            this.path = path;
            int last = path.LastIndexOf("/")+1;
            name = path.Substring(last, this.path.Length - last);
            Nodes = new List<NodePath>();
            Parts = new List<NodePart>();
        }

        public string path;
        public string name;
        public List<NodePath> Nodes;
        public List<NodePart> Parts;
    }
  
}

