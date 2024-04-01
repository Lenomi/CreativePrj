using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlgorithmFindPath;
using BoardGame;
using UnityEngine.UI;
using System.Linq;

public class Board : MonoBehaviour
{
    [SerializeField]
    BoardType BoardType;
    [SerializeField]
    int Row;
    [SerializeField]
    int Colum;
    [SerializeField]
    GameObject BoardItemPrefab;
    [SerializeField]
    Transform Content;
    List<BoardItem> _items = new List<BoardItem>();
    List<BoardItem> _obstacles = new List<BoardItem>();

    [SerializeField]
    int SelectType = 0;

    BoardItem _start;
    BoardItem _end;

    // Start is called before the first frame update
    void Start()
    {
        //{
        //    // create maps
        //    var boardSixDirect = new BoardSixDirect(7, 7);
        //    var aStarSystem = new AStarSystem();
        //    aStarSystem.Init(boardSixDirect.Maps);
        //    //

        //    NodeBoardGame nodeStar = boardSixDirect.Maps[boardSixDirect.GetIndexFromRowAndColum(0, 0)];
        //    NodeBoardGame nodeEnd = boardSixDirect.Maps[boardSixDirect.GetIndexFromRowAndColum(3, 6)];
        //    aStarSystem.FindPath(nodeStar, nodeEnd,
        //        (pathResult) =>
        //        {
        //            Debug.Log("nodes: " + pathResult.Path.Count + ";" + pathResult.Distance);
        //        }, (e) =>
        //        {
        //            Debug.Log("error: " + e);
        //        });
        //}
        //{
        //    // create maps
        //    var boardFourDirect = new BoardFourDirect(7, 7);
        //    var aStarSystem = new AStarSystem();
        //    aStarSystem.Init(boardFourDirect.Maps);
        //    //

        //    NodeBoardGame nodeStar = boardFourDirect.Maps[boardFourDirect.GetIndexFromRowAndColum(0, 0)];
        //    NodeBoardGame nodeEnd = boardFourDirect.Maps[boardFourDirect.GetIndexFromRowAndColum(3, 6)];
        //    aStarSystem.FindPath(nodeStar, nodeEnd,
        //        (pathResult) =>
        //        {
        //            Debug.Log("nodes: " + pathResult.Path.Count + ";" + pathResult.Distance);
        //        }, (e) =>
        //        {
        //            Debug.Log("error: " + e);
        //        });
        //}
        //{
        //    // create maps
        //    var boardEightDirect = new BoardEightDirect(7, 7);
        //    var aStarSystem = new AStarSystem();
        //    aStarSystem.Init(boardEightDirect.Maps);
        //    //

        //    NodeBoardGame nodeStar = boardEightDirect.Maps[boardEightDirect.GetIndexFromRowAndColum(0, 0)];
        //    NodeBoardGame nodeEnd = boardEightDirect.Maps[boardEightDirect.GetIndexFromRowAndColum(3, 6)];
        //    aStarSystem.FindPath(nodeStar, nodeEnd,
        //        (pathResult) =>
        //        {
        //            Debug.Log("nodes: " + pathResult.Path.Count + ";" + pathResult.Distance);
        //        }, (e) =>
        //        {
        //            Debug.Log("error: " + e);
        //        });
        //}

        InitBoard();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private BoardGameBase _boardDirect;

    private AStarSystem aStarSystem = new AStarSystem();

    public void InitBoard()
    {
        _start = null;
        _end = null;
        for (int i = 0; i < _items.Count; i++)
        {
            Destroy(_items[i].gameObject);
        }
        _items.Clear();
        _obstacles.Clear();

        void CreateBoard(BoardType boardType)
        {
            if(boardType == BoardType.Four)
            {
                _boardDirect = new BoardFourDirect(Row, Colum);
            }
            else if (boardType == BoardType.Six)
            {
                _boardDirect = new BoardSixDirect(Row, Colum);
            }
            else if (boardType == BoardType.Eight)
            {
                _boardDirect = new BoardEightDirect(Row, Colum);
            }
            aStarSystem.Init(_boardDirect.Maps);
            var size = BoardItemPrefab.GetComponent<RectTransform>().rect.size;
            for (int i = 0; i < _boardDirect.Maps.Count; i++)
            {
                var node = _boardDirect.Maps[i];

                var boardItemGO = Instantiate(BoardItemPrefab, Content);
                boardItemGO.SetActive(true);
                var boardItem = boardItemGO.GetComponent<BoardItem>();
                boardItem.Init(node);
                _items.Add(boardItem);

                var x = 0f;
                var y = 0f;
                if(boardType == BoardType.Four || boardType == BoardType.Eight)
                {
                    x = (node.J - Colum / 2) * size.x + (Colum % 2 == 0 ? (size.x / 2) : 0);
                    y = (node.I - Row / 2) * size.y + (Row % 2 == 0 ? (size.y / 2) : 0);
                }
                else
                {
                    x = (node.J - Colum / 2) * size.x + (Colum % 2 == 0 ? (size.x / 2) : 0) + (node.I % 2 == 1 ? (-size.x / 2) : 0);
                    y = (node.I - Row / 2) * size.y + (Row % 2 == 0 ? (size.y / 2) : 0);
                }
                boardItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);

                var button = boardItem.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    if (SelectType == 0)
                    {
                        if (_start != null)
                        {
                            _start.SetSelect(-1);
                        }
                        _start = boardItem;
                        _start.SetSelect(0);

                        if (_end == _start)
                        {
                            _end = null;
                        }
                    }
                    else if (SelectType == 1)
                    {
                        if (_end != null)
                        {
                            _end.SetSelect(-1);
                        }
                        _end = boardItem;
                        _end.SetSelect(1);

                        if (_start == _end)
                        {
                            _start = null;
                        }
                    }
                    else if (SelectType == 2)
                    {
                        if (!_obstacles.Contains(boardItem))
                        {
                            _obstacles.Add(boardItem);
                            boardItem.SetSelect(3);
                        }
                        else
                        {
                            _obstacles.Remove(boardItem);
                            boardItem.SetSelect(0);
                        }
                    }
                });
            }
        }
        CreateBoard(BoardType);
    }

    public void Find()
    {
        if (_boardDirect != null)
        {
            if (_start != null && _end != null)
            {
                NodeBoardGame nodeStar = _boardDirect.Maps[_boardDirect.GetIndexFromRowAndColum(_start.NodeBoardGame.I, _start.NodeBoardGame.J)];
                NodeBoardGame nodeEnd = _boardDirect.Maps[_boardDirect.GetIndexFromRowAndColum(_end.NodeBoardGame.I, _end.NodeBoardGame.J)];
                aStarSystem.FindPath(nodeStar, nodeEnd,
                    (pathResult) =>
                    {
                        Debug.Log("nodes: " + pathResult.Path.Count + ";" + pathResult.Distance + ";" + pathResult.Result);
                        for (int i = 0; i < pathResult.Path.Count; i++)
                        {
                            var node = pathResult.Path[i];
                            var boardItem = _items.FirstOrDefault(x => x.NodeBoardGame == node);
                            if (i > 0 && i < pathResult.Path.Count - 1)
                            {
                                boardItem.SetSelect(2);
                            }
                            if(!pathResult.Result && i == pathResult.Path.Count - 1)
                            {
                                boardItem.SetSelect(2);
                            }
                        }
                    }, (e) =>
                    {
                        Debug.Log("error: " + e);
                    });
            }
        }
    }

    public void OnChangeBoardType(int value)
    {
        BoardType = (BoardType)value;
        InitBoard();
    }

    public void OnChangeCellType(int value)
    {
        SelectType = value;
    }
}

public enum BoardType
{
    Four,
    Six,
    Eight
}
