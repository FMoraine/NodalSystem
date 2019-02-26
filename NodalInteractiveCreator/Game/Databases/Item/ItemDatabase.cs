// Machinika Museum Prototype v.01
// © Littlefield Studio
// Writted by Rémi Carreira - 2015
//
// ItemDatabase.cs

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using NodalInteractiveCreator.Controllers;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NodalInteractiveCreator.Database
{
    public class ItemDatabase : ScriptableObject
    {
        public SentenceTranslate _sentenceTranslate;
        public List<ItemData> _items = new List<ItemData>();

        public ItemData FindItem(int itemId)
        {
            foreach (ItemData iterator in _items)
            {
                if (null != iterator && _items.IndexOf(iterator) == itemId)
                {
                    return iterator;
                }
            }
            return null;
        }

        public List<string> GetAllItemsName()
        {
            List<string> _itemsName = new List<string>();
            foreach (ItemData itemData in _items)
                _itemsName.Add(itemData._name);

            return _itemsName;
        }

        public void CreateItem(string name, bool printable, int nbMax, Sprite iconInv, GameObject objInGame, GameObject objInInv)
        {
            ItemData newItem = new ItemData();

            newItem._name = name != null ? name : "New_Item";
            newItem._printable = printable;
            newItem._numberMax = nbMax;
            newItem._iconInventory = iconInv;
            newItem._objectInGame = objInGame != null ? objInGame : objInInv;
            newItem._objectInInventory = objInInv != null ? objInInv : objInGame;

            _items.Add(newItem);

#if UNITY_EDITOR
            EditorUtility.DisplayDialog("Create New Item Success", "SUCCESS !", "Ok");
#endif
        }

        #region Editor
#if UNITY_EDITOR

        [CustomEditor(typeof(ItemDatabase))]
        [CanEditMultipleObjects]
        public class ItemDatabaseEditor : Editor
        {
            public ItemDatabase _myItemDatabase;

            private static string defaultName = "Database";
            private static string extension = "asset";
            private static string folder = "";
            private static string parentFolder = "Assets";

            private static string FilePath { get { return parentFolder + '/' + folder + '/' + defaultName + '.' + extension; } }
            private static string FolderPath { get { return parentFolder + '/' + folder; } }

            private void OnEnable()
            {
                _myItemDatabase = (ItemDatabase)target;
            }

            public override void OnInspectorGUI()
            {
                _myItemDatabase._sentenceTranslate = (SentenceTranslate)EditorGUILayout.ObjectField("Sentence Translate", _myItemDatabase._sentenceTranslate, typeof(SentenceTranslate), true);

                if (null != _myItemDatabase && _myItemDatabase._sentenceTranslate != null)
                {
                    serializedObject.Update();

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_items.Array.size"), new GUIContent("Items Size"), true);

                    foreach (ItemData item in _myItemDatabase._items)
                    {
                        int idItem = _myItemDatabase._items.IndexOf(item);

                        EditorGUI.indentLevel++;

                        if (EditorGUILayout.PropertyField(serializedObject.FindProperty("_items.Array.data[" + idItem + "]"), new GUIContent(item._name), false))
                        {
                            EditorGUI.indentLevel++;

                            EditorGUILayout.PropertyField(serializedObject.FindProperty("_items.Array.data[" + idItem + "]._name"), new GUIContent("Name"), false);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("_items.Array.data[" + idItem + "]._numberMax"), new GUIContent("Number Max"), false);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("_items.Array.data[" + idItem + "]._printable"), new GUIContent("Printable ?"), false);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("_items.Array.data[" + idItem + "]._iconInventory"), new GUIContent("Icon"), false);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("_items.Array.data[" + idItem + "]._objectInGame"), new GUIContent("Obj in Game"), false);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("_items.Array.data[" + idItem + "]._objectInInventory"), new GUIContent("Obj in Inventory"), false);

                            _myItemDatabase._sentenceTranslate.Load(_myItemDatabase._sentenceTranslate._file);

                            List<string> keyList = new List<string>();

                            foreach (SentenceTranslate.Row key in _myItemDatabase._sentenceTranslate.GetRowList())
                                keyList.Add(key.KEY);

                            item._idDescription = EditorGUILayout.Popup("Description", item._idDescription, keyList.ToArray());
                            EditorGUI.indentLevel--;
                        }

                        EditorGUI.indentLevel--;
                        EditorGUILayout.LabelField("--------------------------------------");
                    }

                    serializedObject.ApplyModifiedProperties();
                }
            }

            [MenuItem("Machinika/Create/Database/Machine")]
            public static ItemDatabase Create()
            {
                ItemDatabase asset = ScriptableObject.CreateInstance<ItemDatabase>();
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
        }
#endif
        #endregion
    }
}
