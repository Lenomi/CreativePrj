using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
using System.Linq;
using FixRound;

namespace FixRound{
	public static class FloatValue {
		public static float Round(float x){
			return (float)Math.Round (x + 0.00001f, 3);
		}
	}
	public static class Vector3Value {
		public static UnityEngine.Vector3 Round(UnityEngine.Vector3 vector3){
			return new UnityEngine.Vector3(FloatValue.Round(vector3.x), FloatValue.Round(vector3.y), FloatValue.Round(vector3.z));
		}
	}
	public static class Vector2Value {
		public static UnityEngine.Vector2 Round(UnityEngine.Vector2 vector2){
			return new UnityEngine.Vector2(FloatValue.Round(vector2.x), FloatValue.Round(vector2.y));
		}
	}
}

namespace AlgorithmFindPath{

	public abstract class NodeBase
	{
		public List<NodeBase> Children = new List<NodeBase>();

		protected NodeBase(){
			
		}

		public abstract float DistanceToNode(NodeBase node);
		public abstract float HeuristicToNode(NodeBase node);
		public abstract bool IsEmpty();
		public abstract void InitChildren();
	}

	internal sealed class Node
	{
		public NodeBase NodeBase { get; set; }
		public bool IsOpened { get; set; }
		public bool IsClosed { get; set; }
		public List<Node> Children = new List<Node>();

		public Path Path { get; set; }

		private Node()
		{
			Path = new Path();
		}

		public Node(NodeBase nodeBase)
		{
			NodeBase = nodeBase;
			Path = new Path();
		}

		public float DistanceToNode(Node node)
		{
			return NodeBase.DistanceToNode(node.NodeBase);
		}

		public float HeuristicToNode(Node node)
		{
			return NodeBase.HeuristicToNode(node.NodeBase);
		}

		public bool IsEmpty()
		{
			return NodeBase.IsEmpty();
		}
	}

	internal sealed class Path
	{
		public Node CurrentNode { get; set; }
		public List<Node> Nodes { get; private set; }
		public float Distance { get; set; }
		public float Heuristic { get; set; }
		public float TotalDistance{
			get{
				return Distance + Heuristic;
			}
		}

		public Path(){
			Nodes = new List<Node>(); 
		}

		public void Add(Node node){
			Nodes.Add(node);
		}

		public void AddRange(IEnumerable<Node> nodes){
			Nodes.AddRange(nodes);
		}
	}
	#region base
	internal class FindPath
	{
		public Node Start {get;set;}
		public Node End {get;set;}

		public List<Path> Opens {get;set;}
		public List<Node> Closes {get;set;}
		public Path MinHeuristicClosePath { get; set; }
		protected Path _lowestPath;
		public Path LowestPath
		{
			get{
				return _lowestPath;
			}
		}

		public virtual void Reset(){
		}
	}


	#endregion

	#region A*
	internal sealed class  AStar : FindPath
	{
		public List<Node> Maps;

		public AStar(){
			Closes = new List<Node>();
			Opens = new List<Path>();
			Maps = new List<Node> ();
		}

		public void AddOpen(Path currentPath, Node next, float distance = 0, float heuristic = 0){
			Path path = new Path();
			path.CurrentNode = next;
			next.Path = path;
			next.IsOpened = true;
			if (currentPath != null) {
				path.AddRange(currentPath.Nodes);
			}
			path.Add(next);
			path.Distance = distance;
			path.Heuristic = heuristic;
			if (_lowestPath == null || _lowestPath.TotalDistance > path.TotalDistance) {
				_lowestPath = path;
			}
			Opens.Add(path);
		}

		public void CheckAdd(Path currentPath, Node next, float distance = 0, float heuristic = 0){
			
			var nodePathCheck = next.Path;
			if (nodePathCheck.TotalDistance > distance + heuristic) {
				Opens.Remove(next.Path);
				AddOpen(currentPath, next, distance, heuristic);
			}
		}

		public Path GetLowestPath(){
			var path = _lowestPath;
			Opens.Remove(_lowestPath);
			Path min = null;
			for (int i = 0; i < Opens.Count; i++) {
				var temp = Opens[i];
				if (min == null || min.TotalDistance > temp.TotalDistance) {
					min = temp;
				}
			}
			_lowestPath = min;
			return path;
		}

		public bool IsSameEnd(){
			return _lowestPath != null && _lowestPath.CurrentNode == End;
		}

