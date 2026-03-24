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

    public void Move(Vector3 point)
    {

        Vector3 moveVector = point - transform.position;
        moveVector.y = 0;
        transform.position=transform.position+(moveVector.normalized.

            .normalized * _moveSpeed * Time.deltaTime;

        transform.position = new Vector3();
    }
}
