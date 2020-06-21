using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : Building
{
    public Tile.TileTypes scales_with;
    public Vector2Int neighbour_range;
    public List<GameManager.ResourceTypes> input_resources;
    public GameManager.ResourceTypes output_resources;

    public void init_efficiency(List<Tile> neighbours)
    {
        if (scales_with != Tile.TileTypes.Empty) 
        {
            int scale_count = 0;
            foreach (var neighbour in neighbours)
            {
                if (neighbour._type == scales_with)
                    scale_count += 1;
            }
            efficiency = Mathf.Min(Mathf.Max(0.0f, (scale_count - neighbour_range.x + 1.0f) / (neighbour_range.y - neighbour_range.x + 1.0f)), 1.0f);
        }

    }

}
