using System.Collections.Generic;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using UnityEngine;

namespace NodalInteractiveCreator.Objects.Puzzle
{
    public partial class PuzzleSystem : InteractiveObject
    {
        public int idCount;
        [Versioning("gridSize")]
        public Vector2Integer gridSize;
        [Versioning("tileSize")]
        public float tileSize = 2;

        public static Vector2Integer minGridSize = new Vector2Integer(2, 2);
        public static Vector2Integer maxGridSize = new Vector2Integer(50, 50);

        [Versioning("elements")]
        public List<PuzzleObjectDesc> elements = new List<PuzzleObjectDesc>();
        [Versioning("actions")]
        public List<PuzzleAction> actions = new List<PuzzleAction>();

        public PuzzleCell[] playGrid;

        protected PuzzleObject[] elementsOnBoard;

        public Transform t;

        Transform T
        {
            get
            {
                if (!t) t = GetComponent<Transform>();
                return t;
            }
        }

        protected override void Awake()
        {
            t = transform;
            base.Awake();
            InitializeGrid();

            for (int i = 0; i < elementsOnBoard.Length; i++)
                elementsOnBoard[i].PlayFeedback(0);
        }

        void InitializeGrid()
        {
            if (playGrid == null)
                return;
            int elementCount = 0;
            for (var i = 0; i < gridSize.x * gridSize.y; i++)
            {
                PuzzleCell cell = playGrid[i];

                int size = cell.content.Count;


                for (int x = 0; x < size; x++)
                {
                    InitNewObject(cell.content, cell);

                    PuzzleObjectDesc pObj = elements[cell.content[x].IDdesc];
                    if (pObj.element)
                    {
                        InteractivePuzzleElement iPE = Instantiate(elements[cell.content[x].IDdesc].element, transform);

                        iPE.name += "_" + elementCount;
                        iPE.transform.localPosition = cell.localPosition;
                        iPE.transform.localRotation = cell.Orientation;

                        cell.content[x].AssociateInteractive(iPE);
                        cell.content[x].OnCompleteAction += OnInteractiveAction;

                    }

                    elementCount++;
                }

                InitCellLinks(cell);
            }


            elementsOnBoard = new PuzzleObject[elementCount];
            elementCount = 0;
            for (var i = 0; i < gridSize.x * gridSize.y; i++)
                foreach (var obj in playGrid[i].content)
                    elementsOnBoard[elementCount++] = obj;

        }

        public void OnGridSizeChange()
        {
            gridSize = new Vector2Integer(Mathf.Clamp(gridSize.x, minGridSize.x, maxGridSize.x), Mathf.Clamp(gridSize.y, minGridSize.y, maxGridSize.y));

            ClearPlayGrid();

        }

        public void OnValidate()
        {
            tileSize = Mathf.Clamp(tileSize, 0.4f, 20);
            gridSize = new Vector2Integer(Mathf.Clamp(gridSize.x, minGridSize.x, maxGridSize.x), Mathf.Clamp(gridSize.y, minGridSize.y, maxGridSize.y));
        }

        void ClearPlayGrid()
        {
            t = transform;
            idCount = 0;
            PuzzleCell[] newGrid = new PuzzleCell[(int)(gridSize.x * gridSize.y)];


            for (var i = 0; i < gridSize.x * gridSize.y; i++)
            {
                PuzzleCell p;

                if (playGrid != null && i < playGrid.Length && playGrid[i] != null)
                    p = playGrid[i];
                else
                    p = new PuzzleCell(new List<PuzzleObject>(), PuzzleMath.ItoG(i, gridSize))
                    {
                        localPosition = PuzzleMath.ItoW(i, gridSize, tileSize)
                    };

                p.SetOrientation(Quaternion.identity, transform.rotation);
                p.paths.Clear();
                newGrid[i] = p;

            }
            playGrid = new PuzzleCell[(int)(gridSize.x * gridSize.y)];
            int size = (int)(gridSize.x * gridSize.y);
            for (int i = 0; i < size; i++)
            {
                playGrid[i] = newGrid[i];
            }


            ClearEmptyConnections();
        }

