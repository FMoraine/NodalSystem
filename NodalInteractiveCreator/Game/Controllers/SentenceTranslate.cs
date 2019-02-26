using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodalInteractiveCreator.HUD;

namespace NodalInteractiveCreator.Controllers
{
    public class SentenceTranslate : MonoBehaviour
    {
        public TextAsset _file;
        List<Row> rowList = new List<Row>();
        bool isLoaded = false;

        private InfoBox _box;

        [System.Serializable]
        public class Row
        {
            public string ID;
            public string KEY;
            public string SENTENCE_FR;
            public string SENTENCE_EN;
        }

        private void Start()
        {
            if (null == _box)
                _box = HUDManager.GetInfoBox();

            Load(_file);
        }

        public void OpenBox(int idSentence)
        {
            string sentence;

            if (Application.systemLanguage == SystemLanguage.French)
                sentence = GetAt(idSentence).SENTENCE_FR;
            else
                sentence = GetAt(idSentence).SENTENCE_EN;

            _box.DisplayInfo(sentence);
        }

        public void OpenBox(string sentence)
        {
            _box.DisplayInfo(sentence);
        }

        public bool IsLoaded()
        {
            return isLoaded;
        }

        public List<Row> GetRowList()
        {
            return rowList;
        }

        public void Load(TextAsset csv)
        {
            rowList.Clear();
            string[][] grid = CsvParser2.Parse(csv.text);
            for (int i = 1; i < grid.Length; i++)
            {
                Row row = new Row();
                row.ID = grid[i][0];
                row.KEY = grid[i][1];
                row.SENTENCE_FR = grid[i][2];
                row.SENTENCE_EN = grid[i][3];

                rowList.Add(row);
            }
            isLoaded = true;
        }

        public int NumRows()
        {
            return rowList.Count;
        }

        public Row GetAt(int i)
        {
            if (rowList.Count <= i)
                return null;
            return rowList[i];
        }

        public Row Find_ID(string find)
        {
            return rowList.Find(x => x.ID == find);
        }
        public List<Row> FindAll_ID(string find)
        {
            return rowList.FindAll(x => x.ID == find);
        }
        public Row Find_KEY(string find)
        {
            return rowList.Find(x => x.KEY == find);
        }
        public List<Row> FindAll_KEY(string find)
        {
            return rowList.FindAll(x => x.KEY == find);
        }
        public Row Find_SENTENCE_FR(string find)
        {
            return rowList.Find(x => x.SENTENCE_FR == find);
        }
        public List<Row> FindAll_SENTENCE_FR(string find)
        {
            return rowList.FindAll(x => x.SENTENCE_FR == find);
        }
        public Row Find_SENTENCE_EN(string find)
        {
            return rowList.Find(x => x.SENTENCE_EN == find);
        }
        public List<Row> FindAll_SENTENCE_EN(string find)
        {
            return rowList.FindAll(x => x.SENTENCE_EN == find);
        }
    }
}
