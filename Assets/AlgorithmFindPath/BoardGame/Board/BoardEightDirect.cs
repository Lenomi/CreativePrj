using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoardGame
{
    public sealed class BoardEightDirect : BoardGameBase
    {
        public BoardEightDirect(int row, int colum)
        {
            _row = row;
            _colum = colum;
            Maps.Clear();

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < colum; j++)
                {
                    NodeEightBoardGame nodeInGame = new NodeEightBoardGame();
                    nodeInGame.I = i;
                    nodeInGame.J = j;

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
                CheckAddDependencies(node, DirectionType.TopLeft, node.I - 1, node.J - 1);
                CheckAddDependencies(node, DirectionType.Top, node.I - 1, node.J);
                CheckAddDependencies(node, DirectionType.TopRight, node.I - 1, node.J + 1);
                CheckAddDependencies(node, DirectionType.Right, node.I, node.J + 1);
                CheckAddDependencies(node, DirectionType.BottomRight, node.I + 1, node.J + 1);
                CheckAddDependencies(node, DirectionType.Bottom, node.I + 1, node.J);
                CheckAddDependencies(node, DirectionType.BottomLeft, node.I + 1, node.J - 1);
                CheckAddDependencies(node, DirectionType.Left, node.I, node.J - 1);
            }
            for (int i = 0; i < Maps.Count; i++)
            {
                Maps[i].InitChildren();
            }
        }
    }
}
