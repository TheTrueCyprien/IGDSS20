using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{

    #region Map generation
    private Tile[,] _tileMap; //2D array of all spawned tiles

    public List<GameObject> tile_prefabs;
    public List<float> height_limits;
    public Texture2D heightmap;
    public float tile_offset_x = 8.66f;
    public float tile_offset_z = 10;
    public float odd_row_offset = 5;
    public float height_multiplier = 20;

    // Generates terrain by iterating through the heightmap and instantiating tiles with the lowest matching height limit
    void generate_terrain()
    {
        // make sure there is enough height limits for assigned prefabs
        Assert.AreEqual(tile_prefabs.Count, height_limits.Count);
        // ensure ascending order of height limits
        for (int i = 1; i < height_limits.Count; i++)
        {
            Assert.IsTrue(height_limits[i - 1] < height_limits[i]);
        }
        _tileMap = new Tile[heightmap.height, heightmap.width];
        // iterate through each pixel
        Color[] pixels = heightmap.GetPixels();
        for (int y = 0; y < heightmap.height; y++)
        {
            for (int x = 0; x < heightmap.width; x++)
            {
                // get gray value of current pixel as height
                float height = pixels[y * heightmap.width + x].grayscale;
                // find lowest matching height limit
                for (int i = 0; i < height_limits.Count; i++)
                {
                    if (height <= height_limits[i])
                    {
                        // offset every odd row to align hexes properly
                        float offset = y % 2 == 1 ? odd_row_offset : 0;
                        // instantiate corresponding tile
                        GameObject tile = Instantiate(tile_prefabs[i], new Vector3(y * tile_offset_x, height_multiplier * height, x * tile_offset_z + offset), Quaternion.identity);
                        _tileMap[y, x] = tile.GetComponent<Tile>();
                        break;
                    }
                }
            }
        }
        // attach neighbours to each tile
        foreach (var tile in _tileMap)
        {
            List<Tile> neighbours = FindNeighborsOfTile(tile);
            tile._neighborTiles = neighbours;
        }
    }

    // accessor for mousemanager limits
    public Vector2 map_boundaries()
    {
        return new Vector2(heightmap.width * tile_offset_z, heightmap.height * tile_offset_x);
    }
    #endregion

    #region Buildings
    public GameObject[] _buildingPrefabs; //References to the building prefabs
    public int _selectedBuildingPrefabIndex = 0; //The current index used for choosing a prefab to spawn from the _buildingPrefabs list
    public int _money;
    private Dictionary<Building.BuildingType, int> _buildingInstances = new Dictionary<Building.BuildingType, int>();
    #endregion


    #region Resources
    private Dictionary<ResourceTypes, float> _resourcesInWarehouse = new Dictionary<ResourceTypes, float>(); //Holds a number of stored resources for every ResourceType

    //A representation of _resourcesInWarehouse, broken into individual floats. Only for display in inspector, will be removed and replaced with UI later
    [SerializeField]
    private float _ResourcesInWarehouse_Fish;
    [SerializeField]
    private float _ResourcesInWarehouse_Wood;
    [SerializeField]
    private float _ResourcesInWarehouse_Planks;
    [SerializeField]
    private float _ResourcesInWarehouse_Wool;
    [SerializeField]
    private float _ResourcesInWarehouse_Clothes;
    [SerializeField]
    private float _ResourcesInWarehouse_Potato;
    [SerializeField]
    private float _ResourcesInWarehouse_Schnapps;
    #endregion

    #region Enumerations
    public enum ResourceTypes { None, Fish, Wood, Planks, Wool, Clothes, Potato, Schnapps }; //Enumeration of all available resource types. Can be addressed from other scripts by calling GameManager.ResourceTypes
    #endregion

    #region MonoBehaviour
    // Start is called before the first frame update
    void Start()
    {
        generate_terrain();
        PopulateResourceDictionary();
        PopulateBuildingDictionary();
        InvokeRepeating("UpdateEconomyTick", 0.0f, 60.0f);
    }

    // Update is called once per frame
    void Update()
    {
        HandleKeyboardInput();
        UpdateInspectorNumbersForResources();
    }
    #endregion

    #region Methods
    //Makes the resource dictionary usable by populating the values and keys
    void PopulateResourceDictionary()
    {
        _resourcesInWarehouse.Add(ResourceTypes.None, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Fish, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Wood, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Planks, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Wool, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Clothes, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Potato, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Schnapps, 0);
    }

    void PopulateBuildingDictionary()
    {
        _buildingInstances.Add(Building.BuildingType.Base, 0);
        _buildingInstances.Add(Building.BuildingType.Fishery, 0);
        _buildingInstances.Add(Building.BuildingType.Lumberjack, 0);
        _buildingInstances.Add(Building.BuildingType.PotatoFarm, 0);
        _buildingInstances.Add(Building.BuildingType.Sawmill, 0);
        _buildingInstances.Add(Building.BuildingType.SchnappsDistillery, 0);
        _buildingInstances.Add(Building.BuildingType.SheepFarm, 0);
        _buildingInstances.Add(Building.BuildingType.Weavery, 0);
    }

    //Sets the index for the currently selected building prefab by checking key presses on the numbers 1 to 0
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _selectedBuildingPrefabIndex = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _selectedBuildingPrefabIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _selectedBuildingPrefabIndex = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _selectedBuildingPrefabIndex = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _selectedBuildingPrefabIndex = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            _selectedBuildingPrefabIndex = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            _selectedBuildingPrefabIndex = 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            _selectedBuildingPrefabIndex = 7;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            _selectedBuildingPrefabIndex = 8;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            _selectedBuildingPrefabIndex = 9;
        }
    }

    //Updates the visual representation of the resource dictionary in the inspector. Only for debugging
    void UpdateInspectorNumbersForResources()
    {
        _ResourcesInWarehouse_Fish = _resourcesInWarehouse[ResourceTypes.Fish];
        _ResourcesInWarehouse_Wood = _resourcesInWarehouse[ResourceTypes.Wood];
        _ResourcesInWarehouse_Planks = _resourcesInWarehouse[ResourceTypes.Planks];
        _ResourcesInWarehouse_Wool = _resourcesInWarehouse[ResourceTypes.Wool];
        _ResourcesInWarehouse_Clothes = _resourcesInWarehouse[ResourceTypes.Clothes];
        _ResourcesInWarehouse_Potato = _resourcesInWarehouse[ResourceTypes.Potato];
        _ResourcesInWarehouse_Schnapps = _resourcesInWarehouse[ResourceTypes.Schnapps];
    }

    //Updates the resource upkeep of all buildings 
    void UpdateEconomyTick()
    {
        //constant income
        _money += 100;

        //upkeep cost
        foreach (var building_prefab in _buildingPrefabs)
        {
            Building.BuildingType type = building_prefab.GetComponent<Building>().type;
            int upkeep = building_prefab.GetComponent<Building>().upkeep;
            _money -= _buildingInstances[type] * upkeep;
        }
    }

    //Production cycle for building
    private IEnumerator production_cycle(Building building)
    {
        while (true)
        {
            bool active = true;
            foreach (var resource in building.input_resources)
            {
                if (!HasResourceInWarehoues(resource))
                {
                    active = false;
                    break;
                }
            }
            if (active)
            {
                yield return new WaitForSeconds(building.cycle_time());
                _resourcesInWarehouse[building.output_resources] += building.output_count;
            }
            else
            {
                yield return null;
            }
        }
    }

    //Checks if there is at least one material for the queried resource type in the warehouse
    public bool HasResourceInWarehoues(ResourceTypes resource)
    {
        return _resourcesInWarehouse[resource] >= 1;
    }

    //Is called by MouseManager when a tile was clicked
    //Forwards the tile to the method for spawning buildings
    public void TileClicked(int height, int width)
    {
        Tile t = _tileMap[height, width];

        PlaceBuildingOnTile(t);
    }

    //Checks if the currently selected building type can be placed on the given tile and then instantiates an instance of the prefab
    private void PlaceBuildingOnTile(Tile tile)
    {
        //if there is building prefab for the number input
        if (_selectedBuildingPrefabIndex < _buildingPrefabs.Length)
        {
            GameObject building_prefab = _buildingPrefabs[_selectedBuildingPrefabIndex];
            Building building_script = building_prefab.GetComponent<Building>();

            if (_resourcesInWarehouse[ResourceTypes.Planks] >= building_script.cost_planks && 
                _money >= building_script.cost_money &&
                building_script.buildable_on.Contains(tile._type))
            {
                _resourcesInWarehouse[ResourceTypes.Planks] -= building_script.cost_planks;
                _money -= building_script.cost_money;

                GameObject b = Instantiate(building_prefab, tile.transform.position, tile.transform.rotation);
                b.GetComponent<Building>().tile = tile;
                _buildingInstances[building_script.type] += 1;

                StartCoroutine(production_cycle(building_script));
            }

        }
    }

    //Returns a list of all neighbors of a given tile
    private List<Tile> FindNeighborsOfTile(Tile tile)
    {
        List<Tile> result = new List<Tile>();

        Collider[] hit = Physics.OverlapSphere(tile.transform.position, tile_offset_z, LayerMask.GetMask("Tiles"));

        foreach (var t in hit)
        {
            Tile neighbour = t.GetComponentInParent<Tile>();
            if (neighbour != tile)
            {
                result.Add(neighbour);
            }
        }

        return result;
    }
    #endregion
}