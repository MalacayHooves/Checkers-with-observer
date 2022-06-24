using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

namespace Checkers
{
    public class CellComponent : BaseClickComponent
    {
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (_isSelected) return;
            if (Pair != null && Pair.IsSelected) return;
            Highlight = HighlightCondition.Highlighted;
            CallBackEvent(this, true);
        }


        public override void OnPointerExit(PointerEventData eventData)
        {
            if (_isSelected) return;
            if (Pair != null && Pair.IsSelected) return;
            Highlight = HighlightCondition.NotHighlighted;
            CallBackEvent(this, false);
        }

        private void OnEnable()
        {
            switch (GetColor)
            {
                case ColorType.White:
                    AddAdditionalMaterial(Resources.Load<Material>("Materials/WhiteCellMaterial"), 0);
                    AddAdditionalMaterial(Resources.Load<Material>("Materials/WhiteCellMaterialHighlighted"), 1);
                    break;
                case ColorType.Black:
                    AddAdditionalMaterial(Resources.Load<Material>("Materials/BlackCellMaterial"), 0);
                    AddAdditionalMaterial(Resources.Load<Material>("Materials/BlackCellMaterialHighlighted"), 1);
                    break;
                default:
                    break;
            }

            AddAdditionalMaterial(Resources.Load<Material>("Materials/CanMoveToCellMaterial"), 2);
        }
    }
}