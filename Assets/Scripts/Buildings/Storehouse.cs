using System;
using System.Collections.Generic;
using UnityEngine;

public class Storehouse : Building
{
    private const int DEFAULT_CAPACITY = 100;

    [SerializeField] private ItemsSlot _storage;

    public ItemsSlot Storage => _storage;

    public override IEnumerable<Item> ItemsToGive()
    {
        List<Item> items = new List<Item>();
        if (_storage.Quantity > 0 && _storage.Item != null)
            items.Add(_storage.Item);
        return items;
    }

    public override int ItemsOfTypeToGive(Item item, out ItemsSlot slot)
    {
        slot = _storage;
        return _storage.Available(item);
    }

    public override int ItemsOfTypeToGet(Item item, out ItemsSlot slot)
    {
        slot = _storage;
        return _storage.FreeCapacity(item);
    }

    public void Init(int playerNumber, int capacity)
    {
        base.Init(playerNumber);
        _storage = new ItemsSlot(capacity);
        _storage.OnContentChanged += OnParameterChangedInvoke;
    }
    protected override void Start()
    {

        base.Start();

        if (_storage == null)
        {
            Init(_playerNumber, DEFAULT_CAPACITY);
            print("Crutch. Storehouse center initiated by Start()");
        }
    }
}
