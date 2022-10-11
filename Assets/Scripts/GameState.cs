using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SerializedCellContent
{
    public CellContentData data;
    public HexCoordinates position;
}

[Serializable]
public class GameState
{
    [SerializeField] private int m_width;
    [SerializeField] private int[] m_terrain;
    [SerializeField] private List<SerializedCellContent> m_cells = new();
    [SerializeField] private ResourceStorage m_resources = new();

    public static GameState Save(HexGrid grid)
    {
        var save = new GameState
        {
            m_width = grid.size.x,
            m_terrain = new int[grid.size.y * grid.size.x],
            m_resources = new(Player.instance.resources)
        };

        for (int i = 0; i < save.m_terrain.Length; i++)
        {
            var cell = grid.GetCell(i);

            save.m_terrain[i] = (int)cell.terrainType;
            if (cell.content != null)
                save.m_cells.Add(new SerializedCellContent { position = cell.position, data = cell.content.data });
        }

        return save;
    }

    public static void Load(GameState state, HexGrid grid)
    {
        grid.Initialize(state.m_terrain, state.m_width, state.m_cells);

        Player.instance.resources.Clear();
        Player.instance.resources.AddResources(state.m_resources);
    }
}
