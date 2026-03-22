using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemBlueprint", menuName = "BlueprintForItem")]
public class BlueprintForItem : ScriptableObject
{    
    [SerializeField] private List<Item> _production;
    [SerializeField] private List<int> _productionQuantities;
    [SerializeField] private int _time;
    [SerializeField] private List<Item> _resources;
    [SerializeField] private List<int> _resourcesQuantities;

    public IReadOnlyList<Item> Production => _production;
    public IReadOnlyList<int> ProductionQuantities => _productionQuantities;
    public int Time => _time;
    public IReadOnlyList<Item> Resources => _resources;
    public IReadOnlyList<int> ResourcesQuantities => _resourcesQuantities;

}
