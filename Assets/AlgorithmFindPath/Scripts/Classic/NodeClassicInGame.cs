using System.Collections;
using System.Collections.Generic;
using Classic;
using UnityEngine;

public class NodeClassicInGame : MonoBehaviour
{
    NodeClassic _nodeClassic;
    public NodeClassic NodeClassic
    {
        get
        {
            return _nodeClassic;
        }
    }

    public NodeClassicInGame[] Dependencies;

    private void Awake()
    {
        _nodeClassic = new NodeClassic();
        _nodeClassic.Point = transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        for (int i = 0; i < Dependencies.Length; i++)
        {
            var node = Dependencies[i];
            if (node == null) continue;
            _nodeClassic.Dependencies.Add(node.NodeClassic);
        }
        _nodeClassic.InitChildren();
    }
}
