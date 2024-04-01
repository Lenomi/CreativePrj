using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlgorithmFindPath;
using System;

namespace BoardGame
{
	public class NodeFourBoardGame : NodeBoardGame
	{
		public NodeFourBoardGame()
		{
			Dependencies.Add(DirectionType.Top, null);
			Dependencies.Add(DirectionType.Right, null);
			Dependencies.Add(DirectionType.Bottom, null);
			Dependencies.Add(DirectionType.Left, null);
		}

		public override float HeuristicToNode(NodeBase node)
		{
			var nodeBoardGame = node as NodeBoardGame;
			return Math.Abs(I - nodeBoardGame.I) + Math.Abs(J - nodeBoardGame.J);
		}
	}
}
