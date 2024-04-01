using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlgorithmFindPath;
using Classic;
using System.Linq;
using System;


public class Map : MonoBehaviour
{
    [SerializeField]
    GameObject Nodes;
    [SerializeField]
    GameObject Obstacles;
    [SerializeField]
    GameObject NPCs;
    [SerializeField]
    GameObject NPCsInLineUp;
    [SerializeField]
    GameObject NodesInLineUp;
    [SerializeField]
    public NodeClassicInLine[] StartNodeInLineUp;
    [SerializeField]
    public NodeClassicInLine[] LastNodeInLineUp;
    [SerializeField]
    public float StoppingDistance = 0.1f;
    private AStarSystem aStarSystem = new AStarSystem();
    Dictionary<NodeClassic, NodeClassicInGame> data = new Dictionary<NodeClassic, NodeClassicInGame>();
    List<NodeClassicInGame> nodes = new List<NodeClassicInGame>();
    List<Obstacle> obstacles = new List<Obstacle>();
    List<NPC> npcs = new List<NPC>();
    List<NPC> npcsDisable = new List<NPC>();

    private System.Random _random = new System.Random();
    [SerializeField]
    int Seed = 0;
    [SerializeField]
    RigidbodyInterpolation RigidbodyInterpolation = RigidbodyInterpolation.Extrapolate;
    [SerializeField]
    CollisionDetectionMode CollisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    [SerializeField]
    float RigidbodyDrag = 999999999f;
    [SerializeField]
    float RigidbodyAngularDrag = 999999999f;

    // Start is called before the first frame update
    void Start()
    {
        if(Seed == 0)
        {
            Seed = Guid.NewGuid().GetHashCode();
            _random = new System.Random(Seed);
        }
        else
        {
            _random = new System.Random(Seed);
        }
        if(NPCs != null)
        {
            var npcs1 = NPCs.GetComponentsInChildren<NPC>().ToList();
            npcs.AddRange(npcs1);
        }
        if(NPCsInLineUp != null)
        {
            var npcs2 = NPCsInLineUp.GetComponentsInChildren<NPC>().ToList();
            for (int i = 0; i < npcs2.Count; i++)
            {
                npcs2[i].InLineUp = true;
            }
            npcs.AddRange(npcs2);
        }
        
        if(Nodes != null)
        {
            var nodes1 = Nodes.GetComponentsInChildren<NodeClassicInGame>().ToList();
            nodes.AddRange(nodes1);
        }
        if(NodesInLineUp != null)
        {
            var nodes2 = NodesInLineUp.GetComponentsInChildren<NodeClassicInGame>().ToList();
            nodes.AddRange(nodes2);
        }
        if(Obstacles != null)
        {
            obstacles = Obstacles.GetComponentsInChildren<Obstacle>().ToList();
        }
        
        List<NodeClassic> nodesClassic = new List<NodeClassic>();
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].Init();
        }
        for (int i = 0; i < nodes.Count; i++)
        {
            nodesClassic.Add(nodes[i].NodeClassic);
            data.Add(nodes[i].NodeClassic, nodes[i]);
        }
        aStarSystem.Init(nodesClassic);
        for(int i = 0; i < npcs.Count; i++)
        {
            var npc = npcs[i];
            var rigidBody = npc.GetComponent<Rigidbody>();
            if(rigidBody != null)
            {
                rigidBody.interpolation = RigidbodyInterpolation;
                rigidBody.collisionDetectionMode = CollisionDetectionMode;
                rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                rigidBody.drag = RigidbodyDrag;
                rigidBody.angularDrag = RigidbodyAngularDrag;
                rigidBody.useGravity = false;
            }
            npc.Init(this);
            //npc.ID = i;
            if (!npc.gameObject.activeInHierarchy)
            {
                npcsDisable.Add(npc);
            }
            npc.FindDestination = () =>
            {
                FindDestination(npc);
            };
        }

    }

    private void LateUpdate()
    {
        for (int i = 0; i < npcs.Count; i++)
        {
            var npc = npcs[i];
            npc.UpdateLogic(Time.deltaTime);
        }
    }

    private IEnumerator Appear()
    {
        while(npcsDisable.Count > 0)
        {
            var npc = npcsDisable[0];
            yield return new WaitForSeconds(0.2f);
            npc.Appear();
            npcsDisable.RemoveAt(0);
        }
    }

    public void StartAppearNPCs()
    {
        StartCoroutine(Appear());
    }

    protected void FindDestination(NPC npc)
    {
        Obstacle obstacle = null;
        var endNode = GetNextNodeForNPC(out obstacle);
        if (endNode != null)
        {
            aStarSystem.FindPath(npc.LastestNode.NodeClassic, endNode.NodeClassic, (result) =>
            {
                if (result.Result)
                {
                    List<NodeClassicInGame> path = new List<NodeClassicInGame>();
                    for (int i = 0; i < result.Path.Count; i++)
                    {
                        var node = result.Path[i];
                        path.Add(data[node]);
                    }
                    npc.RemoveUsingNode();
                    npc.Destination = obstacle;
                    npc.Move(path);
                    npc.Destination.SetUsingNode(path[path.Count - 1]);
                }
            }, e =>
            {

            });
        }
    }

    protected NodeClassicInGame GetNextNodeForNPC(out Obstacle destination)
    {
        destination = null;
        List<Obstacle> obstaclesAvaiable = new List<Obstacle>();
        for(int i = 0; i < obstacles.Count; i++)
        {
            var obstacle = obstacles[i];
            if (obstacle.CanGetAvaiableNode())
            {
                obstaclesAvaiable.Add(obstacle);
            }
        }
        if(obstaclesAvaiable.Count > 0)
        {
            Obstacle obstacle = obstaclesAvaiable[GetRandomInt(0, obstaclesAvaiable.Count)];
            var getAvaiableNode = obstacle.GetAvaiableNode();
            if(getAvaiableNode != null)
            {
                destination = obstacle;
                return getAvaiableNode;
            }
        }
        return null;
    }

    public double GetRandomFloat(double from, double to)
    {
        return from + _random.NextDouble() * (to - from);
    }

    public int GetRandomInt(int from, int to)
    {
        return _random.Next(from, to);
    }


