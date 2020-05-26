using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public enum BuildingType
    {
        Fishery, Lumberjack, Sawmill, SheepFarm, Weavery, PotatoFarm, SchnappsDistillery
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

    public float efficiency = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        int scale_count = 0;
        foreach (var neighbour in tile._neighborTiles)
        {
            if (neighbour._type == scales_with)
                scale_count += 1;
        }
        efficiency = Mathf.Min(Mathf.Max(0.0f, (scale_count - neighbour_range.x + 1.0f) / (neighbour_range.y - neighbour_range.x + 1.0f)), 1.0f);
    }

    public float cycle_time()
    {
        return efficiency > 0.0f ? 1.0f / efficiency : -1.0f;
    }

}
