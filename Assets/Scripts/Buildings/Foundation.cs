using UnityEngine;

public class Foundation : MonoBehaviour
{
    [SerializeField] private ConstructionSite _underConstructionPrefab;

    public bool IsInstantiated { get; private set; } = false;
    private void Start()
    {
        if (_underConstructionPrefab == null)
            Debug.LogError("Link is null!");

        IsInstantiated = true;
    }


    public void BuildFactory(int playerNumber, BlueprintForBuilding buildingBlueprint, BlueprintForItem blueprintToSetup)
    {
        ConstructionSite site = Instantiate<ConstructionSite>(_underConstructionPrefab, transform.position, _underConstructionPrefab.transform.rotation);
        site.Init(playerNumber, buildingBlueprint, this, blueprintToSetup, null);
    }
    public void BuildUnitProducer(int playerNumber, BlueprintForBuilding buildingBlueprint, BlueprintForUnit blueprintToSetup)
    {
        ConstructionSite site = Instantiate<ConstructionSite>(_underConstructionPrefab, transform.position, _underConstructionPrefab.transform.rotation);
        site.Init(playerNumber, buildingBlueprint, this, null, blueprintToSetup);

    }
}
