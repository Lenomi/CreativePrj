using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlgorithmFindPath;
using System;

namespace Classic
{
	public class NodeClassic : NodeBase
	{
		public List<NodeClassic> Dependencies = new List<NodeClassic>();
		public Vector3 Point;

		protected bool _isEmpty = true;
		public void SetEmpty(bool value)
		{
			_isEmpty = value;
		}

		public NodeClassic()
		{

		}

		public override float DistanceToNode(NodeBase node)
		{
			var nodeClassic = node as NodeClassic;
			return Vector3.Distance(nodeClassic.Point, Point);
		}

		public override float HeuristicToNode(NodeBase node)
		{
			var nodeClassic = node as NodeClassic;
			return Vector3.Distance(nodeClassic.Point, Point);
		}

		public override bool IsEmpty()
		{
			return _isEmpty;
		}

        public sealed override void InitChildren()
        {
            Children.Clear();
			Children.AddRange(Dependencies);
		}
    }
}

