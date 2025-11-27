using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    void Start()
    {
        sr.enabled = false;
    }
}
