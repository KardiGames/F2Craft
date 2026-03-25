using UnityEngine;
using WorkerLogic;
using static UnityEngine.GraphicsBuffer;


public class Worker : Unit
{
    
    [SerializeField] private Building _connectedBuilding;
    [SerializeField] private ItemsSlot _itemsSlot;
    [SerializeField] private ICommand _command;

    private void Start() //TODO Delete this crutch
    {
        Init(5, 5f, 5);
    }
    public void Init (int slotCapaciity, float moveSpeed, int hp)
    {
        _hp = hp;
        _moveSpeed = moveSpeed;
        _itemsSlot = new ItemsSlot(slotCapaciity);
    }

    private void Update()
    {
        _command?.Execute(this);    
    }

    public void GetCommand(ICommand command)
    {
        if (_command == null)
            return;

        _command?.Reset();
        _command = command;
    }

    public bool TryReachThePoint(Vector3 point)
    {
        Vector3 current = transform.position;
        Vector3 delta = point - current;

        float sqrDelta = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;

        if (sqrDelta <= _moveSpeed*_moveSpeed*Time.deltaTime*Time.deltaTime)
        {
            transform.position = point;
            return true;
        }

        float distance = Mathf.Sqrt(sqrDelta);
        transform.position = current + _moveSpeed * Time.deltaTime * (delta / distance);
        return false;
    }
}
