// Machinika Museum Prototype v.01
// © Littlefield Studio
// Writted by Rémi Carreira - 2015
//
// Edited by Franck-Olivier FILIN - 2017
//
// InspectSystem.cs

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NodalInteractiveCreator.Managers;
using NodalInteractiveCreator.Controllers;
using NodalInteractiveCreator.HUD;

namespace NodalInteractiveCreator.Inventory
{
    public class InspectSystem : MonoBehaviour
    {
        public Image InspectBackground = null;
        public RawImage ScreenShotBackground = null;
        public RenderTexture ScreenShotTexture = null;
        public Transform ObjectPlaceHolder = null;
        public ReplayButton ReplayButton = null;
        public InfoBox InfoBox = null;

        private static bool inspecting = false;

        private int itemId = -1;
        private GameObject objectInstantiate = null;
        private CamerasManager camerasMgr = null;

        void Awake()
        {
            if (ReplayButton == null)
                ReplayButton = FindObjectOfType<ReplayButton>();

            if (InfoBox == null)
                InfoBox = FindObjectOfType<InfoBox>();

            if (null != ScreenShotBackground)
            {
                ScreenShotBackground.texture = ScreenShotTexture;
            }

            EnableBackgrounds(false);
        }

        void Start()
        {
            camerasMgr = GameManager.GetCamerasManager();
        }

        void EnableBackgrounds(bool Enable)
        {
            if (null != InspectBackground)
            {
                InspectBackground.gameObject.SetActive(Enable);
            }
            if (null != ScreenShotBackground)
            {
                ScreenShotBackground.gameObject.SetActive(Enable);
            }
        }

        void InstantiateObject(GameObject Prefab)
        {
            if (null == objectInstantiate)
            {
                objectInstantiate = Instantiate(Prefab, ObjectPlaceHolder);
                objectInstantiate.layer = LayerMask.NameToLayer("InspectSystem");

                if (null == objectInstantiate.GetComponent<Collider>())
                    objectInstantiate.AddComponent<BoxCollider>().isTrigger = true;
                else
                    objectInstantiate.GetComponent<Collider>().isTrigger = true;

                objectInstantiate.AddComponent<ItemBox>();
            }
            else
            {
                DestroyImmediate(objectInstantiate);
                InstantiateObject(Prefab);
            }

            InfoBox._closeAllow = true;
            InfoBox.HideInfo();
        }

        void DestroyObject()
        {
            if (null != objectInstantiate)
                Destroy(objectInstantiate);
        }

        void CreateScreenshot()
        {
            if (null != camerasMgr && null != camerasMgr.MainCamera)
            {
                Transform mainCameraTransform = camerasMgr.MainCamera.transform;
                if (null != mainCameraTransform)
                    camerasMgr.ScreenShot(ScreenShotTexture, mainCameraTransform.position, mainCameraTransform.rotation);
            }
        }

        void EnableInventoryCamera(bool Enable)
        {
            if (null != camerasMgr)
            {
                if (true == Enable)
                    camerasMgr.ChangeCamera(CamerasManager.IndexCamera.INVENTORY_CAMERA);
                else
                    camerasMgr.ChangeCamera(CamerasManager.IndexCamera.MAIN_CAMERA);
            }
        }

        public void LaunchItemInspection(int ItemId)
        {
            inspecting = true;
            itemId = ItemId;
            ReplayButton.GetComponent<Image>().sprite = ReplayButton._iconBackInventory;

            //GameManager.GetCamerasManager().InventoryCamera.GetComponent<CameraController>().MoveToViewpointCameraPosInit();

            CreateScreenshot();
            EnableInventoryCamera(true);
            EnableBackgrounds(true);

            if (GameManager.GetGameState()._myCutscene._showToolsHUD)
                HUDManager.GetToolsHUD().UnshowTools();

            DestroyObject();

            if (null != Inventory.Instance && null != Inventory.Instance.ItemDatabase)
            {
                ItemData data = Inventory.Instance.ItemDatabase.FindItem(ItemId);
                if (null != data && null != data._objectInInventory)
                {
                    InstantiateObject(data._objectInInventory);
                    objectInstantiate.GetComponent<ItemBox>()._myTranslate = GameManager.GetSentenceTranslate();
                    objectInstantiate.GetComponent<ItemBox>()._idSentence = data._idDescription;
                }
            }
        }

        public void CloseItemInspection()
        {
            inspecting = false;
            itemId = -1;
            ReplayButton.GetComponent<Image>().sprite = ReplayButton._iconQuit;

            InfoBox._closeAllow = true;
            InfoBox.HideInfo();

            EnableInventoryCamera(false);
            EnableBackgrounds(false);

            if (GameManager.GetGameState()._myCutscene._showToolsHUD)
                HUDManager.GetToolsHUD().ShowTools();

            GameManager.GetCamerasManager().InventoryCamera.GetComponent<CameraController>().MoveToViewpointCameraPosInit();

            DestroyObject();
        }

        public static bool IsInspecting()
        {
            return inspecting;
        }

        public int ItemInspecting()
        {
            return itemId;
        }
    }
}