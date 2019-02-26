using System.Collections.Generic;
using UnityEngine;

namespace NodalInteractiveCreator.Objects.Puzzle
{
    public partial class PuzzleSystem
    {        
        public static bool displayElements = true;
        public static bool displayConnectors = false;

        void OnDrawGizmos()
        {
            if (playGrid == null || Application.isPlaying)
                return;

            RecalculateRotation();

            DisplayGridElements();


        }

        void DisplayGridElements()
        {
            Vector3 o = t.position;

            for (var i = 0; i < gridSize.x * gridSize.y; i++)
            {
               PuzzleCell cell = playGrid[i];
               if (displayElements)
                {

                    int size = playGrid[i].content.Count;
                    for (int x = 0; x < size; x++)
                    {
                        PuzzleObjectDesc objDesc = elements[playGrid[i].content[x].IDdesc];
                        Gizmos.color = objDesc.colorTag;
                      
                        if (objDesc.element)
                        {
                            MeshFilter m = objDesc.element.GetComponent<MeshFilter>();
                            if (m)
                                Gizmos.DrawWireMesh(m.sharedMesh, cell.worldPos + -T.up * tileSize / 2 * x, cell.worldOrientation, Vector3.one * tileSize / (x + 1));
                            else
                                Gizmos.DrawSphere(cell.worldPos + -T.up * tileSize / 2 * x, tileSize / (x + 2) / 1.2f);
                        }
                        else
                        {
                            Gizmos.DrawSphere(cell.worldPos + -T.up * tileSize / 2 * x, tileSize / (x + 2) / 1.2f);
                        }
                        
                    }
                }

                Gizmos.color = Color.green;
                Gizmos.DrawCube(cell.worldPos, Vector3.one * (tileSize / 5));
                DisplayCellConnection(cell);
            }
        }

        void DisplayCellConnection(PuzzleCell cell)
        {
            return;

            if (!displayConnectors)
                return;

            Vector3 o = t.position;
            List<ConnexionPath> listPath = cell.paths;

            for (int i = 0; i < listPath.Count; i++)           
            {
                int index = Machinika.Objects.Puzzle.PuzzleMath.GtoI(listPath[i].gridPos, gridSize);

                if (playGrid.Length <= index || index < 0)
                    continue;
                
                
                Gizmos.color = Color.Lerp(Color.red, Color.blue, (float)i / listPath.Count);
              
            
            }
        }
    }
}
