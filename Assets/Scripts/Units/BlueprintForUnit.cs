using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitBlueprint", menuName = "BlueprintForUnit")]
public class BlueprintForUnit : ScriptableObject
{
    [SerializeField] private Unit _producedUnit;
    [SerializeField] private int _time;
    [SerializeField] private List<Item> _resources;
    [SerializeField] private List<int> _resourcesQuantities;
    [SerializeField] private List<Item> _optionalResources;

    public Unit ProducedUnit => _producedUnit;
    public int Time => _time;
    public IReadOnlyList<Item> Resources => _resources;
    public IReadOnlyList<int> ResourcesQuantities => _resourcesQuantities;
    public IReadOnlyCollection<Item> OptionalResources => _optionalResources;

}