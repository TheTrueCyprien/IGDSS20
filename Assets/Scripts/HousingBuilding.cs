using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingBuilding : Building
{
    #region MonoBehavior
    public void Start() {
        //spawn two workers
        for (int i = 0; i < 2; i++) {
            spawn_worker();
        }
        InvokeRepeating("spawn_worker", 0.0f, cycle_time());
    }
    #endregion

    #region Methods
    protected override void calc_efficiency() {
        efficiency = happiness_efficiency();
    }

    public void spawn_worker() {
        if (_workers.Count < worker_capacity) {
            Worker w = new Worker();
            WorkerAssignedToBuilding(w);
            JobManager.instance.RegisterWorker(w);
        }
    }
    #endregion
}
