using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

using Category = FarseerPhysics.Dynamics.Category;

[System.Serializable]
public class FSCategorySettings
{
	public string CatAll = "All";
	public string CatNone = "None";
	public string[] Cat131;
	
	public FSCategorySettings()
	{
		Cat131 = new string[31];
		for(int i = 0; i < Cat131.Length; i++)
		{
			Cat131[i] = "Cat"+(i+1).ToString();
		}
	}
}

[System.Serializable]
public class FSCoreSettings
{
	// Common
	
	//GABS
	public bool FixedUpdate = true;
	
	/// <summary>
    /// Enabling diagnostics causes the engine to gather timing information.
    /// You can see how much time it took to solve the contacts, solve CCD
    /// and update the controllers.
    /// NOTE: If you are using a debug view that shows performance counters,
    /// you might want to enable this.
    /// </summary>
    public bool EnableDiagnostics = true;
	
	/// <summary>
    /// The number of velocity iterations used in the solver.
    /// </summary>
    public int VelocityIterations = 8;

    /// <summary>
    /// The number of position iterations used in the solver.
    /// </summary>
    public int PositionIterations = 3;

    /// <summary>
    /// Enable/Disable Continuous Collision Detection (CCD)
    /// </summary>
    public bool ContinuousPhysics = true;

    /// <summary>
    /// The number of velocity iterations in the TOI solver
    /// </summary>
    public int TOIVelocityIterations = 8;

    /// <summary>
    /// The number of position iterations in the TOI solver
    /// </summary>
    public int TOIPositionIterations = 20;

    /// <summary>
    /// Maximum number of sub-steps per contact in continuous physics simulation.
    /// </summary>
	public int MaxSubSteps = 8;

    /// <summary>
    /// Enable/Disable warmstarting
    /// </summary>
    public bool EnableWarmstarting = true;
	
	/// <summary>
    /// Enable/Disable sleeping
    /// </summary>
    public bool AllowSleep = true;

    /// <summary>
    /// The maximum number of vertices on a convex polygon.
    /// </summary>
    public int MaxPolygonVertices = 8;

    /// <summary>
    /// Farseer Physics Engine has a different way of filtering fixtures than Box2d.
    /// We have both FPE and Box2D filtering in the engine. If you are upgrading
    /// from earlier versions of FPE, set this to true and DefaultFixtureCollisionCategories
	/// to Category.All.
    /// </summary>
    public bool UseFPECollisionCategories = true;

	/// <summary>
	/// This is used by the Fixture constructor as the default value 
	/// for Fixture.CollisionCategories member. Note that you may need to change this depending
	/// on the setting of UseFPECollisionCategories, above.
	/// </summary>
	public Category DefaultFixtureCollisionCategories = Category.Cat1;

	/// <summary>
	/// This is used by the Fixture constructor as the default value 
	/// for Fixture.CollidesWith member.
	/// </summary>
	public Category DefaultFixtureCollidesWith = Category.All;


	/// <summary>
	/// This is used by the Fixture constructor as the default value 
	/// for Fixture.IgnoreCCDWith member.
	/// </summary>
	public Category DefaultFixtureIgnoreCCDWith = Category.None;

    /// <summary>
    /// Conserve memory makes sure that objects are used by reference instead of cloned.
    /// When you give a vertices collection to a PolygonShape, it will by default copy the vertices
    /// instead of using the original reference. This is to ensure that objects modified outside the engine
    /// does not affect the engine itself, however, this uses extra memory. This behavior
    /// can be turned off by setting ConserveMemory to true.
    /// </summary>
    public bool ConserveMemory = false;

    /// <summary>
    /// The maximum number of contact points between two convex shapes.
    /// </summary>
    public int MaxManifoldPoints = 2;

    /// <summary>
    /// This is used to fatten AABBs in the dynamic tree. This allows proxies
    /// to move by a small amount without triggering a tree adjustment.
    /// This is in meters.
    /// </summary>
    public float AABBExtension = 0.1f;

    /// <summary>
    /// This is used to fatten AABBs in the dynamic tree. This is used to predict
    /// the future position based on the current displacement.
    /// This is a dimensionless multiplier.
    /// </summary>
    public float AABBMultiplier = 2.0f;

    /// <summary>
    /// A small length used as a collision and constraint tolerance. Usually it is
    /// chosen to be numerically significant, but visually insignificant.
    /// </summary>
    public float LinearSlop = 0.005f;

    /// <summary>
    /// A small angle used as a collision and constraint tolerance. Usually it is
    /// chosen to be numerically significant, but visually insignificant.
    /// </summary>
    public float AngularSlop = (2.0f / 180.0f * Mathf.PI);

    /// <summary>
    /// The radius of the polygon/edge shape skin. This should not be modified. Making
    /// this smaller means polygons will have an insufficient buffer for continuous collision.
    /// Making it larger may create artifacts for vertex collision.
    /// </summary>
    public float PolygonRadius = 2.0f * 0.005f;//(2.0f * LinearSlop);
	
	// Dynamics

    /// <summary>
    /// Maximum number of contacts to be handled to solve a TOI impact.
    /// </summary>
    public int MaxTOIContacts = 32;

    /// <summary>
    /// A velocity threshold for elastic collisions. Any collision with a relative linear
    /// velocity below this threshold will be treated as inelastic.
    /// </summary>
    public float VelocityThreshold = 1.0f;

    /// <summary>
    /// The maximum linear position correction used when solving constraints. This helps to
    /// prevent overshoot.
    /// </summary>
    public float MaxLinearCorrection = 0.2f;

