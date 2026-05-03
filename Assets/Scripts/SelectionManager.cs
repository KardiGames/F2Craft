using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;


public class SelectionManager : MonoBehaviour
{
    public event Action OnSelectionChanged;
    public event Action OnCurrentChanged;
    [SerializeField] private LayerMask _clickable;
    [SerializeField] private List<ActiveEntity> _selectedEntities = new();
    private int _current = 0;
    private bool _isGrouped = false;
    private List<Unit> _allUnits = new();
    private List<Building> _allBuildings = new();

    private Camera _camera;
    public IEnumerable<ActiveEntity> SelectedEntities => _selectedEntities;
    public ActiveEntity CurrentInSelection
    {
        get
        {
            if (_selectedEntities.Count == 0)
                return null;
            if (_current >= _selectedEntities.Count)
            {
                _current = 0;
                OnCurrentChanged?.Invoke();
                Debug.LogError("Wrong current selected");
            }
            return _selectedEntities[_current];
        }
    }
    public ActiveEntity GetNextCurrent()
    {
        if (_isGrouped == false)
            GroupSelection();

        _current++;
        if (_current >= _selectedEntities.Count)
            _current = 0;
        OnCurrentChanged?.Invoke();
        OnSelectionChanged?.Invoke();
        return _selectedEntities[_current];
    }
    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Start()
    {
        if (_camera == null || _clickable == 0)
            Debug.LogError("Link is not set");
    }

    private void OnEnable()
    {
        ActiveEntity.AddObserver(RefreshEntitiesLists);
        ActiveEntity.S_OnEntityRemoved += UnselectRemovedEntity;
    }
    private void OnDisable()
    {
        ActiveEntity.RemoveObserver(RefreshEntitiesLists);
        ActiveEntity.S_OnEntityRemoved -= UnselectRemovedEntity;
    }

    private void GroupSelection()
    {
        if (_selectedEntities.Count <= 1)
            return;

        bool isChanged = false;
        ActiveEntity currentSelectedEntity = _selectedEntities[_current];
        ActiveEntity cashedEntity;
        for (int i = 0; i < _selectedEntities.Count; i++)
        {
            for (int j = i + 1; j < _selectedEntities.Count; j++)
            {
                if (_selectedEntities[i].GetType() != _selectedEntities[j].GetType())
                { 
                    continue; 
                }
                else if (j > i + 1)
                {
                    i++;
                    cashedEntity = _selectedEntities[j];
                    _selectedEntities[j] = _selectedEntities[i];
                    _selectedEntities[i] = cashedEntity;
                    if (currentSelectedEntity == cashedEntity)
                        _current = i;
                    else if (currentSelectedEntity == _selectedEntities[j])
                        _current = j;

                    isChanged = true;
                }
                else
                {
                    break;
                }
            }
        }
        _isGrouped = true;
        if (isChanged)
            OnSelectionChanged?.Invoke();
    }

    private void UnselectRemovedEntity (ActiveEntity removedEntity)
    {
        if (removedEntity == null) 
            {
            Debug.LogError("Lost link to removed entity"); 
            return; }
        bool isRemovedCurrent = false;
        if (removedEntity == _selectedEntities[_current])
            isRemovedCurrent = true;

        if (_selectedEntities.Contains(removedEntity))
        {
            _selectedEntities.Remove(removedEntity);
            OnSelectionChanged?.Invoke();
        }

        if (isRemovedCurrent)
        {
            _current = 0;
            OnCurrentChanged?.Invoke();
        }
    }
    private void RefreshEntitiesLists(object sender, NotifyCollectionChangedEventArgs e)
    {
        _allBuildings.Clear();
        _allUnits.Clear();
        foreach (ActiveEntity entity in ActiveEntity.GetEntitiesList())
        {
            switch (entity)
            {
                case (Unit unit):
                    _allUnits.Add(unit);
                    break;
                case (Building building):
                    _allBuildings.Add(building);
                    break;
            }
        }
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _clickable))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    ExtendSelection(hit.collider.gameObject);
                }
                else
                {
                    SelectByClick(hit.collider.gameObject);
                }

            }
            else if (Input.GetKey(KeyCode.LeftShift) == false && Input.GetKey(KeyCode.RightShift) == false)
            {
                DeselectAll();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            foreach (ActiveEntity entity in _selectedEntities)
            {

                if ((entity is Worker worker) && Physics.Raycast(ray, out hit, Mathf.Infinity, _clickable))
                {
                    Building target = hit.collider.gameObject.GetComponent<Building>();
                    if (target != null)
                    {
                        worker.GetComponent<WorkerLogic.Commander>().Interact(target, (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)));
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab))
            GetNextCurrent();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_selectedEntities.Count == 1 && _selectedEntities[0] is Worker worker)
            {
                worker.GetComponent<WorkerLogic.Commander>().SwitchProgrammedMode();
            }
            else if (_selectedEntities.Count == 1 && _selectedEntities[0] is UnitProducer producer)
            {
                producer.AddUnitToQueue();
            }
        }
    }

    private void DeselectAll()
    {
        foreach (ActiveEntity entity in _selectedEntities)
        {
            entity?.SetSelection(false);
        }
        _selectedEntities.Clear();
        _current = 0;
        OnSelectionChanged?.Invoke();
        OnCurrentChanged?.Invoke();
    }

    public void DragSelect(ActiveEntity entity)
    {
        if (_selectedEntities.Contains(entity) || entity == null)
            return;
        if (_selectedEntities.Count > 0)
            if (entity is Unit && _selectedEntities[0] is not Unit)
                DeselectAll();
            else if (entity is not Unit && _selectedEntities[0] is Unit)
                return;

        _selectedEntities.Add(entity);
        if (_selectedEntities.Count == 1)
            OnCurrentChanged?.Invoke();
        _isGrouped = false;
        entity.SetSelection(true);
        OnSelectionChanged?.Invoke();
    }

    private void SelectByClick(GameObject target)
    {
        DeselectAll();
        ActiveEntity selectedEntity = target.GetComponent<ActiveEntity>();
        if (selectedEntity == null)
            return;
        _selectedEntities.Add(selectedEntity);
        OnCurrentChanged?.Invoke();
        _isGrouped = false;
        selectedEntity.SetSelection(true);
        OnSelectionChanged?.Invoke();
    }

    private void ExtendSelection(GameObject target)
    {
        if (_selectedEntities.Count == 0)
        {
            SelectByClick(target);
            return;
        }

        ActiveEntity selectedEntity = target.GetComponent<ActiveEntity>();
        if (selectedEntity == null || _selectedEntities.Contains(selectedEntity))
            return;

        if ((selectedEntity is Unit) == (_selectedEntities[0] is Unit))
        {
            _selectedEntities.Add(selectedEntity);
            _isGrouped = false;
            selectedEntity.SetSelection(true);
            OnSelectionChanged?.Invoke();
        }
    }


}
