using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public static class OrdersExtensions
{
    public static bool OrderExists(this DynamicBuffer<OrderBufferElement> buffer, Entity requester, FixedString32 requestedResource)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            if (buffer[i].Requester == requester && buffer[i].RequestedResourceName == requestedResource)
                return true;
        }
        return false;
    }

    public static int OrderIndex(this DynamicBuffer<OrderBufferElement> buffer, Entity requester, FixedString32 requestedResource, 
        ExchangeRecipeBufferElement recipe, float requestedAmount)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            {
                var order = buffer[i];
                if (order.Requester == requester && order.RequestedResourceName == requestedResource 
                    && order.Recipe.SourceResourcesNames == recipe.SourceResourcesNames
                    && order.Recipe.ResultResourcesNames == recipe.ResultResourcesNames
                    && order.Recipe.SourceResourcesMultipliers == recipe.SourceResourcesMultipliers
                    && order.Recipe.ResultResourcesMultipliers == recipe.ResultResourcesMultipliers
                    && order.RequestedAmount == requestedAmount)
                    return i;
            }
        }
        return -1;
    }

    public static void TakeOrder(this DynamicBuffer<OrderBufferElement> buffer, OrderBufferElement order)
    {
        var index = buffer.OrderIndex(order.Requester, order.RequestedResourceName, order.Recipe, order.RequestedAmount);
        order.IsTaken = true;
        buffer[index] = order;
    }

    public static bool AreThereFreeOrders(this DynamicBuffer<OrderBufferElement> buffer)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            if (!buffer[i].IsTaken)
                return true;
        }
        return false;
    }
}
