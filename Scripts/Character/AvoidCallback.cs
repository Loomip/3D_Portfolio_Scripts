using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidCallback : MonoBehaviour
{
    Health isAvoid;

    private void Start()
    {
        isAvoid = GetComponent<Health>();
    }

    public void IsAvoid()
    {
        isAvoid.CanTakeDamage = true;
    }
}
