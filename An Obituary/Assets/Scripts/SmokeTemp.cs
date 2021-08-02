using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeTemp : MonoBehaviour
{
    protected float percent;
    protected ParticleSystem particles;

    // Start is called before the first frame update
    void Start()
    {
        particles = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        ParticleSystem.MainModule m = particles.main;
        m.startColor = new Color(percent, percent, percent, 1.0f);
    }

    void FixedUpdate()
    {
        percent -= Time.deltaTime / 3;
    }

    void OnEnable()
    {
        percent = 1.0f;
    }
}
