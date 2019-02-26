using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.CustomEditing;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.Nodal;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.Nodal.GUIConnector;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.Settings;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem
{
    public class DraggableNode : DrawableContent
    {
        protected const float decalBetweenNode = 5;

        public NodalInfos node;

        public List<ReceiverConnector> receiver = new List<ReceiverConnector>();
        public List<EmitterConnector> emitter = new List<EmitterConnector>();
  
        public Action<BoxConnector, NodalLayer> onBoxClicked;

        protected static Dictionary<Type, NodalEditorCustom> TypeToCustom;

        [InitializeOnLoadMethod]
        private static void InitCustomClasses()
        {
            TypeToCustom = new Dictionary<Type, NodalEditorCustom>();

            var type = typeof(NodalEditorCustom);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p != typeof(NodalEditorCustom));

            foreach (var t in types)
            {
                var attr = t.GetCustomAttributes(true);
                foreach (var a in attr)
                {
                    if (a is CustomNodalEditor)
                    {
                        Type customTarget = ((CustomNodalEditor)a).subjectType;
                        TypeToCustom.Add(customTarget, (NodalEditorCustom)Activator.CreateInstance(t, new object[] { customTarget }));
                    }
                }
            }
        }

        public DraggableNode(NodalInfos pNode ,  int pID , Vector2 position) : base(pID)
        {
            _bounds.position = position;
            draggable = true;
            node = pNode;
            GetNodesLinks();
            windowName = pNode.Subject.GetName();
        }

        bool IsCustomEditor(Type subject)
        {
            return TypeToCustom.ContainsKey(subject);
        }

        void DrawCustomEditor(Type subject)
        {
            NodalEditorCustom editor = null;
            editor = IsCustomEditor(subject) ? TypeToCustom[subject] : new NodalEditorCustom(subject);

            editor.Subject = node.Subject;

            if (!node.IsEmpty())
                editor.OnGUI();

        }

        void GetNodesLinks()
        {
            List<FieldInfo> l = NodalTools.GetAllConnectionsFields<NodeConnector>(node.Subject);


            foreach (var n in l)
            {
                if (n.FieldType.IsAssignableFrom(typeof(NodeReceiver)) || n.FieldType.IsSubclassOf(typeof(NodeReceiver)))
                {
                    ReceiverConnector c =
                        new ReceiverConnector((NodeReceiver)n.GetValue(node.Subject)) { OnClicked = OnBoxClick , style = ConnectorsStyle.GetStyleOfField(n)};
                    receiver.Add(c);
                }

                else if (n.FieldType.IsAssignableFrom(typeof(NodeEmitter)) || n.FieldType.IsSubclassOf(typeof(NodeEmitter)))
                {
                    EmitterConnector c =
                        new EmitterConnector((NodeEmitter)n.GetValue(node.Subject)) { OnClicked = OnBoxClick, style = ConnectorsStyle.GetStyleOfField(n) };
                    emitter.Add(c);
                }
            }
        }

        void OnBoxClick(BoxConnector b, NodalLayer layer)
        {
            if(onBoxClicked != null)
                onBoxClicked.Invoke(b, layer);
        }

        public void Delete()
        {
            receiver.Clear();
            emitter.Clear();
        }

        public override void DrawContentOverride()
        {
            if(node.IsEmpty())
                return; 
            base.DrawContentOverride();
            DrawConnectors();  
        }

    
        protected override void DrawContent(int id)
        {
            DrawCustomEditor(node.Subject.GetType());
            base.DrawContent(id);
        }

        protected override Vector2 GetSize()
        {
            if (node.Subject == null)
                return base.GetSize();

            Type subject = node.Subject.GetType();
            NodalEditorCustom editor = null;
            editor = IsCustomEditor(subject) ? TypeToCustom[subject] : new NodalEditorCustom(subject);
            editor.Subject = node.Subject;

            Vector2 editorSize = editor.GetCustomBounds().size;

            if(editorSize.y > _bounds.height)
                 _bounds.height = editorSize.y;
            _bounds.width = editorSize.x;

            return _bounds.size;
        }

        float heightReceiver = 0;
        float heightEmitter = 0;
        void DrawConnectors()
        {
            heightEmitter = 0;
            heightReceiver = 0;

            for (int i = 0; i < receiver.Count; i++)
                heightReceiver += receiver[i].Draw(offsetBounds , heightReceiver )+ decalBetweenNode;
            
            for (int i = 0; i < emitter.Count; i++)
                heightEmitter += emitter[i].Draw(offsetBounds, heightEmitter )+ decalBetweenNode;
            
            if (Mathf.Max(heightEmitter , heightReceiver) > _bounds.height)
                _bounds.height = Mathf.Max(heightEmitter, heightReceiver);
        }
    }
}
