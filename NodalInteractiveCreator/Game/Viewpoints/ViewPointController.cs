// Machinika Museum Prototype v.01
// © Littlefield Studio
// Writted by Franck-Olivier FILIN - 2017
//
// ViewPointController.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodalInteractiveCreator.HUD;
using NodalInteractiveCreator.Objects;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NodalInteractiveCreator.Viewpoints
{
    public abstract class ViewPointController : MonoBehaviour
    {
        private static List<ViewPoint> _listAllViewpoints = new List<ViewPoint>();
        public static List<ViewPoint> ListAllViewpoints { get { return _listAllViewpoints; } set { _listAllViewpoints = value; } }

        void Start()
        {
            ListAllViewpoints.AddRange(FindObjectsOfType<ViewPoint>());

            if (Tuto._isTuto)
                LockAllVp();
        }

        public static void LockAllVp()
        {
            foreach (ViewPoint vp in ListAllViewpoints)
                vp.LockViewpoint();
        }

        public static void UnlockAllVp()
        {
            foreach (ViewPoint vp in ListAllViewpoints)
                vp.UnlockViewpoint();
        }

        #region Editor

#if UNITY_EDITOR

        #region MainVP
        [MenuItem("Machinika/Create/Viewpoint/Main Viewpoint")]
        static void InitMain()
        {
            GameObject parentObj = GameObject.Find("Viewpoints");

            int option;
            if (Selection.activeGameObject)
            {
                option = EditorUtility.DisplayDialogComplex("Create a new main viewpoint",
                    "Do you want create a new 'Main Viewpoint' ?",
                    "With linking",
                    "Cancel",
                    "Without linking");
            }
            else
            {
                option = 2;
            }
            
            switch (option)
            {
                // With linking
                case 0:
                    Create(parentObj, Selection.activeGameObject);
                    break;

                // Cancel.
                case 1:
                    Debug.Log("The 'Viewpoint' creation has been canceled.");
                    break;

                // Without linking
                case 2:

                    Create(parentObj, null);
                    break;

                default:
                    Debug.LogError("Main Viewpoint Error.");
                    break;
            }
        }
        #endregion

    #region SubVP
        [MenuItem("Machinika/Create/Viewpoint/Sub Viewpoint")]

        static void InitSub()
        {
            GameObject parentObj = null;
            int option;
            if (Selection.activeGameObject)
            {
                parentObj = Selection.activeGameObject;
                option = EditorUtility.DisplayDialogComplex("Create a new Sub viewpoint",
                    "Do you want create a new 'Sub Viewpoint' ?",
                    "With linking",
                    "Cancel",
                    "Without linking");
            }
            else
            {
                EditorUtility.DisplayDialog("Sub Viewpoint Error", "Can't create sub viewpoint because don't selected a GameObject in scene", "Ok");
                option = -1;
            }

            switch (option)
            {
                // With linking
                case 0:
                    Create(parentObj, Selection.activeGameObject);
                    break;

                // Cancel.
                case 1:
                    Debug.Log("The 'Viewpoint' creation has been canceled.");
                    break;

                // Without linking
                case 2:

                    Create(parentObj, null);
                    break;

                default:
                    Debug.LogError("Sub Viewpoint Error.");
                    break;
            }
        }
        #endregion

    #region Create
        public static GameObject Create(GameObject parent, GameObject selected)
        {
            if (null == Camera.main)
            {
                EditorUtility.DisplayDialog("Viewpoint Error", "Can't create viewpoint because don't have camera in scene", "Ok");
                return null;
            }
            
            GameObject viewpointObject = CreateViewpoint(parent, selected);

            if (null != viewpointObject)
            {
                Selection.activeGameObject = viewpointObject;
            }
            return viewpointObject;
        }

        private static GameObject CreateViewpoint(GameObject parent, GameObject linkPart)
        {
            GameObject viewpointObject = new GameObject();
            Vector3 targetPos = linkPart != null ? linkPart.transform.position : parent.transform.position;
            Vector3 cameraPos = new Vector3(targetPos.x, targetPos.y + 15, targetPos.z);

            if (null != viewpointObject)
            {
                viewpointObject.tag = Tags.Viewpoint;
                viewpointObject.name = linkPart == null ? "New_Viewpoint" : linkPart.name + "_Viewpoint";

                viewpointObject.transform.SetParent(parent.transform);

                viewpointObject.transform.position = targetPos ;

                Collider boxCollider = viewpointObject.AddComponent<BoxCollider>();
                if (null != boxCollider)
                {
                    boxCollider.isTrigger = true;
                }
                else
                {
                    EditorUtility.DisplayDialog("Viewpoint Error", "Failed to add box collider in viewpoint object", "Ok");
                    Destroy(viewpointObject);
                    return null;
                }

                ViewPoint viewpointComponent = viewpointObject.AddComponent<ViewPoint>();
                if (null != viewpointComponent)
                {
                    if(null != linkPart)
                        viewpointComponent.PartsLinked.Add(linkPart);

                    viewpointComponent.TargetPosition = targetPos;
                    viewpointComponent.CameraPosition = cameraPos;
                }
                else
                {
                    EditorUtility.DisplayDialog("Viewpoint Error", "Failed to add viewpoint script in viewpoint object", "Ok");
                    Destroy(viewpointObject);
                    return null;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Viewpoint Error", "Create viewpoint object failed", "Ok");
            }
            return viewpointObject;
        }
#endregion

#endif

        #endregion
    }
}