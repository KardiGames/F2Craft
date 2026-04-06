using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UnitProducer : Building
{
    private const int OPTIONAL_SLOTS_NUMBER = 2;

    [SerializeField] private BlueprintForUnit _blueprint;
    [SerializeField] private List<ItemsSlot> _resourcesStorage = new();
    [SerializeField] private int _optionalResoursesSlots;
    [SerializeField] private Dictionary<Item, ItemsSlot> _forcedOptionalResources = new();
    [SerializeField] private bool _autoproduction;
    [SerializeField] private int _queuedUnits = 0;
    private ItemRecycler _recycler;
    private float _timer = 0f;
    private bool _isProducting = false;
    private List<Item> _spentOptionalResources = new();
    [SerializeField] private BlueprintForUnit _tmpBlueprint;
    [SerializeField] private Foundation _tmpFoundation;

    public bool Autoproduction { get => _autoproduction; set => _autoproduction = value; }
    public int QueuedUnits => _queuedUnits;
    private void Start()
    {
        //CRUTCH: go.Find
        if (_tmpBlueprint != null)
        {
            Foundation fdt = Instantiate<Foundation>(_tmpFoundation, transform.position, _tmpFoundation.transform.rotation);
            Init(0, 250, 250, GameObject.Find("SingleScripts").GetComponent<ActiveEntitiesManager>(), fdt, _tmpBlueprint);
            fdt.gameObject.SetActive(false);
        }
    }
    public void Init(int playerNumber, int hp, int maxHp, ActiveEntitiesManager activeEntitiesManager, Foundation foundation, BlueprintForUnit unitBlueprint)
    {
        if (foundation == null || unitBlueprint == null)
        {
            print("Error! Unit producer havn't built");
            Destroy();
            return;
        }
        Init(playerNumber, hp, maxHp, activeEntitiesManager);
        _foundation = foundation;
        SetupBlueprint(unitBlueprint);
        _optionalResoursesSlots = OPTIONAL_SLOTS_NUMBER;
    }
    public void SetupBlueprint(BlueprintForUnit blueprint)
    {
        if (blueprint == null)
            return;
        RemoveBlueprint();
        _blueprint = blueprint;

        for (int i = 0; i < _blueprint.Resources.Count; i++)
            _resourcesStorage.Add(new ItemsSlot(_blueprint.Resources[i], _blueprint.ResourcesQuantities[i] * STORAGE_QUANTITY_MULTIPLER));

        for (int i = 0; i < _optionalResoursesSlots; i++)
        {
            _resourcesStorage.Add(new ItemsSlot(STORAGE_QUANTITY_MULTIPLER, blueprint.OptionalResources, null));
        }
    }

    public void AddUnitToQueue()
    {
        _queuedUnits++;
    }
    public void RemoveUnitFromQueue()
    { 
        _queuedUnits--; 
    }

    public void ForceOptionalResource(Item item)
    {
        if (item == null && !_blueprint.OptionalResources.Contains(item) && _forcedOptionalResources.ContainsKey(item))
        {
            print("Error! Forcing optional resourse doesn't happen");
            return;
        }

        ItemsSlot slot = _resourcesStorage.Find((ItemsSlot current) => current.Item == item);
        if (slot is null)
        {
            print("Error! Optional resourse slot hasn't found");
            return;
        }

        slot.SetAccessLists(new Item[] { item }, null);
        _forcedOptionalResources.Add(item, slot);
    }

    public void UnforceOptionalResource(Item item)
    {
        if (item == null && !_forcedOptionalResources.ContainsKey(item))
        {
            print("Error! Unforcing optional resourse doesn't happen");
            return;
        }

        ItemsSlot slot = _forcedOptionalResources[item];
        if (slot is null)
        {
            print("Error! Optional resourse slot to unforce hasn't found");
            return;
        }
        slot.SetAccessLists(_blueprint.OptionalResources, null);
        _forcedOptionalResources.Remove(item);

    }

    public override int ItemsOfTypeToGet(Item item, out ItemsSlot slot)
    {
        int foundSlotCapacity = 0;
        int capacity;
        slot = null;
        for (int i = 0; i < _resourcesStorage.Count; i++)
        {
            if (_resourcesStorage[i].Item == item)
            {
                slot = _resourcesStorage[i];
                return slot.FreeCapacity(item);
            }

            if (foundSlotCapacity > 0)
                continue;
          
            capacity = _resourcesStorage[i].FreeCapacity(item);
            if (capacity>0)
            {
                slot = _resourcesStorage[i];
                foundSlotCapacity = capacity;
            }
        }
        return foundSlotCapacity;
    }

    private void RemoveBlueprint()
    {
        if (_blueprint == null)
            return;

        if (_recycler == null)
            _recycler = GetComponent<ItemRecycler>();

        foreach (ItemsSlot slot in _resourcesStorage)
        {
            if (slot.Item != null && slot.Quantity > 0)
                _recycler.Recycle(slot.Item, slot.Quantity);
        }
        _resourcesStorage.Clear();

        _blueprint = null;
    }

    private void Update()
    {
        if (_isProducting)
        {
            if (_timer > 0f)
                _timer -= Time.deltaTime;
            else
                ProduceUnit();
        }
        else
        {
            StartProduction();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            AddUnitToQueue();
        }
    }

    private void ProduceUnit()
    {
        if (_blueprint.ProducedUnit is BattleUnit producedUnit) {
            BattleUnit unit = Instantiate <BattleUnit> (producedUnit, transform.position, producedUnit.transform.rotation);
            unit.Init(0, 50, 50, _activeEntitiesManager);
        } else if (_blueprint.ProducedUnit is Worker worker)
        {
            Worker newWorker = Instantiate<Worker>(worker, transform.position, worker.transform.rotation);
            worker.Init(5, 5, 5, _activeEntitiesManager);
        }

            _isProducting = false;
        if (_autoproduction == false)
            _queuedUnits--;
        StartProduction();
    }

    private void StartProduction()
    {
        if (IsAbleToStartProduction()
            && TrySpendResources()
            )
        {
            _timer += _blueprint.Time;
            _isProducting = true;
        }
    }

    private bool IsAbleToStartProduction()
    {
        if (!_isProducting
            && _queuedUnits>0
            && _blueprint != null
            //TODO add here check for empty exit
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

        int forcedResources = _forcedOptionalResources.Count;
        _spentOptionalResources.Clear();

        Item currentItem;
        for (int i = _blueprint.Resources.Count; i < _resourcesStorage.Count; i++)
        {
            if (_resourcesStorage[i].Quantity < 0)
                continue;

            currentItem = _resourcesStorage[i].Item;
            if (_resourcesStorage[i].TrySpend(currentItem, 1))
            {
                _spentOptionalResources.Add(currentItem);
                if (_forcedOptionalResources.ContainsKey(currentItem))
                    forcedResources--;
            }
        }

        if (forcedResources > 0)
        {
            print("Error. Not all of forced resouces have spent. Try failed.");
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

        foreach (KeyValuePair<Item, ItemsSlot> forcedResource in _forcedOptionalResources)
        {
            if (forcedResource.Value.Available(forcedResource.Key) < 1)
                return false;
        }

        return true;
    }
}