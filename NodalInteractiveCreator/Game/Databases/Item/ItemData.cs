// Machinika Museum Prototype v.01
// © Littlefield Studio
// Writted by Rémi Carreira - 2015
//
// Edited by Franck-Olivier FILIN - 2017/2018
//
// ItemData.cs

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ItemData
{
    public string _name = "Item";
    [Range(1, 4)]
    public int _numberMax = 1;
    public bool _printable = false;
    public Sprite _iconInventory = null;
    public GameObject _objectInGame = null;
    public GameObject _objectInInventory = null;
    public int _idDescription = 0;
}

