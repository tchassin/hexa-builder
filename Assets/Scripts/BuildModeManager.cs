using System.Collections.Generic;
using UnityEngine;

public class BuildModeManager : MonoBehaviour
{
    public static BuildModeManager instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private Building m_buildingPrefab;
    [SerializeField] private Building m_roadPrefab;
    [SerializeField] private Prop m_treePrefab;

    [Header("Preview")]
    [SerializeField] private Material m_previewMaterial;
    [SerializeField] private Color m_previewColor = Color.green;
    [SerializeField] private Color m_invalidPreviewColor = Color.red;

    public Building buildingPrefab => m_buildingPrefab;
    public Prop treePrefab => m_treePrefab;


    private HexGrid m_grid;
    private GameObject m_preview;

    private void Awake()
    {
        instance = this;
        m_grid = FindObjectOfType<HexGrid>();
    }

    public void EnterBuildMode(GameObject previewPrefab, HexDirection facingDirection)
    {
        Debug.Assert(m_grid != null);
        Debug.Assert(previewPrefab != null);

        if (m_preview != null)
            Destroy(m_preview);

        Quaternion previewRotation = Quaternion.Euler(0, facingDirection.ToAngle(), 0);
        m_preview = Instantiate(previewPrefab, Vector3.zero, previewRotation, m_grid.transform);

        var meshRenderers = new List<MeshRenderer>();
        m_preview.GetComponentsInChildren(meshRenderers);
        foreach (var meshRenderer in meshRenderers)
            meshRenderer.material = m_previewMaterial;

        m_preview.SetActive(false);
    }

    public void LeaveBuildMode()
    {
        if (m_preview != null)
            Destroy(m_preview);
    }

    public bool PlaceBuilding(BuildingData buildingData, HexCell cell)
    {
        if (!buildingData.CanBePlacedOn(cell))
            return false;

        if (!buildingData.CanBeAfforded())
            return false;

        var building = Instantiate(m_buildingPrefab, cell.transform);
        building.Initialize(buildingData);
        building.RotateMeshToward(buildingData.GetFacingDirection());
        cell.SetContent(building);

        Player.instance.resources.UseResources(buildingData.resourceCost);

        m_preview.SetActive(false);

        return true;
    }

    public bool PlaceProp(Prop propPrefab, HexCell cell)
    {
        if (!propPrefab.CanBePlacedOn(cell))
            return false;

        var prop = Instantiate(propPrefab, cell.transform);
        prop.transform.rotation = Quaternion.Euler(0, Random.Range(-2, 3) * 60.0f, 0);
        cell.SetContent(prop);

        m_preview.SetActive(false);

        return true;
    }

    public bool PlaceRoads(RoadData roadData, List<HexCell> path)
    {
        foreach (var cell in path)
        {
            if (!roadData.CanBePlacedOn(cell))
                continue;

            if (!Player.instance.resources.HasResources(roadData.resourceCost))
                break;

            var road = Instantiate(m_buildingPrefab, cell.transform);
            road.Initialize(roadData);
            cell.SetContent(road);

            Player.instance.resources.UseResources(roadData.resourceCost);
        }

        return true;
    }

    public void UpdatePreview(HexCellContent content, HexCell cell)
    {
        m_preview.SetActive(cell != null);
        if (!m_preview.activeInHierarchy)
            return;

        m_preview.transform.position = cell.transform.position;

        bool canBePlaced = !cell.isOccupied && content.CanBePlacedOn(cell);

        var meshRenderers = new List<MeshRenderer>();
        m_preview.GetComponentsInChildren(meshRenderers);
        foreach (var meshRenderer in meshRenderers)
        {
            var properties = new MaterialPropertyBlock();
            if (meshRenderer.HasPropertyBlock())
                meshRenderer.GetPropertyBlock(properties);
            properties.SetColor("_Color", canBePlaced ? m_previewColor : m_invalidPreviewColor);
            meshRenderer.SetPropertyBlock(properties);
        }
    }

    public void UpdatePreview(BuildingData buildingData, HexCell cell)
    {
        m_preview.SetActive(cell != null);
        if (!m_preview.activeInHierarchy)
            return;

        m_preview.transform.position = cell.transform.position;

        bool canBeBuilt = !cell.isOccupied && buildingData.CanBeAfforded() && buildingData.CanBePlacedOn(cell);

        var meshRenderers = new List<MeshRenderer>();
        m_preview.GetComponentsInChildren(meshRenderers);
        foreach (var meshRenderer in meshRenderers)
        {
            var properties = new MaterialPropertyBlock();
            if (meshRenderer.HasPropertyBlock())
                meshRenderer.GetPropertyBlock(properties);
            properties.SetColor("_Color", canBeBuilt ? m_previewColor : m_invalidPreviewColor);
            meshRenderer.SetPropertyBlock(properties);
        }
    }
}
