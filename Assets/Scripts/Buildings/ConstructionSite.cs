using System;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionSite : Building
{
    [SerializeField] private BlueprintForBuilding _buildingBlueprint;
    [SerializeField] private BlueprintForItem _itemBlueprint;
    [SerializeField] private BlueprintForUnit _unitBlueprint;
    [SerializeField] private List<ItemsSlot> _resourcesStorage = new();

    private Renderer _renderer;
    private int _totalResourcesCost = 0;
    private float _timeForResource;
    private float _timer = 0f;
    [SerializeField] private int _stageResuorcesCost = 0; //CRUTCH delete SerField

    public string ConstructedBuildingName => _buildingBlueprint.ConstructingBuilding.name;
    public IEnumerable<ItemsSlot> ResourcesStorage => _resourcesStorage;
    public int StageTimer { get; private set; }
    public BlueprintForBuilding BuildingBlueprint => _buildingBlueprint;
    public BlueprintForItem ItemBlueprint => _itemBlueprint;

    public void Init(int playerNumber, BlueprintForBuilding buildingBlueprint, Foundation foundation, BlueprintForItem itemBlueprint, BlueprintForUnit unitBlueprint)
    {
        if (buildingBlueprint == null || foundation == null)
        {
            print("Error! ConstructionSite initialisation aborted");
            return;
        }

        if ((buildingBlueprint.ConstructingBuilding is Factory && itemBlueprint == null) && (buildingBlueprint.ConstructingBuilding is UnitProducer && unitBlueprint == null))
        {
            print("Error! ConstructionSite initialisation aborted");
            return;
        }

        if (_buildingBlueprint != null || _itemBlueprint != null || _foundation != null)
        {
            print("Error. ConstructionSite re-initialisation aborted");
            return;
        }

        _maxHp = buildingBlueprint.ConstructingBuilding.MaxHp;
        Init(playerNumber);
        _buildingBlueprint = buildingBlueprint;
        _itemBlueprint = itemBlueprint;
        _unitBlueprint = unitBlueprint;
        _foundation = foundation;

        for (int i = 0; i < _buildingBlueprint.Resources.Count; i++)
        {
            _resourcesStorage.Add(new ItemsSlot(_buildingBlueprint.Resources[i], BuildingBlueprint.ResourcesQuantities[i]));
            _totalResourcesCost += BuildingBlueprint.ResourcesQuantities[i];
        }
        foreach (var resource in _resourcesStorage)
            resource.OnContentChanged += OnParameterChangedInvoke;

        if (_totalResourcesCost <= 0)
        {
            Debug.LogError("ResourcesCost for building is 0. Destroying.");
            Destroy(gameObject);
            return;
        }

        _timeForResource = (float)_buildingBlueprint.Time / _totalResourcesCost;
        _foundation.gameObject.SetActive(false);
    }

    public override int ItemsOfTypeToGet(Item item, out ItemsSlot slot)
    {
        int capacity;
        slot = null;
        for (int i = 0; i < _resourcesStorage.Count; i++)
        {
            capacity = _resourcesStorage[i].FreeCapacity(item);
            if (capacity > 0)
            {
                slot = _resourcesStorage[i];
                return capacity;
            }
        }
        return 0;
    }
    private void Update()
    {
        if (_timer > 0f)
        {
            _timer -= Time.deltaTime;
            if (_timer + 1 < StageTimer)
            {
                StageTimer = (int)_timer + 1;
                OnParameterChangedInvoke();
            }
        }
        else if (_stageResuorcesCost > 0)
            FinishStage();
        else
            StartStage();
    }

    private void StartStage()
    {
        int resourcesSpent = 0;
        int quantity;

        foreach (ItemsSlot slot in _resourcesStorage)
        {
            quantity = slot.Quantity;
            if (slot.TryPermanentSpend(slot.Item, quantity))
                resourcesSpent += quantity;
        }

        if (resourcesSpent > 0)
        {
            _timer += resourcesSpent * _timeForResource;
            StageTimer = (int)_timer + 1;
            _stageResuorcesCost = resourcesSpent;
            OnParameterChangedInvoke();
        }
    }

    private void FinishStage()
    {
        _hp = Mathf.Min(_hp + Mathf.CeilToInt((float)_stageResuorcesCost / _totalResourcesCost * _buildingBlueprint.ConstructingBuilding.MaxHp), _maxHp);


        int currentResourcesCost = 0;
        foreach (ItemsSlot slot in _resourcesStorage)
            currentResourcesCost += slot.QuantityLimit;

        _stageResuorcesCost = 0;

        if (currentResourcesCost <= 0)
        {
            FinishConstruction();
        }
        else
        {
            if (_renderer == null)
                _renderer = gameObject.GetComponent<Renderer>();
            Color color = _renderer.material.color;
            color.a = MathF.Min(((float)_totalResourcesCost - currentResourcesCost) / _totalResourcesCost, 1f);
            _renderer.material.color = color;
            StageTimer = 0;
            OnParameterChangedInvoke();
        }
    }

    private void FinishConstruction()
    {
        if (_buildingBlueprint.ConstructingBuilding is Factory factoryBlueprint)
        {
            Factory factory = Instantiate<Factory>(factoryBlueprint, transform.position, factoryBlueprint.transform.rotation);
            factory.Init(_playerNumber, _hp, _foundation, _itemBlueprint);
            Replace(this, factory);
        }
        else if (_buildingBlueprint.ConstructingBuilding is UnitProducer unitProducerBlueprint)
        {
            UnitProducer producer = Instantiate<UnitProducer>(unitProducerBlueprint, transform.position, unitProducerBlueprint.transform.rotation);
            producer.Init(_playerNumber, _hp, _foundation, _unitBlueprint);
            Replace(this, producer);
        }
    }
}


