using System;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Game.TheoricalNodes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using NodalInteractiveCreator.Controllers;
using NodalInteractiveCreator.Managers;
using NodalInteractiveCreator.Viewpoints;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools
{
    [Serializable]
    [NodeStyle("MoveToViewpoint", PathCategory.ACTIONS, "Viewpoint")]
    public class MoveToViewpointNode : ActionNode
    {
        public int _idMoveTo;
        public SerializerObject<ViewPoint> _vp = new SerializerObject<ViewPoint>();

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void ActiveNode()
        {
            base.ActiveNode();
            Debug.Log(_idMoveTo);
            switch(_idMoveTo)
            {
                case 0:
                    GameManager.GetCamerasManager().MainCamera.GetComponent<CameraController>().ChangeModeToZoomTo(_vp);
                    break;

                case 1:
                    GameManager.GetCamerasManager().MainCamera.GetComponent<CameraController>().ChangeModeToZoomOutTo(_vp);
                    break;
            }
        }
    }
}