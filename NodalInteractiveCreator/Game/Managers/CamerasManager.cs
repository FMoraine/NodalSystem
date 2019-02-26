using UnityEngine;
using System.Collections;

namespace NodalInteractiveCreator.Managers
{
    public class CamerasManager : MonoBehaviour
    {
        public enum IndexCamera
        {
            NONE = 0,
            MAIN_CAMERA,
            INVENTORY_CAMERA,
            CUTSCENE_CAMERA,
            //ENDO_CAMERA
        }

        public Camera MainCamera = null;
        public Camera InventoryCamera = null;
        public Camera ScreenShotCamera = null;
        public Camera CutsceneCamera = null;

        private IndexCamera currentCamera = IndexCamera.NONE;

        void Awake()
        {
            //Disable, Only used in the screenshot function
            if (null != ScreenShotCamera)
            {
                ScreenShotCamera.gameObject.SetActive(false);
            }

            //Default, Set to main camera
            ChangeCamera(IndexCamera.CUTSCENE_CAMERA);
        }

        public void ScreenShot(RenderTexture Texture, Vector3 Position, Quaternion Rotation)
        {
            if (null != ScreenShotCamera)
            {
                ScreenShotCamera.fieldOfView = MainCamera.fieldOfView;
                ScreenShotCamera.transform.position = Position;
                ScreenShotCamera.transform.rotation = Rotation;

                ScreenShotCamera.targetTexture = Texture;
                ScreenShotCamera.gameObject.SetActive(true);

                ScreenShotCamera.Render();

                ScreenShotCamera.gameObject.SetActive(false);
                ScreenShotCamera.targetTexture = null;
            }
        }

        public void ChangeCamera(IndexCamera Index)
        {
            currentCamera = Index;

            if (null != MainCamera)
            {
                MainCamera.enabled = IndexCamera.MAIN_CAMERA == Index;
                //MainCamera.gameObject.SetActive(IndexCamera.MAIN_CAMERA == Index);
            }
            if (null != InventoryCamera)
            {
                InventoryCamera.enabled = IndexCamera.INVENTORY_CAMERA == Index;
                //InventoryCamera.gameObject.SetActive(IndexCamera.INVENTORY_CAMERA == Index);
            }
            if (null != CutsceneCamera)
            {
                CutsceneCamera.enabled = IndexCamera.CUTSCENE_CAMERA == Index;
                //CutsceneCamera.gameObject.SetActive(IndexCamera.CUTSCENE_CAMERA == Index);
            }

        }

        public Camera GetCurrentCamera()
        {
            switch (currentCamera)
            {
                case IndexCamera.MAIN_CAMERA:
                    return MainCamera;
                case IndexCamera.INVENTORY_CAMERA:
                    return InventoryCamera;
                case IndexCamera.CUTSCENE_CAMERA:
                    return CutsceneCamera;
            }
            return null;
        }
    }
}