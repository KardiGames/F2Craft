using System;
using System.Collections.Generic;
using UnityEngine;

public class Building : ActiveEntity
{
    protected const int STORAGE_QUANTITY_MULTIPLER = 2;
    [SerializeField] protected Foundation _foundation;
    public int Level => 1;

    public virtual int ItemsOfTypeToGive (Item item, out ItemsSlot slot)
    {
        slot = null;
        return 0;
    }
    public virtual int ItemsOfTypeToGet (Item item, out ItemsSlot slot)
    {
        slot = null;
        return 0;
    }

    public virtual IEnumerable<Item> ItemsToGive() {
        return Array.Empty<Item>();
    }

    protected override void Destroy()
    {
        _foundation?.gameObject.SetActive(true);
        base.Destroy();
    }

    protected override void Start()
    {
        base.Start();
        if (_foundation == null)
        {
            Debug.LogError("Empty foundation on " + gameObject.name+ " . Destroying");
            Destroy();
            return;
        }
        if (_foundation.IsInstantiated == false)
        {
            print("Crutch. Instantiating foundation for " + gameObject.name);
            _foundation=Instantiate(_foundation, transform.position, _foundation.transform.rotation);
        }
        _foundation.gameObject.SetActive(false);
    }
}
