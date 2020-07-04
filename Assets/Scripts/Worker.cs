using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Worker : MonoBehaviour
{
    public bool _employed = false;
    public float _age = 0.0f; // The age of this worker
    public float _happiness = 1.0f; // The happiness of this worker
    public float consumption_period = 150.0f;

    public List<GameManager.ResourceTypes> consumables = new List<GameManager.ResourceTypes> { GameManager.ResourceTypes.Fish, GameManager.ResourceTypes.Clothes, GameManager.ResourceTypes.Schnapps };

    private float _age_clock = 0.0f; // Counter to track 15sec intervals
    private Dictionary<GameManager.ResourceTypes, float> _consumption_clock = new Dictionary<GameManager.ResourceTypes, float>(); // Counter to track consumption intervals
    private Vector2Int _navigation_targets = new Vector2Int();
    private int _navigation_next_id = -1;
    private Vector3 _navigation_next_pos = new Vector3();
    private float waiting_progress = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.increment_population();

        foreach (var resource in consumables)
        {
            _consumption_clock[resource] = 0.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Consume();
        Age();

        if (_employed)
            make_step();
    }

    private void Consume()
    {
        foreach (var resource in consumables)
        {
            _consumption_clock[resource] += Time.deltaTime;
            if (_consumption_clock[resource] >= consumption_period && 
                GameManager.instance.ConsumeResource(resource)) {
                _consumption_clock[resource] %= consumption_period;
            }
        }
        int resources_consumed = _consumption_clock.Where(x => x.Value < consumption_period).Count();
        _happiness = (resources_consumed + (_employed ? 1 : 0)) / (consumables.Count+1.0f);
    }

    private void Age()
    {
        //When becoming of age, the worker enters the job market, and leaves it when retiring.
        //Eventually, the worker dies and leaves an empty space in his home. His Job occupation is also freed up.
        _age_clock += Time.deltaTime;
        if (_age_clock >= 15)
        {
            _age++;
            _age_clock %= 15;

            if (_age == 14)
            {
                BecomeOfAge();
            }

            if (_age == 64)
            {
                Retire();
            }

            if (_age == 100)
            {
                Die();
            }
        }
    }


    public void BecomeOfAge()
    {
        JobManager.instance.RegisterWorker(this);
    }

    private void Retire()
    {
        JobManager.instance.RemoveWorker(this);
    }

    private void Die()
    {
        GameManager.instance.decrement_population();
        Destroy(this.gameObject, 1f);
    }

    private Vector3 get_next_step()
    {
        LayerMask mask = LayerMask.GetMask("Tiles");
        RaycastHit hit;
        if (_navigation_next_id != -1 &&
            Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, Mathf.Infinity, mask))
        {
            Tile current_tile = hit.collider.GetComponent<Tile>();
            if (current_tile.potentials[_navigation_next_id] == 0.0f)
            {
                waiting_progress += Time.deltaTime;
                if (waiting_progress >= 5.0f)
                {
                    _navigation_next_id = _navigation_targets.x == _navigation_next_id ? _navigation_targets.y : _navigation_targets.x;
                }
                else
                {
                    return transform.position;
                }
            }

            Tile target_tile = current_tile._neighborTiles.Aggregate((t1, t2) => t1.potentials[_navigation_next_id] < t2.potentials[_navigation_next_id] ? t1 : t2);
            return target_tile.transform.position;
        }
        return transform.position;
    }

    private void make_step()
    {
        if (Vector3.Distance(transform.position, _navigation_next_pos) < 0.1f) 
        {
            _navigation_next_pos = get_next_step();
        }
        transform.position = Vector3.Lerp(transform.position, _navigation_next_pos, Time.deltaTime);
    }

    public void request_route()
    {
        _navigation_targets = NavigationManager.instance.request_route(this);
        _navigation_next_id = _navigation_targets.y;
        _navigation_next_pos = get_next_step();
    }
}
