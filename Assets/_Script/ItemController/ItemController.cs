using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public abstract class ItemController : MonoBehaviour
{
    public abstract void ApplyEffect(PlayerController player);
   
}
