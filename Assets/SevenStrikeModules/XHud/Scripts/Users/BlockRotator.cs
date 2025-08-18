using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRotator : MonoBehaviour
{
    public float Angle;
    public float RandomDealy = 0.5f;

    void Start()
    {
        float min = Angle - (Angle * RandomDealy);
        float max = Angle + (Angle * RandomDealy);
        Angle = Random.Range(min, max);
    }

    void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * Angle, Space.Self);
    }
}
