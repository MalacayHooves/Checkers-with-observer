using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public abstract class Player : MonoBehaviour
    {
        [SerializeField] protected Camera _camera;
        [SerializeField] protected Player _oppositePlayer;

        [SerializeField] protected ChipComponent _chip;
        [SerializeField] protected CellComponent _destinationOne;
        [SerializeField] protected CellComponent _destinationTwo;
        [SerializeField] protected ChipComponent _targetOne;
        [SerializeField] protected ChipComponent _targetTwo;

        [SerializeField] protected Transform _cameraPosition1;
        [SerializeField] protected Transform _cameraPosition2;
        [SerializeField] protected Transform _cameraPosition3;

        [SerializeField] protected ColorType _currentPlayerColor;
        [Tooltip("Время движения фишки"), SerializeField] protected float _chipMoveTime = 1f;
        [Tooltip("Время движения камеры"), SerializeField] protected float _cameraMoveTime = 5f;

        protected bool _disableInput = true;

        protected int _chipCount = 12;
        protected int ChipCount
        {
            get { return _chipCount; }
            set {
                _chipCount = value; 
                if (_chipCount <= 0)
                {
                    print($"{_oppositePlayer.name} Win!");
                }
            }
        }

        protected CellComponent[,] _cells = new CellComponent[8,8];
        public CellComponent[,] Cells { get { return _cells; } }

        protected void Awake()
        {
            _camera = Camera.main;
            SetPlayerColor();
            SetCellsArray();
            SetOppositePlayer();
            _cameraPosition1 = transform.Find("CameraPosition1");
            _cameraPosition2 = transform.Find("CameraPosition2");
            _cameraPosition3 = transform.Find("CameraPosition3");
        }

        protected abstract void SetPlayerColor();

        protected abstract void SetCellsArray();

        protected abstract void SetOppositePlayer();
        
        protected void OnEnable()
        {
            BaseClickComponent.OnClickEventHandler += OnClick;

            ChipComponent[] temp = FindObjectsOfType<ChipComponent>();
            List<ChipComponent> chips = new List<ChipComponent>();
            foreach (ChipComponent chip in temp)
            {
                if (chip.GetColor == _currentPlayerColor) chips.Add(chip);
            }
            ChipCount = chips.Count;

            if (ChipCount <= 0) return;
            _disableInput = true;
            if (Vector3.Distance(_camera.transform.position, _cameraPosition3.position) > 1f)
            {
                StartCoroutine(MoveCamera());
            }
            else
            {
                _disableInput = false;
            }
        }

        protected void OnDisable()
        {
            BaseClickComponent.OnClickEventHandler -= OnClick;
        }

        protected void ClickCaller(string componentName)
        {

        }

        protected void OnClick(BaseClickComponent component)
        {
            if (_disableInput) return;
            var type = component.GetType();
            if (type == typeof(ChipComponent) && component.GetColor == _currentPlayerColor)
            {
                if (_chip != null)
                {
                    _chip.DeselectChip();
                    SetCellsAndChipsHighlight(BaseClickComponent.HighlightCondition.NotHighlighted, BaseClickComponent.HighlightCondition.NotHighlighted, false);
                }
                _chip = (ChipComponent)component;
                GetDestinationsAndTargets(_chip, _chip.Pair);
                SetCellsAndChipsHighlight(BaseClickComponent.HighlightCondition.CanMoveToCell, BaseClickComponent.HighlightCondition.CanBeEatenChip, true);
            }
            else if (type == typeof(CellComponent))
            {
                if (_destinationOne != null && component.name == _destinationOne.name || _destinationTwo != null && component.name == _destinationTwo.name)
                {
                    _chip.DeselectChip();
                    SetCellsAndChipsHighlight(BaseClickComponent.HighlightCondition.NotHighlighted, BaseClickComponent.HighlightCondition.NotHighlighted, false);
                    _chip.Unpair();
                    StartCoroutine(_chip.MoveChip((CellComponent)component, _chipMoveTime));
                    if (_destinationOne != null && component.name == _destinationOne.name)
                    {
                        if (_targetOne != null) StartCoroutine(DisableChip(_targetOne, 0.5f * _chipMoveTime));
                    }
                    else if (_destinationTwo != null && component.name == _destinationTwo.name)
                    {
                        if (_targetTwo != null) StartCoroutine(DisableChip(_targetTwo, 0.5f * _chipMoveTime));
                    }

                    _chip = null;
                    _destinationOne = null;
                    _destinationTwo = null;
                    _targetOne = null;
                    _targetTwo = null;
                    _disableInput = true;
                    if (GetIndexes(component).cellIndexZ == 7)
                    {
                        _oppositePlayer.ChipCount = 0;
                    }
                    else
                    {
                        StartCoroutine(SwitchTurn(_chipMoveTime));
                    }
                }
            }
        }

        protected void GetDestinationsAndTargets(ChipComponent chip, BaseClickComponent cell)
        {
            int cellIndexX = GetIndexes(cell).cellIndexX; 
            int cellIndexZ = GetIndexes(cell).cellIndexZ;

            CellComponent destinationOne = null, destinationTwo = null;
            ChipComponent targetOne = null, targetTwo = null;

            if (CheckIndexExisting(Cells, cellIndexX - 1, cellIndexZ + 1))
            {
                destinationOne = GetDestination(cellIndexX - 1, cellIndexZ + 1);
                if (destinationOne.Pair != null)
                {
                    targetOne = (ChipComponent)destinationOne.Pair;
                    if (targetOne.GetColor == chip.GetColor)
                    {
                        destinationOne = null;
                        targetOne = null;
                    }
                    else
                    {
                        if (CheckIndexExisting(Cells, cellIndexX - 2, cellIndexZ + 2))
                        {
                            destinationOne = GetDestination(cellIndexX - 2, cellIndexZ + 2);
                            if (destinationOne.Pair != null)
                            {
                                destinationOne = null;
                                targetOne = null;
                            }
                        }
                        else
                        {
                            destinationOne = null;
                            targetOne = null;
                        }
                    }
                }
            }

            if (CheckIndexExisting(Cells, cellIndexX + 1, cellIndexZ + 1))
            {
                destinationTwo = GetDestination(cellIndexX + 1, cellIndexZ + 1);
                if (destinationTwo.Pair != null)
                {
                    targetTwo = (ChipComponent)destinationTwo.Pair;
                    if (targetTwo.GetColor == chip.GetColor)
                    {
                        destinationTwo = null;
                        targetTwo = null;
                    }
                    else
                    {
                        if (CheckIndexExisting(Cells, cellIndexX + 2, cellIndexZ + 2))
                        {
                            destinationTwo = GetDestination(cellIndexX + 2, cellIndexZ + 2);
                            if (destinationTwo.Pair != null)
                            {
                                destinationTwo = null;
                                targetTwo = null;
                            }
                        }
                        else
                        {
                            destinationTwo = null;
                            targetTwo = null;
                        }
                    }
                }
            }
            _destinationOne = destinationOne;
            _destinationTwo = destinationTwo;
            _targetOne = targetOne;
            _targetTwo = targetTwo;
        }

        protected bool CheckIndexExisting(Array array, int x, int z)
        {
            bool isExist = false;
            if (x >= 0 && z >= 0 && x <= array.GetLength(0) - 1 && z <= array.GetLength(1) - 1) isExist = true;
            return isExist;
        }

        protected (int cellIndexX, int cellIndexZ) GetIndexes(BaseClickComponent cell)
        {
            int cellIndexX = 0, cellIndexZ = 0;

            for (int x = 0; x < _cells.GetLength(0); x++)
            {
                for (int z = 0; z < _cells.GetLength(1); z++)
                {
                    if (_cells[x, z].name == cell.name)
                    {
                        cellIndexX = x;
                        cellIndexZ = z;
                    }
                }
            }
            return (cellIndexX, cellIndexZ);
        }

        protected CellComponent GetDestination(int x, int z)
        {
            return _cells[x, z];
        }

        protected void SetCellsAndChipsHighlight(BaseClickComponent.HighlightCondition cellsHighlight, BaseClickComponent.HighlightCondition chipsHighlight, bool isSelected)
        {
            if (_destinationOne != null)
            {
                _destinationOne.Highlight = cellsHighlight;
                _destinationOne.IsSelected = isSelected;
            }

            if (_destinationTwo != null)
            {
                _destinationTwo.Highlight = cellsHighlight;
                _destinationTwo.IsSelected = isSelected;
            }

            if (_targetOne != null)
            {
                _targetOne.Highlight = chipsHighlight;
                _targetOne.IsSelected = isSelected;
            }

            if (_targetTwo != null)
            {
                _targetTwo.Highlight = chipsHighlight;
                _targetTwo.IsSelected = isSelected;
            }
        }

        protected IEnumerator DisableChip(ChipComponent chip, float time)
        {
            yield return new WaitForSeconds(time);
            chip.Unpair();
            chip.gameObject.SetActive(false);
        }

        protected IEnumerator SwitchTurn(float time)
        {
            yield return new WaitForSeconds(time);
            _oppositePlayer.enabled = true;
            this.enabled = false;
        }

        protected IEnumerator MoveCamera()
        {
            yield return MoveFromTo(_camera.transform.position, _cameraPosition1.position,
                _camera.transform.rotation, _cameraPosition1.transform.rotation, _cameraMoveTime / 4);
            yield return MoveFromTo(_cameraPosition1.position, _cameraPosition2.position,
                _cameraPosition1.rotation, _cameraPosition2.transform.rotation, _cameraMoveTime / 2);
            yield return MoveFromTo(_cameraPosition2.position, _cameraPosition3.position,
                _cameraPosition2.rotation, _cameraPosition3.rotation, _cameraMoveTime / 4);
            _disableInput = false;
        }


        protected IEnumerator MoveFromTo(Vector3 startPosition, Vector3 endPosition, Quaternion startRotation, Quaternion endRotation, float time)
        {
            var currentTime = 0f;
            while (currentTime < time)
            {
                _camera.transform.position = Vector3.Lerp(startPosition, endPosition, 1 - (time - currentTime) / time);
                _camera.transform.rotation = Quaternion.Slerp(startRotation, endRotation, 1 - (time - currentTime) / time);
                currentTime += Time.deltaTime;
                yield return null;
            }

            _camera.transform.position = endPosition;
            _camera.transform.rotation = endRotation;
        }
    }
}
