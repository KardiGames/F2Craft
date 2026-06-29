using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.Port;

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

    public void Init(int playerNumber, int capacity, Foundation foundation)
    {
        base.Init(playerNumber);
        if (foundation == null || foundation.IsInstantiated == false)
        {
            Debug.LogError("Error! Unit producer havn't built");
            Destroy();
            return;
        }

        _foundation = foundation;
        _storage = new ItemsSlot(capacity);
        _storage.OnContentChanged += OnParameterChangedInvoke;
    }
    protected override void Start()
    {

        base.Start();

        if (_storage == null)
        {
            if (_foundation == null)
            {
                Debug.LogError("Foundation link on storehouse is null! Destriying");
                Destroy();
                return;
            }
            _storage = new ItemsSlot(DEFAULT_CAPACITY);
            Debug.Log("Crutch. Storehouse center initiated by Start()");
        }
    }
}
