using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HeavyCubeScript : MonoBehaviour
{
    private Rigidbody heavyCube;
    public Rigidbody lightCube;

    public int springConstant = 0; // N/m

    private bool inCollision = false;
    private float springLength;

    private float currentTimeStep; // s
    private List<List<float>> timeSeries;



    // Start is called before the first frame update
    void Start()
    {
        heavyCube = GetComponent<Rigidbody>();
        timeSeries = new List<List<float>>();
        Physics.IgnoreCollision(heavyCube.GetComponent<Collider>(), lightCube.GetComponent<Collider>());
    }


    // FixedUpdate can be called multiple times per frame
    void FixedUpdate()
    {
        if (!inCollision && checkCollisionEnter())
        {
            OnCollision();
        }
        float forceX = 0; // N

        if (inCollision)
        {
            if (checkCollisionExit())
            {
                springConstant = 0;
                lightCube.GetComponent<CubeController>().setSpringConstant(springConstant);
            }
            // Calculate spring force for x component

            forceX = ((lightCube.position.x - heavyCube.position.x) - springLength) * springConstant;
        }

        heavyCube.AddForce(new Vector3(forceX, 0f, 0f));

        currentTimeStep += Time.deltaTime;
        var energie = 0.5f * heavyCube.mass * (float)Math.Pow(heavyCube.velocity.x, 2);
        timeSeries.Add(new List<float>() { currentTimeStep, heavyCube.position.x, heavyCube.velocity.x, forceX, heavyCube.mass * -heavyCube.velocity.x, energie });
    }

    private void OnCollision()
    {
        inCollision = true;
        lightCube.GetComponent<CubeController>().OnCollision();
        springLength = lightCube.position.x - heavyCube.position.x;
        lightCube.GetComponent<CubeController>().setSpringConstant(springConstant);
        lightCube.GetComponent<CubeController>().setSpringLength(springLength);
    }

    private bool checkCollisionEnter()
    {
        return ((lightCube.position.x - lightCube.transform.localScale.x /8*7) <= (heavyCube.position.x + heavyCube.transform.localScale.x/8*7));
    }

    private bool checkCollisionExit()
    {
        return ((lightCube.position.x - lightCube.transform.localScale.x /8*7) > (heavyCube.position.x + heavyCube.transform.localScale.x / 8*7));
    }



    //TimeSeries

    void OnApplicationQuit()
    {
        WriteTimeSeriesToCSV();
    }

    void WriteTimeSeriesToCSV()
    {
        using (var streamWriter = new StreamWriter("time_series_heavy.csv"))
        {
            streamWriter.WriteLine("t,x(t),v(t),F(t) (added),p(t),E(t)");

            foreach (List<float> timeStep in timeSeries)
            {
                streamWriter.WriteLine(string.Join(",", timeStep));
                streamWriter.Flush();
            }
        }
    }
}
