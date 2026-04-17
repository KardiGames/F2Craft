using System.Collections.Generic;
using UnityEngine;

public class CommandCenter : Building
{
    [SerializeField] private Item _materiaItem;
    [SerializeField] private float _producingTime;
    private ItemsSlot _materiaStorage;
    [SerializeField] private int _materiaCapacity;
    private float _timer = 0f;
    private bool _isProducting = false;

    public override IEnumerable<Item> ItemsToGive()
    {
        List<Item> items = new List<Item>();
            if (_materiaStorage.Quantity > 0 && _materiaStorage.Item != null)
                items.Add(_materiaItem);
        return items;
    }

    public override int ItemsOfTypeToGive(Item item, out ItemsSlot slot)
    {
        slot = _materiaStorage;
        return _materiaStorage.Available(item);
    }

    public new void Init(int playerNumber, ActiveEntitiesManager activeEntitiesManager)
    {
        base.Init(playerNumber, activeEntitiesManager);
        _materiaStorage = new ItemsSlot(_materiaItem, _materiaCapacity);
    }
    private void Start()
    {
        if (_materiaItem == null)
        {
            print("Error! Materia item isn't installed. Destroying");
            Destroy();
            return;
        }
        if (_activeEntitiesManager==null && _materiaStorage == null)
        {
            Init(_playerNumber, GameObject.Find("SingleScripts").GetComponent<ActiveEntitiesManager>());
            print("Crutch. Command center initiated by Start()");
        }
    }
    private void Update()
    {
        if (_isProducting)
        {
            if (_timer > 0f)
                _timer -= Time.deltaTime;
            else
                ProduceMateria();
        }
        else
        {
            StartProduction();
        }

    }

    private void ProduceMateria()
    {


        if (!_materiaStorage.TryStore(_materiaItem, 1))
                print("Error. Something goes wrong with materia production");


        _isProducting = false;
        StartProduction();
    }

    private void StartProduction()
    {
        if (IsAbleToStartProduction())
        {
            _timer += _producingTime;
            _isProducting = true;
        }
    }

    private bool IsAbleToStartProduction()
    {
        if (!_isProducting
            && _materiaItem != null
            && HaveFreeSpaceForProduction()
            )
            return true;
        return false;
    }

    private bool HaveFreeSpaceForProduction()
    {
        if (_materiaStorage.Quantity < _materiaStorage.QuantityLimit)
            return true;
        else
            return false;
    }
}
