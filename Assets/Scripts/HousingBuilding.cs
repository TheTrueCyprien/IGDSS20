using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingBuilding : Building
{
    #region MonoBehavior
    public void Start() {
        //spawn two workers
        for (int i = 0; i < 2; i++)
        {
            spawn_worker();
        }
        foreach (Worker w in _workers)
        {
            Debug.Log("set age of spawned worker");
            w._age = 14;
            w.BecomeOfAge();
        }
    }

    private void Update()
    {
        if (progress >= generation_interval)
        {
            progress = 0.0f;
            spawn_worker();
        }
        efficiency = happiness_efficiency();
        progress += efficiency * Time.deltaTime;
    }
    #endregion

    #region Methods
    public GameObject worker_prefab;

    public void spawn_worker() {
        if (_workers.Count < worker_capacity) {
            GameObject worker_obj = Instantiate(worker_prefab, transform.position, transform.rotation);
            Worker w = worker_obj.GetComponent<Worker>();
            WorkerAssignedToBuilding(w);
        }
        else {
            Debug.Log("No more workers created, capacity full!");
        }
    }
    #endregion
}