using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexCellPriorityQueue
{
    private readonly List<List<Cell>> m_cells = new List<List<Cell>>();
    private readonly List<List<float>> m_priorities = new List<List<float>>();
    private int m_count = 0;
    private int m_minimum = int.MaxValue;

    public int Count => m_count;

    public void Enqueue(Cell cell, float priority)
    {
        if (priority < 0)
            return;

        m_count += 1;
        int index = Mathf.FloorToInt(priority);
        if (priority < m_minimum)
            m_minimum = index;

        while (index >= m_cells.Count)
        {
            m_cells.Add(new List<Cell>());
            m_priorities.Add(new List<float>());
        }

        m_cells[index].Add(cell);
        m_priorities[index].Add(priority);
    }

    public Cell Dequeue()
    {
        m_count -= 1;
        for (; m_minimum < m_cells.Count; m_minimum++)
        {
            if (m_priorities[m_minimum].Count == 0)
                continue;

            float min = m_priorities[m_minimum].Min();
            int index = m_priorities[m_minimum].IndexOf(min);
            var cell = m_cells[m_minimum][index];

            m_cells[m_minimum].RemoveAt(index);
            m_priorities[m_minimum].RemoveAt(index);

            return cell;
        }

        return null;
    }

    public void Change(Cell cell, float oldPriority, float priority)
    {
        var index = m_cells[Mathf.FloorToInt(oldPriority)].IndexOf(cell);
        m_cells[Mathf.FloorToInt(oldPriority)].RemoveAt(index);
        m_priorities[m_minimum].RemoveAt(index);
        m_count -= 1;

        Enqueue(cell, priority);
    }

    public void Clear()
    {
        m_cells.Clear();
        m_priorities.Clear();
        m_count = 0;
        m_minimum = int.MaxValue;
    }
}