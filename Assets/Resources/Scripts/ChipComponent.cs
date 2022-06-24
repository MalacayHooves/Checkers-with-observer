using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public class ChipComponent : BaseClickComponent
    {
        protected Player _player;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (_isSelected) return;
            Highlight = HighlightCondition.Highlighted;
            if (Pair != null && !Pair.IsSelected) Pair.Highlight = HighlightCondition.Highlighted;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (_isSelected) return;
            Highlight = HighlightCondition.NotHighlighted;
            if (Pair != null && !Pair.IsSelected) Pair.Highlight = HighlightCondition.NotHighlighted;
        }

        private void OnEnable()
        {
            switch (GetColor)
            {
                case ColorType.White:
                    AddAdditionalMaterial(Resources.Load<Material>("Materials/WhiteChipMaterial"), 0);
                    AddAdditionalMaterial(Resources.Load<Material>("Materials/WhiteChipMaterialHighlighted"), 1);
                    _player = FindObjectOfType<PlayerWhite>();
                    break;
                case ColorType.Black:
                    AddAdditionalMaterial(Resources.Load<Material>("Materials/BlackChipMaterial"), 0);
                    AddAdditionalMaterial(Resources.Load<Material>("Materials/BlackChipMaterialHighlighted"), 1);
                    _player = FindObjectOfType<PlayerBlack>();
                    break;
                default:
                    break;
            }

            AddAdditionalMaterial(Resources.Load<Material>("Materials/CanBeEatenMaterial"), 2);

            Invoke("GetPair", Time.deltaTime);
        }

        public void DeselectChip()
        {
            _isSelected = false;
            Highlight = HighlightCondition.NotHighlighted;
            if (Pair != null) Pair.Highlight = HighlightCondition.NotHighlighted;
        }

        private void GetPair()
        {
            Pair = _player.Cells[Mathf.RoundToInt(gameObject.transform.localPosition.x), Mathf.RoundToInt(gameObject.transform.localPosition.z)].GetComponent<BaseClickComponent>();
            Pair.Pair = this;
        }

        public void Unpair()
        {
            Pair.Pair = null;
        }

        public IEnumerator MoveChip(CellComponent end, float time)
        {
            Vector3 startPosition = transform.position;
            Vector3 endPosition = end.transform.position;
            Vector3 position;
            float currentTime = 0f;
            while (currentTime <= 0.5 * time)
            {
                position.x = Mathf.Lerp(startPosition.x, endPosition.x, 1 - (time - currentTime) / time);
                position.y = Mathf.Lerp(startPosition.y, endPosition.y + 1, 1 - (time - currentTime) / time);
                position.z = Mathf.Lerp(startPosition.z, endPosition.z, 1 - (time - currentTime) / time);
                transform.position = position;
                currentTime += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(time/10);
            while (currentTime > 0.5 * time && currentTime <= time)
            {
                position.x = Mathf.Lerp(startPosition.x, endPosition.x, 1 - (time - currentTime) / time);
                position.y = Mathf.Lerp(startPosition.y + 1, endPosition.y, 1 - (time - currentTime) / time);
                position.z = Mathf.Lerp(startPosition.z, endPosition.z, 1 - (time - currentTime) / time);
                transform.position = position;
                currentTime += Time.deltaTime;
                yield return null;
            }

            transform.position = end.transform.position;
            GetPair();
        }
    }
}