		public override void Reset(){
			for (int i = 0; i < Maps.Count; i++) {
				var node = Maps[i];
				node.IsOpened = false;
				node.IsClosed = false;
			}
			Closes.Clear();
			Opens.Clear();
			MinHeuristicClosePath = null;
		}
	}

	public class AStarSystem
	{
		public AStarSystem() {
		}

		List<Node> _maps = new List<Node>();

		public void Init<T>(List<T> maps) where T : NodeBase
		{
			_maps.Clear();
			Dictionary<int, int> dict = new Dictionary<int, int>();
			for (int i = 0; i < maps.Count; i++)
			{
				var t = maps[i];
				dict.Add(t.GetHashCode(), i);
				var node = new Node(t);
				_maps.Add(node);
			}
			for (int i = 0; i < _maps.Count; i++)
			{
				var t = _maps[i];
				for (int j = 0; j < t.NodeBase.Children.Count; j++)
				{
					var nodeBase = t.NodeBase.Children[j];
					var index = dict[nodeBase.GetHashCode()];
					var child = _maps[index];
					t.Children.Add(child);
				}
			}
		}

		public void FindPath<T>(T start, T end, Action<PathResult<T>> onComplete, Action<string> onError) where T : NodeBase {
			Node nodeStart = null;
			Node nodeEnd = null;
			int j = 0;
			if(_maps.Count == 0)
            {
				onError("Have no Maps");
				return;
			}
			for (int i = 0; i < _maps.Count; i++)
			{
				var node = _maps[i];
				if (node.NodeBase == start)
				{
					nodeStart = node;
					j++;
				}
				if (node.NodeBase == end)
				{
					nodeEnd = node;
					j++;
				}
				if (j == 2)
				{
					break;
				}
			}
			if (nodeStart == null || nodeEnd == null)
            {
				onError("Can't find Node Start or Node End");
				return;
            }
			AStar aStar = new AStar();
			aStar.Start = nodeStart;
			aStar.End = nodeEnd;
			aStar.Maps = _maps;
			var heuristic = start.HeuristicToNode(end);
			aStar.AddOpen(null, nodeStart, 0, heuristic);
			while (aStar.Opens.Count > 0) {
				if (aStar.IsSameEnd()) {
					aStar.Reset();
					List<T> list = new List<T>();
					for(int i = 0; i < aStar.LowestPath.Nodes.Count; i++)
                    {
						var node = aStar.LowestPath.Nodes[i];
						list.Add(node.NodeBase as T);
					}
					onComplete(new PathResult<T>(list, aStar.LowestPath.Distance, true));
					return;
				}
				CheckPathForNode(aStar);
			}
			if(aStar.MinHeuristicClosePath != null)
            {
				var pathMinHeuristic = aStar.MinHeuristicClosePath;
				List<T> list = new List<T>();
				for(int i = 0; i < pathMinHeuristic.Nodes.Count; i++)
                {
					list.Add(pathMinHeuristic.Nodes[i].NodeBase as T);
				}
				aStar.Reset();
				onComplete(new PathResult<T>(list, pathMinHeuristic.Distance, false));
			}
            else
            {
				aStar.Reset();
				onComplete(new PathResult<T>());

			}
		}

		void CheckPathForNode(AStar aStar)
		{
			var path = aStar.GetLowestPath();
			var node = path.CurrentNode;
			aStar.Closes.Add(node);
			if(aStar.MinHeuristicClosePath == null)
            {
				aStar.MinHeuristicClosePath = path;
			}
            else if(aStar.MinHeuristicClosePath.Heuristic > path.Heuristic)
            {
				aStar.MinHeuristicClosePath = path;
			}
			node.IsClosed = true;
			for (int i = 0; i < node.Children.Count; i++) {
				var child = node.Children[i];
				if (child != null && child.IsEmpty() && !child.IsClosed) {
					var distance = node.DistanceToNode(child) + path.Distance;
					var heuristic = child.HeuristicToNode(aStar.End);
					if (child.IsOpened) {
						aStar.CheckAdd(path, child, distance, heuristic);
					} else {
						aStar.AddOpen(path, child, distance, heuristic);
					}
				}
			}
		}
	}

	public sealed class PathResult<T> where T : NodeBase
	{
		public List<T> Path = new List<T>();
		public bool Result;
		public float Distance;

		internal PathResult()
        {

        }

		internal PathResult(List<T> list, float distance, bool result)
		{
			Path = list;
			Result = result;
			Distance = distance;
		}
	}
    #endregion
}




