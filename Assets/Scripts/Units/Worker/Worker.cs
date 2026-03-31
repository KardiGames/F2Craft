using UnityEngine;
using WorkerLogic;


public class Worker : Unit
{

    [SerializeField] private Building _connectedBuilding;
    [SerializeField] private ItemsSlot _itemsSlot;
    [SerializeField] private ICommand _command;

    public Building ConnectedBuilding => _connectedBuilding;
    public ItemsSlot ItemsSlot => _itemsSlot;

    public ICommand Command {get => _command; set 
    {
        _command?.Cancel();

        //THINK m.b. do check is this == _command.Worker
        _command = value;
    }
}
    private void Start() //TODO Delete this crutch
    {
        Init(5, 5f, 5);
    }
    public void Init(int slotCapaciity, float moveSpeed, int hp)
    {
        _hp = hp;
        _moveSpeed = moveSpeed;
        _itemsSlot = new ItemsSlot(slotCapaciity);
    }


    public bool TryReachThePoint(Vector3 point)
    {
        Vector3 current = transform.position;
        point.y = current.y;
        Vector3 delta = point - current;

        float sqrDelta = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;

        if (sqrDelta <= _moveSpeed * _moveSpeed * Time.deltaTime * Time.deltaTime)
        {
            transform.position = point;
            return true;
        }

        _connectedBuilding = null;
        float distance = Mathf.Sqrt(sqrDelta);
        transform.position = current + _moveSpeed * Time.deltaTime * (delta / distance);
        return false;
    }

    public bool TryConnectBuilding (Building building)
    {
        if (building==null)
            return false;

        Vector3 delta = building.transform.position - transform.position;
        float sqrFlatDelta = delta.x * delta.x + delta.z * delta.z;

        if (sqrFlatDelta <= Time.deltaTime * _moveSpeed)
        {
            _connectedBuilding = building;
            return true;
        }
        return false;
    }
    private void Update()
    {
        _command?.Execute();
    }
}
