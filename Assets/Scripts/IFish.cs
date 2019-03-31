using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFish 
{
    float GetSize();
    Fish.TYPE GetType();
    void Eaten();
}
