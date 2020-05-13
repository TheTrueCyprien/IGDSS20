using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class GameManager : MonoBehaviour
{
    public List<GameObject> tile_prefabs;
    public List<float> height_limits;
    public Texture2D heightmap;
    public float tile_offset_x = 8.66f;
    public float tile_offset_z = 10;
    public float odd_row_offset = 5;
    public float height_multiplier = 20; 

    // Generates terrain by iterating through the heightmap and instantiating tiles with the lowest matching height limit
    void Start()
    {
        // make sure there is enough height limits for assigned prefabs
        Assert.AreEqual(tile_prefabs.Count, height_limits.Count);
        // ensure ascending order of height limits
        for (int i = 1; i < height_limits.Count; i++)
        {
            Assert.IsTrue(height_limits[i - 1] < height_limits[i]);
        }
        // iterate through each pixel
        Color[] pixels = heightmap.GetPixels();
        for (int y = 0; y <  heightmap.height; y++)
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
                        break;
                    }
                }
            }
        }
    }

    // accessor for mousemanager limits
    public Vector2 map_boundaries()
    {
        return new Vector2(heightmap.width * tile_offset_z, heightmap.height * tile_offset_x);
    }
}
