using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoardGame;
using UnityEngine.UI;

public class BoardItem : MonoBehaviour
{
    NodeBoardGame _nodeBoardGame;
    public NodeBoardGame NodeBoardGame
    {
        get
        {
            return _nodeBoardGame;
        }
    }
    [SerializeField]
    Color32 DefaultColor;
    [SerializeField]
    Color32 StartColor;
    [SerializeField]
    Color32 EndColor;
    [SerializeField]
    Color32 ObstacleColor;
    [SerializeField]
    Color32 PathColor;

    [SerializeField]
    Image Content;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(NodeBoardGame nodeBoardGame)
    {
        _nodeBoardGame = nodeBoardGame;
        gameObject.SetActive(nodeBoardGame.IsEnable);
    }

    public void SetSelect(int selectType)
    {
        _nodeBoardGame.SetEmpty(true);
        if (selectType == 0)
        {
            Content.color = StartColor;
        }
        else if(selectType == 1)
        {
            Content.color = EndColor;
        }
        else if (selectType == 2)
        {
            Content.color = PathColor;
        }
        else if (selectType == 3)
        {
            Content.color = ObstacleColor;
            _nodeBoardGame.SetEmpty(false);
        }
        else if (selectType == -1)
        {
            Content.color = DefaultColor;
        }
    }
}
