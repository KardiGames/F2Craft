using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingBlueprint", menuName = "BlueprintForBuilding")]
public class BlueprintForBuilding : ScriptableObject
{    
    [SerializeField] private Building _buildingPefub;
    [SerializeField] private int _time;
    [SerializeField] private List<Item> _resources;
    [SerializeField] private List<int> _resourcesQuantities;

    public Building ConstructingBuilding => _buildingPefub;
    public int Time => _time;
    public IReadOnlyList<Item> Resources => _resources;
    public IReadOnlyList<int> ResourcesQuantities => _resourcesQuantities;

}
