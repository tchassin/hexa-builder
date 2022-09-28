using System.Collections.Generic;
using UnityEngine;

public class Building : HexCellContent
{
    public int upgradeCount => m_upgradePath.Count;
    public bool canBeDowngraded => upgradeCount > 1;
    public BuildingData data => m_upgradePath.Count > 0 ? m_upgradePath.Peek() : null;
    public float progress { get; set; }
    public int workers => m_workers;
    public int maxWorkers => data != null ? data.maxWorkers : 0;
    public ResourceStorage storedResources => m_storedResources;

    public override TerrainType requiredTerrainType => TerrainType.Ground;
    private readonly Stack<BuildingData> m_upgradePath = new Stack<BuildingData>();
    private readonly ResourceStorage m_storedResources = new ResourceStorage();
    private int m_workers = 0;

    private void Update()
    {
        if (data == null)
            return;

        data.OnInstanceUpdated(this);
    }

    public void AddWorkers(int workers)
    {
        Debug.Assert(m_workers + workers <= maxWorkers, $"Invalid worker number: {m_workers + workers}/{maxWorkers}", this);
        m_workers += workers;

        if (data is HousingData)
            Player.instance.AddPopulation(workers);
        else
            Player.instance.AssignWorkers(workers);
    }
    public void RemoveWorkers(int workers)
    {
        Debug.Assert(workers <= m_workers, $"Invalid worker number: {workers}/{m_workers}", this);
        m_workers -= workers;

        if (data is HousingData)
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

    public void GetNeighborBuildings(out List<HexDirection> directions)
    {
        directions = new List<HexDirection>();

        for (int i = 0; i < 6; i++)
        {
            if (cell.neighbors[i] != null && cell.neighbors[i].content is Building)
                directions.Add((HexDirection)i);
        }
    }

    public void GetNeighborBuildings(BuildingData buildingData, out List<HexDirection> directions)
    {
        directions = new List<HexDirection>();

        for (int i = 0; i < 6; i++)
        {
            if (cell.neighbors[i] != null && cell.neighbors[i].content is Building building && building.data == buildingData)
                directions.Add((HexDirection)i);
        }
    }

    public void GetNeighborBuildings(out List<Building> neighbors)
    {
        neighbors = new List<Building>();

        for (int i = 0; i < 6; i++)
        {
            if (cell.neighbors[i] != null && cell.neighbors[i].content is Building building)
                neighbors.Add(building);
        }
    }

    public void GetNeighborBuildings(BuildingData buildingData, out List<Building> neighbors)
    {
        neighbors = new List<Building>();

        for (int i = 0; i < 6; i++)
        {
            if (cell.neighbors[i] != null && cell.neighbors[i].content is Building building && building.data == buildingData)
                neighbors.Add(building);
        }
    }

    public override void OnPlacedOn(HexCell cell)
    {
        base.OnPlacedOn(cell);

        Debug.Assert(data != null, this);
        data.OnInstanceBuilt(this);
    }

    public override void OnRemoved()
    {
        RemoveWorkers(workers);

        if (data)
            data.OnInstanceDemolished(this);
    }

    public bool CanBeUpgraded()
        => data != null && data.hasUpgrade && data.upgrade.CanBeAfforded() && data.upgrade.CanBePlacedOn(cell);

    public void Upgrade()
    {
        Debug.Assert(data != null, this);
        Debug.Assert(CanBeUpgraded(), this);

        data.OnInstanceUpgradedFrom(this);

        m_upgradePath.Pop();
        OnDataChanged();

        Player.instance.resources.UseResources(data.resourceCost);

        data.OnInstanceUpgradedTo(this);
    }

    public void Downgrade()
    {
        Debug.Assert(data != null, this);
        Debug.Assert(canBeDowngraded, this);

        data.OnInstanceDowngradedFrom(this);

        m_upgradePath.Push(data.upgrade);
        OnDataChanged();

        data.OnInstanceDowngradedTo(this);
    }
    private void OnDataChanged()
    {
        Debug.Assert(data != null, this);
        UpdateModel(data.buildingPrefab);
    }
}
