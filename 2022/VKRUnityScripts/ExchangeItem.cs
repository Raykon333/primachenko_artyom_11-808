using Unity.Collections;

public struct ExchangeItem
{
    public FixedString32 ResourceName;
    public float Multiplier;

    public ExchangeItem(FixedString32 resourceName, float multiplier)
    {
        ResourceName = resourceName;
        Multiplier = multiplier;
    }
}
