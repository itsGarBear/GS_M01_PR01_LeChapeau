using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNTController : MonoBehaviour
{
    public float speed = 5.0f;
    public Material startColor;
    public Material endColor;
    private float startTime;

    void OnEnable()
    {
        startTime = Time.time;
    }

    void Update()
    {
        float t = Mathf.Sin((Time.time - startTime) * speed);
        GetComponent<Renderer>().material.color = Color.Lerp(startColor.color, endColor.color, t);
        //GetComponent<Renderer>().material.color = Color.Lerp(endColor.color, startColor.color, t);
    }
}
