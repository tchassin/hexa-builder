using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResourceStorage
{
    [SerializeField] private List<ResourceNumber> m_resources = new List<ResourceNumber>();

    public List<ResourceData> storedResources => m_resources.ConvertAll(rn => rn.resource);

    public ResourceStorage() { }

    public ResourceStorage(ResourceStorage other)
    {
        m_resources.AddRange(other.m_resources);
    }

    public void Clear() => m_resources.Clear();

    public int GetResource(ResourceData resource)
    {
        int index = m_resources.FindIndex(rn => rn.resource == resource);

        return index != -1 ? m_resources[index].count : 0;
    }

    public void SetResource(ResourceNumber resourceNumber)
    {
        var index = m_resources.FindIndex(resourceNumber.IsSameResource);
        if (index == -1)
            m_resources.Add(new ResourceNumber(resourceNumber));
        else
            m_resources[index] = resourceNumber;
    }

    public bool HasResource(ResourceNumber resourceNumber)
    {
        return GetResource(resourceNumber.resource) >= resourceNumber.count;
    }

    public bool HasResources(List<ResourceNumber> resourceNumbers)
    {
        foreach (var resourceCost in resourceNumbers)
        {
            if (!HasResource(resourceCost))
                return false;
        }

        return true;
    }

    public void AddResource(ResourceNumber resourceNumber)
    {
        var index = m_resources.FindIndex(resourceNumber.IsSameResource);
        if (index == -1)
            m_resources.Add(new ResourceNumber(resourceNumber));
        else
            m_resources[index] = m_resources[index] + resourceNumber.count;
    }

    public void AddResources(ResourceStorage resourceStorage)
    {
        foreach (var resourceNumber in resourceStorage.m_resources)
            AddResource(resourceNumber);
    }

    public bool UseResource(ResourceNumber resourceNumber)
    {
        int index = m_resources.FindIndex(resourceNumber.IsSameResource);

        if (index == -1 || m_resources[index].count < resourceNumber.count)
            return false;

        m_resources[index] = m_resources[index] - resourceNumber.count;

        return true;
    }

    public bool UseResources(List<ResourceNumber> resourceNumbers)
    {
        if (!HasResources(resourceNumbers))
            return false;

        foreach (var resourceNumber in resourceNumbers)
            UseResource(resourceNumber);

        return true;
    }
}
