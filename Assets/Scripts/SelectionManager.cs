using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;


public class SelectionManager : MonoBehaviour
{
    [SerializeField] private LayerMask _clickable;
    [SerializeField] private Foundation _selectedFoundation;
    [SerializeField] private List<ActiveEntity> _selectedEntities=new();
    private List<Unit> _allUnits=new();
    private List<Building> _allBuildings=new();

    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Start()
    {
        if (_camera == null || _clickable==0)
            Debug.LogError("Link is not set");
    }

    private void OnEnable()
    {
        ActiveEntity.AddObserver(RefreshEntitiesLists);
    }
    private void OnDisable()
    {
        ActiveEntity.RemoveObserver(RefreshEntitiesLists);
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
                } else
                {
                    SelectByClick(hit.collider.gameObject);
                }
                    
            } else
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
                        worker.GetComponent<WorkerLogic.Commander>().Interact(target);
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_selectedEntities.Count==1 && _selectedEntities[0] is Worker worker)
            {
                worker.GetComponent<WorkerLogic.Commander>().SwitchProgrammedMode();
            } else if (_selectedEntities.Count == 1 && _selectedEntities[0] is UnitProducer producer)
            {
                producer.AddUnitToQueue();
            } else if (_selectedFoundation != null)
            {
                _selectedFoundation.ConstructCurrentTempSetup();
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
        _selectedFoundation?.SetSelection(false);
        _selectedFoundation = null;
    }


    public void DragSelect(ActiveEntity entity)
    {
        if (_selectedEntities.Contains(entity)==false)
        {
            _selectedEntities.Add(entity);
            entity.SetSelection(true);
        }
    }

    private void SelectByClick(GameObject target)
    {
        DeselectAll();
        ActiveEntity selectedEntity = target.GetComponent<ActiveEntity>();
        if (selectedEntity != null)
        {
            _selectedEntities.Add(selectedEntity);
            selectedEntity.SetSelection(true);
            return;
        }
        _selectedFoundation = target.GetComponent<Foundation>();
        _selectedFoundation?.SetSelection(true);

        if (_selectedFoundation == null)
            Debug.LogError("Nothing is selected. Must be");
    }

    private void ExtendSelection (GameObject target)
    {
        if (_selectedEntities.Count == 0)
        {
            SelectByClick(target);
            return;
        }
        
        ActiveEntity selectedEntity = target.GetComponent<ActiveEntity>();
        if (selectedEntity == null)
            return;

        if ((selectedEntity is Unit) == (_selectedEntities[0] is Unit))
        {
            _selectedEntities.Add(selectedEntity);
            selectedEntity.SetSelection(true);
        }
    }

}
