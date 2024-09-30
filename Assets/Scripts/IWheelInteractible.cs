using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWheelInteractible
{
    abstract void OnSpin(float deltaRotation);
    void OnStartSpin() { }
    void OnStopSpin() { }
}
