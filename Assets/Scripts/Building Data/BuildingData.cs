using UnityEngine;

public abstract class BuildingData : CellContentData
{
    [SerializeField] private int m_cost;
    [SerializeField] private GameObject m_buildingPrefab;
    [SerializeField] private BuildingData m_upgrade;

    public int cost => m_cost;
    public GameObject buildingPrefab => m_buildingPrefab;
    public BuildingData upgrade => m_upgrade;
    public bool hasUpgrade => m_upgrade != null;
    public virtual int maxWorkers => 0;

    public virtual void OnInstanceBuilt(Building building) { }
    public virtual void OnInstanceDemolished(Building building) { }
    public virtual void OnInstanceUpgradedTo(Building building) { }
    public virtual void OnInstanceUpgradedFrom(Building building) { }
    public virtual void OnInstanceDowngradedTo(Building building) { }
    public virtual void OnInstanceDowngradedFrom(Building building) { }
    public virtual void OnInstanceUpdated(Building building) { }

    public bool CanBeAfforded()
        => Player.instance.gold >= m_cost;
}
