using System;
using Unity.Entities;
using Unity.Jobs;

public class ThreadSafeInventorySystem : JobComponentSystem
{
    GameManager gameManager;

    EndSimulationEntityCommandBufferSystem ecbSystem;

    protected override void OnStartRunning()
    {
        ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var inventoryData = GetBufferFromEntity<InventoryBufferElement>(false);
        var ecb = ecbSystem.CreateCommandBuffer();

        inputDeps.Complete();

        Entities
            .WithNativeDisableParallelForRestriction(inventoryData)
            .ForEach((Entity entity, in ThreadSafeAdditionComponent addition) =>
            {
                if (inventoryData.HasComponent(entity))
                {
                    var inventory = inventoryData[addition.InventoryEntity];
                    inventory.AddAmountOf(addition.ResourceName, addition.AddedAmount);
                }
                ecb.DestroyEntity(entity);
            }).Run();

        Entities
            .WithNativeDisableParallelForRestriction(inventoryData)
            .ForEach((Entity entity, in ThreadSafeTransferComponent transfer) =>
            {
                if (!transfer.Sender.Equals(transfer.Receiver))
                {
                    if (inventoryData.HasComponent(transfer.Sender) && inventoryData.HasComponent(transfer.Receiver))
                    {
                        var senderInventory = inventoryData[transfer.Sender];
                        var receiverInventory = inventoryData[transfer.Receiver];
                        senderInventory.TransferTo(receiverInventory, transfer.ResourceName, transfer.ResourceAmount);
                    }
                }
                ecb.DestroyEntity(entity);
            }).Run();

        Entities
            .WithNativeDisableParallelForRestriction(inventoryData)
            .ForEach((Entity entity, in ThreadSafeExchangeComponent exchange) =>
            {
                if (!exchange.SourceEntity.Equals(exchange.ResultEntity))
                {
                    if (inventoryData.HasComponent(exchange.SourceEntity) && inventoryData.HasComponent(exchange.ResultEntity))
                    {
                        var sourceInventory = inventoryData[exchange.SourceEntity];
                        var resultInventory = inventoryData[exchange.ResultEntity];
                        sourceInventory.ExchangeWith(resultInventory, exchange.SourceResourceName, exchange.ResultResourceName, exchange.ResultResourceAmount, exchange.ExchangeRatio);
                    }
                }
                ecb.DestroyEntity(entity);
            }).Run();

        Entities
            .WithNativeDisableContainerSafetyRestriction(inventoryData)
            .ForEach((Entity entity, in ThreadSafeExchangeAsBuyerComponent exchange) =>
            {
                if (!exchange.BuyerEntity.Equals(exchange.SellerEntity))
                {
                    if (inventoryData.HasComponent(exchange.BuyerEntity) && inventoryData.HasComponent(exchange.SellerEntity))
                    {
                        var buyerInventory = inventoryData[exchange.BuyerEntity];
                        var sellerInventory = inventoryData[exchange.SellerEntity];
                        buyerInventory.ExchangeAsBuyer(sellerInventory, exchange.Recipe, exchange.RequestedResourceName, exchange.RequestedResourceAmount, exchange.IsWorkplaceTask);
                    }
                }
                ecb.DestroyEntity(entity);
            }).Run();

        Entities
            .WithNativeDisableContainerSafetyRestriction(inventoryData)
            .ForEach((Entity entity, in ThreadSafeCraftComponent craft) =>
            {
                if (inventoryData.HasComponent(craft.CraftStationEntity))
                {
                    bool craftIsValid = true;
                    var craftStationInventory = inventoryData[craft.CraftStationEntity];
                    var sources = craft.Recipe.SourceResourcesNames;
                    for (int i = 0; i < sources.Length; i++)
                    {
                        var sourceName = sources[i];
                        var sourceAmount = craft.Recipe.NormalizeResourceAmount(ResourceType.Source, sourceName, craft.RequestedResourceName, craft.RequestedResourceAmount);
                        craftIsValid &= craftStationInventory.GetAmountOf(sourceName) >= sourceAmount;
                    }
                    if (craftIsValid)
                    {
                        for (int i = 0; i < sources.Length; i++)
                        {
                            var sourceName = sources[i];
                            var sourceAmount = craft.Recipe.NormalizeResourceAmount(ResourceType.Source, sourceName, craft.RequestedResourceName, craft.RequestedResourceAmount);
                            craftStationInventory.AddAmountOf(sourceName, sourceAmount * -1);
                        }
                        var results = craft.Recipe.ResultResourcesNames;
                        for (int i = 0; i < results.Length; i++)
                        {
                            var resultName = results[i];
                            var resultAmount = craft.Recipe.NormalizeResourceAmount(ResourceType.Result, resultName, craft.RequestedResourceName, craft.RequestedResourceAmount);
                            craftStationInventory.AddAmountOf(resultName, resultAmount);
                        }
                    }
                }
                ecb.DestroyEntity(entity);
            }).Run();

        return default;
    }
}
