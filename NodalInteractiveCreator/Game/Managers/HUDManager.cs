// Machinika Museum
// © Littlefield Studio
// Writted by Rémi Carreira - 22/02/2016
//
// HUDManager.cs

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace NodalInteractiveCreator.HUD
{
    public class HUDManager : MonoBehaviour
    {
        public static HUDManager Instance { get { return HUDManager.instance; } }
        private static HUDManager instance = null;

        public GraphicRaycaster Raycaster = null;
        public InfoBox InfoBox = null;
        public ToolsHUD ToolsHUD = null;
        public ItemsHUD ItemHUD = null;
        public Tuto Tuto = null;

        private void Awake()
        {
            instance = this;
        }

        public static GraphicRaycaster GetGraphicRaycaster()
        {
            if (null != HUDManager.Instance)
            {
                return HUDManager.Instance.Raycaster;
            }
            return null;
        }

        public static InfoBox GetInfoBox()
        {
            if (null != HUDManager.Instance)
            {
                return HUDManager.Instance.InfoBox;
            }
            return null;
        }

        public static ToolsHUD GetToolsHUD()
        {
            if (null != HUDManager.Instance)
            {
                return HUDManager.Instance.ToolsHUD;
            }
            return null;
        }

        public static ItemsHUD GetItemHUD()
        {
            if (null != HUDManager.Instance)
            {
                return HUDManager.Instance.ItemHUD;
            }
            return null;
        }

        public static Tuto GetTuto()
        {
            if (null != HUDManager.Instance)
            {
                return HUDManager.Instance.Tuto;
            }
            return null;
        }

    }
}