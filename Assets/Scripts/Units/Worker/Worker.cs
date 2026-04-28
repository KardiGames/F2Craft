using UnityEngine;
using WorkerLogic;


public class Worker : Unit
{
    public const int DEFAULT_SLOT_CAPACITY = 5;

    [SerializeField] private Commander _commander;
    [SerializeField] private Building _connectedBuilding;
    [SerializeField] private ItemsSlot _itemsSlot;
    [SerializeField] private ICommand _command;

    public Building ConnectedBuilding => _connectedBuilding;
    public ItemsSlot ItemsSlot => _itemsSlot;

    public Commander Commander => _commander;

    public ICommand Command
    {
        get => _command; set
        {
            if (_command == value)
                return;
            _command?.Cancel();
            //THINK m.b. do check is this == _command.Worker
            _command = value;
            OnParameterChangedInvoke();
        }
    }
    public void Init(int playerNumber, int slotCapacity)
    {
        Init(playerNumber);
        _itemsSlot = new ItemsSlot(slotCapacity);
        _itemsSlot.OnContentChanged += OnParameterChangedInvoke;
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
            OnParameterChangedInvoke();
            return true;
        }

        _connectedBuilding = null;
        float distance = Mathf.Sqrt(sqrDelta);
        transform.position = current + _moveSpeed * Time.deltaTime * (delta / distance);
        return false;
    }

    public bool TryConnectBuilding(Building building)
    {
        if (building == null)
            return false;

        Vector3 delta = building.transform.position - transform.position;
        float sqrFlatDelta = delta.x * delta.x + delta.z * delta.z;

        if (sqrFlatDelta <= Time.deltaTime * _moveSpeed)
        {
            _connectedBuilding = building;
            OnParameterChangedInvoke();
            return true;
        }
        return false;
    }

    private void Start()
    {
        if (_commander == null)
            Debug.LogError("Link is not set", this);
    }
    private void Update()
    {
        _command?.Execute();
    }
}
