using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemsSlot
{
    [SerializeField] private Item _item;
    [SerializeField] private int _quantity = 0;
    [SerializeField] private int _quantityLimit = 1;
    [SerializeField] private List<Item> _whiteList = new List<Item>();
    [SerializeField] private List<Item> _blackList = new List<Item>();
    ItemsSlot(int quantityLimit, IEnumerable<Item> whileList, IEnumerable<Item> blackList) {
        if (quantityLimit < 1)
            quantityLimit = 1;
        _quantityLimit=quantityLimit;

        if (whileList != null)
            _whiteList.AddRange(whileList);
        if (blackList != null) 
            _blackList.AddRange(blackList);
    }

    public ItemsSlot (int quantityLimit)
    {
        if (quantityLimit<1)
            quantityLimit = 1;
        _quantityLimit = quantityLimit;
    }
    public ItemsSlot (Item item, int quantityLimit) : this (quantityLimit)
    {
        _whiteList.Add(item);
    }
    public Item Item => _item;
    public int Quantity => _quantity;
    public int QuantityLimit => _quantityLimit;
   
    public int FreeCapacity (Item item)
    {
        if ((_item != null && _item != item)
            || _blackList.Contains(item)
            || (_whiteList.Count > 0 && !_whiteList.Contains(item))
            )
            return 0;

        return _quantityLimit - _quantity;
    }

    public int Available (Item item)
    {
        if (_item != Item 
            || _item == null
            )
            return 0;
        return _quantity;
    }

    public void Clear ()
    {
        _quantity = 0;
        _item = null;
    }

    public bool TrySpend (Item item, int quantity)
    {
        if (item == null && quantity<0) 
            return false;

        if (_item != item && _quantity < quantity)
            return false;

        _quantity -= quantity;
        if (_quantity == 0)
            _item = null;

        return true;
    }

    public bool TryGive (Item item, int quantity)
    {
        if (Available(item) >= quantity)
        {
            _quantity -= quantity;
            if (_quantity == 0)
                _item = null;
            return true;
        }
        return false;
    }

    public bool TryGet (Item item, int quantity)
    {
        if (FreeCapacity (item)  >= quantity)
        {
            _quantity += quantity;
            return true;
        }
        return false;
    }
}
