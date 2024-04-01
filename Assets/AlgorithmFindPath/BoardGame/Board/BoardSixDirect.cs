using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoardGame
{
    public class BoardSixDirect : BoardGameBase
    {
        protected BoardSixDirect()
        {

        }

        public BoardSixDirect(int row, int colum)
        {
            _row = row;
            _colum = colum;
            Maps.Clear();

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < colum; j++)
                {
                    NodeSixBoardGame nodeInGame = new NodeSixBoardGame();
                    nodeInGame.I = i;
                    nodeInGame.J = j;

                    if (i % 2 == 1)
                    {
                        if (j == 0)
                        {
                            nodeInGame.IsEnable = false;
                        }
                    }

                    Maps.Add(nodeInGame);
                }
            }

            bool ValidateIJ(int i, int j)
            {
                return i >= 0 && i < _row && j >= 0 && j < _colum;
            }

            void CheckAddDependencies(NodeBoardGame node, DirectionType directionType, int i, int j)
            {
                if (ValidateIJ(i, j))
                {
                    var child = Maps[GetIndexFromRowAndColum(i, j)];
                    if (child.IsEnable)
                    {
                        node.Dependencies[directionType] = Maps[GetIndexFromRowAndColum(i, j)];
                    }
                }
            }

            for (int i = 0; i < Maps.Count; i++)
            {
                var node = Maps[i];
                if (node.I % 2 == 0)
                {
                    CheckAddDependencies(node, DirectionType.TopLeft, node.I - 1, node.J);
                    CheckAddDependencies(node, DirectionType.TopRight, node.I - 1, node.J + 1);
                    CheckAddDependencies(node, DirectionType.Right, node.I, node.J + 1);
                    CheckAddDependencies(node, DirectionType.BottomRight, node.I + 1, node.J + 1);
                    CheckAddDependencies(node, DirectionType.BottomLeft, node.I + 1, node.J);
                    CheckAddDependencies(node, DirectionType.Left, node.I, node.J - 1);
                }
                else
                {
                    CheckAddDependencies(node, DirectionType.TopLeft, node.I - 1, node.J - 1);
                    CheckAddDependencies(node, DirectionType.TopRight, node.I - 1, node.J);
                    CheckAddDependencies(node, DirectionType.Right, node.I, node.J + 1);
                    CheckAddDependencies(node, DirectionType.BottomRight, node.I + 1, node.J);
                    CheckAddDependencies(node, DirectionType.BottomLeft, node.I + 1, node.J - 1);
                    CheckAddDependencies(node, DirectionType.Left, node.I, node.J - 1);
                }
            }
            for (int i = 0; i < Maps.Count; i++)
            {
                Maps[i].InitChildren();
            }
        }
    }
}
