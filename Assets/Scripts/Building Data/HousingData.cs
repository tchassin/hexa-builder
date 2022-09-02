using UnityEngine;

[CreateAssetMenu(menuName = "Building Data/Housing")]
public class HousingData : BuildingData
{
    [SerializeField] private int m_capacity;

    public int capacity => m_capacity;
}
