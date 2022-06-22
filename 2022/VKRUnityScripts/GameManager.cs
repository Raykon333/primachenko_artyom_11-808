using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Initialization serialized fields
    [SerializeField] Material UnitMaterial;
    [SerializeField] Material TransporterMaterial;
    [SerializeField] Material HouseMaterial;
    [SerializeField] Material MineMaterial;
    [SerializeField] Material RefineryMaterial;
    [SerializeField] Material ForestMaterial;
    [SerializeField] Material LumbermillMaterial;
    [SerializeField] Material WorkshopMaterial;
    [SerializeField] Material FarmMaterial;
    [SerializeField] Material MillMaterial;
    [SerializeField] Material AnimalFarmMaterial;
    [SerializeField] Material SlaughterhouseMaterial;
    [SerializeField] Material FoodFactoryMaterial;
    [SerializeField] Material ShopMaterial;
    [SerializeField] Mesh SpriteMesh;
    [SerializeField] GameObject BlockPrefab;
    [SerializeField] Transform BlocksParent;

    //Grid parameters
    [SerializeField] int UnitsCount;
    [SerializeField] int HousesCount;
    [SerializeField] int MineCount;
    [SerializeField] int RefineryCount;
    [SerializeField] int ForestCount;
    [SerializeField] int LumbermillCount;
    [SerializeField] int WorkshopCount;
    [SerializeField] int FarmCount;
    [SerializeField] int MillCount;
    [SerializeField] int AnimalFarmCount;
    [SerializeField] int SlaughterhouseCount;
    [SerializeField] int FoodFactoryCount;
    [SerializeField] int StoresCount;
    [SerializeField] bool ShowcaseGrid;
    [SerializeField] bool Autosize;
    [SerializeField] public int Width;
    [SerializeField] public int Height;

    int WorkplaceCount => MineCount + RefineryCount + ForestCount + LumbermillCount + WorkshopCount
        + FarmCount + MillCount + AnimalFarmCount + SlaughterhouseCount + FoodFactoryCount;

    public static readonly float SpacingMultiplier = 2f;
    public static readonly float BuildingsScale = 1.5f;
    public static float UnitScale = 0.5f;

    //Runtime serialized fields
    [SerializeField] float SimulationSpeed;

    //Non-serialized fields
    private static int LocationTypesAmount => Enum.GetNames(typeof(LocationType)).Length;
    private readonly Material[] materials = new Material[LocationTypesAmount];
    private Quaternion rotationAngle = Quaternion.Euler(90, 0, 0);

    public EntityArchetype UnitArchetype { get; private set; }
    public EntityArchetype[] LocationArchetypes { get; private set; }
    public EntityArchetype LocationArchetype { get; private set; }
    public EntityArchetype VacancyArchetype { get; private set; }
    public EntityArchetype ResourceArchetype { get; private set; }

    public EntityManager EntityManager { get; private set; }
    public Entity StaticEntity { get; private set; }
    public List<Entity> StatTrackers = new List<Entity>();

    private NativeList<Entity>[] locationLists;
    private NativeList<Entity> vacancies;
    private NativeList<Entity> Units;

    int LocationsCounter
    {
        get
        {
            int result = 0;
            for (int i = 0; i < locationLists.Length; i++)
                result += locationLists[i].Length;
            return result;
        } }


    private void Awake()
    {
        if (!Application.isEditor)
            ParseSettingsFromFile("Settings.json");

        if (ShowcaseGrid)
        {
            UnitsCount = 20;
            HousesCount = 3;
            MineCount = 1;
            RefineryCount = 1;
            ForestCount = 1;
            LumbermillCount = 1;
            WorkshopCount = 1;
            FarmCount = 1;
            MillCount = 1;
            AnimalFarmCount = 1;
            SlaughterhouseCount = 1;
            FoodFactoryCount = 1;
            StoresCount = 1;

            Width = 4;
            Height = 4;
        }

        InitializeGame();
    }

    private void ParseSettingsFromFile(string path)
    {
        var json = File.ReadAllText(path);
        var settings = JsonUtility.FromJson<Initialization>(json);
        UnitsCount = settings.UnitsCount;
        HousesCount = settings.HousesCount;
        MineCount = settings.MineCount;
        RefineryCount = settings.RefineryCount;
        ForestCount = settings.ForestCount;
        LumbermillCount = settings.LumbermillCount;
        WorkshopCount = settings.WorkshopCount;
        FarmCount = settings.FarmCount;
        MillCount = settings.MillCount;
        AnimalFarmCount = settings.AnimalFarmCount;
        SlaughterhouseCount = settings.SlaughterhouseCount;
        FoodFactoryCount = settings.FoodFactoryCount;
        StoresCount = settings.StoresCount;
        ShowcaseGrid = settings.ShowcaseGrid;
        Autosize = settings.Autosize;
        Width = settings.Width;
        Height = settings.Height;
    }

    void InitializeGame()
    {
        materials[0] = HouseMaterial;
        materials[1] = MineMaterial;
        materials[2] = RefineryMaterial;
        materials[3] = ForestMaterial;
        materials[4] = LumbermillMaterial;
        materials[5] = WorkshopMaterial;
        materials[6] = FarmMaterial;
        materials[7] = MillMaterial;
        materials[8] = AnimalFarmMaterial;
        materials[9] = SlaughterhouseMaterial;
        materials[10] = FoodFactoryMaterial;
        materials[11] = ShopMaterial;

        EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        #region Archetype declaration
        ComponentType[] genericLocationComponents = new ComponentType[]
        {
            typeof(LocationTagComponent),
            typeof(IdComponent),

            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(RenderBounds),

            typeof(Translation),
            typeof(Rotation),
            typeof(Scale),
            typeof(SizeComponent),

            typeof(LocationInfoComponent),

            typeof(NeedsInventoryComponent),
            typeof(NeedsStartingResourcesTagComponent),
            typeof(BuyerComponent),
            typeof(SellerComponent),
            typeof(ManufacturerComponent),
            typeof(WorkplaceInfoComponent),
        };
        LocationArchetype = EntityManager.CreateArchetype(genericLocationComponents);
        

        UnitArchetype = EntityManager.CreateArchetype(
            typeof(UnitTagComponent),
            typeof(IdComponent),
            typeof(NeedsInventoryComponent),

            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(RenderBounds),

            typeof(Translation),
            typeof(Rotation),
            typeof(Scale),

            typeof(ParentLocationComponent),
            typeof(MovementTargetComponent),
            typeof(RadialComponent),
            typeof(HungerComponent),
            typeof(SpeedComponent),
            typeof(NeedsAndDutiesComponent),
            typeof(SpecializationComponent)
            );

        VacancyArchetype = EntityManager.CreateArchetype(
            typeof(VacancyTagComponent),
            typeof(IdComponent),

            typeof(ParentLocationComponent),
            typeof(WeekScheduleComponent),
            typeof(PaymentComponent),
            typeof(RelatedUnitComponent),
            typeof(VacancyTypeComponent)
            );
        #endregion

        locationLists = new NativeList<Entity>[LocationTypesAmount];
        for (int i = 0; i < LocationTypesAmount; i++)
            locationLists[i] = new NativeList<Entity>(Allocator.Persistent);
        
        
        #region Location generation
        var numbersAmounts = new int[12];
        numbersAmounts[(int)LocationType.LivingHouse] = HousesCount;
        numbersAmounts[(int)LocationType.Store] = StoresCount;
        numbersAmounts[(int)LocationType.Mine] = MineCount;
        numbersAmounts[(int)LocationType.Refinery] = RefineryCount;
        numbersAmounts[(int)LocationType.Forest] = ForestCount;
        numbersAmounts[(int)LocationType.Lumbermill] = LumbermillCount;
        numbersAmounts[(int)LocationType.Workshop] = WorkshopCount;
        numbersAmounts[(int)LocationType.Farm] = FarmCount;
        numbersAmounts[(int)LocationType.Mill] = MillCount;
        numbersAmounts[(int)LocationType.AnimalFarm] = AnimalFarmCount;
        numbersAmounts[(int)LocationType.Slaughterhouse] = SlaughterhouseCount;
        numbersAmounts[(int)LocationType.FoodFactory] = FoodFactoryCount;
        int[,] grid;
        if (ShowcaseGrid)
            grid = ShowcaseMatrix();
        else if (Autosize)
            grid = GenerateSquareMatrixWithAutoSize(numbersAmounts);
        else
            grid = GenerateMatrix(numbersAmounts, Width, Height);
        CreateLocationsGrid(grid);

        var allLocations = new NativeList<Entity>(Allocator.Persistent);
        for (int i = 0; i < locationLists.Length; i++)
            for (int j = 0; j < locationLists[i].Length; j++)
                allLocations.Add(locationLists[i][j]);

        foreach (var location in allLocations)
            EntityManager.AddBuffer<ExchangeRecipeBufferElement>(location);

        foreach (var mine in locationLists[(int)LocationType.Mine])
        {
            var sources = new ExchangeItem[] { new ExchangeItem("tools", 1) };
            var results = new ExchangeItem[] { new ExchangeItem("ore", 3) };
            mine.Manufactures(sources, results, "ore", 3);

            mine.Buys("tools");
            mine.Sells("ore", 5, 6);
        }
        foreach (var refinery in locationLists[(int)LocationType.Refinery])
        {
            var sources = new ExchangeItem[] { 
                new ExchangeItem("ore", 1),
                new ExchangeItem("tools", 1)};
            var results = new ExchangeItem[] { new ExchangeItem("metal", 2) };
            refinery.Manufactures(sources, results, "metal", 2);

            refinery.Buys("tools");
            refinery.Buys("ore");
            refinery.Sells("metal", 7, 8);
        }
        foreach (var forest in locationLists[(int)LocationType.Forest])
        {
            var sources = new ExchangeItem[] { new ExchangeItem("tools", 1) };
            var results = new ExchangeItem[] { new ExchangeItem("logs", 3) };
            forest.Manufactures(sources, results, "logs", 3);

            forest.Buys("tools");
            forest.Sells("logs", 5, 6);
        }
        foreach (var lumbermill in locationLists[(int)LocationType.Lumbermill])
        {
            var sources = new ExchangeItem[] { 
                new ExchangeItem("logs", 1),
                new ExchangeItem("tools", 1)};
            var results = new ExchangeItem[] { new ExchangeItem("wood", 2) };
            lumbermill.Manufactures(sources, results, "wood", 2);

            lumbermill.Buys("tools");
            lumbermill.Buys("logs");
            lumbermill.Sells("wood", 7, 8);
        }
        foreach (var workshop in locationLists[(int)LocationType.Workshop])
        {
            var sources = new ExchangeItem[] {
                new ExchangeItem("metal", 1),
                new ExchangeItem("wood", 1)};
            var results = new ExchangeItem[] { new ExchangeItem("tools", 5) };
            workshop.Manufactures(sources, results, "tools", 5);

            workshop.Buys("metal");
            workshop.Buys("wood");
            workshop.Sells("tools", 9, 10);
        }
        foreach (var farm in locationLists[(int)LocationType.Farm])
        {
            var sources = new ExchangeItem[] { new ExchangeItem("tools", 1) };
            var results = new ExchangeItem[] { 
                new ExchangeItem("veggies", 1),
                new ExchangeItem("grain", 1)};
            farm.Manufactures(sources, results, "grain", 1);

            farm.Buys("tools");
            farm.Sells("veggies", 6, 7);
            farm.Sells("grain", 6, 7);
        }
        foreach (var mill in locationLists[(int)LocationType.Mill])
        {
            var sources = new ExchangeItem[] { new ExchangeItem("grain", 1) };
            var results = new ExchangeItem[] { new ExchangeItem("animalFeed", 1) };
            mill.Manufactures(sources, results, "animalFeed", 1);

            mill.Buys("grain");
            mill.Sells("animalFeed", 8, 9);
        }
        foreach (var animalFarm in locationLists[(int)LocationType.AnimalFarm])
        {
            var sources = new ExchangeItem[] { new ExchangeItem("animalFeed", 1) };
            var results = new ExchangeItem[] { new ExchangeItem("livestock", 1) };
            animalFarm.Manufactures(sources, results, "livestock", 1);

            animalFarm.Buys("animalFeed");
            animalFarm.Sells("livestock", 10, 11);
        }
        foreach (var slaughterhouse in locationLists[(int)LocationType.Slaughterhouse])
        {
            var sources = new ExchangeItem[] { new ExchangeItem("livestock", 1) };
            var results = new ExchangeItem[] { new ExchangeItem("meat", 1) };
            slaughterhouse.Manufactures(sources, results, "meat", 1);

            slaughterhouse.Buys("livestock");
            slaughterhouse.Sells("meat", 12, 13);
        }
        foreach (var foodFactory in locationLists[(int)LocationType.FoodFactory])
        {
            var sources = new ExchangeItem[] { 
                new ExchangeItem("meat", 1),
                new ExchangeItem("veggies", 1)};
            var results = new ExchangeItem[] { new ExchangeItem("rawFood", 3) };
            foodFactory.Manufactures(sources, results, "rawFood", 3);

            foodFactory.Buys("meat");
            foodFactory.Buys("veggies");
            foodFactory.Sells("rawFood", 21, 22);
        }
        foreach (var store in locationLists[(int)LocationType.Store])
        {
            store.Buys("rawFood");
            store.Sells("rawFood", 23, 24);
        }

        for (int i = (int)LocationType.Mine; i <= (int)LocationType.Store; i++)
            foreach (var buyer in locationLists[i])
            {
                var buyerComponent = EntityManager.GetComponentData<BuyerComponent>(buyer);
                for (int j = 0; j < buyerComponent.BoughtResources.Length; j++)
                {
                    var boughtResource = buyerComponent.BoughtResources[j];
                    var sellers = new NativeList<Entity>(Allocator.Temp);
                    var sellersHashes = new List<int>();
                    foreach (var seller in allLocations)
                    {
                        if (!EntityManager.HasComponent<SellerComponent>(seller))
                            continue;
                        var sellerComponent = EntityManager.GetComponentData<SellerComponent>(seller);
                        var soldResources = sellerComponent.SoldResources;
                        foreach (var soldResource in soldResources)
                            if (soldResource.ResourceName == boughtResource.ResourceName && EntityManager.GetComponentData<LocationInfoComponent>(seller).Type != LocationType.Store)
                            {
                                sellers.Add(seller);
                                sellersHashes.Add(soldResource.RecipeHash);
                            }
                    }
                    if (sellers.Length == 0)
                    {
                        continue;
                    }
                    var partnerNumber = UnityEngine.Random.Range(0, sellers.Length);
                    var partner = sellers[partnerNumber];
                    var partnerHash = sellersHashes[partnerNumber];
                    var recipes = EntityManager.GetBuffer<ExchangeRecipeBufferElement>(partner);
                    var recipe = recipes.GetRecipe(partnerHash);
                    buyerComponent.BoughtResources[j] = new BoughtResource()
                        {
                            ResourceName = boughtResource.ResourceName,
                            OrderSize = 20,
                            ReserveSize = 100,
                            Provider = partner,
                            RecipeHash = recipe.Hash
                        };
                    sellers.Dispose();
                }
                EntityManager.SetComponentData(buyer, buyerComponent);
            }
        allLocations.Dispose();
        #endregion

        vacancies = new NativeList<Entity>(Allocator.Persistent);
        for (int i = (int)LocationType.Mine; i <= (int)LocationType.FoodFactory; i++)
            foreach (var workplace in locationLists[i])
            {
                int amount = EntityManager.GetComponentData<WorkplaceInfoComponent>(workplace).VacanciesAmount;
                CreateVacancies(workplace, amount);
            }

        Units = new NativeList<Entity>(Allocator.Persistent);
        CreateUnits(UnitsCount);

        #region Static entity
        StaticEntity = EntityManager.CreateEntity(typeof(StaticComponent), typeof(TimeUniqueComponent));
        EntityManager.SetComponentData(StaticEntity, new StaticComponent { SimulationSpeed = SimulationSpeed, SpacingMultiplier = SpacingMultiplier, UnitsCount = UnitsCount });
        EntityManager.SetComponentData(StaticEntity, new TimeUniqueComponent { Day = 1, Seconds = 0 });
        var locationBuffer = EntityManager.AddBuffer<StoreBufferElement>(StaticEntity);
        for (int i = 0; i < locationLists[(int)LocationType.Store].Length; i++)
                locationBuffer.Add(new StoreBufferElement { Location = locationLists[(int)LocationType.Store][i] });
        var vacancyBuffer = EntityManager.AddBuffer<VacancyInfoBufferElement>(StaticEntity);
        for (int i = 0; i < vacancies.Length; i++)
        {
            var vacancy = vacancies[i];
            var employee = EntityManager.GetComponentData<RelatedUnitComponent>(vacancy).Entity;
            bool isFree = employee == default;
            vacancyBuffer.Add(new VacancyInfoBufferElement { Vacancy = vacancy, IsFree = isFree, Employee = employee });
        }
        #endregion

        vacancies.Dispose();

        StatTrackers.Add(CreateSatietyTracker("Unit satiety", true, true, true, Units));
        StatTrackers.Add(CreateResourceStatTracker("Unit", "money", true, true, true, Units));
        StatTrackers.Add(CreateResourceStatTracker("Mine", "ore", false, false, true, locationLists[(int)LocationType.Mine]));
        StatTrackers.Add(CreateResourceStatTracker("Refinery", "metal", false, false, true, locationLists[(int)LocationType.Refinery]));
        StatTrackers.Add(CreateResourceStatTracker("Forest", "logs", false, false, true, locationLists[(int)LocationType.Forest]));
        StatTrackers.Add(CreateResourceStatTracker("Lumbermill", "wood", false, false, true, locationLists[(int)LocationType.Lumbermill]));
        StatTrackers.Add(CreateResourceStatTracker("Workshop", "tools", false, false, true, locationLists[(int)LocationType.Workshop]));
        StatTrackers.Add(CreateResourceStatTracker("Farm", "veggies", false, false, true, locationLists[(int)LocationType.Farm]));
        StatTrackers.Add(CreateResourceStatTracker("Farm", "grain", false, false, true, locationLists[(int)LocationType.Farm]));
        StatTrackers.Add(CreateResourceStatTracker("Mill", "animalFeed", false, false, true, locationLists[(int)LocationType.Mill]));
        StatTrackers.Add(CreateResourceStatTracker("Animal Farm", "livestock", false, false, true, locationLists[(int)LocationType.AnimalFarm]));
        StatTrackers.Add(CreateResourceStatTracker("Slaughterhouse", "meat", false, false, true, locationLists[(int)LocationType.Slaughterhouse]));
        StatTrackers.Add(CreateResourceStatTracker("Food factory", "rawFood", false, false, true, locationLists[(int)LocationType.FoodFactory]));
        StatTrackers.Add(CreateResourceStatTracker("Store", "money", false, false, true, locationLists[(int)LocationType.Store]));

        Units.Dispose();

        for (int i = 0; i < locationLists.Length; i++)
            locationLists[i].Dispose();

        GetComponent<UiManager>().started = true;
    }

    int[,] GenerateSquareMatrixWithAutoSize(int[] numbersAmounts)
    {
        int nonZerosTotalAmount = numbersAmounts.Sum();
        int matrixSize = (int)Math.Ceiling(Math.Sqrt(nonZerosTotalAmount));
        Width = matrixSize;
        Height = matrixSize;
        return GenerateMatrix(numbersAmounts, matrixSize, matrixSize);
    }

    //Random matrix of numbers from -1 to numbersAmounts.Length - 1
    //Number i appears numbersAmounts[i] times, the rest of the matrix is filled with -1
    int[,] GenerateMatrix(int[] numbersAmounts, int width, int height)
    {
        int nonZerosTotalAmount = numbersAmounts.Sum();
        int matrixLength = width * height;

        if (nonZerosTotalAmount > matrixLength)
            throw new ArgumentException();

        int zerosAmount = matrixLength - nonZerosTotalAmount;
        int[] probabilities = numbersAmounts.Concat(new int[] { zerosAmount }).ToArray();

        int[,] result = new int[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int randomNumber = UnityEngine.Random.Range(1, probabilities.Sum() + 1);
                int type = 0;
                while (randomNumber > probabilities[type])
                {
                    randomNumber -= probabilities[type];
                    type++;
                }
                probabilities[type] = probabilities[type] - 1;

                if (type == numbersAmounts.Length)
                    result[i, j] = -1;
                else
                    result[i, j] = type;
            }
        }
        return result;
    }

    int[,] ShowcaseMatrix()
    {
        var result = new int[4, 4]
        {
            { (int)LocationType.LivingHouse, (int)LocationType.Mine, (int)LocationType.Workshop, (int)LocationType.Forest },
            { -1, (int)LocationType.Refinery, (int)LocationType.Farm, (int)LocationType.Lumbermill },
            { (int)LocationType.Store, (int)LocationType.FoodFactory, (int)LocationType.Mill, -1 },
            { (int)LocationType.LivingHouse, (int)LocationType.Slaughterhouse, (int)LocationType.AnimalFarm, (int)LocationType.LivingHouse }
        };
        var result2 = new int[4, 4];
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                result2[i, 3 - j] = result[j, i];
        return result2;
    }

    void CreateLocationsGrid(int[,] locationsTypeDistribution)
    {
        for (int i = 0; i < locationsTypeDistribution.GetLength(0); i++)
            for (int j = 0; j < locationsTypeDistribution.GetLength(1); j++)
            {
                float3 translation = new float3((2 * i - locationsTypeDistribution.GetLength(0) + 1f) * SpacingMultiplier / 2, 0, (2 * j - locationsTypeDistribution.GetLength(1) + 1f) * SpacingMultiplier / 2);
                var block = Instantiate(BlockPrefab, translation + new float3(0, -1, 0), Quaternion.Euler(90, 0, 0), BlocksParent);
                var scale = SpacingMultiplier * 3.5f;
                block.transform.localScale = new Vector3(scale, scale, scale);

                var type = locationsTypeDistribution[i, j];

                if (type >= 0 && type < LocationTypesAmount)
                    CreateLocation(translation, (LocationType)locationsTypeDistribution[i, j], BuildingsScale);
            }
    }

    Entity CreateLocation(float3 translation, LocationType type, float size)
    {
        int locationId = LocationsCounter;

        Entity location = EntityManager.CreateEntity(LocationArchetype);
        EntityManager.SetComponentData(location, new SizeComponent { Size = size });
        EntityManager.SetComponentData(location, new Scale { Value = size });
        EntityManager.SetComponentData(location, new Translation { Value = translation });
        EntityManager.SetComponentData(location, new Rotation { Value = rotationAngle });
        EntityManager.SetComponentData(location, new IdComponent { Id = locationId });
        EntityManager.SetComponentData(location, new LocationInfoComponent { Type = type });

        Mesh mesh = SpriteMesh;
        Material material = materials[(int)type];
        EntityManager.SetSharedComponentData(location, new RenderMesh
        {
            mesh = mesh,
            material = material
        });

        switch((int)type)
        {
            case (int)LocationType.LivingHouse:
                break;
            case int n when (n > (int)LocationType.LivingHouse && n < (int)LocationType.Store):
                float avg = UnitsCount / WorkplaceCount;
                int vacanciesAmount;
                if (ShowcaseGrid)
                    vacanciesAmount = 2;
                else
                {
                    int minVacanciesAmount = 2;
                    int randomVacanciesAmount = (int)Mathf.RoundToInt(UnityEngine.Random.Range(avg * 0.8f, avg * 1.1f));
                    vacanciesAmount = Math.Max(minVacanciesAmount, randomVacanciesAmount);
                }
                EntityManager.SetComponentData(location, new NeedsInventoryComponent { StartingCapital = vacanciesAmount * 1500f});
                EntityManager.SetComponentData(location, new WorkplaceInfoComponent { VacanciesAmount = vacanciesAmount });
                break;
            case (int)LocationType.Store:
                EntityManager.SetComponentData(location, new NeedsInventoryComponent { StartingCapital = 1500f });
                break;
        }

        locationLists[(int)type].Add(location);
        return location;
    }

    private void CreateVacancies(Entity parentLocation, int amount)
    {
        for (int j = 0; j < amount; j++)
        {
            CreateVacancy(parentLocation);
        }
    }

    private void CreateVacancy(Entity parentLocation)
    {
        var vacancy = EntityManager.CreateEntity(VacancyArchetype);

        var weekSchedule = new WeekScheduleComponent();
        int workHours = 0;
        for (int i = 0; i < 7; i++)
        {
            byte start, end;
            if (i <= 5)
            {
                start = (byte)UnityEngine.Random.Range(8, 13);
                byte length = (byte)UnityEngine.Random.Range(5, 10);
                end = (byte)(start + length);
            }
            else
            {
                start = 0;
                end = 0;
            }
            weekSchedule[i] = new TimeInterval(start, end);
            workHours += end - start;
        }
        EntityManager.SetComponentData(vacancy, weekSchedule);
        EntityManager.SetComponentData(vacancy, new IdComponent { Id = vacancies.Length });
        EntityManager.SetComponentData(vacancy, new ParentLocationComponent { Location = parentLocation });
        EntityManager.SetComponentData(vacancy, new PaymentComponent { Value = UnityEngine.Random.Range(2, 6) * 5 * workHours });
        EntityManager.SetComponentData(vacancy, new VacancyTypeComponent { Value = RandomVacancyType() });

        vacancies.Add(vacancy);
    }

    private bool vacancyTypeSwitch = true;
    private VacancyType RandomVacancyType()
    {
        if (vacancyTypeSwitch)
        {
            vacancyTypeSwitch = !vacancyTypeSwitch;
            return VacancyType.Producer;
        }
        else
        {
            vacancyTypeSwitch = !vacancyTypeSwitch;
            return VacancyType.Transporter;
        }
    }

    void CreateUnits(int count)
    {
        HashSet<int> vacanciesSet = new HashSet<int>();
        for (int i = 0; i < vacancies.Length; i++)
        {
            vacanciesSet.Add(i);
        }

        for (int i = 0; i < count; i++)
        {
            int homeId = UnityEngine.Random.Range(0, locationLists[(int)LocationType.LivingHouse].Length);
            Entity vacancy = default;
            if (vacanciesSet.Count() != 0)
            {
                var vacancyId = vacanciesSet.ElementAt(UnityEngine.Random.Range(0, vacanciesSet.Count()));
                vacancy = vacancies[vacancyId];
                vacanciesSet.Remove(vacancyId);
            }

            CreateUnit(i, homeId, vacancy);
        }
    }

    void CreateUnit(int id, int homeId, Entity vacancy = default)
    {
        Entity unit = EntityManager.CreateEntity(UnitArchetype);
        EntityManager.SetComponentData(unit, new Rotation { Value = rotationAngle });
        EntityManager.SetComponentData(unit, new Scale { Value = UnitScale });

        EntityManager.SetComponentData(unit, new ParentLocationComponent { Location = locationLists[(int)LocationType.LivingHouse][homeId] });
        EntityManager.SetComponentData(unit, new RadialComponent { Angle = UnityEngine.Random.Range(0, Mathf.PI * 2), RadiusMult = UnityEngine.Random.Range(0, 0.3f) });
        EntityManager.SetComponentData(unit, new MovementTargetComponent { Destination = locationLists[(int)LocationType.LivingHouse][homeId] });
        EntityManager.SetComponentData(unit, new SpeedComponent { Value = UnityEngine.Random.Range(5f, 8f) });
        EntityManager.SetComponentData(unit, new IdComponent { Id = id });
        EntityManager.SetComponentData(unit, new HungerComponent { MetabolismSpeed = UnityEngine.Random.Range(0.7f, 1.3f), Satiety = 100f });

        var material = UnitMaterial;
        if (vacancy != default)
        {
            EntityManager.AddComponentData(unit, new EmploymentComponent { Vacancy = vacancy });
            EntityManager.SetComponentData(vacancy, new RelatedUnitComponent { Entity = unit });
            if (EntityManager.GetComponentData<VacancyTypeComponent>(vacancy).Value == VacancyType.Transporter)
                material = TransporterMaterial;
        }
        EntityManager.SetSharedComponentData(unit, new RenderMesh
        {
            mesh = SpriteMesh,
            material = material
        });

        Units.Add(unit);
    }

    Entity CreateResourceStatTracker(FixedString32 name, FixedString32 trackedResource, bool trackMin, bool trackMax, bool trackAvg, NativeList<Entity> trackedEntities)
    {
        var componentTypeList = new List<ComponentType>() { typeof(StatTrackingComponent) };
        if (trackMin)
            componentTypeList.Add(typeof(ResourceMinComponent));
        if (trackMax)
            componentTypeList.Add(typeof(ResourceMaxComponent));
        if (trackAvg)
            componentTypeList.Add(typeof(ResourceAvgComponent));
        var statTracker = EntityManager.CreateEntity(componentTypeList.ToArray());
        EntityManager.SetComponentData(statTracker, new StatTrackingComponent { TrackedResource = trackedResource, Name = name});
        var statBuffer = EntityManager.AddBuffer<TrackedEntitiesBufferElement>(statTracker);
        foreach (var entity in trackedEntities)
            statBuffer.Add(new TrackedEntitiesBufferElement { TrackedEntity = entity });
        return statTracker;
    }

    Entity CreateSatietyTracker(FixedString32 name, bool trackMin, bool trackMax, bool trackAvg, NativeList<Entity> trackedEntities)
    {
        var componentTypeList = new List<ComponentType>() { typeof(SatietyTrackingComponent) };
        if (trackMin)
            componentTypeList.Add(typeof(ResourceMinComponent));
        if (trackMax)
            componentTypeList.Add(typeof(ResourceMaxComponent));
        if (trackAvg)
            componentTypeList.Add(typeof(ResourceAvgComponent));
        var statTracker = EntityManager.CreateEntity(componentTypeList.ToArray());
        EntityManager.SetComponentData(statTracker, new SatietyTrackingComponent { Name = name });
        var statBuffer = EntityManager.AddBuffer<TrackedEntitiesBufferElement>(statTracker);
        foreach (var entity in trackedEntities)
            statBuffer.Add(new TrackedEntitiesBufferElement { TrackedEntity = entity });
        return statTracker;
    }

    private bool RenderingIsDisabled = false;
    public void SwitchRendering()
    {
        if (RenderingIsDisabled)
        {
            World.DefaultGameObjectInjectionWorld.GetExistingSystem<RenderMeshSystemV2>().Enabled = true;
        }
        else
        {
            World.DefaultGameObjectInjectionWorld.GetExistingSystem<RenderMeshSystemV2>().Enabled = false;
        }
        RenderingIsDisabled = !RenderingIsDisabled;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}