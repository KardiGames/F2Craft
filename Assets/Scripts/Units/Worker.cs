using UnityEngine;

public class Worker : Unit
{
    
    [SerializeField] Building _connectedBuilding;
    ItemsSlot _itemsSlot;

    private void Start() //TODO Delete this crutch
    {
        print(_itemsSlot == null);
        Init(5);
    }
    public void Init (int slotCapaciity)
    {
        _itemsSlot = new ItemsSlot(slotCapaciity);
    }

    private void Update()
    {
        if (_connectedBuilding != null)
        {

        }
    }
}
