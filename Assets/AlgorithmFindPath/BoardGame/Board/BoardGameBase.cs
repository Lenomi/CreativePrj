using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoardGame
{
    public abstract class BoardGameBase
    {
        public List<NodeBoardGame> Maps = new List<NodeBoardGame>();
        protected int _row = 0;
        protected int _colum = 0;

        protected BoardGameBase()
        {

        }

        public BoardGameBase(int row, int colum)
        {
            
        }

        public int GetIndexFromRowAndColum(int i, int j)
        {
            return i * _colum + j;
        }
    }
}
