using System;

namespace NodalInteractiveCreator.Objects.Puzzle
{
    [Serializable]
    public class PuzzleAction
    {
        public string keyName = "action lambda";
        public int subjectsFlag = 0;
        public float angleMove = 0;
        public int uniqueID = -1;
    }
}