        public void ClearEmptyConnections(PuzzleCell cell)
        {
            for (int i = cell.paths.Count - 1; i >= 0; i--)
            {
                int index = PuzzleMath.GtoI(cell.paths[i].gridPos, gridSize);
                if (index >= playGrid.Length || index < 0)
                    cell.paths.RemoveAt(i);
            }
        }

        public void SetCircleMode(float offsetCenter, float angleCurve = 0)
        {

            float anglePerX = 360.0f / gridSize.x;

            var adjustedForward = Quaternion.AngleAxis(angleCurve, Vector3.right) * Vector3.forward;

            for (int i = 0; i < gridSize.x * gridSize.y; i++)
            {
                var cell = playGrid[i];
                var gridPos = PuzzleMath.ItoG(i, gridSize);
                cell.paths.Clear();

                var localPos = Quaternion.AngleAxis(anglePerX * gridPos.x, Vector3.up) * adjustedForward *
                               (offsetCenter + tileSize * (gridSize.y - gridPos.y - 1));
                cell.localPosition = localPos;
                var forward = (Vector3.zero - localPos).normalized;

                var right = new Vector2Integer(gridPos.x >= gridSize.x - 1 ? 0 : gridPos.x + 1, gridPos.y);
                cell.AddConnection(90, 70, right, new BezierCurve(Vector3.left, Vector3.right));

                var left
                    = new Vector2Integer(gridPos.x - 1 < 0 ? gridSize.x - 1 : gridPos.x - 1, gridPos.y);
                cell.AddConnection(270, 70, left, new BezierCurve(Vector3.right, Vector3.left));

                var top = new Vector2Integer(gridPos.x, gridPos.y == 0 ? gridSize.y - 1 : gridPos.y - 1);
                cell.AddConnection(0, 70, top, BezierCurve.Linear);

                var bottom = new Vector2Integer(gridPos.x, gridPos.y >= gridSize.y - 1 ? 0 : gridPos.y + 1);
                cell.AddConnection(180, 70, bottom, BezierCurve.Linear);

                var pRight = Vector3.Cross(forward, Vector3.up);

                cell.SetOrientation(Quaternion.LookRotation(forward, Vector3.Cross(pRight, forward)),
                    transform.rotation);
            }

            ClearEmptyConnections();
        }

        public void SetGridMode()
        {
            for (int i = 0; i < gridSize.x * gridSize.y; i++)
            {
                playGrid[i].paths.Clear();
                Vector2Integer gridPos = PuzzleMath.ItoG(i, gridSize);

                playGrid[i].localPosition = PuzzleMath.ItoW(i, gridSize, tileSize);

                playGrid[i].AddConnection(0, 70, new Vector2Integer(gridPos.x, gridPos.y - 1), BezierCurve.Linear);
                playGrid[i].AddConnection(90, 70, new Vector2Integer(gridPos.x + 1, gridPos.y), BezierCurve.Linear);
                playGrid[i].AddConnection(180, 70, new Vector2Integer(gridPos.x, gridPos.y + 1), BezierCurve.Linear);
                playGrid[i].AddConnection(270, 70, new Vector2Integer(gridPos.x - 1, gridPos.y), BezierCurve.Linear);
                playGrid[i].SetOrientation(Quaternion.identity, transform.rotation);
            }

            ClearEmptyConnections();
        }

        public void ClearEmptyConnections()
        {
            foreach (var cell in playGrid)
                ClearEmptyConnections(cell);
        }

        public void RecalculateRotation()
        {
            for (var i = 0; i < gridSize.x * gridSize.y; i++)
            {
                PuzzleCell cell = playGrid[i];
                cell.localRotPos = t.rotation * cell.localPosition;
                cell.worldPos = t.position + t.rotation * cell.localPosition;
                cell.worldOrientation = t.rotation * cell.Orientation;
            }
        }
    }
}