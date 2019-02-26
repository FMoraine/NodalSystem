using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NodalInteractiveCreator.Objects;
using UnityEngine;

namespace NodalInteractiveCreator.Endoscop
{
    public class Joystick3D : TouchableElement
    {
        [SerializeField] private bool horizontal = true;
        [SerializeField] private bool vertical = true;
        [SerializeField] private Transform stick;
        [SerializeField] private float radius = 1f;
        [HideInInspector]
        public Vector2 inputValue;

        private Vector3 origin;
        private Vector3 originLocal;
        private Plane trackPlan;

        public float smoothT = 0.3f;
        private Vector3 toStick;
        private Vector3 pos;
        private Vector3 velocity;

        protected override void Awake()
        {

            originLocal = stick.localPosition;
            origin = new  Vector3(transform.localScale.z * stick.localPosition.z, transform.localScale.y * stick.localPosition.y, transform.localScale.z * stick.localPosition.z);
        }

        public override void DeselectObject()
        {
            base.DeselectObject();
            toStick = originLocal;
            SetInputPos(0);		
        }

        public override void SelectObject(Camera Camera, Vector3 InputPosition)
        {

            base.SelectObject(Camera, InputPosition);
            toStick = originLocal;
            SetInputPos(0 );		

        }

        void Update()
        {
            stick.localPosition =
                Vector3.SmoothDamp(stick.transform.localPosition, toStick, ref velocity, smoothT);
        }

        public override void MoveObject(Vector3 InputPosition)
        {
            base.MoveObject(InputPosition);
            InputPosition.z = 1f;
            InputPosition = _mainCamera.ScreenToWorldPoint(InputPosition);

            Vector3 worldOrigin = transform.position +  transform.rotation * origin;
            trackPlan = new Plane(transform.up, worldOrigin);

            Vector3 dir = (InputPosition - _mainCamera.transform.position).normalized;
            Ray r = new Ray(_mainCamera.transform.position , dir );

            float enter = 0;
            trackPlan.Raycast(r, out enter);
            
            pos = _mainCamera.transform.position + dir * enter;

            Vector3 fromOrigin = pos - worldOrigin;
           
            float magnitude = fromOrigin.magnitude;

            if (magnitude <= radius)
                toStick = transform.InverseTransformPoint(pos);
            else
            {
                toStick = transform.InverseTransformPoint(worldOrigin + fromOrigin.normalized * radius);
               
                magnitude = radius;
            }

            if(!horizontal)
                toStick = new Vector3(originLocal.x , toStick.y , toStick.z);
            if (!vertical)
                toStick = new Vector3(toStick.x, toStick.y, originLocal.z);

            SetInputPos(magnitude);
        }
         
        void SetInputPos(float intensity)
        {
            Vector3 fromOriginLocal = toStick - originLocal;

            Vector3 inputValue3d = fromOriginLocal.normalized * (intensity / radius);
            inputValue = new Vector2(inputValue3d.x, inputValue3d.z);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            if(stick)
                Gizmos.DrawWireSphere(stick.position , radius);
            if(Application.isPlaying)
                Gizmos.DrawWireSphere( pos, 0.2f);
            Gizmos.color = Color.white;
        }
    }
}
