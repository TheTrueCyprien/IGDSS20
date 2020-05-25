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
    public List<GameManager.ResourceTypes> input_ressources;
    public GameManager.ResourceTypes output_ressources;

    private float efficiency = 1.0f;
    private float progress = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
