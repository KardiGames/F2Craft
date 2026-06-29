using System;
using UnityEngine;

public class Foundation : MonoBehaviour
{
    [SerializeField] private ConstructionSite _underConstructionPrefab;

    public bool IsInstantiated { get; private set; } = false;

    private void Awake()
    {
        IsInstantiated = true;
    }
    private void Start()
    {
        if (_underConstructionPrefab == null)
            Debug.LogError("Link is null!");
    }


    public ConstructionSite BuildFactory(int playerNumber, BlueprintForBuilding buildingBlueprint, BlueprintForItem blueprintToSetup)
    {
        if (!isActiveAndEnabled)
        {
            Debug.LogError("Building error. Foundation is occupyed");
            return null;
        }

        ConstructionSite site = Instantiate<ConstructionSite>(_underConstructionPrefab, transform.position, _underConstructionPrefab.transform.rotation);
        site.Init(playerNumber, buildingBlueprint, this, blueprintToSetup, null);
        return site;
    }
    public ConstructionSite BuildUnitProducer(int playerNumber, BlueprintForBuilding buildingBlueprint, BlueprintForUnit blueprintToSetup)
    {
        if (!isActiveAndEnabled)
        {
            Debug.LogError("Building error. Foundation is occupyed");
            return null;
        }
        ConstructionSite site = Instantiate<ConstructionSite>(_underConstructionPrefab, transform.position, _underConstructionPrefab.transform.rotation);
        site.Init(playerNumber, buildingBlueprint, this, null, blueprintToSetup);
        return site;

    }
    public ConstructionSite BuildStorehouse(int playerNumber, BlueprintForBuilding buildingBlueprint)
    {
        if (!isActiveAndEnabled)
        {
            Debug.LogError("Building error. Foundation is occupyed");
            return null;
        }
        ConstructionSite site = Instantiate<ConstructionSite>(_underConstructionPrefab, transform.position, _underConstructionPrefab.transform.rotation);
        site.Init(playerNumber, buildingBlueprint, this, null, null);
        return site;
    }
}
