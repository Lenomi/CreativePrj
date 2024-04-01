using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlgorithmFindPath;
using System;

namespace BoardGame
{
	public class NodeSixBoardGame : NodeBoardGame
	{
		public NodeSixBoardGame()
		{
			Dependencies.Add(DirectionType.TopLeft, null);
			Dependencies.Add(DirectionType.TopRight, null);
			Dependencies.Add(DirectionType.Right, null);
			Dependencies.Add(DirectionType.BottomRight, null);
			Dependencies.Add(DirectionType.BottomLeft, null);
			Dependencies.Add(DirectionType.Left, null);
		}

		public override float HeuristicToNode(NodeBase node)
		{
			var nodeBoardGame = node as NodeBoardGame;
			var x = Math.Abs(I - nodeBoardGame.I);
			var y = Math.Abs(J - nodeBoardGame.J) - (x % 2 + 1);
			return x + (y >= 0 ? (y + 1) : 0);
		}
	}
}
