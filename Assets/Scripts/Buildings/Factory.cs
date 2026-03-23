using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class Factory : Building
{
    [SerializeField] private BlueprintForItem _blueprint;
    [SerializeField] private List<ItemsSlot> _productionStorage;
    [SerializeField] private List<ItemsSlot> _resourcesStorage;
    private ItemRecycler _recycler;
    private float _timer=0f;
    private bool _isProducting=false;
    [SerializeField] private BlueprintForItem _tmpBlueprint;

    private void Start()
    {
        SetupBlueprint(_tmpBlueprint);
    }
    public void SetupBlueprint(BlueprintForItem blueprint)
    {
        if (blueprint == null)
            return;
        RemoveBlueprint();
        _blueprint = blueprint;
        
        for (int i=0; i<_blueprint.Production.Count; i++)
            _productionStorage.Add(new ItemsSlot(_blueprint.Production[i], _blueprint.ProductionQuantities[i]*STORAGE_QUANTITY_MULTIPLER));
        for (int i = 0; i < _blueprint.Resources.Count; i++)
            _resourcesStorage.Add(new ItemsSlot(_blueprint.Resources[i], _blueprint.ResourcesQuantities[i] * STORAGE_QUANTITY_MULTIPLER));

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
                _recycler.Recycle(slot.Item, slot.Quantity);
        }
        _productionStorage.Clear();

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
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
                ProduceItem();
        } 
        else
        {
            StartProduction();
        }
            
    }

    private void ProduceItem ()
    {

        for (int i = 0; i < _blueprint.Production.Count; i++)
        {
            if (!_productionStorage[i].TryGet(_blueprint.Production[i], _blueprint.ProductionQuantities[i]))
                print ("Error. Something goes wrong with production "+_blueprint.Production[i]);
        }

        _isProducting = false;
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
            && _blueprint != null
            && HaveFreeSpaceForProduction ()
            && HaveEnoughResources()
            )
            return true;
        return false;
    }

    private bool TrySpendResources ()
    {
        for (int i = 0; i < _blueprint.Resources.Count; i++)
        {
            if (!_resourcesStorage[i].TrySpend(_blueprint.Resources[i], _blueprint.ResourcesQuantities[i]))
                return false;
        }
        return true;
    }

    private bool HaveFreeSpaceForProduction ()
    {
        for (int i=0; i<_blueprint.Production.Count; i++)
        {
            if (_productionStorage[i].FreeCapacity(_blueprint.Production[i]) < _blueprint.ProductionQuantities[i])
                return false;
        }

        return true;
    }

    private bool HaveEnoughResources ()
    {
        for (int i = 0; i < _blueprint.Resources.Count; i++)
        {
            if (_resourcesStorage[i].Available(_blueprint.Resources[i]) < _blueprint.ResourcesQuantities[i])
                return false;
        }

        return true;
    }
}