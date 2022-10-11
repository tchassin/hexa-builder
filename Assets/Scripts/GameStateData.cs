using UnityEngine;

[CreateAssetMenu(menuName = "Asset/Data/Game State")]
public class GameStateData : ScriptableObject
{
    [SerializeField] private GameState m_state;

    public GameState state => m_state;

#if UNITY_EDITOR
    public void Capture()
    {
        if (!Application.isPlaying)
            return;

        var grid = FindObjectOfType<HexGrid>();

        if (grid != null)
            m_state = GameState.Save(grid);
    }
#endif // UNITY_EDITOR
}
