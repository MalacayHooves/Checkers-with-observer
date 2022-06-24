using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Checkers
{
    public class PlayerWhite : Player
    {
        protected override void SetPlayerColor()
        {
            _currentPlayerColor = ColorType.White;
        }

        protected override void SetCellsArray()
        {
            CellComponent[] cells = FindObjectsOfType<CellComponent>();
            foreach (CellComponent cell in cells)
            {
                _cells[Mathf.RoundToInt(cell.gameObject.transform.position.x), Mathf.RoundToInt(cell.gameObject.transform.position.z)] = cell;
            }
        }

        protected override void SetOppositePlayer()
        {
            _oppositePlayer = FindObjectOfType<PlayerBlack>();
        }
    }
}
