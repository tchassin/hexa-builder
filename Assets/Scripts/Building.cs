using System.Collections.Generic;
using UnityEngine;

public class Building : HexCellContent
{
    public int upgradeCount => m_upgradePath.Count;
    public bool canBeDowngraded => upgradeCount > 1;
    public BuildingData data => m_upgradePath.Peek();

    public override TerrainType requiredTerrainType => TerrainType.Ground;

    private GameObject m_model;
    private readonly Stack<BuildingData> m_upgradePath = new Stack<BuildingData>();

    public void Initialize(BuildingData data)
    {
        Debug.Assert(upgradeCount == 0, this);
        m_upgradePath.Push(data);
        OnDataChanged();
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
        => data != null && data.hasUpgrade && data.upgrade.CanBeAfforded() && data.upgrade.CanBeBuiltOn(cell);

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

    private void OnDataChanged()
    {
        Debug.Assert(data != null, this);

        if (m_model != null)
            Destroy(m_model);

        if (data.buildingPrefab != null)
            m_model = Instantiate(data.buildingPrefab, transform);
    }
}
