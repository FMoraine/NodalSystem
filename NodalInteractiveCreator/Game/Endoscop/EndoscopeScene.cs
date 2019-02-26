using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using UnityEngine;

namespace NodalInteractiveCreator.Endoscop
{
	public partial class EndoscopeScene : GUIDBehaviour
	{
		[SerializeField]
		[Versioning("camera")]
		private Camera endoCamera;
		[SerializeField]
		[Versioning("controller")]
		public CameraEndoscope endoController;
		[SerializeField]
		private GameObject skyBox;
		[Versioning("renderTexture")]
		public RenderTexture renderText;
		[Versioning("skybox")]
		public Material skyBoxMat;
		[Versioning("data")]
		public EndoscopeData specs;
		public float sceneSize;

		public bool isActive = false;

		public Action OnClose;

		void Awake()
		{
			if (skyBox != null) skyBox.gameObject.SetActive(true);
			endoCamera.gameObject.SetActive(isActive);
			if (skyBox != null) skyBox.GetComponent<Renderer>().material = skyBoxMat;
			if (skyBox != null) EndoscopeMath.ReverseNormals(skyBox.GetComponent<MeshFilter>());
		}
		public void Activate(bool state)
		{
			if (isActive == state)
				return;

			isActive = state;
			endoCamera.gameObject.SetActive(state);

			if (isActive)
				OnActivate();
			else 
				OnDisactivate();
		}

		void OnActivate()
		{
			endoController.Initialise(specs);
			endoCamera.targetTexture = renderText;
		}

		void OnDisactivate()
		{
			if(OnClose != null)
				OnClose.Invoke();

			endoController.Disactivate();
		}

		public void ControlCameraOrientation(Vector2 axisInput)
		{
			endoController.axisLook = axisInput;
		}

		public void ControlCameraMovement(float pAxis)
		{
            if (!CameraEndoscope._onMove)
                endoController.axisMovement = pAxis;
		}

		public void StartEntrence()
		{
			endoController.StartCoroutine(endoController.StartMove());
		}
	}
}
