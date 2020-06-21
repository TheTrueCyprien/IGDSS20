using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : Building
{
    public Tile.TileTypes scales_with;
    public Vector2Int neighbour_range;
    public List<GameManager.ResourceTypes> input_resources;
    public GameManager.ResourceTypes output_resources;
    private float base_efficiency = 1.0f;

    void Start()
    {
        // generate jobs
        for (int j = 0; j < worker_capacity; j++)
        {
            Job new_job = new Job(this);
            JobManager.instance.RegisterJob(new_job);
        }
    }

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
            base_efficiency = Mathf.Min(Mathf.Max(0.0f, (scale_count - neighbour_range.x + 1.0f) / (neighbour_range.y - neighbour_range.x + 1.0f)), 1.0f);
        }

    }

    protected override void calc_efficiency()
    {
        efficiency = (base_efficiency * happiness_efficiency() * _workers.Count) / (3 * worker_capacity);
    }
}
