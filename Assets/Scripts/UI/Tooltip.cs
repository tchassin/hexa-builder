using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_defaultTextPrefab;

    private readonly List<GameObject> m_content = new();
    private CanvasGroup m_canvasGroup;
    private RectTransform m_rectTransform;

    private void Awake()
    {
        TryGetComponent(out m_rectTransform);
        TryGetComponent(out m_canvasGroup);
        m_canvasGroup.interactable = false;
        m_canvasGroup.blocksRaycasts = false;
        Hide();
    }

    public void AddText(string text)
    {
        if (m_defaultTextPrefab == null)
            return;

        var label = Instantiate(m_defaultTextPrefab, m_rectTransform);
        label.text = text;

        m_content.Add(label.gameObject);
    }

    public void AddContent(GameObject contentItem)
    {
        if (contentItem == null)
            return;

        contentItem.transform.SetParent(m_rectTransform);
        m_content.Add(contentItem);
    }

    public void Show()
    {
        m_rectTransform.anchoredPosition = Input.mousePosition;
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
