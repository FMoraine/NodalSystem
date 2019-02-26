// Machinika Museum
// © Littlefield Studio
// Writted by Rémi Carreira - 2016
//
// SpecificRotations.cs

using System.Collections.Generic;
using UnityEngine;

namespace NodalInteractiveCreator.Objects
{
    [RequireComponent(typeof(RotatableObject))]
    public class SpecificRotations : MonoBehaviour
    {
        public bool Snap = false;
        public float SnapDuration = 1f;
        public float Precision = 1f;

        [SerializeField]
        private List<float> SpecificAngles = new List<float>();

        private int indexAngle = -1;
        private float deltaAngle = 0f;

        public bool CheckSpecificAngle(float Angle)
        {
            for (indexAngle = 0; indexAngle < SpecificAngles.Count; ++indexAngle)
            {
                deltaAngle = Mathf.DeltaAngle(SpecificAngles[indexAngle], Angle);

                if (deltaAngle >= -Precision && deltaAngle <= Precision)
                {
                    return true;
                }
            }
            return false;
        }

        public int GetIndex()
        {
            return indexAngle;
        }

        public float GetAngleAt(int Index)
        {
            if (Index < SpecificAngles.Count)
            {
                return SpecificAngles[Index];
            }
            return 0.0f;
        }

        public float GetDeltaAngle()
        {
            return deltaAngle;
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            for (int index = 0; index < SpecificAngles.Count; ++index)
            {
                SpecificAngles[index] = Angles.ClampAngle(SpecificAngles[index]);
                if (SpecificAngles[index] < -180f)
                {
                    SpecificAngles[index] += 360f;
                }
                else if (SpecificAngles[index] > 180.0f)
                {
                    SpecificAngles[index] -= 360.0f;
                }
            }
        }

#endif
    }


}
