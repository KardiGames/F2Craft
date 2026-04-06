using UnityEngine;

public class Foundation : MonoBehaviour
{
    [SerializeField] private ConstructionSite _underConstructionPrefab;
    [SerializeField] private BlueprintForBuilding _tmpBuildingBlueprint;
    [SerializeField] private BlueprintForItem _tmpItemToProduce;
    [SerializeField] private BlueprintForUnit _tmpUnitToProduce;

    private void Start()
    {
        if (_underConstructionPrefab == null)
            print("Error! Link is null!");
    }

    private void OnMouseUpAsButton()
    {
        StartUnitProducerConstruction (_tmpBuildingBlueprint, _tmpUnitToProduce);
    }

    private void StartFactoryConstruction (BlueprintForBuilding buildingBlueprint, BlueprintForItem blueprintToSetup)
    {
        ConstructionSite site = Instantiate<ConstructionSite>(_underConstructionPrefab, transform.position, _underConstructionPrefab.transform.rotation);
        //CRUTCH: go.Find
        site.Init(0, GameObject.Find("SingleScripts").GetComponent<ActiveEntitiesManager>(), buildingBlueprint, this, blueprintToSetup, null);
    }
    private void StartUnitProducerConstruction(BlueprintForBuilding buildingBlueprint, BlueprintForUnit blueprintToSetup)
    {
        ConstructionSite site = Instantiate<ConstructionSite>(_underConstructionPrefab, transform.position, _underConstructionPrefab.transform.rotation);
        //CRUTCH: go.Find
        site.Init(0, GameObject.Find("SingleScripts").GetComponent<ActiveEntitiesManager>(), buildingBlueprint, this, null, blueprintToSetup);

    }
}
