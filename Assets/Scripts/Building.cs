using System.Collections.Generic;
using UnityEngine;

public class Building : HexCellContent
{
    public int upgradeCount => m_upgradePath.Count;
    public bool canBeDowngraded => upgradeCount > 1;
    private BuildingData buildingData => m_upgradePath.Count > 0 ? m_upgradePath.Peek() : null;
    public override CellContentData data => buildingData;
    public float progress { get; set; }
    public int workers => m_workers;
    public int maxWorkers => buildingData != null ? buildingData.maxWorkers : 0;
    public ResourceStorage storedResources => m_storedResources;

    private readonly Stack<BuildingData> m_upgradePath = new Stack<BuildingData>();
    private readonly ResourceStorage m_storedResources = new ResourceStorage();
    private int m_workers = 0;

    private void Update()
    {
        if (buildingData == null)
            return;

        buildingData.OnInstanceUpdated(this);
    }

    public void AddWorkers(int workers)
    {
        Debug.Assert(m_workers + workers <= maxWorkers, $"Invalid worker number: {m_workers + workers}/{maxWorkers}", this);
        m_workers += workers;

        if (buildingData is HousingData)
            Player.instance.AddPopulation(workers);
        else
            Player.instance.AssignWorkers(workers);
    }

    public void RemoveWorkers(int workers)
    {
        Debug.Assert(workers <= m_workers, $"Invalid worker number: {workers}/{m_workers}", this);
        m_workers -= workers;

        if (buildingData is HousingData)
            Player.instance.RemovePopulation(workers);
        else
            Player.instance.FreeWorkers(workers);
    }

    public void Initialize(BuildingData data)
    {
        Debug.Assert(upgradeCount == 0, this);
        m_upgradePath.Push(data);
        OnDataChanged();
    }

    public override void OnPlacedOn(HexCell cell)
    {
        base.OnPlacedOn(cell);

        Debug.Assert(buildingData != null, this);
        buildingData.OnInstanceBuilt(this);
    }

    public override void OnRemoved()
    {
        RemoveWorkers(workers);

        if (buildingData)
            buildingData.OnInstanceDemolished(this);
    }

    public bool CanBeUpgraded()
        => buildingData != null && buildingData.hasUpgrade && buildingData.upgrade.CanBeAfforded() && buildingData.upgrade.CanBePlacedOn(cell);

    public void Upgrade()
    {
        Debug.Assert(buildingData != null, this);
        Debug.Assert(CanBeUpgraded(), this);

        buildingData.OnInstanceUpgradedFrom(this);

        m_upgradePath.Pop();
        OnDataChanged();

        Player.instance.resources.UseResources(buildingData.resourceCost);

        buildingData.OnInstanceUpgradedTo(this);
    }

    public void Downgrade()
    {
        Debug.Assert(buildingData != null, this);
        Debug.Assert(canBeDowngraded, this);

        buildingData.OnInstanceDowngradedFrom(this);

        m_upgradePath.Push(buildingData.upgrade);
        OnDataChanged();

        buildingData.OnInstanceDowngradedTo(this);
    }

    private void OnDataChanged()
    {
        Debug.Assert(buildingData != null, this);
        UpdateModel(buildingData.buildingPrefab);
    }
}
