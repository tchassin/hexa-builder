using System.Collections.Generic;
using UnityEngine;

public class Building : HexCellContent
{
    public int upgradeCount => m_upgradePath.Count;
    public bool canBeDowngraded => upgradeCount > 1;
    public BuildingData data => m_upgradePath.Peek();

    public override TerrainType requiredTerrainType => TerrainType.Ground;
    public MeshFilter meshFilter => m_meshFilter;

    private GameObject m_model;
    private MeshFilter m_meshFilter;
    private readonly Stack<BuildingData> m_upgradePath = new Stack<BuildingData>();

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
        Player.instance.UseGold(data.cost);

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

    public void RotateMeshToward(HexDirection direction)
        => RotateMesh(HexDirection.E.DistanceTo(direction));

    public void RotateMesh(int distance)
    {
        // Clamp angle to ]-180; 180]
        int clampedDistance = distance > 3 ? distance - 6 : distance <= -3 ? distance + 6 : distance;
        m_meshFilter.transform.localEulerAngles = Vector3.up * (clampedDistance * 60.0f);
    }

    private void OnDataChanged()
    {
        Debug.Assert(data != null, this);

        if (m_model != null)
        {
            Destroy(m_model);
            m_meshFilter = null;
        }

        if (data.buildingPrefab != null)
        {
            m_model = Instantiate(data.buildingPrefab, transform);
            m_model.TryGetComponent(out m_meshFilter);
        }

    }
}