    /// <summary>
    /// The maximum angular position correction used when solving constraints. This helps to
    /// prevent overshoot.
    /// </summary>
    public float MaxAngularCorrection = (8.0f / 180.0f * Mathf.PI);

    /// <summary>
    /// This scale factor controls how fast overlap is resolved. Ideally this would be 1 so
    /// that overlap is removed in one time step. However using values close to 1 often lead
    /// to overshoot.
    /// </summary>
    public float Baumgarte = 0.2f;
    public float TOIBaumgarte = 0.75f;
	
	// Sleep

    /// <summary>
    /// The time that a body must be still before it will go to sleep.
    /// </summary>
    public float TimeToSleep = 0.5f;

    /// <summary>
    /// A body cannot sleep if its linear velocity is above this tolerance.
    /// </summary>
    public float LinearSleepTolerance = 0.01f;

    /// <summary>
    /// A body cannot sleep if its angular velocity is above this tolerance.
    /// </summary>
    public float AngularSleepTolerance = (2.0f / 180.0f * Mathf.PI);

    /// <summary>
    /// The maximum linear velocity of a body. This limit is very large and is used
    /// to prevent numerical problems. You shouldn't need to adjust this.
    /// </summary>
    public float MaxTranslation = 2.0f;
    //public float MaxTranslationSquared = (MaxTranslation * MaxTranslation);

    /// <summary>
    /// The maximum angular velocity of a body. This limit is very large and is used
    /// to prevent numerical problems. You shouldn't need to adjust this.
    /// </summary>
    public float MaxRotation = (0.5f * Mathf.PI);
    //public float MaxRotationSquared = (MaxRotation * MaxRotation);
	
	// @gabs 
	public bool EdShowCommon = true;
	public bool EdShowDynamics = true;
	public bool EdShowSleep = true;
}

public static class FSSettings
{
	private static FSCoreSettings coreSettings;
	private static FSCategorySettings categorySettings;
	
	private static FSSettingsPlatform lastPlatform = FSSettingsPlatform.Default;
	
	public static FSSettingsPlatform LastLoadedPlatform
	{
		get
		{
			return lastPlatform;
		}
	}
	
	public static void Load()
	{
		Load(FSSettingsPlatform.Default);
	}
	
	public static void Load(FSSettingsPlatform platform)
	{
		// path setup
		string path = Application.dataPath + "/FarseerUnity/Editor/SerializedSettings";
		//Debug.Log("PATH: " +path);
		if(!Directory.Exists(path))
			Directory.CreateDirectory(path);
		
		//setup vars
		FileStream fs;
		XmlSerializer xmls;
		
		//Debug.Log("PATH2: " +path + "/FSCoreSettings_" + platform.ToString() + ".cfg");
		//FSCoreSettings
		if(File.Exists(path + "/FSCoreSettings_" + platform.ToString() + ".cfg"))
		{
			//Debug.Log("EXISTS");
			xmls = new XmlSerializer(typeof(FSCoreSettings));
			fs = new FileStream(path + "/FSCoreSettings_" + platform.ToString() + ".cfg", FileMode.Open);
			coreSettings = xmls.Deserialize(fs) as FSCoreSettings;
			//Debug.Log(coreSettings);
			fs.Close();
		}
		else
		{
			coreSettings = new FSCoreSettings();
		}
		
		//FSCategorySettings
		if(File.Exists(path + "/FSCategorySettings.cfg"))
		{
			xmls = new XmlSerializer(typeof(FSCategorySettings));
			fs = new FileStream(path + "/FSCategorySettings.cfg", FileMode.Open);
			categorySettings = xmls.Deserialize(fs) as FSCategorySettings;
			fs.Close();
		}
		else
		{
			categorySettings = new FSCategorySettings();
		}
		
		lastPlatform = platform;
	}
	
	public static void Save()
	{
		Save (lastPlatform);
	}
	
	public static void Save(FSSettingsPlatform platform)
	{
		// path setup
		string path = Application.dataPath + "/FarseerUnity/Editor/SerializedSettings";
		if(!Directory.Exists(path))
			Directory.CreateDirectory(path);
		
		//setup vars
		FileStream fs;
		XmlSerializer xmls;
		StreamWriter sw;
		
		//FSCoreSettings
		xmls = new XmlSerializer(typeof(FSCoreSettings));
		if(File.Exists(path + "/FSCoreSettings_" + platform.ToString() + ".cfg"))
			fs = File.Open(path + "/FSCoreSettings_" + platform.ToString() + ".cfg", FileMode.Truncate);
		else
			fs = File.Create(path + "/FSCoreSettings_" + platform.ToString() + ".cfg");
		sw = new StreamWriter(fs);
		xmls.Serialize(sw, coreSettings);
		sw.Close();
		
		//FSCategorySettings
		xmls = new XmlSerializer(typeof(FSCategorySettings));
		if(File.Exists(path + "/FSCategorySettings.cfg"))
			fs = File.Open(path + "/FSCategorySettings.cfg", FileMode.Truncate);
		else
			fs = File.Create(path + "/FSCategorySettings.cfg");
		sw = new StreamWriter(fs);
		xmls.Serialize(sw, categorySettings);
		sw.Close();
	}
	
	//
	
	public static FSCoreSettings CoreSettings
	{
		get
		{
			if(coreSettings == null)
				Load();
			return coreSettings;
		}
		set
		{
			coreSettings = value;
			//Save();
		}
	}
	
	public static FSCategorySettings CategorySettings
	{
		get
		{
			if(categorySettings == null)
				Load();
			return categorySettings;
		}
		set
		{
			categorySettings = value;
			//Save();
		}
	}
}
public enum FSSettingsPlatform
{
	Default,
	Desktop,
	Web,
	iOS,
	Android
}