#if UNITY_EDITOR

    private List<NodeClassicInGame> nodesInEditor = new List<NodeClassicInGame>();
    [Serializable]
    private class NodeDataEditor
    {
        public NodeClassicInGame From;
        public NodeClassicInGame To;
        public int Count;
    }
    private List<NodeDataEditor> list = new List<NodeDataEditor>();

    void OnDrawGizmos()
    {
        {
            nodesInEditor.Clear();
            list.Clear();
            if(Nodes != null)
            {
                var nodes1 = Nodes.GetComponentsInChildren<NodeClassicInGame>().ToList();
                nodesInEditor.AddRange(nodes1);
            }
            if(NodesInLineUp != null)
            {
                var nodes2 = NodesInLineUp.GetComponentsInChildren<NodeClassicInGame>().ToList();
                nodesInEditor.AddRange(nodes2);
            }

            for (int i = 0; i < nodesInEditor.Count; i++)
            {
                var node = nodesInEditor[i];
                for (int j = 0; j < node.Dependencies.Length; j++)
                {
                    var node2 = node.Dependencies[j];
                    if (node2 == null) continue;
                    var nodeData = list.FirstOrDefault(x => x.From == node2 && x.To == node);
                    if(nodeData != null)
                    {
                        nodeData.Count++;
                    }
                    else
                    {
                        nodeData = new NodeDataEditor();
                        nodeData.From = node;
                        nodeData.To = node2;
                        nodeData.Count = 1;
                        list.Add(nodeData);
                    }
                }
            }
        }
        //Debug.Log(nodesInEditor.Count + ";" + list.Count);
        for (int i = 0; i < nodesInEditor.Count; i++)
        {
            Gizmos.color = Color.green;
            var node = nodesInEditor[i];
            Gizmos.DrawSphere(node.transform.position, 0.05f);
        }
        for (int i = 0; i < list.Count; i++)
        {
            var node = list[i];
            Gizmos.color = node.Count == 1 ? Color.yellow : Color.green;
            Gizmos.DrawLine(node.From.transform.position, node.To.transform.position);
        }
    }

    public void Clear()
    {
        Debug.Log("Clear");
        nodesInEditor.Clear();
        list.Clear();
    }
#endif
}
