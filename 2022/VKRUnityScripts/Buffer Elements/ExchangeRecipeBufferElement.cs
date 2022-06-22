using Unity.Collections;
using Unity.Entities;

public enum ResourceType
{
    Source,
    Result
}

public struct ExchangeRecipeBufferElement : IBufferElementData
{
    public FixedList128<FixedString32> SourceResourcesNames;
    public FixedListFloat32 SourceResourcesMultipliers;
    public FixedList128<FixedString32> ResultResourcesNames;
    public FixedListFloat32 ResultResourcesMultipliers;
    public int Hash;
    public bool IsCrafting;

    public float GetSourceMultiplier(FixedString32 resourceName)
    {
        float output = 0f;
        for (int i = 0; i < SourceResourcesNames.Length; i++)
        {
            if (SourceResourcesNames[i].Equals(resourceName))
            {
                output = SourceResourcesMultipliers[i];
                break;
            }
        }
        return output;
    }

    public float GetResultMultiplier(FixedString32 resourceName)
    {
        float output = 0f;
        for (int i = 0; i < ResultResourcesNames.Length; i++)
        {
            if (ResultResourcesNames[i].Equals(resourceName))
            {
                output = ResultResourcesMultipliers[i];
                break;
            }
        }
        return output;
    }

    public float GetMultiplier(ResourceType resourceType, FixedString32 resourceName)
    {
        if (resourceType == ResourceType.Source)
            return GetSourceMultiplier(resourceName);
        else
            return GetResultMultiplier(resourceName);
    }

    public float NormalizeResourceAmount(ResourceType resourceType, FixedString32 resourceName, FixedString32 requestedResourceName, float requstedResourceAmount)
    {
        float normalizableMultiplier = GetMultiplier(resourceType, resourceName);
        float requestedMultiplier = GetResultMultiplier(requestedResourceName);
        return requstedResourceAmount / requestedMultiplier * normalizableMultiplier;
    }

    public ExchangeRecipeBufferElement(ExchangeItem[] sourceItems, ExchangeItem[] resultItems, float timeRequired = 0, bool isCrafting = false)
    {
        FixedList128<FixedString32> sourceResourcesNames = new FixedList128<FixedString32>();
        FixedListFloat32 sourceResourcesMultipliers = new FixedListFloat32();
        FixedList128<FixedString32> resultResourcesNames = new FixedList128<FixedString32>();
        FixedListFloat32 resultResourcesMultipliers = new FixedListFloat32();
        for (int i = 0; i < sourceItems.Length; i++)
        {
            sourceResourcesNames.Add(sourceItems[i].ResourceName);
            sourceResourcesMultipliers.Add(sourceItems[i].Multiplier);
        }
        for (int i = 0; i < resultItems.Length; i++)
        {
            resultResourcesNames.Add(resultItems[i].ResourceName);
            resultResourcesMultipliers.Add(resultItems[i].Multiplier);
        }

        SourceResourcesNames = sourceResourcesNames;
        SourceResourcesMultipliers = sourceResourcesMultipliers;
        ResultResourcesNames = resultResourcesNames;
        ResultResourcesMultipliers = resultResourcesMultipliers;
        Hash = SourceResourcesNames.GetHashCode()
            + SourceResourcesMultipliers.GetHashCode()
            + ResultResourcesNames.GetHashCode()
            + ResultResourcesMultipliers.GetHashCode();
        IsCrafting = isCrafting;
    }
}
