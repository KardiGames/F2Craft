using System;
using System.Collections.Generic;
using UnityEngine;

public class Building : ActiveEntity
{
    protected const int STORAGE_QUANTITY_MULTIPLER = 2;
    public int Level => 1;
    protected Foundation _foundation;

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
}
