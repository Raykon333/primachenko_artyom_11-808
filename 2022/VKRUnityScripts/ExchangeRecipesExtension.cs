using System;
using Unity.Collections;
using Unity.Entities;

public static class ExchangeRecipesExtension
{
    public static ExchangeRecipeBufferElement GetRecipe(this DynamicBuffer<ExchangeRecipeBufferElement> buffer, 
        FixedList128<FixedString32> sourceResources, FixedList128<FixedString32> resultResources)
    {
        ExchangeRecipeBufferElement output = default;
        for (int i = 0; i < buffer.Length; i++)
        {
            var recipe = buffer[i];
            if (sourceResources.Length != recipe.SourceResourcesNames.Length
                || resultResources.Length != recipe.ResultResourcesNames.Length)
                continue;
            var flag = true;
            for (int j = 0; j < sourceResources.Length && flag; j++)
                flag &= recipe.SourceResourcesNames.Contains(sourceResources[j]);
            for (int j = 0; j < resultResources.Length && flag; j++)
                flag &= recipe.ResultResourcesNames.Contains(resultResources[j]);
            if (flag)
            {
                output = recipe;
                break;
            }
        }
        return output;
    }

    public static ExchangeRecipeBufferElement GetRecipe(this DynamicBuffer<ExchangeRecipeBufferElement> buffer,
        FixedString32 sourceResource, FixedString32 resultResource)
    {
        return buffer.GetRecipe(
            sourceResources: new FixedList128<FixedString32>()
            {
                sourceResource
            },
            resultResources: new FixedList128<FixedString32>()
            {
                resultResource
            });
    }

    public static ExchangeRecipeBufferElement GetRecipe(this DynamicBuffer<ExchangeRecipeBufferElement> buffer,
        int hash)
    {
        for(int i = 0; i < buffer.Length; i++)
        {
            var recipe = buffer[i];
            if (recipe.Hash == hash)
                return recipe;
        }
        return default;
    }

    public static ExchangeRecipeBufferElement Add(this DynamicBuffer<ExchangeRecipeBufferElement> buffer,
        FixedString32 sourceName, float sourceAmount, FixedString32 resultName, float resultAmount)
    {
        var recipe = new ExchangeRecipeBufferElement(
            sourceItems: new ExchangeItem[]
            {
                new ExchangeItem(sourceName, sourceAmount)
            },
            resultItems: new ExchangeItem[]
            {
                new ExchangeItem(resultName, resultAmount)
            });
        buffer.Add(recipe);
        return recipe;
    }

    public static ExchangeRecipeBufferElement Add(this DynamicBuffer<ExchangeRecipeBufferElement> buffer,
        ExchangeItem[] sources, ExchangeItem[] results, float timeRequired, bool isCrafting)
    {
        var recipe = new ExchangeRecipeBufferElement(
            sourceItems: sources,
            resultItems: results,
            timeRequired: timeRequired,
            isCrafting: isCrafting);
        buffer.Add(recipe);
        return recipe;
    }
}
