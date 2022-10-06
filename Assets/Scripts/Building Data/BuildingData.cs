using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Data/Building/Basic")]
public class BuildingData : CellContentData
{
    [SerializeField] private GameObject m_buildingPrefab;
    [SerializeField] private BuildingUpgradeData m_upgrade;
    [SerializeField] private bool m_hasFacingDirection;
    [SerializeField] private HexDirection m_facingDirection;

    [Header("Time")]
    [SerializeField, FormerlySerializedAs("m_productionTime")] private float m_cycleDuration = 1.0f;

    [Header("Resources")]
    [SerializeField] private List<ResourceNumber> m_resourceCost;
    [SerializeField] private ResourceNumber m_input;
    [SerializeField] private ResourceNumber m_output;

    [Header("Required props & building")]
    [SerializeField] private CellContentData m_requiredContent;
    [SerializeField] private int m_maxContent = 3;

    public List<ResourceNumber> resourceCost => m_resourceCost;
    public GameObject buildingPrefab => m_buildingPrefab;
    public BuildingUpgradeData upgrade => m_upgrade;
    public bool hasUpgrade => m_upgrade.buildingData != null;
    public virtual int maxWorkers => 0;
    public CellContentData requiredNeighborData => m_requiredContent;
    public int requiredNeighborCount => m_maxContent;
    public float cycleDuration => m_cycleDuration;
    public ResourceNumber output => m_output;
    public ResourceNumber input => m_input;

    // Amount of resource consummed per seconds at max efficiency
    public float maxResourceConsumption
        => (input.resource != null && cycleDuration > 0.0f) ? input.count / cycleDuration : 0.0f;
    // Amount of resource produced per seconds at max efficiency
    public float maxResourceProduction
        => (output.resource != null && cycleDuration > 0.0f) ? output.count / cycleDuration : 0.0f;

    public HexDirection GetFacingDirection()
        => m_hasFacingDirection ? m_facingDirection : (HexDirection)Random.Range(0, 6);

    public virtual void OnInstanceBuilt(Building building) { }
    public virtual void OnInstanceDemolished(Building building) { }
    public virtual void OnInstanceUpgradedTo(Building building, BuildingData previousData) { }
    public virtual void OnInstanceUpgradedFrom(Building building, BuildingData newData) { }
    public virtual void OnInstanceDowngradedTo(Building building, BuildingData previousData) { }
    public virtual void OnInstanceDowngradedFrom(Building building, BuildingData newData) { }
    public virtual void OnInstanceUpdated(Building building) { }
    protected virtual void OnCycleCompleted(Building building, bool carryProgress = true)
    {
        building.progress = carryProgress ? Mathf.Max(float.Epsilon, building.progress - 1.0f) : 0.0f;

        if (building.CanBeUpgraded() && upgrade.isAutomatic)
            building.Upgrade();
    }

    public bool CanBeAfforded()
        => Player.instance.resources.HasResources(m_resourceCost);
}
