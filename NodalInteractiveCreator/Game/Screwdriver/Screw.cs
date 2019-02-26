using UnityEngine;
using System.Collections;
using NodalInteractiveCreator.Managers;
using NodalInteractiveCreator.Controllers;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NodalInteractiveCreator.Screwdriver
{
    [RequireComponent(typeof(Collider))]
    public class Screw : MonoBehaviour
    {
        [System.Serializable]
        public class Form
        {
            public int FormIndex = 0;
            public int TranslateIndex = 0;
            public int RotateIndex = 0;
        }

        [Header("Camera")]
        public float DistanceToTarget = 1.0f;
        public float XAngle = 25.0f;
        public float YAngle = 45.0f;

        [Header("Offsets")]
        public Vector3 ScrewdriverStartOffset = Vector3.zero;
        public Vector3 ScrewdriverFinishOffset = Vector3.zero;

        public Form FirstForm = new Form();
        public Form SecondForm = new Form();

        private Collider selfCollider = null;

        private CameraController cameraController = null;

        private bool check = false;
        private bool done = false;

        private void Awake()
        {
            selfCollider = GetComponent<Collider>();
        }

        private IEnumerator CheckScrew()
        {
            Screwdriver Instance = Screwdriver.GetInstance();
            if (null != Instance)
            {
                check = true;

                while (true == check && false == done)
                {
                    done = Instance.CheckScrewdriver(this);
                    if (true == done)
                    {
                        Debug.Log("Screw is done");
                    }

                    yield return new WaitForEndOfFrame();
                }
            }
        }

        public void ExecuteTrigger()
        {
            CamerasManager CamerasMgr = GameManager.GetCamerasManager();
            if (null != CamerasMgr)
            {
                Camera CurrentCamera = CamerasMgr.GetCurrentCamera();
                if (null != CurrentCamera)
                {
                    CameraController controller = CurrentCamera.GetComponent<CameraController>();
                    if (null != controller)
                    {
                        Vector3 CameraPosition = transform.position - (Quaternion.AngleAxis(XAngle, Vector3.up) * Quaternion.AngleAxis(YAngle, Vector3.right) * Vector3.forward * DistanceToTarget);

                        controller.ChangeModeToZoomScrew(this, CameraPosition, Quaternion.LookRotation(-(CameraPosition - (transform.position + ScrewdriverFinishOffset))));
                        controller.OnPositionReached += EnterScrewdriver;
                        cameraController = controller;
                    }
                }
            }

            selfCollider.enabled = false;
        }

        private void EnterScrewdriver()
        {
            Screwdriver Instance = Screwdriver.GetInstance();
            if (null != Instance)
            {
                Instance.gameObject.SetActive(true);
                Instance.SetPosition(transform.position + ScrewdriverStartOffset);
                Instance.LookAt(transform.position);
                Instance.Move(transform.position + ScrewdriverStartOffset, transform.position + ScrewdriverFinishOffset);
            }
            if (null != cameraController)
            {
                cameraController.OnPositionReached -= EnterScrewdriver;
            }

            StartCoroutine(CheckScrew());
        }

        public void QuitScrew()
        {
            Screwdriver Instance = Screwdriver.GetInstance();
            if (null != Instance)
            {
                Instance.Move(transform.position + ScrewdriverFinishOffset, transform.position + ScrewdriverStartOffset);
            }

            EnableCollider(!done);
        }

        public void EnableCollider(bool Enable)
        {
            selfCollider.enabled = Enable;
        }

#if UNITY_EDITOR

        public void OnDrawGizmosSelected()
        {
            if (UnityEditor.Selection.Contains(this.gameObject))
            {
                Vector3 CameraPosition = transform.position - (Quaternion.AngleAxis(XAngle, Vector3.up) * Quaternion.AngleAxis(YAngle, Vector3.right) * Vector3.forward * DistanceToTarget);

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(CameraPosition, 0.1f);
                Gizmos.DrawLine(transform.position, CameraPosition);

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(transform.position + ScrewdriverFinishOffset, 0.1f);
                Gizmos.DrawLine(transform.position, transform.position + ScrewdriverFinishOffset);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position + ScrewdriverStartOffset, 0.1f);
                Gizmos.DrawLine(transform.position + ScrewdriverFinishOffset, transform.position + ScrewdriverStartOffset);
                Gizmos.color = Color.white;
            }
        }

#endif
    }
}