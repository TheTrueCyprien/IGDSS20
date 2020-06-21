using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobManager : MonoBehaviour
{

    private List<Job> _availableJobs = new List<Job>();
    private List<Job> _occupiedJobs = new List<Job>();
    public List<Worker> _unoccupiedWorkers = new List<Worker>();
    public static JobManager instance = null;


    #region MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleUnoccupiedWorkers();
    }
    #endregion


    #region Methods

    private void HandleUnoccupiedWorkers()
    {
        if (_unoccupiedWorkers.Count > 0 && _availableJobs.Count > 0)
        {
            Worker applicant = _unoccupiedWorkers[0];
            Job job_listing = _availableJobs[Random.Range(0, _availableJobs.Count)];
            job_listing.AssignWorker(applicant);
            _unoccupiedWorkers.Remove(applicant);
            applicant._employed = true;
            _availableJobs.Remove(job_listing);
            _occupiedJobs.Add(job_listing);
        }
    }

    public void RegisterWorker(Worker w)
    {
        _unoccupiedWorkers.Add(w);
    }

    public void RegisterJob(Job j)
    {
        _availableJobs.Add(j);
    }

    public void RemoveWorker(Worker w)
    {
        _unoccupiedWorkers.Remove(w);
    }

    #endregion
}
