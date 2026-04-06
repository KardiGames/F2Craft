using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;

[Serializable]
public class ItemsSlot
{
    public event Action OnContentChanged;
    public event Action OnItemsAdded;
    public event Action OnItemsRemoved;

    [SerializeField] private Item _item;
    [SerializeField] private int _quantity = 0;
    [SerializeField] private int _quantityLimit = 1;
    [SerializeField] private List<Item> _whiteList = new List<Item>();
    [SerializeField] private List<Item> _blackList = new List<Item>();
    public ItemsSlot(int quantityLimit, IEnumerable<Item> whileList, IEnumerable<Item> blackList)
    {
        if (quantityLimit < 1)
            quantityLimit = 1;
        _quantityLimit = quantityLimit;

        if (whileList != null)
            _whiteList.AddRange(whileList);
        if (blackList != null)
            _blackList.AddRange(blackList);
    }

    public ItemsSlot(int quantityLimit)
    {
        if (quantityLimit < 1)
            quantityLimit = 1;
        _quantityLimit = quantityLimit;
    }
    public ItemsSlot(Item item, int quantityLimit) : this(quantityLimit)
    {
        _whiteList.Add(item);
    }
    public Item Item => _item;
    public int Quantity => _quantity;
    public int QuantityLimit => _quantityLimit;

    public int FreeCapacity(Item item)
    {
        if (item == null
            || (_item != null && _item != item)
            || _blackList.Contains(item)
            || (_whiteList.Count > 0 && !_whiteList.Contains(item))
            )
            return 0;

        return _quantityLimit - _quantity;
    }

    public int Available(Item item)
    {
        if (item == _item
            && _item != null
            )
            return _quantity;
        return 0;
    }

    public void Clear()
    {
        if (_item == null)
            return;

        _quantity = 0;
        _item = null;
        OnItemsRemoved?.Invoke();
        OnContentChanged?.Invoke();

    }

    public void SetAccessLists(IEnumerable<Item> whitelist, IEnumerable<Item> blacklist)
    {
        if (_item != null && (blacklist != null && blacklist.Contains(_item)) || (whitelist != null && whitelist.Contains(_item) == false))
        {

                Debug.Log("Error! New Black-while-lists doesn't set of ItemsList");
            return;
        }
        _whiteList.Clear();
        if (whitelist is not null)
            _whiteList?.AddRange(whitelist);
        if (blacklist is not null)
            _blackList.Clear();
        _blackList?.AddRange(blacklist);
    }

    public bool TrySpend(Item item, int quantity)
    {
        if (item == null && quantity < 0)
            return false;

        if (_item != item && _quantity < quantity)
            return false;

        _quantity -= quantity;
        OnItemsRemoved?.Invoke();
        OnContentChanged?.Invoke();

        if (_quantity == 0)
            _item = null;

        return true;
    }

    public bool TryPermanentSpend(Item item, int quantity)
    {
        if (TrySpend(item, quantity))
        {
            OnItemsRemoved?.Invoke();
            OnContentChanged?.Invoke();
            _quantityLimit -= quantity;
            return true;
        }
        return false;
    }

    public bool TryGive(ItemsSlot slot, int quantity)
    {
        if (slot == null || quantity <= 0)
            return false;

        if (_item != null && _quantity >= quantity && slot.TryStore(_item, quantity))
        {
            _quantity -= quantity;
            OnItemsRemoved?.Invoke();
            OnContentChanged?.Invoke();
            if (_quantity == 0)
                _item = null;
            return true;
        }
        return false;
    }

    public bool TryStore(Item item, int quantity)
    {
        if (quantity <= 0 || item == null)
            return false;

        if (FreeCapacity(item) >= quantity)
        {
            _item = item;
            _quantity += quantity;
            OnItemsAdded?.Invoke();
            OnContentChanged?.Invoke();
            return true;
        }
        return false;
    }
}
