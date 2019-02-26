using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem
{
    public partial class NodalEditor
    {
        public void DeleteNode(NodalInfos infos)
        {
            Target.RemoveNode(infos.Subject);
            Refresh();
            MarkSceneDirty();
        }

        public static void MarkSceneDirty()
        {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        public void Refresh()
        {
            if (!Target)
                return;
       
            Rebind();
            Repaint();
        }


        void Rebind()
        {
            Debug.Log("Refresh Nic..."); 
            nodeUI.target = Target;
            addUI.target = Target;

            nodeUI.Clear();
            addUI.Clear();

            for (int i = 0; i < Target.nodes.Count; i++)
            {
                nodeUI.AddNode(Target.nodes[i], i);
            }

            Target.RefreshLinks();
            nodeUI.GenerateLink();

        }  

        void Init()
        {
            addUI.nodePanel = nodeUI;
            nodeUI.InitNecessities();
            addUI.InitNecessities();
        }

    }
}
 