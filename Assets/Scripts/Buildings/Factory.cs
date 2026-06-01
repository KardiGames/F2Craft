using System.Collections.Generic;
using UnityEngine;

public class Factory : Building
{
    [SerializeField] private BlueprintForItem _blueprint;
    [SerializeField] private List<ItemsSlot> _productionStorage;
    [SerializeField] private List<ItemsSlot> _resourcesStorage;
    private ItemRecycler _recycler;
    private float _timer = 0f;
    private bool _isProducting = false;
    [SerializeField] private BlueprintForItem _tmpBlueprint;

    public int ItemTimer { get; private set; }
    public BlueprintForItem Blueprint => _blueprint;
    public IEnumerable<ItemsSlot> ResourcesStorage => _resourcesStorage;
    public IEnumerable<ItemsSlot> ProductionStorage => _productionStorage;

    protected override void Start()
    {
        base.Start();
        if (_tmpBlueprint != null)
        {
            SetupBlueprint(_tmpBlueprint);
            print("Crutch. Factory blueprint set by Start()");
        }
    }
    public void Init(int playerNumber, int hp, Foundation foundation, BlueprintForItem itemBlueprint)
    {
        if (foundation == null || foundation.IsInstantiated == false || itemBlueprint == null)
        {
            Debug.LogError("Factory initialisation aborted. Destroying. GO: " + gameObject.name);
            Destroy();
            return;
        }
        _hp = hp;
        Init(playerNumber);
        _foundation = foundation;
        SetupBlueprint(itemBlueprint);

    }
    public void SetupBlueprint(BlueprintForItem blueprint)
    {
        if (blueprint == null)
            return;
        RemoveBlueprint();
        _blueprint = blueprint;

        for (int i = 0; i < _blueprint.Production.Count; i++)
            _productionStorage.Add(new ItemsSlot(_blueprint.Production[i], _blueprint.ProductionQuantities[i] * STORAGE_QUANTITY_MULTIPLER));
        foreach (ItemsSlot slot in _productionStorage)
            slot.OnContentChanged += OnParameterChangedInvoke;
        for (int i = 0; i < _blueprint.Resources.Count; i++)
            _resourcesStorage.Add(new ItemsSlot(_blueprint.Resources[i], _blueprint.ResourcesQuantities[i] * STORAGE_QUANTITY_MULTIPLER));
        foreach (ItemsSlot slot in _resourcesStorage)
            slot.OnContentChanged += OnParameterChangedInvoke;
        OnParameterChangedInvoke();
    }

    public override IEnumerable<Item> ItemsToGive()
    {
        List<Item> items = new List<Item>();
        foreach (ItemsSlot slot in _productionStorage)
            if (slot.Quantity > 0 && slot.Item != null)
                items.Add(slot.Item);
        return items;
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

    public override int ItemsOfTypeToGive(Item item, out ItemsSlot slot)
    {
        slot = null;
        for (int i = 0; i < _productionStorage.Count; i++)
        {
            if (_productionStorage[i].Item == item)
            {
                slot = _productionStorage[i];
                return _productionStorage[i].Available(item);
            }
        }
        return 0;
    }

    private void RemoveBlueprint()
    {
        if (_blueprint == null)
            return;

        if (_recycler == null)
            _recycler = GetComponent<ItemRecycler>();

        foreach (ItemsSlot slot in _productionStorage)
        {
            if (slot.Item != null && slot.Quantity > 0)
            {
                _recycler.Recycle(slot.Item, slot.Quantity);
                slot.OnContentChanged -= OnParameterChangedInvoke;
            }
        }
        _productionStorage.Clear();

        foreach (ItemsSlot slot in _resourcesStorage)
        {
            if (slot.Item != null && slot.Quantity > 0)
            {
                _recycler.Recycle(slot.Item, slot.Quantity);
                slot.OnContentChanged -= OnParameterChangedInvoke;
            }
        }
        _resourcesStorage.Clear();

        _blueprint = null;
        OnParameterChangedInvoke();
    }

    private void Update()
    {
        if (_isProducting)
        {
            if (_timer > 0f)
            {
                _timer -= Time.deltaTime;
                if (_timer + 1 < ItemTimer)
                {
                    ItemTimer = (int)_timer + 1;
                    OnParameterChangedInvoke();
                }
            }
            else
                ProduceItem();
        }
        else
        {
            StartProduction();
        }

    }

    private void ProduceItem()
    {

        for (int i = 0; i < _blueprint.Production.Count; i++)
        {
            if (!_productionStorage[i].TryStore(_blueprint.Production[i], _blueprint.ProductionQuantities[i]))
                print("Error. Something goes wrong with production " + _blueprint.Production[i]);
        }

        _isProducting = false;
        ItemTimer = 0;
        OnParameterChangedInvoke();
        StartProduction();
    }

    private void StartProduction()
    {
        if (IsAbleToStartProduction()
            && TrySpendResources()
            )
        {
            _timer += _blueprint.Time;
            ItemTimer = (int)_timer + 1;
            _isProducting = true;
            OnParameterChangedInvoke();
        }
    }

    private bool IsAbleToStartProduction()
    {
        if (!_isProducting
            && _blueprint != null
            && HaveFreeSpaceForProduction()
            && HaveEnoughResources()
            )
            return true;
        return false;
    }

    private bool TrySpendResources()
    {
        for (int i = 0; i < _blueprint.Resources.Count; i++)
        {
            if (!_resourcesStorage[i].TrySpend(_blueprint.Resources[i], _blueprint.ResourcesQuantities[i]))
                return false;
        }
        return true;
    }

    private bool HaveFreeSpaceForProduction()
    {
        for (int i = 0; i < _blueprint.Production.Count; i++)
        {
            if (_productionStorage[i].FreeCapacity(_blueprint.Production[i]) < _blueprint.ProductionQuantities[i])
                return false;
        }

        return true;
    }

    private bool HaveEnoughResources()
    {
        for (int i = 0; i < _blueprint.Resources.Count; i++)
        {
            if (_resourcesStorage[i].Available(_blueprint.Resources[i]) < _blueprint.ResourcesQuantities[i])
                return false;
        }

        return true;
    }
}