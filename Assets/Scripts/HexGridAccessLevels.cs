using System;
using System.Collections.Generic;
using UnityEngine;

public class HexGridAccessLevels
{
    private int cellCount => m_grid != null ? m_grid.size.x * m_grid.size.y : 0;

    private int[] m_roadAccess;
    private int[] m_workerAccess;
    private readonly Dictionary<ResourceData, int[]> m_resourceAccess = new();

    private HexGrid m_grid;

    private readonly List<Building> m_roads = new();

    public HexGridAccessLevels(HexGrid hexGrid)
    {
        m_grid = hexGrid;
        m_roadAccess = new int[cellCount];
        m_workerAccess = new int[cellCount];
    }

    public int GetAccessToRoad(HexCell hexCell)
    {
        int cellIndex = m_grid.GetCellIndex(hexCell);

        return cellIndex != -1 ? m_roadAccess[cellIndex] : 0;
    }

    public bool HasAccessToRoad(HexCell hexCell)
        => GetAccessToRoad(hexCell) > 0;

    public int GetAccessToWorkers(HexCell hexCell)
    {
        int cellIndex = m_grid.GetCellIndex(hexCell);

        return cellIndex != -1 ? m_workerAccess[cellIndex] : 0;
    }

    public bool HasAccessToWorkers(HexCell hexCell)
        => GetAccessToWorkers(hexCell) > 0;

    public int GetAccessToResource(HexCell hexCell, ResourceData resource)
    {
        if (resource == null || !m_resourceAccess.ContainsKey(resource))
            return 0;

        int cellIndex = m_grid.GetCellIndex(hexCell);

        return cellIndex != -1 ? m_resourceAccess[resource][cellIndex] : 0;
    }

    public bool HasAccessToResource(HexCell hexCell, ResourceData resource)
        => GetAccessToResource(hexCell, resource) > 0;

    public void OnBuildingAdded(Building building)
    {
        if (building.data is RoadData)
        {
            m_roads.Add(building);

            int index = m_grid.GetCellIndex(building.cell);
            m_roadAccess[index]++;

            foreach (var neighborCell in building.cell.neighbors)
            {
                if (neighborCell == null)
                    continue;

                int neighborIndex = m_grid.GetCellIndex(neighborCell);
                m_roadAccess[neighborIndex]++;
            }

            RecomputeAccessLevels();
        }
        else if (building.data is HousingData || building.data is ProductionBuildingData)
        {
            RecomputeAccessLevels();
        }
    }

    public void OnBuildingRemoved(Building building)
    {
        if (building.data is RoadData)
        {
            m_roads.Remove(building);
            int index = m_grid.GetCellIndex(building.cell);
            m_roadAccess[index]--;

            foreach (var neighborCell in building.cell.neighbors)
            {
                if (neighborCell == null)
                    continue;

                int neighborIndex = m_grid.GetCellIndex(neighborCell);
                m_roadAccess[neighborIndex]--;
            }

            RecomputeAccessLevels();
        }
        else if (building.data is HousingData || building.data is ProductionBuildingData)
        {
            RecomputeAccessLevels();
        }
    }

    private void RecomputeAccessLevels()
    {
        Array.Fill(m_workerAccess, 0);
        foreach (var ressourceAccess in m_resourceAccess)
            Array.Fill(ressourceAccess.Value, 0);

        List<HexCell> roadCells = new(m_roads.Count);
        foreach (var road in m_roads)
            roadCells.Add(road.cell);

        while (roadCells.Count > 0)
            RecomputeRoadNetwork(roadCells);

        void RecomputeRoadNetwork(List<HexCell> unvisitedRoadCells)
        {
            List<HexCell> visitedCells = new();
            Queue<HexCell> searchFrontier = new();

            var start = unvisitedRoadCells[0];
            searchFrontier.Enqueue(start);
            visitedCells.Add(start);

            int workerAccess = 0;
            Dictionary<ResourceData, int> resourceAccess = new(); ;

            while (searchFrontier.Count > 0)
            {
                var currentCell = searchFrontier.Dequeue();

                foreach (var neighbor in currentCell.neighbors)
                {
                    if (neighbor == null || visitedCells.Contains(neighbor))
                        continue;

                    visitedCells.Add(neighbor);

                    if (neighbor.content == null)
                        continue;

                    if (neighbor.content.data is HousingData housingData)
                    {
                        workerAccess++;
                    }
                    else if (neighbor.content.data is ProductionBuildingData productionBuildingData)
                    {
                        var resourceData = productionBuildingData.outputResource;
                        if (resourceData != null)
                        {
                            if (resourceAccess.ContainsKey(resourceData))
                                resourceAccess[resourceData]++;
                            else
                                resourceAccess[resourceData] = 1;
                        }
                    }
                    else if (neighbor.content.data is RoadData road)
                    {
                        // Check that the road is hasn't been visited and is not being destroyed
                        if (unvisitedRoadCells.Contains(neighbor))
                            searchFrontier.Enqueue(neighbor);
                    }
                }
            }

            // Initialize new resources if necessary
            foreach (var ressource in resourceAccess.Keys)
            {
                if (!m_resourceAccess.ContainsKey(ressource))
                    m_resourceAccess[ressource] = new int[cellCount];
            }

            // Set the access level of each cell connected to the network
            foreach (var cell in visitedCells)
            {
                int index = m_grid.GridPositionToIndex(cell.position);
                m_workerAccess[index] = Mathf.Max(workerAccess, m_workerAccess[index]);
                foreach (var ressource in resourceAccess.Keys)
                    m_resourceAccess[ressource][index] = resourceAccess[ressource];
            }

            // Remove network cells from cell pool
            unvisitedRoadCells.RemoveAll(cell => visitedCells.Contains(cell));
        }
    }
}
