using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingHelper : MonoBehaviour
{
    public List<Mood> moods;
    public List<CustomLight> lights;

    public static LightingHelper Instance;

    private Vector4[] ambientColors;
    private Vector4[] specularColors;
    private float[] glossinessValues;
    private Vector4[] fogColors;

    public RoomManager roomManager;
    
    public Camera fogCam;
    public Shader fogMapShader;
    private RenderTexture _fogTexture;

    public bool randomizeMoods;
    
	private const float lightOffsetFromCeilingAmount = -0.1f;
    
    //#mood hook to auto-generate enum
	public enum Moods
	{
		Default,
		Misty,
		Emblazen,
		Darkness,
		Cozy,
		Flourescent,
	}
	//#endmood
	
    //#light hook to auto-generate enum
	public enum Lights
	{
		Spot,
		Flourescent,
		Recessed,
	}
	//#endlight

	public enum LightPositions
	{
		Center,
		North,
		South,
		East,
		West
	}
    
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
        for (int i = 0; i < moods.Count && i < 32; i++)
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

    // This is only meant to be called at runtime when a mood changes in the inspector
    // it's a fairly expensive operation since it resets all of the lighting data for every room
    // note that it cannot actually change the mood associated with a room
    // This is meant for easier iteration in the editor
    public void UpdateLightingDataFromInspectorGuiChange()
    {
	    SetBasicUniforms();
	    
	    // reposition lights
	    foreach (Room room in roomManager.GetRoomList())
	    {
		    SpawnPointLights(room, room.mood);
	    }
	    
	    // respawn particles
	    foreach (Room room in roomManager.GetRoomList())
	    {
		    SpawnParticles(room, room.mood);
	    }
    }

    public void SpawnPointLights(Room room, Mood mood)
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
		
	    float gridSize = Mathf.Sqrt(1f / mood.lightSourceDensity);
	    int numCols = Mathf.FloorToInt(room.size.x / gridSize);
	    int numRows = Mathf.FloorToInt(room.size.y / gridSize);
	    
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

	    Material emissionMat = new Material(Shader.Find("Standard"));
	    emissionMat.EnableKeyword("_EMISSION");
	    emissionMat.SetColor("_EmissionColor", mood.lightColor);

	    foreach (Vector3 pos in room.lightPosTemp)
	    {
		    GameObject pointLight = new GameObject("Point Light");
		    pointLight.transform.position = pos + Vector3.down * lightOffsetFromCeilingAmount;
		    pointLight.transform.SetParent(room.Ceiling.transform);
		    
		    Light light = pointLight.AddComponent<Light>();
		    light.type = LightType.Point;
		    Color col = mood.lightColor;
		    col.a = room.roomIndex / 255f;
		    light.color = col;
		    light.range = mood.lightRadius;
		    light.shadows = LightShadows.None;
		    
		    //mesh
		    if (mood.lightFixture != null)
		    {
				GameObject fixture = Instantiate(mood.lightFixture, pointLight.transform);
				fixture.transform.position = pointLight.transform.position + Vector3.up * lightOffsetFromCeilingAmount;
				MeshRenderer renderer = fixture.GetComponent<MeshRenderer>();
				Material[] mats = renderer.materials;
				mats[^1] = emissionMat;
				renderer.materials = mats;
		    }
	    }
    }

    public void AddLight(Room room, Lights lightType, LightPositions position)
    {
	    CustomLight lightData = lights[(int)lightType];
	    
	    Vector3 pos = room.centerOfMass;

	    switch (position)
	    {
		    case LightPositions.North:
			    pos = room.centerOfMass + room.zAxis * room.size.y * 0.25f;
			    break;
		    case LightPositions.East:
			    pos = room.centerOfMass + room.xAxis * room.size.x * 0.25f;
			    break;
		    case LightPositions.South:
			    pos = room.centerOfMass - room.zAxis * room.size.y * 0.25f;
			    break;
		    case LightPositions.West:
			    pos = room.centerOfMass - room.xAxis * room.size.x * 0.25f;
			    break;
	    }
	    
		GameObject customLight = new GameObject("Custom light");
		customLight.transform.position = pos + Vector3.down * lightOffsetFromCeilingAmount;
		customLight.transform.SetParent(room.Ceiling.transform);
		customLight.transform.forward = Vector3.down;
		
		Light light = customLight.AddComponent<Light>();
		light.type = lightData.type;
		switch (light.type)
		{
			case LightType.Spot:
				light.spotAngle = lightData.spotAngle;
				break;
		}
		Color col = lightData.color;
		col.a = room.roomIndex / 255f;
		light.color = col;
		light.range = lightData.range;
		light.shadows = LightShadows.None;
		
		//mesh
		if (lightData.fixture != null)
		{
			Material emissionMat = new Material(Shader.Find("Standard"));
			emissionMat.EnableKeyword("_EMISSION");
			emissionMat.SetColor("_EmissionColor", lightData.color);
	    
			GameObject fixture = Instantiate(lightData.fixture, customLight.transform);
			fixture.transform.position = customLight.transform.position + Vector3.up * lightOffsetFromCeilingAmount;
			fixture.transform.rotation = Quaternion.identity;
			MeshRenderer renderer = fixture.GetComponent<MeshRenderer>();
			Material[] mats = renderer.materials;
			mats[^1] = emissionMat;
			renderer.materials = mats;
		}
    }

    public void UpdateFog()
    {
	    fogCam.Render();
    }

    public void SpawnParticles(Room room, Mood mood)
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

		GameObject particlePrefab = mood.particlePrefab;
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
			Debug.LogError("Please assign particle system to particlePrefab in Mood: " + mood.name);
			Destroy(particleSystem);
			return;
		}
		
		ParticleSystem.ShapeModule shape = ps.shape;
		Vector3 boxSize = new Vector3(room.size.x, roomHeightGuess, room.size.y);
		shape.scale = boxSize;
		
		ParticleSystem.EmissionModule emission = ps.emission;
		float roomVolume = Mathf.Abs(boxSize.x * boxSize.y * boxSize.z);
		float emissionRate = roomVolume * mood.particleDensity;
		emission.rateOverTime = emissionRate;
    }

    public Mood CreateMood(Color specColor, float smoothness, Color ambientColor, float lightSourceDensity, float lightRadius,
	    Color lightColor, Color fogColor, GameObject particlePrefab, float particleDensity)
    {
	    if(moods.Count >= 32)
	    {
		    Debug.LogError("Cannot create more than 32 moods");
		    return null;
	    }
	    
	    Mood newMood = ScriptableObject.CreateInstance<Mood>();
	    
	    newMood.Setup(specColor, smoothness, ambientColor, lightSourceDensity, lightRadius, lightColor,
		    fogColor, particlePrefab, particleDensity);

	    moods.Add(newMood);
	    
	    SetBasicUniforms();
	    
	    return newMood;
    }

    public int GetMoodIndex(Mood mood)
    {
	    for(int i = 0; i < moods.Count; i++)
	    {
		    if (moods[i] == mood)
		    {
			    return i;
		    }
	    }

	    Debug.LogError("Error - mood not found in moods list");
	    return -1;
    }

    public Mood GetMood(Moods mood)
    {
	    return moods[(int)mood];
    }
}
