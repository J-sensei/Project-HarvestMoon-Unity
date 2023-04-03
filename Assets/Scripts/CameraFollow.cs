using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float smoothness = 0.3f;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    private Vector3 _velocity;

    private void Start()
    {
        if(target == null)
        {
            target = GameObject.Find("Player").transform;
        }
    }

    private void Update()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _velocity, smoothness);
    }
}
