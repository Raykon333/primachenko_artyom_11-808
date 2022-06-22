using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public static class InventoryExtensions
{
    public static void SetAmountOf(this DynamicBuffer<InventoryBufferElement> buffer, FixedString32 resourceName, float newAmount)
    {
        var index = -1;
        for (int i = 0; i < buffer.Length; i++)
        {
            if (buffer[i].ResourceName == resourceName)
            {
                index = i;
                break;
            }
        }
        if (index != -1)
            buffer.RemoveAt(index);

        if (newAmount != 0f)
            buffer.Add(new InventoryBufferElement { ResourceName = resourceName, ResourceAmount = newAmount });
    }

    public static float GetAmountOf(this DynamicBuffer<InventoryBufferElement> buffer, FixedString32 resourceName)
    {
        float amount = 0;
        for (int i = 0; i < buffer.Length; i++)
        {
            if (buffer[i].ResourceName == resourceName)
            {
                amount = buffer[i].ResourceAmount;
                break;
            }
        }
        return amount;
    }

    public static void AddAmountOf(this DynamicBuffer<InventoryBufferElement> buffer, FixedString32 resourceName, float addedAmount)
    {
        var index = -1;
        for (int i = 0; i < buffer.Length; i++)
        {
            if (buffer[i].ResourceName == resourceName)
            {
                index = i;
                break;
            }
        }
        float currentAmount;
        if (index != -1)
        {
            currentAmount = buffer[index].ResourceAmount;
            buffer.RemoveAt(index);
        }
        else
            currentAmount = 0f;

        if (currentAmount + addedAmount != 0f)
            buffer.Add(new InventoryBufferElement { ResourceName = resourceName, ResourceAmount = currentAmount + addedAmount });
    }

    public static void SubtractAmountOf(this DynamicBuffer<InventoryBufferElement> buffer, FixedString32 resourceName, float subtractedAmount)
    {
        buffer.AddAmountOf(resourceName, -subtractedAmount);
    }

    public static void TransferTo(this DynamicBuffer<InventoryBufferElement> buffer, DynamicBuffer<InventoryBufferElement> otherInventory, FixedString32 resourceName, float transferredAmount)
    {
        buffer.SubtractAmountOf(resourceName, transferredAmount);
        otherInventory.AddAmountOf(resourceName, transferredAmount);
    }

    public static void ExchangeWith(this DynamicBuffer<InventoryBufferElement> buffer, DynamicBuffer<InventoryBufferElement> otherInventory, FixedString32 sourceResourceName,
        FixedString32 resultResourceName, float resultResourceAmount, float exchangeRatio)
    {
        buffer.TransferTo(otherInventory, sourceResourceName, resultResourceAmount / exchangeRatio);
        otherInventory.TransferTo(buffer, resultResourceName, resultResourceAmount);
    }

    public static void ExchangeAsBuyer(this DynamicBuffer<InventoryBufferElement> buffer, DynamicBuffer<InventoryBufferElement> otherInventory, ExchangeRecipeBufferElement recipe,
        FixedString32 requestedResourceName, float requestedAmount, bool IsWorkplaceTask)
    {
        var requestedResourceMultiplier = recipe.GetResultMultiplier(requestedResourceName);
        var commonMultiplier = requestedAmount / requestedResourceMultiplier;

        for (int i = 0; i < recipe.SourceResourcesNames.Length; i++)
        {
            var name = recipe.SourceResourcesNames[i];
            var multiplier = recipe.SourceResourcesMultipliers[i];
            if (IsWorkplaceTask)
                otherInventory.TransferTo(buffer, name, commonMultiplier * multiplier);
            else
                buffer.TransferTo(otherInventory, name, commonMultiplier * multiplier);
        }

        for (int i = 0; i < recipe.ResultResourcesNames.Length; i++)
        {
            var name = recipe.ResultResourcesNames[i];
            var multiplier = recipe.ResultResourcesMultipliers[i];
            if (IsWorkplaceTask)
                buffer.TransferTo(otherInventory, name, commonMultiplier * multiplier);
            else
                otherInventory.TransferTo(buffer, name, commonMultiplier * multiplier);
        }
    }
} 
