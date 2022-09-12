using System;

[Serializable]
public class ResourceNumber
{
    public ResourceData resource;
    public int count;

    public bool IsSameResource(ResourceNumber other)
        => other != null && resource == other.resource;
}
