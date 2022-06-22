using Unity.Collections;
using Unity.Entities;

static class EntityExtensions
{
    public static void Buys(this Entity entity, FixedString32 resource)
    {
        var EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var buyer = EntityManager.GetComponentData<BuyerComponent>(entity);
        buyer.BoughtResources.Add(
            new BoughtResource()
            {
                ResourceName = resource,
                OrderSize = 20,
                ReserveSize = 10000
            }
        );
        EntityManager.SetComponentData(entity, buyer);
    }

    public static void Sells(this Entity entity, FixedString32 resource, float forMin, float forMax)
    {
        var EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var recipes = EntityManager.GetBuffer<ExchangeRecipeBufferElement>(entity);
        var seller = EntityManager.GetComponentData<SellerComponent>(entity);
        float moneyMultiplier = UnityEngine.Random.Range(forMin, forMax);
        var recipe = recipes.Add("money", moneyMultiplier, resource, 1f);
        seller.SoldResources.Add(new SoldResource()
        {
            ResourceName = resource,
            RecipeHash = recipe.Hash
        });
        EntityManager.SetComponentData(entity, seller);

        EntityManager.AddBuffer<OrderBufferElement>(entity);
    }

    public static void Manufactures(this Entity entity, ExchangeItem[] sources, ExchangeItem[] results, FixedString32 resultResource, float efficiencyMultiplier)
    {
        var EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var recipes = EntityManager.GetBuffer<ExchangeRecipeBufferElement>(entity);
        var manufacturer = EntityManager.GetComponentData<ManufacturerComponent>(entity);
        var recipe = recipes.Add(sources, results, 0, true);
        manufacturer.ManufacturedResources.Add(new ManufacturedResource()
        {
            RecipeHash = recipe.Hash,
            ResourceName = resultResource,
            EfficiencyMultiplier = efficiencyMultiplier
        });
        EntityManager.SetComponentData(entity, manufacturer);
    }
}
