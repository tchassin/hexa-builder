using UnityEngine;

[CreateAssetMenu(menuName = "Data/Resource")]
public class ResourceData : ScriptableObject
{
    [SerializeField] private string m_displayName;
    [SerializeField] private Sprite m_icon;

    public string displayName => m_displayName;
    public Sprite icon => m_icon;
}
