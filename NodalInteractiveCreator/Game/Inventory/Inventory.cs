// Machinika Museum Prototype v.01
// © Littlefield Studio
// Writted by Rémi Carreira - 2015
//
// Inventory.cs

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using NodalInteractiveCreator.HUD;
using NodalInteractiveCreator.Controllers;
using NodalInteractiveCreator.Database;

namespace NodalInteractiveCreator.Inventory
{
    public class Inventory : MonoBehaviour
    {
        public static Inventory Instance = null;

        //      ToolsDatabase
        public ItemDatabase ItemDatabase = null;
        public DigitDatabase DigitDatabase = null;
        public InspectSystem InspectSystem = null;

        //List  ToolsSlot
        public List<ItemSlot> ItemSlots = new List<ItemSlot>();

        void Awake()
        {
            Instance = this;

            if (InspectSystem == null)
                InspectSystem = FindObjectOfType<InspectSystem>();
        }

        public bool AddItem(int ItemId)
        {
            ItemSlot slot = FindItemSlot(ItemId);
            if (null == slot || false == slot.IsUsed())
            {
                ItemSlot secondSlot = FindFreeItemSlot();
                if (null != secondSlot)
                {
                    secondSlot.SetItemId(ItemId);
                    secondSlot.SetNbItem(1);
                    return true;
                }
            }
            else
            {
                if (slot.GetNbItem() < 1)
                {
                    slot.SetItemId(ItemId);
                    slot.SetNbItem(1);
                }
                else
                {
                    slot.DigitImage.GetComponent<Animation>().enabled = true;
                    slot.DigitImage.GetComponent<Animation>().Play();
                    slot.SetNbItem(slot.GetNbItem() + 1);
                }
                return true;
            }
            return false;
        }

        public void RemoveItem(int ItemId)
        {
            ItemSlot slot = FindItemSlot(ItemId);
            if (null != slot && true == slot.IsUsed())
            {
                int nbItem = slot.GetNbItem();
                if (nbItem > 1)
                {
                    slot.SetNbItem(nbItem - 1);
                }
                else
                {
                    slot.SetItemId(-1);
                    slot.SetNbItem(-1);
                }
            }
        }

        ItemSlot FindFreeItemSlot()
        {
            foreach (ItemSlot iterator in ItemSlots)
            {
                if (null != iterator && false == iterator.IsUsed())
                {
                    return iterator;
                }
            }
            return null;
        }

        ItemSlot FindItemSlot(int ItemId)
        {
            foreach (ItemSlot iterator in ItemSlots)
            {
                if (null != iterator && iterator.GetItemId() == ItemId)
                {
                    return iterator;
                }
            }
            return null;
        }

        public ItemData GetItem(int ItemId)
        {
            return ItemDatabase.FindItem(ItemId);
        }
    }
}