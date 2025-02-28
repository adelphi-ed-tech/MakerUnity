using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingHelper : MonoBehaviour
{
    public Mood[] moods;

    public static LightingHelper Instance;

    private Vector4[] ambientColors;
    private Vector4[] specularColors;
    private float[] glossinessValues;
    private Vector4[] fogColors;

    public RoomManager roomManager;
    
    public Camera fogCam;
    public Shader fogMapShader;
    private RenderTexture _fogTexture;
    
    //#hook to auto-generate enum
	public enum Moods
	{
		Default,
		Misty,
	}
	//#endhook
    
    void Awake()
    {
        Instance = this;
        
        // Initialize ambient colors
        ambientColors = new Vector4[32];
        for (int i = 0; i < 32; i++)
        {
	        ambientColors[i] = Vector4.zero;
        }
        
        //initialize specular colors
        specularColors = new Vector4[32];
        for (int i = 0; i < 32; i++)
        {
	        specularColors[i] = Vector4.zero;
        }
        
        //initialize glossiness values
        glossinessValues = new float[32];
        for (int i = 0; i < 32; i++)
        {
	        glossinessValues[i] = 0.5f;
        }
        
        //initialize fog colors
        fogColors = new Vector4[32];
        for (int i = 0; i < 32; i++)
		{
	        fogColors[i] = Vector4.zero;
		}
        
        SetBasicUniforms();
        
        
        // init fog
        _fogTexture = fogCam.targetTexture;
        Shader.SetGlobalTexture("_FogMap", _fogTexture);
        fogCam.SetReplacementShader(fogMapShader, "RenderType");
        Vector4 fogCamBounds = new Vector4(fogCam.transform.position.x - fogCam.orthographicSize,
	        fogCam.transform.position.z - fogCam.orthographicSize, fogCam.orthographicSize * 2, fogCam.orthographicSize * 2);
        Shader.SetGlobalVector("_FogBounds", fogCamBounds);
    }

    Vector4 ColorToVec4(Color c)
    {
	    return new Vector4(c.r, c.g, c.b, c.a);
    }

    void SetBasicUniforms()
    {
        for (int i = 0; i < moods.Length && i < 32; i++)
        {
	        ambientColors[i] = ColorToVec4(moods[i].ambientColor);
	        specularColors[i] = ColorToVec4(moods[i].specularColor);
	        glossinessValues[i] = moods[i].smoothness;
	        fogColors[i] = ColorToVec4(moods[i].fogColor);
        }
        
        Shader.SetGlobalVectorArray("_AmbientColors", ambientColors);
        Shader.SetGlobalVectorArray("_SpecularColors", specularColors);
        Shader.SetGlobalFloatArray("_GlossinessValues", glossinessValues);
        Shader.SetGlobalVectorArray("_FogColors", fogColors);
    }

    public void UpdateLightingData()
    {
	    SetBasicUniforms();
	    
	    // reposition lights
	    foreach (Room room in roomManager.GetRoomList())
	    {
		    SpawnPointLights(room, (Moods)room.moodIndex);
	    }
	    
	    // respawn particles
	    foreach (Room room in roomManager.GetRoomList())
	    {
		    SpawnParticles(room, (Moods)room.moodIndex);
	    }
    }

    public void SpawnPointLights(Room room, Moods mood)
    {
	    // clear old lights
		// might be more efficient to pool, we shall see...
		room.lightPosTemp.Clear();
		for (int i = room.Ceiling.transform.childCount - 1; i >= 0; i--)
		{
			GameObject child = room.Ceiling.transform.GetChild(i).gameObject;
			if (child.GetComponent<Light>() != null)
			{
				GameObject.Destroy(room.Ceiling.transform.GetChild(i).gameObject);
			}
		}
		
	    Mood myMood = moods[(int)mood];
	    
	    float gridSize = Mathf.Sqrt(1f / myMood.lightSourceDensity);
	    int numCols = Mathf.FloorToInt(room.size.x / gridSize);
	    int numRows = Mathf.FloorToInt(room.size.y / gridSize);
	    
	    //add a couple rows and columns as a buffer of sorts to help with non rectangular rooms
	    //numCols += numCols;
	    //numRows += numRows;
	    
	    Vector3 gridStart = room.centerOfMass;
	    gridStart -= room.xAxis * (((numCols - 1) / 2f) * gridSize);
	    gridStart -= room.zAxis * (((numRows - 1) / 2f) * gridSize);
	    for (int y = 0; y < numRows; y++)
	    {
		    for (int x = 0; x < numCols; x++)
		    {
			    Vector3 pos = gridStart + room.xAxis * (x * gridSize) + room.zAxis * (y * gridSize);
			    room.lightPosTemp.Add(pos);
		    }
	    }

	    foreach (Vector3 pos in room.lightPosTemp)
	    {
		    GameObject pointLight = new GameObject("Point Light");
		    pointLight.transform.position = pos + Vector3.down * 0.5f;
		    pointLight.transform.SetParent(room.Ceiling.transform);
		    
		    Light light = pointLight.AddComponent<Light>();
		    light.type = LightType.Point;
		    Color col = myMood.lightColor;
		    col.a = room.roomIndex / 255f;
		    light.color = col;
		    light.range = myMood.lightRadius;
	    }
    }

    public void UpdateFog()
    {
	    fogCam.Render();
    }

    public void SpawnParticles(Room room, Moods mood)
    {
	    //remove previous particle system
		for (int i = room.Ceiling.transform.childCount - 1; i >= 0; i--)
		{
			GameObject child = room.Ceiling.transform.GetChild(i).gameObject;
			if (child.GetComponent<ParticleSystem>() != null)
			{
				GameObject.Destroy(room.Ceiling.transform.GetChild(i).gameObject);
			}
		}

	    Mood myMood = moods[(int)mood];
		GameObject particlePrefab = myMood.particlePrefab;
		if (particlePrefab == null)
		{
			return;
		}

		//room height is not specified. It could be calculated, but for now we can just guess / hardcode
		float roomHeightGuess = 3f;
		Vector3 min = room.origin;
		Vector3 max = room.origin + room.xAxis * room.size.x + room.zAxis * room.size.y;
		min.y = max.y - roomHeightGuess;
		Vector3 center = (min + max) / 2f;
		Quaternion rotation = Quaternion.LookRotation(room.zAxis, Vector3.up);

		GameObject particleSystem = Instantiate(particlePrefab, center, rotation, room.Ceiling.transform);
		ParticleSystem ps = particleSystem.GetComponent<ParticleSystem>();
		if (ps == null)
		{
			Debug.LogError("Please assign particle system to particlePrefab in Mood: " + myMood.name);
			Destroy(particleSystem);
			return;
		}
		
		ParticleSystem.ShapeModule shape = ps.shape;
		Vector3 boxSize = new Vector3(room.size.x, roomHeightGuess, room.size.y);
		shape.scale = boxSize;
		
		ParticleSystem.EmissionModule emission = ps.emission;
		float roomVolume = Mathf.Abs(boxSize.x * boxSize.y * boxSize.z);
		float emissionRate = roomVolume * myMood.particleDensity;
		emission.rateOverTime = emissionRate;
    }
}
