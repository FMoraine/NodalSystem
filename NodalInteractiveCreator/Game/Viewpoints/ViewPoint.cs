// Machinika Museum Prototype v.01
// © Littlefield Studio
// Writted by Rémi Carreira - 2015 - 2016 
// 
// Edited by Franck-Olivier FILIN - 2017
//
// ViewPoint.cs

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Machinika.Objects;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NodalInteractiveCreator.Viewpoints
{
    [RequireComponent(typeof(Collider))]
    public class ViewPoint : ViewPointController
    {
        public bool OnSelect { get; set; }

        [SerializeField]
        private bool _lockViewpointOnStart = false;
        public bool LockViewpointOnStart { get { return _lockViewpointOnStart; } set { _lockViewpointOnStart = value; } }

        [Header("Parts")]
        [SerializeField]
        private List<GameObject> _partsLinked = new List<GameObject>();
        /// <summary>
        /// Get or set the part linked at this viewpoint.
        /// </summary>
        public List<GameObject> PartsLinked { get { return _partsLinked; } set { _partsLinked = value; } }
        
        public Vector3 TargetPosition
        {
            get { return transform.position + transform.rotation * _targetPosLocal; }
            set { _targetPosLocal = Quaternion.Inverse(transform.rotation) * (value - transform.position); }
        }
        public Vector3 CameraPosition 
        {
            get { return transform.position + transform.rotation * _camPosLocal; }
            set { _camPosLocal = Quaternion.Inverse(transform.rotation) * (value - transform.position); }
        }

        [SerializeField] private Vector3 _camPosLocal;
        [SerializeField] private Vector3 _targetPosLocal;

        [Header("Angles Limits && FOV")]
        [Range(0, -180.0f)]
        public float MinXAngle = -90.0f;
        [Range(0, -180.0f)]
        public float MinYAngle = -90.0f;
        [Range(0, 180.0f)]
        public float MaxXAngle = 90.0f;
        [Range(0, 180.0f)]
        public float MaxYAngle = 90.0f;
        [Range(0, 180.0f)]
        public float FieldOfView = 60f;
        
        [Header("Lists")]
        public List<ViewPoint> SubViewpoints = new List<ViewPoint>();
        public List<TouchableElement> InteractiveObjects = new List<TouchableElement>();
        
        private Collider viewpointCollider = null;

        //public bool isActive = false;

        private void Awake()
        {
            enabled = false;

            if (GetComponent<Collider>() != null)
            {
                viewpointCollider = GetComponent<Collider>();
                viewpointCollider.isTrigger = true;
            }

            ListAllViewpoints.Add(this);
            InteractiveObjsInit();
            SubViewpointInit();
            //InitViewpointsPosition();
        }

        private void Start()
        {
        }

        private void OnEnable()
        {
            if(LockViewpointOnStart)
                this.enabled = false;
            else
                viewpointCollider.enabled = true;
        }

        private void OnDisable()
        {
            if (viewpointCollider != null)
                viewpointCollider.enabled = false;
        }

        public void UseViewpoint()
        {
            this.enabled = false;

            EnableSubViewpoints(true);
            EnableInteractiveObjects(true);

            //if (act_Start != null && OnSelect)
            //{
            //    act_Start.Invoke();
            //    OnSelect = false;
            //}
        }

        public void UnusedViewpoint()
        {
            EnableSubViewpoints(false);
            EnableInteractiveObjects(false);
        }

        public void LockViewpoint()
        {
            LockViewpointOnStart = true;
            this.enabled = false;
            viewpointCollider.enabled = false;
        }

        public void UnlockViewpoint()
        {
            LockViewpointOnStart = false;
            this.enabled = true;
            viewpointCollider.enabled = true;
        }

        public void InitViewpointsPosition()
        {
            if (_partsLinked.Count == 0)
                TargetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }

        public Quaternion GetCameraRotation()
        {
            return Quaternion.LookRotation(TargetPosition - CameraPosition);
        }

        public float GetDistance()
        {
            return Vector3.Distance(TargetPosition, CameraPosition);
        }

        private void EnableSubViewpoints(bool Enable)
        {
            foreach (ViewPoint viewpoint in SubViewpoints)
            {
                if (null != viewpoint)
                {
                    viewpoint.enabled = Enable;
                }
            }
        }

        private void EnableInteractiveObjects(bool Enable)
        {
            foreach (TouchableElement currentObject in InteractiveObjects)
            {
                if (null != currentObject)
                {
                    currentObject.enabled = Enable;
                }
            }
        }

        private void InteractiveObjsInit()
        {
            if (_partsLinked.Count > 0)
                foreach (GameObject part in _partsLinked)
                    foreach (TouchableElement interacObj in GetInteractiveObjs(part))
                        InteractiveObjects.Add(interacObj);
        }

        private List<TouchableElement> GetInteractiveObjs(GameObject obj)
        {
            List<TouchableElement> listGO = new List<TouchableElement>();

            if (obj.transform.GetComponent<TouchableElement>() != null)
                listGO.Add(obj.transform.GetComponent<TouchableElement>());

            if (obj.transform.childCount > 0)
            {
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    if (obj.transform.GetChild(i).childCount > 0)
                        listGO.AddRange(GetInteractiveObjs(obj.transform.GetChild(i).gameObject));
                    else if (obj.transform.GetChild(i).GetComponent<TouchableElement>() != null)
                        listGO.Add(obj.transform.GetChild(i).GetComponent<TouchableElement>());
                }
            }

            return listGO;    
        }

        private void SubViewpointInit()
        {
            if (_partsLinked.Count > 0)
                foreach (GameObject part in _partsLinked)
                    foreach (ViewPoint subViewpoint in GetSubViewpoint(part))
                       SubViewpoints.Add(subViewpoint);
        }

        private List<ViewPoint> GetSubViewpoint(GameObject obj)
        {
            List<ViewPoint> listGO = new List<ViewPoint>();

            if (obj.transform.GetComponent<ViewPoint>() != null)
                listGO.Add(obj.transform.GetComponent<ViewPoint>());

            for (int i = 0; i < obj.transform.childCount; i++)
            {
                if (obj.transform.GetChild(i).childCount > 0)
                    listGO.AddRange(GetSubViewpoint(obj.transform.GetChild(i).gameObject));
                else if (obj.transform.GetChild(i).GetComponent<ViewPoint>() != null)
                    listGO.Add(obj.transform.GetChild(i).GetComponent<ViewPoint>());
            }
            return listGO;
        }

        #region Editor

#if UNITY_EDITOR


        void OnDrawGizmosSelected()
        {
            if (Selection.activeGameObject == this.gameObject && Vector3.zero != (TargetPosition - CameraPosition))
            {
                Quaternion rotation = Quaternion.LookRotation(TargetPosition - CameraPosition);
                Vector3 angles = rotation.eulerAngles;
                float distance = Vector3.Distance(TargetPosition, CameraPosition);
                float sphereRadius = 0.5f;

                //Quaternion.
                Vector3 BottomPosition = TargetPosition - (Quaternion.Euler(angles.x + MinXAngle, angles.y, angles.z) * Vector3.forward * distance);
                Vector3 TopPosition = TargetPosition - (Quaternion.Euler(angles.x + MaxXAngle, angles.y, angles.z) * Vector3.forward * distance);
                Vector3 LeftPosition = TargetPosition - (Quaternion.Euler(angles.x, angles.y + MinYAngle, angles.z) * Vector3.forward * distance);
                Vector3 RightPosition = TargetPosition - (Quaternion.Euler(angles.x, angles.y + MaxYAngle, angles.z) * Vector3.forward * distance);

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(TargetPosition, sphereRadius);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(TargetPosition, CameraPosition);
                Gizmos.DrawSphere(CameraPosition, sphereRadius);
                
                

                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(TargetPosition, BottomPosition);
                Gizmos.DrawSphere(BottomPosition, sphereRadius);
                Gizmos.DrawLine(TargetPosition, TopPosition);
                Gizmos.DrawSphere(TopPosition, sphereRadius);

                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(TargetPosition, LeftPosition);
                Gizmos.DrawSphere(LeftPosition, sphereRadius);
                Gizmos.DrawLine(TargetPosition, RightPosition);
                Gizmos.DrawSphere(RightPosition, sphereRadius);
                
                //WiP
                Handles.color = new Color(1, 1, 1, 0.2f);
                //Handles.DrawSolidDisc(TargetPosition, Quaternion.Euler(angles.x, angles.y, angles.z) * Vector3.down, Vector3.Distance(TargetPosition, CameraPosition));
                //Handles.DrawSolidArc(TargetPosition, Quaternion.Euler(angles.x, angles.y, angles.z) * Vector3.down, Vector3.Cross(TargetPosition, CameraPosition), Vector3.Angle(Vector3.Cross(TargetPosition,CameraPosition),Vector3.Cross(TargetPosition, LeftPosition)), Vector3.Distance(TargetPosition, CameraPosition));
            }
        }

#endif

        #endregion 
        
    }
}