using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class Tooltip : MonoBehaviour
{
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

    public void Show(List<GameObject> content)
    {
        ClearContent();

        foreach (var contentItem in content)
        {
            if (contentItem == null)
                continue;

            contentItem.transform.SetParent(m_rectTransform);
            m_content.Add(contentItem);
        }

        m_rectTransform.anchoredPosition = Input.mousePosition;
        m_canvasGroup.alpha = 1;
    }

    public void Hide()
    {
        m_canvasGroup.alpha = 0;
        ClearContent();
    }

    private void ClearContent()
    {
        foreach (var contentItem in m_content)
        {
            contentItem.SetActive(false);
            Destroy(contentItem);
        }
        m_content.Clear();
    }
}
