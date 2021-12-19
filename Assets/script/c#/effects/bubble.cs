using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{

    public float InitialScale = 0.0f;

    public float ScaleIncrease = 0.02f;
    public float MaxScale = 0.1f;

    public PotController Water;

    private float _curScale;
    public Vector3 Center;
    public float XRadius;
    public float ZRadius;

    private float _decay = 1.0f;

    private float _decayRate = 0.015f;
    private bool _decaying = false;

    // Start is called before the first frame update
    void Start()
    {
        this._curScale = this.InitialScale;
        float xPosition = Center.x + Random.Range(-this.XRadius, this.XRadius);
        float zPosition = Center.z + Random.Range(-this.ZRadius, this.ZRadius);
        this.transform.position = new Vector3(xPosition, this.Center.y, zPosition);
    }

    // Update is called once per frame
    void Update()
    {

        // Update the position on radius spawn area of the bubble
        this.ZRadius = Water.GetXRadius();
        this.XRadius = Water.GetXRadius();
        this.ScaleIncrease = Water.GetSpeed();

        // Set the bubble to be the height of the water
        float bubbleHeight = this.Water.GetWaterHeightAtPosition(this.transform.position);
        this.Center = new Vector3(this.Water.transform.position.x, this.Water.GetWaterPosition().y + bubbleHeight, this.Water.transform.position.z);
        this.transform.position = new Vector3(this.transform.position.x, Center.y-0.05f, this.transform.position.z);
       
        // Update the size of the bubble, and set decay=true if the scale is above the max
        this.transform.localScale = this.transform.localScale + new Vector3(ScaleIncrease/30.0f,ScaleIncrease/30.0f,ScaleIncrease/30.0f);
         if (this.transform.localScale.x > MaxScale){
            //start decay
            _decaying = true;
        }

        // Decay away the bubble if max height is hit
        if (_decaying) {
            _decay -= _decayRate;
        }
        if (_decay <= 0){
            this.transform.localScale = new Vector3(InitialScale,InitialScale,InitialScale);
            float xPosition = Center.x + Random.Range(-XRadius, XRadius);
            float zPosition = Center.z + Random.Range(-ZRadius, ZRadius);
            this.transform.position = new Vector3(xPosition, Center.y, zPosition);
            _decay = 1.0f;
            _decaying = false;
        }

        setShaderProperties();
    }

    void setShaderProperties(){
        Material mat = this.GetComponent<Renderer>().material;
        mat.SetTexture("_MainTex", Water.GetWaterHeightMap());
        mat.SetFloat("_xRad", Water.GetXRadius());
        mat.SetFloat("_zRad", Water.GetXRadius());
        mat.SetFloat("waterSize", Water.GetComponent<PotController>().GetWaterSize());
        mat.SetVector("_Center", Water.GetCenter());
        mat.SetVector("_Color", Water.GetColor());
        mat.SetFloat("_DecayAmount", _decay);
        mat.SetFloat("_MaxHeight", Water.GetComponent<PotController>().GetWaterMaxHeight());
        mat.SetFloat("_WaterLevel", Water.GetComponent<PotController>().GetWaterPosition().y);
        mat.SetVector("_PotCenter", Water.GetComponent<PotController>().GetCenter());
        mat.SetTexture("_HeightMap", Water.GetComponent<PotController>().GetWaterHeightMap());
        mat.SetFloat("_WaterSize", Water.GetComponent<PotController>().GetWaterSize());
    }
}
