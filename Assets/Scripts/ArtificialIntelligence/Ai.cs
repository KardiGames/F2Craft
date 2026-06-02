using System;
using System.Collections.Generic;
using UnityEngine;

public class Ai : MonoBehaviour
{
    [SerializeField] private int _aiNumber;
    private List<Worker> _freeWorkers = new();
    private Dictionary<Worker, Route> _busyWorkers = new Dictionary<Worker, Route>();
    private List<UnitProducer> _unitProducers = new List<UnitProducer>();
    private List<Factory> _factories = new List<Factory>();
    private List<Storehouse> _storehouses = new List<Storehouse>();
    private List<ConstructionSite> _constructionSites = new List<ConstructionSite>();
    private List<Building> _otherBuildings = new List<Building>();
    Dictionary<(Building, Item), int> _blockedItems = new Dictionary<(Building, Item), int>();

    private void OnEnable()
    {
        if (TryGetComponent<ActiveEntity>(out ActiveEntity aiNumber))
        {
            _aiNumber = aiNumber.PlayerNumber;
            ActiveEntity.S_OnEntityRemoved+=
        }
    }

    private void OnDisable()
    {
        
    }

    private void Update()
    {
        RefreshEntities();
        UpdateRoutes();
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
        Route route = _busyWorkers[worker];
        if (route == null)
            return;
        if (_blockedItems.ContainsKey((route.From, route.Item)))
        {
            _blockedItems[(route.From, route.Item)]-=route.Quantity;
            if (_blockedItems[(route.From, route.Item)]<=0)
                _blockedItems.Remove((route.From, route.Item)); //TODO FIX IT!!!
        }
        _busyWorkers[worker] = route;
    }

    private void StartNewRoutes()
    {
        throw new NotImplementedException();
    }

    private void AppendRouteList()
    {
        throw new NotImplementedException();
    }

    private void UpdateRoutes()
    {
        throw new NotImplementedException();
    }

    private void RemoveEntityHandler (ActiveEntity entity)
    {
        if (entity == null) { 
            Debug.LogError("Ai got null on Remove Entity handler");
        return;
        }
        if (entity.PlayerNumber != _aiNumber)
            return;

        List<(Building, Item)> _keysToRemove
        foreach (var blocker in _blockedItems.Keys)
        {
            if (blocker.Item1 == entity)
            {
                _blockedItems.Remove()
            }
        }
        
        //add removing from queue
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
