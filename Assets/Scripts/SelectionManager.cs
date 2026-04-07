using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private LayerMask clickable;
    [SerializeField] private ActiveEntity _selectedEntity;
    [SerializeField] private Foundation _selectedFoundation;
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
        RaycastHit hit;
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickable))
            {
                SelectByClick(hit.collider.gameObject);
            } else
            {
                DeselectAll();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if ((_selectedEntity is Worker worker) && Physics.Raycast(ray, out hit, Mathf.Infinity, clickable))
            {
                Building target = hit.collider.gameObject.GetComponent<Building>();
                if (target != null)
                {
                    worker.GetComponent<WorkerLogic.Commander>().Interact(target);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_selectedEntity is Worker worker)
            {
                worker.GetComponent<WorkerLogic.Commander>().SwitchProgrammedMode();
            } else if (_selectedEntity is UnitProducer producer)
            {
                producer.AddUnitToQueue();
            } else if (_selectedFoundation is not null)
            {
                _selectedFoundation.ConstructCurrenTempSetup();
            }
        }
    }

    private void DeselectAll()
    {
        _selectedEntity?.SetSelection(false);
        _selectedEntity = null;
        _selectedFoundation?.SetSelection(false );
        _selectedFoundation = null;
    }

    private void SelectByClick(GameObject target)
    {
        DeselectAll();
        _selectedEntity=target.GetComponent<ActiveEntity>();
        if (_selectedEntity != null)
        {
            _selectedEntity.SetSelection(true);
            return;
        }
        _selectedFoundation = target.GetComponent<Foundation>();
        _selectedFoundation?.SetSelection(true);

        if (_selectedFoundation == null)
            print("Error. Nothing is selected. Must be");
    }
}
