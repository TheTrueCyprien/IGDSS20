using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingBuilding : Building
{
    #region Methods
    public bool can_spawn() {
        return _workers.Count < capacity;
    }
    #endregion
}
