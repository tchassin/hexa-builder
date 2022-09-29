using UnityEngine;

public class Prop : HexCellContent
{
    [SerializeField] private PropData m_data;

    public override CellContentData data => m_data;
}
