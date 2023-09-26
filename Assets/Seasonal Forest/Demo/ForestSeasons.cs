using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestSeasons : MonoBehaviour {
    // declaring the 3 versions of terrain in the scene (you will need to assign the gameobjects into these 3 generated fields)
    public GameObject TerrainSummer;
    public GameObject TerrainAutumn;
    public GameObject TerrainWinter;
    public GameObject Leaves;
    public GameObject Particles;
    public GameObject Ivys;
    // declaring the array of gameobjects, which have regular & snowy versions (you will need to assign them into the [0] & [1] generated fields)
    public GameObject[] MossyRocks;
    public GameObject[] FallenTrunks;
    public GameObject[] FallenBranches;
    public GameObject[] Mushrooms;
    public Light Sun1;
    public Light Sun2;
    public Light Sun3;
    public Light Sun4;
    public Light Sun5;
    public Light Sun6;
    public Light Sun;
    public float Sun1Int;
    public Color S1AmbSky; //= new Color(0.2313726f, 0.3019608f, 0.3764706f);
    public Color S1AmbEq; //= new Color(0.1086686f, 0.1373753f, 0.2075472f);
    public Color S1AmbGr; //= new Color(0.1411765f, 0.1215686f, 0.09803922f);
    public Color Fog1color; // = new Color(0.5019608f, 0.6235294f, 0.6196079f);
    public float Fog1Dens;
    public float Sun2Int;
    public Color S2AmbSky; //= new Color(0.5215687f, 0.3803922f, 0.2039216f);
    public Color S2AmbEq; //= new Color(0.1568628f, 0.09803922f, 0.172549f);
    public Color S2AmbGr; //= new Color(0.3882353f, 0.2509804f, 0.2156863f);
    public Color Fog2color; // = new Color(0.8901961f, 0.8352941f, 0.7098039f);
    public float Fog2Dens;
    public float Sun3Int;
    public Color S3AmbSky; //= new Color(0.7411765f, 0.7450981f, 0.7450981f);
    public Color S3AmbEq; //= new Color(0.172549f, 0.1921569f, 0.2352941f);
    public Color S3AmbGr; //= new Color(0.2156863f, 0.3137255f, 0.3882353f);
    public Color Fog3color; // = new Color(0.8f, 0.8705882f, 0.8862745f);
    public float Fog3Dens;

	void Start () {
        // only keep the "TerrainSummer" active at start of play
        Setting1();
    }

    // Update is called once per frame
    void Update () {
        // if "1" key pressed 
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Setting1();
        }
        // if "2" key pressed
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Setting2();
        }
        // if "3" key pressed
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Setting3();
        }
        // if "F1" key pressed
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Sun1.enabled = true;
            Sun2.enabled = false;
            Sun3.enabled = false;
            Sun4.enabled = false;
            Sun5.enabled = false;
            Sun6.enabled = false;
            Sun = Sun1;
        }
        // if "F2" key pressed
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Sun1.enabled = false;
            Sun2.enabled = true;
            Sun3.enabled = false;
            Sun4.enabled = false;
            Sun5.enabled = false;
            Sun6.enabled = false;
            Sun = Sun2;
        }
        // if "F3" key pressed
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Sun1.enabled = false;
            Sun2.enabled = false;
            Sun3.enabled = true;
            Sun4.enabled = false;
            Sun5.enabled = false;
            Sun6.enabled = false;
            Sun = Sun3;
        }
        // if "F4" key pressed
        if (Input.GetKeyDown(KeyCode.F4))
        {
            Sun1.enabled = false;
            Sun2.enabled = false;
            Sun3.enabled = false;
            Sun4.enabled = true;
            Sun5.enabled = false;
            Sun6.enabled = false;
            Sun = Sun4;
        }
        // if "F5" key pressed
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Sun1.enabled = false;
            Sun2.enabled = false;
            Sun3.enabled = false;
            Sun4.enabled = false;
            Sun5.enabled = true;
            Sun6.enabled = false;
            Sun = Sun5;
        }
        // if "F6" key pressed
        if (Input.GetKeyDown(KeyCode.F6))
        {
            Sun1.enabled = false;
            Sun2.enabled = false;
            Sun3.enabled = false;
            Sun4.enabled = false;
            Sun5.enabled = false;
            Sun6.enabled = true;
            Sun = Sun6;
        }
    }

    void Setting1()
    {
        // activate "TerrainSummer" and deactive the other terrains
        TerrainSummer.SetActive(true);
        TerrainAutumn.SetActive(false);
        TerrainWinter.SetActive(false);
        Leaves.SetActive(false);
        Particles.SetActive(true);
        MossyRocks[0].SetActive(true);
        MossyRocks[1].SetActive(false);
        FallenTrunks[0].SetActive(true);
        FallenTrunks[1].SetActive(false);
        FallenBranches[0].SetActive(true);
        FallenBranches[1].SetActive(false);
        Mushrooms[0].SetActive(true);
        Mushrooms[1].SetActive(false);
        Ivys.SetActive(true);
        Sun.intensity = Sun1Int;
        // set the ambient lights
        RenderSettings.ambientSkyColor = S1AmbSky;
        RenderSettings.ambientEquatorColor = S1AmbEq;
        RenderSettings.ambientGroundColor = S1AmbGr;
        // set the fog density down;
        RenderSettings.fogDensity = Fog1Dens;
        RenderSettings.fogColor = Fog1color;
    }
    void Setting2()
    {
        // activate "TerrainAutumn" and deactive the other terrains
        TerrainAutumn.SetActive(true);
        TerrainSummer.SetActive(false);
        TerrainWinter.SetActive(false);
        Leaves.SetActive(true);
        Particles.SetActive(true);
        MossyRocks[0].SetActive(true);
        MossyRocks[1].SetActive(false);
        FallenTrunks[0].SetActive(true);
        FallenTrunks[1].SetActive(false);
        FallenBranches[0].SetActive(true);
        FallenBranches[1].SetActive(false);
        Mushrooms[0].SetActive(true);
        Mushrooms[1].SetActive(false);
        Ivys.SetActive(true);
        Sun.intensity = Sun2Int;
        // set the ambient lights
        RenderSettings.ambientSkyColor = S2AmbSky;
        RenderSettings.ambientEquatorColor = S2AmbEq;
        RenderSettings.ambientGroundColor = S2AmbGr;
        // set the fog density & color
        RenderSettings.fogDensity = Fog2Dens;
        RenderSettings.fogColor = Fog2color;
    }
    void Setting3()
    {
        // activate "TerrainWinter" and deactive the other terrains
        TerrainWinter.SetActive(true);
        TerrainSummer.SetActive(false);
        TerrainAutumn.SetActive(false);
        Leaves.SetActive(false);
        Particles.SetActive(false);
        MossyRocks[1].SetActive(true);
        MossyRocks[0].SetActive(false);
        FallenTrunks[1].SetActive(true);
        FallenTrunks[0].SetActive(false);
        FallenBranches[1].SetActive(true);
        FallenBranches[0].SetActive(false);
        Mushrooms[1].SetActive(true);
        Mushrooms[0].SetActive(false);
        Ivys.SetActive(false);
        Sun.intensity = Sun3Int;
        // set the ambient lights
        RenderSettings.ambientSkyColor = S3AmbSky;
        RenderSettings.ambientEquatorColor = S3AmbEq;
        RenderSettings.ambientGroundColor = S3AmbGr;
        // set the fog density & color
        RenderSettings.fogDensity = Fog3Dens;
        RenderSettings.fogColor = Fog3color;
    }
}
