using System;
using System.Collections.Generic;
using UnityEngine;

public class Ai : MonoBehaviour
{
    [SerializeField] private int _aiNumber;
    private List<Worker> _freeWorkers = new();
    private Dictionary<Worker, Route> _busyWorkers = new Dictionary<Worker, Route>();
    //private List<Worker> _busyWorkersCleaner = new List<Worker>();
    private List<UnitProducer> _unitProducers = new List<UnitProducer>();
    private List<Factory> _factories = new List<Factory>();
    private List<Storehouse> _storehouses = new List<Storehouse>();
    private List<ConstructionSite> _constructionSites = new List<ConstructionSite>();
    private List<Building> _otherBuildings = new List<Building>();
    private Dictionary<(Building, Item), int> _blockedItems = new Dictionary<(Building, Item), int>();
    private List<(Building, Item)> _blockedItemsCleaner = new List<(Building, Item)>();
    private List<Route> _routesQueue = new List<Route>();
    private List<Route> _routesLog = new List<Route>();

    private void OnEnable()
    {
        if (TryGetComponent<ActiveEntity>(out ActiveEntity aiNumber))
        {
            _aiNumber = aiNumber.PlayerNumber;
        }
    }

    private void Update()
    {
        RefreshEntities();
        //UpdateRoutes();
        AppendRouteList();
        StartNewRoutes();
    }

    private void RefreshEntities()
    {
        foreach (ActiveEntity entity in ActiveEntity.GetEntitiesList(_aiNumber))
        {
            _unitProducers.Clear();
            _factories.Clear();
            _storehouses.Clear();
            _freeWorkers.Clear();
            _constructionSites.Clear();
            _otherBuildings.Clear();
            switch (entity)
            {
                case Worker worker:
                    if (_busyWorkers.ContainsKey(worker)) {
                        if (worker.Command == null || !_busyWorkers[worker].Correct)
                        {
                            FinishRoute(worker);
                            _freeWorkers.Add(worker);
                        }
                    }
                    else if (worker.Command == null)
                        _freeWorkers.Add(worker);
                    break;
                case Factory factory:
                    _factories.Add(factory);
                    break;
                case UnitProducer producer:
                    _unitProducers.Add(producer);
                    break;
                case ConstructionSite constructionSite:
                    _constructionSites.Add(constructionSite);

                    break;
                case Building building:
                    _otherBuildings.Add(building);
                    break;
            }
        }
    }

    private void FinishRoute(Worker worker)
    {
        if (worker == null)
            return;
        worker.Command = null;

        Route route = _busyWorkers[worker];
        if (route == null)
            return;

        if (_blockedItems.ContainsKey((route.From, route.Item)))
        {
            _blockedItems[(route.From, route.Item)]-=route.Quantity;
            if (_blockedItems[(route.From, route.Item)] <= 0) 
                _blockedItemsCleaner.Add((route.From, route.Item));
        }
        foreach (var blockedItemToRemove in _blockedItemsCleaner)
            _blockedItems.Remove(blockedItemToRemove);
        _blockedItemsCleaner.Clear();

        _busyWorkers.Remove(worker);
    }

    private void StartNewRoutes()
    {
        throw new NotImplementedException();
    }

    private void AppendRouteList()
    {
        if (_freeWorkers.Count == 0)
            return;

        foreach (UnitProducer producer in _unitProducers)
        {
            if (_routesQueue.Exists(route => route.To == producer))
                continue;
            
            foreach (ItemsSlot slot in producer.ResourcesStorage)
            {
                if (slot.Quantity>=slot.QuantityLimit
                    || slot.MonoItem == null)
                    continue;

                Item requestedItem = slot.MonoItem;
                int requestedQuantity = slot.FreeCapacity(requestedItem);
                Building containsRequestedItem = null;
                int availableQuantity = 0;
               
                foreach (Factory factory in _factories)
                {
                    availableQuantity = factory.ItemsOfTypeToGet(requestedItem, out ItemsSlot s);
                    if (availableQuantity>0)
                    {

                    }
                }   
            }

            
        }
    }

    private void UpdateRoutes()
    {
        throw new NotImplementedException();
    }

    private class Route
    {
        public Building From { get; private set; }
        public Building To { get; private set; }
        public Item Item { get; private set; }
        public int Quantity { get; private set; }

        public Route(Building from, Building to, Item item, int quantity)
        {
            From = from;
            To = to;
            Item = item;
            Quantity = quantity;
            if (Quantity < 0)
                Quantity = 0;
        }

        public bool Correct => (From != null && To != null && Item != null);
    }
}
