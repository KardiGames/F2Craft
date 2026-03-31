using UnityEngine;

public class ConstructionSite : Building
{
    [SerializeField] private Building _constructingBuildingPrefub;
    private BlueprintForItem _itemBlueprint;
    public BlueprintForItem ItemBlueprint
    {
        set
        {
            if (_itemBlueprint == null)
            {
                _itemBlueprint = value;
            }
            else
            {
                print("Error. Attempt to set producing item twise. Rejected.");
            }
        }
    }


}
