using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlgorithmFindPath;
using System;

namespace BoardGame
{
	public abstract class NodeBoardGame : NodeBase
	{
		public Dictionary<DirectionType, NodeBoardGame> Dependencies = new Dictionary<DirectionType, NodeBoardGame>();

		public int I { get; set; }
		public int J { get; set; }
		public bool IsEnable { get; set; } = true;

		protected bool _isEmpty = true;
		public void SetEmpty(bool value)
        {
			_isEmpty = value;
		}

		public NodeBoardGame()
		{
			
		}

		public override float DistanceToNode(NodeBase node)
		{
			return 1;
		}

		public override float HeuristicToNode(NodeBase node)
		{
			return 1;
		}

		public override bool IsEmpty()
		{
			return _isEmpty;
		}

		public sealed override void InitChildren()
		{
			Children.Clear();
			foreach (var keyPair in Dependencies)
			{
				if (keyPair.Value != null)
				{
					Children.Add(keyPair.Value);
				}
			}
		}
	}

	public enum DirectionType
	{
		TopLeft,
		Top,
		TopRight,
		Right,
		BottomRight,
		BottomLeft,
		Bottom,
		Left
	}
}
