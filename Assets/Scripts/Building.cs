using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public int upgradeCount => m_upgradePath.Count;
    public bool canBeDowngraded => upgradeCount > 1;
    public BuildingData data => m_upgradePath.Peek();
    public HexCell cell => m_cell;

    private GameObject m_model;
    private HexCell m_cell;
    private readonly Stack<BuildingData> m_upgradePath = new Stack<BuildingData>();

    public bool CanBeUpgraded()
        => data != null && data.hasUpgrade && data.upgrade.CanBeAfforded() && data.upgrade.CanBeBuiltOn(cell);

    public void Build(BuildingData data, HexCell cell)
    {
        Debug.Assert(cell != null, this);
        Debug.Assert(!cell.isOccupied, this);
        m_cell = cell;
        m_cell.SetBuilding(this);


        Debug.Assert(data != null, this);
        Debug.Assert(data.CanBeAfforded(), this);
        Player.instance.UseGold(data.cost);

        Debug.Assert(data.CanBeBuiltOn(cell), this);
        Debug.Assert(upgradeCount == 0, this);
        m_upgradePath.Push(data);
        OnDataChanged();

        data.OnInstanceBuilt(this);
    }

    public void Demolish()
    {
        if (data)
            data.OnInstanceDemolished(this);

        m_cell.SetBuilding(null);

        Destroy(gameObject);
    }

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
