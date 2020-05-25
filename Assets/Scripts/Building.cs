using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public enum BuildingType
    {
        Base, Fishery, Lumberjack, Sawmill, SheepFarm, Weavery, PotatoFarm, SchnappsDistillery
    }

    public BuildingType type;
    public int upkeep;
    public int cost_money;
    public int cost_planks;
    public Tile tile;
    public int generation_interval;
    public int output_count;
    public List<Tile.TileTypes> buildable_on;
    public Tile.TileTypes scales_with;
    public Vector2Int neighbour_range;
    public List<GameManager.ResourceTypes> input_resources;
    public GameManager.ResourceTypes output_resources;

    private float efficiency = 1.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    public float cycle_time()
    {
        return 1 / efficiency;
    }

}
