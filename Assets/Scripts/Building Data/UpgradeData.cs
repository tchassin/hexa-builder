using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct BuildingUpgradeData
{
    public BuildingData buildingData;
    public bool isAutomatic;

    [Header("Requirements")]
    public bool requiresAccessToWokers;
    public List<ResourceData> requiredResourceAccess;
    public List<ResourceNumber> resourceCost;

    public bool CanBeAfforded()
        => Player.instance.resources.HasResources(resourceCost);

    public bool CanBeUpgradedFrom(Building building)
    {
        if (!building.cell.HasAccessToRoad())
            return false;

        if (requiresAccessToWokers && !building.cell.HasAccessToWorkers())
            return false;

        foreach (var resource in requiredResourceAccess)
        {
            if (!building.cell.HasAccessToResource(resource))
                return false;
        }

        return true;
    }
}
