using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingBuilding : Building
{
    #region MonoBehavior
    public void Start() {
        //spawn two workers
        spawn_worker();
        InvokeRepeating("spawn_worker", 0.0f, cycle_time());
        foreach (Worker w in _workers)
        {
            w._age = 14;
        }
    }
    #endregion

    #region Methods
    public GameObject worker_prefab;
    protected override void calc_efficiency() {
        efficiency = happiness_efficiency();
    }

    public void spawn_worker() {
        if (_workers.Count < worker_capacity) {
            GameObject worker_obj = Instantiate(worker_prefab, transform.position, transform.rotation);
            Worker w = worker_obj.GetComponent<Worker>();
            WorkerAssignedToBuilding(w);
        }
    }
    #endregion
}
