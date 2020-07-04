using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager instance = null;

    private Dictionary<Building, int> map_id = new Dictionary<Building, int>();
    private Dictionary<Worker, Vector2Int> worker_route = new Dictionary<Worker, Vector2Int>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void init_map(Tile start)
    {
        int id = start.potentials.Count;
        map_id[start._building] = id;
        // init start potential
        List<Tile> tiles_with_potential = new List<Tile>();
        start.potentials.Add(0);
        tiles_with_potential.Add(start);
        // calc potentials (basically dijkstra)
        int i = 0;
        while (i < tiles_with_potential.Count)
        {
            Tile tile = tiles_with_potential[i];
            foreach (var neighbour in tile._neighborTiles)
            {
                if (id == neighbour.potentials.Count)
                {
                    switch (neighbour._type)
                    {
                        case Tile.TileTypes.Water:
                            neighbour.potentials.Add(tile.potentials[id] + 30);
                            break;
                        case Tile.TileTypes.Sand:
                            neighbour.potentials.Add(tile.potentials[id] + 2);
                            break;
                        case Tile.TileTypes.Grass:
                            neighbour.potentials.Add(tile.potentials[id] + 1);
                            break;
                        case Tile.TileTypes.Forest:
                            neighbour.potentials.Add(tile.potentials[id] + 2);
                            break;
                        case Tile.TileTypes.Stone:
                            neighbour.potentials.Add(tile.potentials[id] + 1);
                            break;
                        case Tile.TileTypes.Mountain:
                            neighbour.potentials.Add(tile.potentials[id] + 3);
                            break;
                    }
                    int insert_index = tiles_with_potential.BinarySearch(neighbour, new ComparePotential(id));
                    insert_index = insert_index < 0 ? ~insert_index : insert_index;
                    tiles_with_potential.Insert(insert_index, neighbour);
                }
            }
            i++;
        }
    }

    public void register_worker(Building b, Worker w)
    {
        // init route if new worker
        Vector2Int route;
        if (worker_route.ContainsKey(w))
        {
            route = worker_route[w];
        }
        else
        {
            route = new Vector2Int();
        }
        // assign id
        if (b is HousingBuilding)
        {
            route.x = map_id[b];
        }
        else
        {
            route.y = map_id[b];
        }
        worker_route[w] = route;
    }

    public void remove_worker(Worker w)
    {
        worker_route.Remove(w);
    }

    public Vector2Int request_route(Worker w)
    {
        return worker_route[w];
    }
}

class ComparePotential : Comparer<Tile>
{
    private int index;

    public ComparePotential(int id)
    {
        index = id;
    }

    public override int Compare(Tile x, Tile y)
    {
        return x.potentials[index].CompareTo(y.potentials[index]);
    }
}