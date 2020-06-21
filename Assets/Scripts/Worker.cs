using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    public bool _employed = false;
    public float _age = 0.0f; // The age of this worker
    public float _happiness = 1.0f; // The happiness of this worker
    public float consumption_period = 150.0f;

    public List<GameManager.ResourceTypes> consumables = new List<GameManager.ResourceTypes> { GameManager.ResourceTypes.Fish, GameManager.ResourceTypes.Clothes, GameManager.ResourceTypes.Schnapps };

    private float _age_clock = 0.0f; // Counter to track 15sec intervals
    private float _consumption_clock = 0.0f; // Counter to track consumption intervals

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Consume();
        Age();
    }

    private void Consume()
    {
        _consumption_clock += Time.deltaTime;
        if (_consumption_clock >= consumption_period)
        {
            int resources_consumed = 0;
            foreach (var resource in consumables)
            {
                if (GameManager.instance.ConsumeResource(resource))
                {
                    resources_consumed += 1;
                }
            }
            _happiness = (resources_consumed + (_employed ? 1 : 0)) / (consumables.Count+1);
            _consumption_clock %= consumption_period;
        }
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
        }

        if (_age > 14)
        {
            BecomeOfAge();
        }

        if (_age > 64)
        {
            Retire();
        }

        if (_age > 100)
        {
            Die();
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
        Destroy(this.gameObject, 1f);
    }
}
