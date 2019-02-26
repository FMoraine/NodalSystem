// Machinika Museum Prototype v.01
// © Littlefield Studio
// Writted by Rémi Carreira - 2015
//
// DigitDatabase.cs

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class DigitDatabase : ScriptableObject
{
    public List<DigitData> Digits = new List<DigitData>();

    public  DigitData       FindDigit(int Number)
    {
        foreach (DigitData iterator in Digits)
        {
            if (null != iterator && iterator._number == Number)
            {
                return iterator;
            }
        }
        return null;
    }

#region Editor
    #if UNITY_EDITOR

    private static string defaultName   = "Database";
    private static string extension     = "asset";
    private static string folder        = "";
    private static string parentFolder  = "Assets";

    private static string FilePath      { get { return parentFolder + '/' + defaultName + '.' + extension; } }
    private static string FolderPath    { get { return parentFolder + '/' + folder; } }

    [MenuItem("Machinika/Create/Database/Digit")]
    public static DigitDatabase Create()
    {
        DigitDatabase asset = ScriptableObject.CreateInstance<DigitDatabase>();
        if (null != asset)
        {
            if (false == AssetDatabase.IsValidFolder(FolderPath))
            {
                AssetDatabase.CreateFolder(parentFolder, folder);
            }

            AssetDatabase.CreateAsset(asset, FilePath);
            AssetDatabase.SaveAssets();
            return asset;
        }
        return null;
    }

    #endif
#endregion
}
