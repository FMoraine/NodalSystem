using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NodalInteractiveCreator.Endoscop
{
    public partial class EndoscopeScene
    {
        public void SetUpEnvironment(float skyBoxSize)
        {
            if (endoCamera)
                return;

            gameObject.name = "EndoscopeScene_" + gameObject.name;

            sceneSize = skyBoxSize;
            skyBox = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            skyBox.transform.parent = this.transform;
            skyBox.name = "EndoSkyBox";
            skyBox.transform.localScale = Vector3.one * skyBoxSize * 2;

            GameObject g = new GameObject("endoCamera");
            g.transform.parent = this.transform;
            endoCamera = g.AddComponent<Camera>();
            endoController = g.AddComponent<CameraEndoscope>();
            skyBox.gameObject.SetActive(false);
            endoController.gameObject.SetActive(false);
            skyBox.transform.localPosition = Vector3.zero;
            endoCamera.transform.localPosition = Vector3.zero;

            if(specs.path == null)
                specs = new EndoscopeData
                {
                    path = new List<PathPoint>(),
                    speed = 3
                };
        }

        void ClearScene()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }
}
