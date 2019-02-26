using NodalInteractiveCreator.Managers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NodalInteractiveCreator.Objects.Puzzle
{
    public partial class PuzzleSystem
    {
        void InitCellLinks(PuzzleCell cell)
        {
            List<ConnexionPath> l = cell.paths;
            for(var i = l.Count -1; i >= 0; i--)
            {
                ConnexionPath path = l[i];
                int index = PuzzleMath.GtoI(path.gridPos, gridSize);

                if (playGrid.Length <= index || index < 0)
                {
                    cell.paths.RemoveAt(i);
                    continue;
                }

                BezierCurve b = path.bezierPath;
                path.destCell = playGrid[index];
                b.point1 = cell.localPosition;
                b.point2 = path.destCell.localPosition;
                b.curvePoint1 = cell.localPosition + b.curvePoint1;
                b.curvePoint2 = path.destCell.localPosition + path.destCell.Orientation * b.curvePoint2;
                path.bezierPath = b;
            }
        }

        void InitNewObject(List<PuzzleObject> l, PuzzleCell cell)
        {
            foreach (var puzzleObj in l)
                InitNewObject(puzzleObj, cell);
        }

        void InitNewObject(PuzzleObject obj, PuzzleCell cell)
        {
            obj.cell = cell;
            obj.desc = elements[obj.IDdesc];

        }

        public void StartAction(int idAction)
        {
            PuzzleAction action = GetActionByID(idAction);
           
            if (action == null)
                return;

            var objGroups = from obj in elementsOnBoard group obj by obj.IDdesc;
          
            foreach (IGrouping<int, PuzzleObject> objGroup in objGroups)
            {
                if (PuzzleMath.IDIsInMask(objGroup.Key , action.subjectsFlag))
                    foreach (var obj in objGroup)
                    {
                        obj.TryMove(action.angleMove);
                    }
            }
        }

        PuzzleAction GetActionByID(int id)
        {
            foreach (var action in actions)
                if (action.uniqueID == id)
                    return action;

            return null;
        }

        void OnInteractiveAction(PuzzleObject obj)
        {
            CheckPuzzleComplete();
        }

        void CheckPuzzleComplete()
        {
            var objGroupss = from obj in elementsOnBoard
                where obj.desc.isTrigger
                select obj;

            var objGroups = from obj in objGroupss group obj by obj.desc.victoryLayer;

            foreach (IGrouping<int, PuzzleObject> objGroup in objGroups)
            {
                bool victory = true;
                foreach (var obj in objGroup)
                {
                    if (!obj.IsValid())
                        victory = false;
                }

                if (victory)
                {
                    GameManager.GetInputsManager().LockPinch = true;
                    GameManager.GetInputsManager().LockInteractivity = true;

                    foreach (var obj in elementsOnBoard)
                    {
                        if(!obj.desc.isLogical)
                        {
                            //Debug.Log("Play Feedback Validate ! " + obj.interactive.name);
                            obj.PlayFeedback(4);
                        }
                    }

                    act_Start.Invoke();
                    //Debug.Log("Puzzle : " + gameObject.name + " is complete");
                    return;
                }
            }
        }
    }
}