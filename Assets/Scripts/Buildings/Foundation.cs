using UnityEngine;

public class Foundation : MonoBehaviour
{
    [SerializeField] private ActiveEntitiesManager _activeEntitiesManager;
    [SerializeField] private ConstructionSite _underConstructionPrefab;
    [SerializeField] private BlueprintForBuilding _tmpBuildingBlueprint;
    [SerializeField] private BlueprintForItem _tmpItemToProduce;
    [SerializeField] private BlueprintForUnit _tmpUnitToProduce;
    [SerializeField] private int _playerNumber;

    public void EnableSelection (bool isSelected)
    {

    }
    public void ConstructCurrenTempSetup()
    {
        if (_tmpItemToProduce != null)
            StartFactoryConstruction(_tmpBuildingBlueprint, _tmpItemToProduce);
        else if (_tmpBuildingBlueprint != null)
            StartUnitProducerConstruction(_tmpBuildingBlueprint, _tmpUnitToProduce);
    }
    private void Start()
    {
        if (_underConstructionPrefab == null)
            print("Error! Link is null!");
        if (_activeEntitiesManager == null)
        {
            print("Error. Link is lost. Got from GO FIND");
            _activeEntitiesManager = GameObject.Find("SingleScripts").GetComponent<ActiveEntitiesManager>();
        }
    }


    private void StartFactoryConstruction (BlueprintForBuilding buildingBlueprint, BlueprintForItem blueprintToSetup)
    {
        ConstructionSite site = Instantiate<ConstructionSite>(_underConstructionPrefab, transform.position, _underConstructionPrefab.transform.rotation);
        //CRUTCH: go.Find
        site.Init(_playerNumber, _activeEntitiesManager, buildingBlueprint, this, blueprintToSetup, null);
    }
    private void StartUnitProducerConstruction(BlueprintForBuilding buildingBlueprint, BlueprintForUnit blueprintToSetup)
    {
        ConstructionSite site = Instantiate<ConstructionSite>(_underConstructionPrefab, transform.position, _underConstructionPrefab.transform.rotation);
        //CRUTCH: go.Find
        site.Init(_playerNumber, _activeEntitiesManager, buildingBlueprint, this, null, blueprintToSetup);

    }
}
