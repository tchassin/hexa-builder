using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_defaultTextPrefab;

    private readonly List<GameObject> m_content = new();
    private CanvasGroup m_canvasGroup;
    private RectTransform m_rectTransform;
    private Vector2 m_defaultAnchoredPosition;

    private void Awake()
    {
        TryGetComponent(out m_rectTransform);
        TryGetComponent(out m_canvasGroup);
        m_canvasGroup.interactable = false;
        m_canvasGroup.blocksRaycasts = false;
        Hide();

        m_defaultAnchoredPosition = m_rectTransform.anchoredPosition;
    }

    public void AddText(string text)
    {
        if (m_defaultTextPrefab == null)
            return;

        AddText(text, m_defaultTextPrefab.color);
    }

    public void AddText(string text, Color color)
    {
        AddContent(m_defaultTextPrefab, label =>
        {
            label.text = text;
            label.color = color;
        });
    }

    public void AddContent<ContentType>(ContentType prefab, UnityAction<ContentType> onInstantiated)
        where ContentType : MonoBehaviour
    {
        if (prefab == null)
            return;

        var contentItem = Instantiate(prefab, m_rectTransform);
        onInstantiated?.Invoke(contentItem);

        m_content.Add(contentItem.gameObject);
    }

    public void Show() => Show(m_defaultAnchoredPosition);

    public void Show(Vector2 position)
    {
        m_rectTransform.anchoredPosition = position;
        m_canvasGroup.alpha = 1;
    }

    public void Hide()
    {
        m_canvasGroup.alpha = 0;
        ClearContent();
    }

    public void ClearContent()
    {
        foreach (var contentItem in m_content)
        {
            contentItem.SetActive(false);
            Destroy(contentItem);
        }
        m_content.Clear();
    }
}
