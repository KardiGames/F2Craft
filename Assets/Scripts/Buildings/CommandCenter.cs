using System.Collections.Generic;
using UnityEngine;

public class CommandCenter : Building
{
    [SerializeField] private Item _materiaItem;
    [SerializeField] private float _producingTime;
    [SerializeField] private ItemsSlot _materiaStorage;
    [SerializeField] private int _materiaCapacity;
    private float _timer = 0f;
    private bool _isProducting = false;
    [SerializeField] private Foundation _tmpFoundation; // CRUTCH delete this

    private void Start()
    {
        //CRUTCH: go.Find

            Foundation fdt = Instantiate<Foundation>(_tmpFoundation, transform.position, _tmpFoundation.transform.rotation);
            Init(0, 500, 500, GameObject.Find("SingleScripts").GetComponent<ActiveEntitiesManager>(), fdt);
            fdt.gameObject.SetActive(false);
    }
    public void Init(int playerNumber, int hp, int maxHp, ActiveEntitiesManager activeEntitiesManager, Foundation foundation)
    {
        if (foundation == null || _materiaItem == null)
        {
            Destroy();
            return;
        }
        Init(playerNumber, hp, maxHp, activeEntitiesManager);
        _foundation = foundation;
        _materiaStorage = new ItemsSlot(_materiaItem, _materiaCapacity);
    }

    public override IEnumerable<Item> ItemsToGive()
    {
        List<Item> items = new List<Item>();
            if (_materiaStorage.Quantity > 0 && _materiaStorage.Item != null)
                items.Add(_materiaItem);
        return items;
    }

    public override int ItemsOfTypeToGive(Item item, out ItemsSlot slot)
    {
        slot = _materiaStorage;
        return _materiaStorage.Available(item);
    }

    private void Update()
    {
        if (_isProducting)
        {
            if (_timer > 0f)
                _timer -= Time.deltaTime;
            else
                ProduceMateria();
        }
        else
        {
            StartProduction();
        }

    }

    private void ProduceMateria()
    {


        if (!_materiaStorage.TryStore(_materiaItem, 1))
                print("Error. Something goes wrong with materia production");


        _isProducting = false;
        StartProduction();
    }

    private void StartProduction()
    {
        if (IsAbleToStartProduction())
        {
            _timer += _producingTime;
            _isProducting = true;
        }
    }

    private bool IsAbleToStartProduction()
    {
        if (!_isProducting
            && _materiaItem != null
            && HaveFreeSpaceForProduction()
            )
            return true;
        return false;
    }

    private bool HaveFreeSpaceForProduction()
    {
        if (_materiaStorage.Quantity < _materiaStorage.QuantityLimit)
            return true;
        else
            return false;
    }
}
