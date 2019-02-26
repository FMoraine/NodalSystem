using System;
using System.Collections.Generic;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.Nodal;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.Nodal.GUIConnector;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.Settings;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using UnityEditor;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem
{
    public class NodePanel : Panel
    {
        public List<DraggableNode> nodes = new List<DraggableNode>();
        public List<BezierLink> links = new List<BezierLink>();
        
        private Rect bounds;

        public PanelInteractions interactions = new PanelInteractions();

        private readonly NodalController controller = new NodalController();

        public NodePanel()
        {

        }

    
        public void AddNode(INodal nodeToAdd , int idInfos, Vector2 offSetStart)
        {
            NodalInfos infos = new NodalInfos() {Subject = nodeToAdd, pos = offSetStart};
            AddNode(infos, idInfos);
        }

        public void AddNode(NodalInfos infos, int idInfos)
        {
            DraggableNode UInode = new DraggableNode(infos, idInfos,
                new Vector2(infos.pos.x, infos.pos.y));

            nodes.Add(UInode);

            UInode.onBoxClicked = controller.OnBoxClicked;

            
        }

        public override void Clear()
        {
            links.Clear();
            nodes.Clear();
            base.Clear();
        }

        public void DrawContent()
        {
            bounds = GetBounds();
            DrawLinks();
            for (int i = nodes.Count-1; i >= 0 ; i--)
            {

                if (nodes[i].node.Subject == null)
                {
                    nodes.RemoveAt(i);
                    continue;
                }
                  
                nodes[i].DrawContentOverride();
            }
        }

        public void DrawUIOverlay()  
        {
            Rect r = new Rect(Screen.width-130, 15, 80, 30);
            if (GUI.Button(r, "clean") && EditorUtility.DisplayDialog("Clear all" ,  "All data will be revert" , "Ok" , "Cancel"))
            {
                target.RemoveAllNodes(); 
                nodes.Clear();
            }
            r.x -= 90;
            if (GUI.Button(r, "refresh"))
            {
                NodalEditor.instance.Refresh();
            }

            r.x = Screen.width - 40;
            r.width = 30;

            if (GUI.Button(r, "X"))
            {
                interactions.offset = Vector2.zero;
                NodalEditor.Target = null;
            }
        }
          
        public void InputDetect()
        {
            controller.target = target;
            controller.panel = this;
            controller.Loop();
        }

        public void DrawWindows()
        {
            Texture2D bg = NICStettings.Settings.background;

            if(!bg)
                bg = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ _GENERAL/Textures/Editor/NodalEditor_BG.jpg");

            bg.wrapMode = TextureWrapMode.Repeat;
           
            GUI.DrawTextureWithTexCoords(bounds, bg, new Rect(0, 0, bounds.width / bg.width, bounds.height / bg.height));
          

            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].DrawWindow(interactions.offset);
                RefreshNodeEditorData(i);
            }

           
        }

        void RefreshNodeEditorData(int i)
        {
           
            target.nodes[i].pos = new Vector2(nodes[i].Bounds.x, nodes[i].Bounds.y);
        }

        public Rect GetBounds()
        {
            return new Rect(0, 0, Screen.width , Screen.height);
        }

        void DrawLinks()
        {
            for (int i = links.Count-1; i >= 0; i--)
            {
                links[i].Draw();
            }
            
        }

        public void GenerateLink()
        {
            links.Clear();
            for (int i = 0; i < nodes.Count; i++)
            {
                DraggableNode n = nodes[i];
                for (int j = 0; j < n.emitter.Count; j++)
                {
                    EmitterLink link = target.GetEmitterLinkByGUID(n.emitter[j].nodelink.guid);

                    for (int k = 0; k < link.linkedReceivers.Count; k++)
                    {
                        ReceiverLink linkReceive = target.GetReceiverLinkByGUID(link.linkedReceivers[k]);
                        ReceiverConnector nodeReceiver = (ReceiverConnector) FindBoxByConnector(linkReceive.receiver);

                        BezierLink bzl = new BezierLink(n.emitter[j], nodeReceiver
                           , link.layers[k], nodeReceiver.nodelink.MainLayer);

                        links.Add(bzl);
                        bzl.OnDelete = OnLinkDelete;

                    }   
                }
            }
        }

        void OnLinkDelete(BezierLink targetLink)
        {
            links.Remove(targetLink);
            target.Disconnect(targetLink.emit.nodelink.guid, targetLink.receive.nodelink.guid, targetLink.layerEmit);
        }

        BoxConnector FindBoxByConnector(NodeReceiver r)
        {

            foreach (var n in nodes)
            {
                foreach (var rcv in n.receiver) 
                {
                   
                    if ( rcv.nodelink.guid == r.guid)
                        return rcv;  
                }
            }

            Debug.Log("no box founded");
            return new BoxConnector();
        }
    }

    public class PanelInteractions
    {
        public Vector2 offset = Vector2.zero;
        public Vector2 zoom = Vector2.one;
    }
}
