using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bubble : MonoBehaviour
{

    public float initialScale = 0.0f;

    public float scaleIncrease = 0.02f;
    public float maxScale = 0.3f;

    private float curScale;
    public Vector3 center;
    public float xRadius;
    public float zRadius;

    // Start is called before the first frame update
    void Start()
    {
        //this.transform.localScale = new Vector3(initialScale,initialScale,initialScale);
        curScale = initialScale;
        float xPosition = center.x + Random.Range(-xRadius, xRadius);
        float zPosition = center.z + Random.Range(-zRadius, zRadius);
        this.transform.position = new Vector3(xPosition, center.y, zPosition);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localScale = this.transform.localScale + new Vector3(scaleIncrease/30.0f,scaleIncrease/30.0f,scaleIncrease/30.0f);
        this.transform.position = new Vector3(this.transform.position.x, center.y-0.05f, this.transform.position.z);
        if (this.transform.localScale.x > maxScale){
            this.transform.localScale = new Vector3(initialScale,initialScale,initialScale);
            float xPosition = center.x + Random.Range(-xRadius, xRadius);
            float zPosition = center.z + Random.Range(-zRadius, zRadius);
            this.transform.position = new Vector3(xPosition, center.y, zPosition);
        }
    }
}
