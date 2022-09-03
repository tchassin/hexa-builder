using UnityEngine;

public class BuildModeClickHandler : IGridClickHandler
{
    private HexGrid m_grid;
    private GameUI m_gameUI;

    private BuildingData m_buildingData;

    private GameObject m_buildingPreview;

    public BuildingData buildingData => m_buildingData;

    public BuildModeClickHandler(GameUI gameUI, HexGrid grid, BuildingData buildingData)
    {
        Debug.Assert(buildingData != null);
        m_buildingData = buildingData;
        Debug.Assert(grid != null);
        m_grid = grid;
        Debug.Assert(gameUI != null);
        m_gameUI = gameUI;

        m_buildingPreview = Object.Instantiate(buildingData.buildingPrefab, m_grid.transform);
        if (m_buildingPreview.TryGetComponent(out MeshRenderer meshRenderer))
            meshRenderer.material = m_gameUI.previewMaterial;

        m_buildingPreview.SetActive(false);
    }

    ~BuildModeClickHandler()
    {
        if (m_buildingPreview != null)
            Object.Destroy(m_buildingPreview);
    }

    public void OnCellHoverBegin(HexCell cell)
    {
        UpdatePreview(cell);
    }

    public void OnCellHoverEnd(HexCell cell)
    {
    }

    public void OnLeftClickBegin(HexCell cell)
    {
        if (cell == null)
            return;

        bool hasBeenBuilt = m_grid.SetBuilding(m_buildingData, cell);

        if (!hasBeenBuilt)
            m_buildingPreview.SetActive(false);
    }

    public void OnLeftClickEnd(HexCell cell)
    {
    }

    public void OnRightClickBegin(HexCell cell)
    {
    }

    public void OnRightClickEnd(HexCell cell)
    {
    }

    private void UpdatePreview(HexCell cell)
    {
        m_buildingPreview.SetActive(cell != null);
        if (!m_buildingPreview.activeInHierarchy)
            return;

        m_buildingPreview.transform.position = cell.transform.position;

        bool canBeBuilt = !cell.isOccupied && buildingData.CanBeAfforded() && buildingData.CanBeBuiltOn(cell);

        if (!m_buildingPreview.TryGetComponent(out MeshRenderer meshRenderer))
            return;

        var properties = new MaterialPropertyBlock();
        if (meshRenderer.HasPropertyBlock())
            meshRenderer.GetPropertyBlock(properties);
        properties.SetColor("_Color", canBeBuilt ? m_gameUI.previewColor : m_gameUI.invalidPreviewColor);
        meshRenderer.SetPropertyBlock(properties);
    }
}
