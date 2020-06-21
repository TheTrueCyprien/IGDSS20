using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    public enum BuildingType
    {
        Fishery, Lumberjack, Sawmill, SheepFarm, Weavery, PotatoFarm, SchnappsDistillery, Residence
    }
    public BuildingType type;
    public int upkeep;
    public int cost_money;
    public int cost_planks;
    public int generation_interval;
    public int output_count;
    public List<Tile.TileTypes> buildable_on;
    public int worker_capacity;

    protected float efficiency = 1.0f;
    protected float progress = 0.0f;

    protected float happiness_efficiency()
    {
        return _workers.Count > 0 ? _workers.Average(w => w._happiness) : 0.0f;
    }
    
    #region Workers
    public List<Worker> _workers; //List of all workers associated with this building, either for work or living
    #endregion

    #region Jobs
    public List<Job> _jobs; // List of all available Jobs. Is populated in Start()
    #endregion
    

    #region Methods   
    public void WorkerAssignedToBuilding(Worker w)
    {
        _workers.Add(w);
    }

    public void WorkerRemovedFromBuilding(Worker w)
    {
        _workers.Remove(w);
    }
    #endregion
}
