using UnityEngine;

public class Foundation : MonoBehaviour
{
    [SerializeField] private ConstructionSite _underConstructionPrefab;
    [SerializeField] private BlueprintForBuilding _tmpBuildingBlueprint;
    [SerializeField] private BlueprintForItem _tmpItemToProduce;

    private void Start()
    {
        if (_underConstructionPrefab == null)
            print("Error! Link is null!");
    }

    private void OnMouseUpAsButton()
    {
        StartConstruction (_tmpBuildingBlueprint, _tmpItemToProduce);
    }

    private void StartConstruction (BlueprintForBuilding buildingBlueprint, BlueprintForItem blueprintToSetup)
    {
        ConstructionSite site = Instantiate<ConstructionSite>(_underConstructionPrefab, transform.position, _underConstructionPrefab.transform.rotation);
        //CRUTCH: go.Find
        site.Init(0, GameObject.Find("SingleScripts").GetComponent<ActiveEntitiesManager>(), buildingBlueprint, blueprintToSetup, this);
        

    }
}
