using System.Collections.Generic;
using UnityEngine;

public class BuildModeManager : MonoBehaviour
{
    public static BuildModeManager instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private Building m_buildingPrefab;
    [SerializeField] private Road m_roadPrefab;
    [SerializeField] private Prop m_treePrefab;

    [Header("Preview")]
    [SerializeField] private Material m_previewMaterial;
    [SerializeField] private Color m_previewColor = Color.green;
    [SerializeField] private Color m_invalidPreviewColor = Color.red;

    public Building buildingPrefab => m_buildingPrefab;
    public Road roadPrefab => m_roadPrefab;
    public Prop treePrefab => m_treePrefab;


    private HexGrid m_grid;
    private GameObject m_preview;

    private void Awake()
    {
        instance = this;
        m_grid = FindObjectOfType<HexGrid>();
    }

    public void EnterBuildMode(GameObject previewPrefab)
    {
        Debug.Assert(m_grid != null);
        Debug.Assert(previewPrefab != null);

        m_preview = Instantiate(previewPrefab, m_grid.transform);
        if (m_preview.TryGetComponent(out MeshRenderer meshRenderer))
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
        if (!buildingData.CanBeBuiltOn(cell))
            return false;

        if (!buildingData.CanBeAfforded())
            return false;

        Player.instance.UseGold(buildingData.cost);

        var building = Instantiate(m_buildingPrefab, cell.transform);
        building.Initialize(buildingData);
        cell.SetContent(building);

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

    public bool PlaceRoads(List<HexCell> path)
    {
        foreach (var cell in path)
        {
            if (!roadPrefab.CanBePlacedOn(cell))
                continue;

            var road = Instantiate(roadPrefab, cell.transform);
            cell.SetContent(road);
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

        if (!m_preview.TryGetComponent(out MeshRenderer meshRenderer))
            return;

        var properties = new MaterialPropertyBlock();
        if (meshRenderer.HasPropertyBlock())
            meshRenderer.GetPropertyBlock(properties);
        properties.SetColor("_Color", canBePlaced ? m_previewColor : m_invalidPreviewColor);
        meshRenderer.SetPropertyBlock(properties);
    }

    public void UpdatePreview(BuildingData buildingData, HexCell cell)
    {
        m_preview.SetActive(cell != null);
        if (!m_preview.activeInHierarchy)
            return;

        m_preview.transform.position = cell.transform.position;

        bool canBeBuilt = !cell.isOccupied && buildingData.CanBeAfforded() && buildingData.CanBeBuiltOn(cell);

        if (!m_preview.TryGetComponent(out MeshRenderer meshRenderer))
            return;

        var properties = new MaterialPropertyBlock();
        if (meshRenderer.HasPropertyBlock())
            meshRenderer.GetPropertyBlock(properties);
        properties.SetColor("_Color", canBeBuilt ? m_previewColor : m_invalidPreviewColor);
        meshRenderer.SetPropertyBlock(properties);
    }
}
