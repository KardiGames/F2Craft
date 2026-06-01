using System;
using UnityEngine;

namespace WorkerLogic
{
    [Serializable]
    public class Program
    {
        [SerializeField] private Building _building;
        [SerializeField] private Item _item;
        [SerializeField] private int _quantity;
        [SerializeField] private bool _isGettingItem;
        [SerializeField] private bool _forceItem;
        [SerializeField] private bool _forceQuantity;
        [SerializeField] private bool _isAborted;

        public Building Building
        {
            get
            {
                if (_building == null) _isAborted = true;
                return _building;
            }
        }
        public Item Item => _item;
        public int Quantity => _quantity;
        public bool IsGettingItem => _isGettingItem;
        public bool ForceItem { get => _forceItem; set => _forceItem = value; }
        public bool ForceQuantity { get => _forceQuantity; set => _forceQuantity = value; }
        public bool IsAborted => _isAborted;

        public Program(Building building, Item item, int quantity, bool isGettingItem)
        {
            _building = building;
            _item = item;
            _quantity = quantity;
            _isGettingItem = isGettingItem;
            _forceItem = true;
            _forceQuantity = false;
            _isAborted = false;
        }
    }

}