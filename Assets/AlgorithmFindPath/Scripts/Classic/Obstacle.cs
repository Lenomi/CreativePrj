using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public NodeClassicInGame[] Nodes;
    private List<NodeClassicInGame> _usingNodes = new List<NodeClassicInGame>();

    public string[] Animations;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanGetAvaiableNode()
    {
        for (int i = 0; i < Nodes.Length; i++)
        {
            if (!_usingNodes.Contains(Nodes[i]))
                return true;
        }
        return false;
    }

    public NodeClassicInGame GetAvaiableNode()
    {
        List<NodeClassicInGame> nodes = new List<NodeClassicInGame>();
        for(int i = 0; i < Nodes.Length; i++)
        {
            if (!_usingNodes.Contains(Nodes[i]))
                nodes.Add(Nodes[i]);
        }
        if(nodes.Count > 0)
        {
            return nodes[UnityEngine.Random.Range(0, nodes.Count)];
        }
        return null;
    }

    public void SetUsingNode(NodeClassicInGame node)
    {
        if (!_usingNodes.Contains(node))
        {
            _usingNodes.Add(node);
        }
    }

    public void RemoveUsingNode(NodeClassicInGame node)
    {
        _usingNodes.Remove(node);
    }
}
