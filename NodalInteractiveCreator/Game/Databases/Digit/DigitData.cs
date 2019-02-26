// Machinika Museum Prototype v.01
// © Littlefield Studio
// Writted by Rémi Carreira - 2015
//
// DigitData.cs


using UnityEngine;
using System.Collections;

[System.Serializable]
public class DigitData
{
    [Range(1, 4)]
    public  int     _number  = 0;
    public  Sprite  _icon    = null;
}
