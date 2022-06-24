using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Checkers
{
    public class PlayerBlack : Player
    {
        protected override void SetPlayerColor()
        {
            _currentPlayerColor = ColorType.Black;
        }

        protected override void SetCellsArray()
        {
            CellComponent[] cells = FindObjectsOfType<CellComponent>();
            foreach (CellComponent cell in cells)
            {
                _cells[Mathf.RoundToInt(Mathf.Abs(cell.gameObject.transform.position.x - 7)), Mathf.RoundToInt(Mathf.Abs(cell.gameObject.transform.position.z - 7))] = cell;
            }
        }

        protected override void SetOppositePlayer()
        {
            _oppositePlayer = FindObjectOfType<PlayerWhite>();
        }
    }
}
