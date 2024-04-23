using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CubeController : MonoBehaviour
{
    private Rigidbody lightCube;
    public Rigidbody heavyCube;

    public int oszillatorSpringConstant;

    public float windSpeed; // m/s

    private float dense = 1.225F; // kg/m^3
    private float resistanceCoefficient = 1.3F;
    private float springLength;
    private int springConstant; // N/m

    private LightCubeStatus status;
    private static float MIN_OSZILLATOR_TIME = 4.0f;

    private float currentTimeStep; // s
    private List<List<float>> timeSeries;



    // Start is called before the first frame update
    void Start()
    {
        lightCube = GetComponent<Rigidbody>();
        status = LightCubeStatus.Oszillator;
        timeSeries = new List<List<float>>();
        Physics.IgnoreCollision(heavyCube.GetComponent<Collider>(), lightCube.GetComponent<Collider>());
    }


    // FixedUpdate can be called multiple times per frame
    void FixedUpdate() {
        float forceX = 0.0f; // N

        switch (status)
        {
            case LightCubeStatus.Oszillator:
                forceX = -lightCube.position.x * oszillatorSpringConstant;
                if (currentTimeStep > MIN_OSZILLATOR_TIME && lightCube.position.x <= 0)
                {
                    status = LightCubeStatus.Wind;
                }
                break;
            case LightCubeStatus.Wind:
                var area = lightCube.transform.localScale.x * lightCube.transform.localScale.y;
                forceX = 0.5f * resistanceCoefficient * dense * area * (float)System.Math.Pow((windSpeed-lightCube.velocity.x), 2) * Math.Sign(windSpeed-lightCube.velocity.x);
                break;
            case LightCubeStatus.Collision:
                forceX = -((lightCube.position.x - heavyCube.position.x) - springLength) * springConstant;
                break;
        }

        lightCube.AddForce(new Vector3(forceX, 0f, 0f));

        currentTimeStep += Time.deltaTime;
        var energie = 0.5f * lightCube.mass * (float)Math.Pow(lightCube.velocity.x,2);
        timeSeries.Add(new List<float>() {currentTimeStep, lightCube.position.x, lightCube.velocity.x, forceX, lightCube.mass*-lightCube.velocity.x,energie});
    }

    public void OnCollision()
    {
        status = LightCubeStatus.Collision;
    }


    //TimeSeries

    void OnApplicationQuit() {
        WriteTimeSeriesToCSV();
    }

    void WriteTimeSeriesToCSV() {
        using (var streamWriter = new StreamWriter("time_series.csv")) {
            streamWriter.WriteLine("t,x(t),v(t),F(t) (added),p(t),E(t)");
            
            foreach (List<float> timeStep in timeSeries) {
                streamWriter.WriteLine(string.Join(",", timeStep));
                streamWriter.Flush();
            }
        }
    }

    public void setSpringLength(float newSpringLength)
    {
        this.springLength = newSpringLength;
    }

    public void setSpringConstant(int newSpringConstant)
    {
        this.springConstant = newSpringConstant;
    }
}
