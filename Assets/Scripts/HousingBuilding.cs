using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingBuilding : Building
{
    public int capacity = 10;

    public bool can_spawn() {
        return _workers.Count < capacity;
    }
}
