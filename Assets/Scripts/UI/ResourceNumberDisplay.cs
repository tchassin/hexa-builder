using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceNumberDisplay : MonoBehaviour
{
    [SerializeField] private Image m_icon;
    [SerializeField] private TextMeshProUGUI m_label;
    [SerializeField] private Color m_defaultColor = Color.white;
    [SerializeField] private Color m_insufficientColor = Color.red;

    public void SetResource(ResourceNumber resourceNumber)
    {
        Debug.Assert(resourceNumber.resource != null, this);

        StopCoroutine(nameof(DisplayCost));

        m_icon.sprite = resourceNumber.resource.icon;
        m_label.text = resourceNumber.count.ToString();
        m_label.color = m_defaultColor;
    }

    public void SetResourceCost(ResourceNumber resourceNumber)
    {
        SetResource(resourceNumber);
        StartCoroutine(DisplayCost(resourceNumber));
    }

    private IEnumerator DisplayCost(ResourceNumber resourceNumber)
    {
        while (true)
        {
            m_label.color = Player.instance.resources.HasResource(resourceNumber)
                ? m_defaultColor
                : m_insufficientColor;

            yield return null;
        }
    }
}
