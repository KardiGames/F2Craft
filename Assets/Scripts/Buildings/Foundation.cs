using UnityEngine;

public class Foundation : MonoBehaviour
{
    [SerializeField] private ConstructionSite _underConstructionPrefab;
    [SerializeField] private BlueprintForBuilding _tmpBuildingBlueprint;
    [SerializeField] private BlueprintForItem _tmpItemToProduce;
    [SerializeField] private BlueprintForUnit _tmpUnitToProduce;
    [SerializeField] private int _tmpPlayerNumber;

    public bool IsInstantiated { get; private set; } = false;
    public void EnableSelection (bool isSelected)
    {

    }
    public void ConstructCurrentTempSetup()
    {
        if (_tmpItemToProduce != null)
            StartFactoryConstruction(_tmpBuildingBlueprint, _tmpItemToProduce);
        else if (_tmpBuildingBlueprint != null)
            StartUnitProducerConstruction(_tmpBuildingBlueprint, _tmpUnitToProduce);
    }
    private void Start()
    {
        if (_underConstructionPrefab == null)
            Debug.LogError("Link is null!");

        IsInstantiated = true;
    }


    private void StartFactoryConstruction (BlueprintForBuilding buildingBlueprint, BlueprintForItem blueprintToSetup)
    {
        ConstructionSite site = Instantiate<ConstructionSite>(_underConstructionPrefab, transform.position, _underConstructionPrefab.transform.rotation);
        //CRUTCH: go.Find
        site.Init(_tmpPlayerNumber, buildingBlueprint, this, blueprintToSetup, null);
    }
    private void StartUnitProducerConstruction(BlueprintForBuilding buildingBlueprint, BlueprintForUnit blueprintToSetup)
    {
        ConstructionSite site = Instantiate<ConstructionSite>(_underConstructionPrefab, transform.position, _underConstructionPrefab.transform.rotation);
        //CRUTCH: go.Find
        site.Init(_tmpPlayerNumber, buildingBlueprint, this, null, blueprintToSetup);

    }
}
