using UnityEngine;

public class Foundation : MonoBehaviour
{
    [SerializeField] private ConstructionSite _underConstructionPrefab;
    [SerializeField] private Building _tmpBuildingToConstruct;
    [SerializeField] private BlueprintForItem _tmpItemToProduce;

    private void Start()
    {
        if (_underConstructionPrefab == null)
            print("Error! Link is null!");
    }

    private void OnMouseUpAsButton()
    {
        StartConstruction (_tmpBuildingToConstruct);
    }

    private void StartConstruction (Building building)
    {
        ConstructionSite site = Instantiate<ConstructionSite>(_underConstructionPrefab, transform.position, _underConstructionPrefab.transform.rotation);
        //CRUTCH: go.Find
        site.Init(0, 1, GameObject.Find("SingleScripts").GetComponent<ActiveEntitiesManager>());
        

    }
}
