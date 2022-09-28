using System;
using UnityEngine;

[Serializable]
public struct ResourceNumber
{
    public ResourceData resource => m_resource;
    public int count => m_count;

    [SerializeField] private ResourceData m_resource;
    [SerializeField] private int m_count;

    public ResourceNumber(ResourceData resource, int count)
    {
        Debug.Assert(resource != null);
        m_resource = resource;
        m_count = count;
    }

    public ResourceNumber(ResourceNumber other)
    {
        Debug.Assert(other.resource != null);
        m_resource = other.resource;
        m_count = other.count;
    }

    public void SetCount(int count)
        => m_count = count;

    static public ResourceNumber operator +(ResourceNumber lhs, int n)
        => new(lhs.m_resource, lhs.count + n);

    static public ResourceNumber operator -(ResourceNumber lhs, int n)
        => new(lhs.m_resource, lhs.count - n);

    static public ResourceNumber operator *(ResourceNumber lhs, int n)
        => new(lhs.m_resource, lhs.count * n);

    static public ResourceNumber operator /(ResourceNumber lhs, int n)
        => new(lhs.m_resource, lhs.count - n);

    public bool IsSameResource(ResourceData resource)
        => m_resource == resource;
    public bool IsSameResource(ResourceNumber other)
        => IsSameResource(other.resource);
}
