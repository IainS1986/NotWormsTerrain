using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using Category = FarseerPhysics.Dynamics.Category;

public class FSProjectSettingsWindow : EditorWindow
{
	private static FSProjectSettingsWindow window = null;
	
	private Vector2 scrollPos = Vector2.zero;
	
	private bool showFSCategorySettings = false;
	private FSCoreSettings loadedFSCoreSettings;
	private FSCategorySettings loadedFSCategorySettings;
	
	private Color c0, c1, c2;
	private bool switchs;
	
	[MenuItem("Edit/Project Settings/FarseerUnity")]
	public static FSProjectSettingsWindow OpenWindow()
	{
		if(window != null)
		{
			window.Close();
			window = null;
		}
		window = CreateInstance<FSProjectSettingsWindow>();
		window.Setup();
		window.Show();
		return window;
	}
	
	public void Setup()
	{
		FSSettings.Load();
		loadedFSCoreSettings = FSSettings.CoreSettings;
		loadedFSCategorySettings = FSSettings.CategorySettings;
	}
	
	private void OnGUI()
	{
		// HARDWARE BT HERE
		EditorGUILayout.BeginHorizontal();
		
		switchs = false;
		if(FSSettings.LastLoadedPlatform == FSSettingsPlatform.Default)
		{
			SetActiveColor();
			GUILayout.RepeatButton("Default");
			ResetActiveColor();
		}
		else
		{
			switchs = GUILayout.RepeatButton("Default");
			if(switchs)
				SwitchSettings(FSSettingsPlatform.Default);
		}
		if(FSSettings.LastLoadedPlatform == FSSettingsPlatform.Desktop)
		{
			SetActiveColor();
			GUILayout.RepeatButton("Desktop");
			ResetActiveColor();
		}
		else
		{
			switchs = GUILayout.RepeatButton("Desktop");
			if(switchs)
				SwitchSettings(FSSettingsPlatform.Desktop);
		}
		if(FSSettings.LastLoadedPlatform == FSSettingsPlatform.Web)
		{
			SetActiveColor();
			GUILayout.RepeatButton("Web");
			ResetActiveColor();
		}
		else
		{
			switchs = GUILayout.RepeatButton("Web");
			if(switchs)
				SwitchSettings(FSSettingsPlatform.Web);
		}
		if(FSSettings.LastLoadedPlatform == FSSettingsPlatform.iOS)
		{
			SetActiveColor();
			GUILayout.RepeatButton("iOS");
			ResetActiveColor();
		}
		else
		{
			switchs = GUILayout.RepeatButton("iOS");
			if(switchs)
				SwitchSettings(FSSettingsPlatform.iOS);
		}
		if(FSSettings.LastLoadedPlatform == FSSettingsPlatform.Android)
		{
			SetActiveColor();
			GUILayout.RepeatButton("Android");
			ResetActiveColor();
		}
		else
		{
			switchs = GUILayout.RepeatButton("Android");
			if(switchs)
				SwitchSettings(FSSettingsPlatform.Android);
		}
		
		EditorGUILayout.EndHorizontal();
		
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		EditorGUILayout.BeginVertical();
		
		GUI_FSCoreSettings();
		showFSCategorySettings = EditorGUILayout.Foldout(showFSCategorySettings, "Collision Categories");
		if(showFSCategorySettings)
			GUI_FSCategorySettings();
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndScrollView();
	}
	
	private void SwitchSettings(FSSettingsPlatform platform)
	{
		GUI.FocusWindow(-1);
		FSSettings.CoreSettings = loadedFSCoreSettings;
		FSSettings.Save();
		FSSettings.Load(platform);
		loadedFSCoreSettings = FSSettings.CoreSettings;
		switchs = false;
	}
	
	private void SetActiveColor()
	{
		c0 = GUI.backgroundColor;
		c1 = GUI.contentColor;
		c2 = GUI.color;
		
		GUI.color = new Color(1f, 1f, 1f, 1f);
		GUI.backgroundColor = new Color(1f, 0f, 0f, 1f);
	}
	private void ResetActiveColor()
	{
		GUI.backgroundColor = c0;
		GUI.contentColor = c1;
		GUI.color = c2;
	}
	
	
	private void GUI_FSCoreSettings()
	{
		// Common
		loadedFSCoreSettings.EdShowCommon = EditorGUILayout.Foldout(loadedFSCoreSettings.EdShowCommon, "Common");
		if(loadedFSCoreSettings.EdShowCommon)
		{
			EditorGUILayout.BeginVertical(GUI.skin.box);
			//FixedUpdate
			loadedFSCoreSettings.FixedUpdate = EditorGUILayout.Toggle("Run with FixedUpdate", loadedFSCoreSettings.FixedUpdate);
			EditorGUILayout.HelpBox("Running with FixedUpdate ensures a fixed deltaTime (it's what Physx does).", MessageType.Info);
			EditorGUILayout.Separator();
			//EnableDiagnostics
			loadedFSCoreSettings.EnableDiagnostics = EditorGUILayout.Toggle("Enable Diagnostics", loadedFSCoreSettings.EnableDiagnostics);
			EditorGUILayout.HelpBox("Enabling diagnostics causes the engine to gather timing information. You can see how much time it took to solve the contacts, solve CCD and update the controllers. NOTE: If you are using a debug view that shows performance counters, you might want to enable this.", MessageType.Info);
			EditorGUILayout.Separator();
			//VelocityIterations
			loadedFSCoreSettings.VelocityIterations = EditorGUILayout.IntField("Velocity Iterations", loadedFSCoreSettings.VelocityIterations);
			EditorGUILayout.HelpBox("The number of velocity iterations used in the solver.", MessageType.Info);
			EditorGUILayout.Separator();
			//PositionIterations
			loadedFSCoreSettings.PositionIterations = EditorGUILayout.IntField("Position Iterations", loadedFSCoreSettings.PositionIterations);
			EditorGUILayout.HelpBox("The number of position iterations used in the solver.", MessageType.Info);
			EditorGUILayout.Separator();
			//ContinuousPhysics
			loadedFSCoreSettings.ContinuousPhysics = EditorGUILayout.Toggle("Continuous Physics", loadedFSCoreSettings.ContinuousPhysics);
			EditorGUILayout.HelpBox("Enable/Disable Continuous Collision Detection (CCD)", MessageType.Info);
			EditorGUILayout.Separator();
			//TOIVelocityIterations
			loadedFSCoreSettings.TOIVelocityIterations = EditorGUILayout.IntField("T.O.I. Velocity Iterations", loadedFSCoreSettings.TOIVelocityIterations);
			EditorGUILayout.HelpBox("The number of velocity iterations in the T.O.I. solver", MessageType.Info);
			EditorGUILayout.Separator();
			//TOIPositionIterations
			loadedFSCoreSettings.TOIPositionIterations = EditorGUILayout.IntField("T.O.I. Position Iterations", loadedFSCoreSettings.TOIPositionIterations);
			EditorGUILayout.HelpBox("The number of position iterations in the T.O.I. solver", MessageType.Info);
			EditorGUILayout.Separator();
			//MaxSubSteps
			loadedFSCoreSettings.MaxSubSteps = EditorGUILayout.IntField("Max SubSteps", loadedFSCoreSettings.MaxSubSteps);
			EditorGUILayout.HelpBox("Maximum number of sub-steps per contact in continuous physics simulation.", MessageType.Info);
			EditorGUILayout.Separator();
			//EnableWarmstarting
			loadedFSCoreSettings.EnableWarmstarting = EditorGUILayout.Toggle("Enable Warmstarting", loadedFSCoreSettings.EnableWarmstarting);
			//AllowSleep
			loadedFSCoreSettings.AllowSleep = EditorGUILayout.Toggle("Allow Sleep", loadedFSCoreSettings.AllowSleep);
			//MaxPolygonVertices
			loadedFSCoreSettings.MaxPolygonVertices = EditorGUILayout.IntField("Max Polygon Vertices", loadedFSCoreSettings.MaxPolygonVertices);
			EditorGUILayout.HelpBox("The maximum number of vertices on a convex polygon.", MessageType.Info);
			EditorGUILayout.Separator();
			//UseFPECollisionCategories
			loadedFSCoreSettings.UseFPECollisionCategories = EditorGUILayout.Toggle("Use F.P.E. Collision Categories", loadedFSCoreSettings.UseFPECollisionCategories);
			EditorGUILayout.HelpBox("Farseer Physics Engine has a different way of filtering fixtures than Box2d. We have both FPE and Box2D filtering in the engine. If you are upgrading from earlier versions of FPE, set this to true and DefaultFixtureCollisionCategories to Category.All.", MessageType.Info);
			EditorGUILayout.Separator();
			//DefaultFixtureCollidesWith
			loadedFSCoreSettings.DefaultFixtureCollidesWith = (Category)EditorGUILayout.EnumPopup("Default Fixture CollidesWith", loadedFSCoreSettings.DefaultFixtureCollidesWith);
			EditorGUILayout.HelpBox("This is used by the Fixture constructor as the default value for Fixture.CollidesWith member.", MessageType.Info);
			EditorGUILayout.Separator();
			//DefaultFixtureIgnoreCCDWith
			loadedFSCoreSettings.DefaultFixtureIgnoreCCDWith = (Category)EditorGUILayout.EnumPopup("Default Fixture IgnoreCCDWith", loadedFSCoreSettings.DefaultFixtureIgnoreCCDWith);
			EditorGUILayout.HelpBox("This is used by the Fixture constructor as the default value for Fixture.IgnoreCCDWith member.", MessageType.Info);
			EditorGUILayout.Separator();
			//ConserveMemory
			loadedFSCoreSettings.ConserveMemory = EditorGUILayout.Toggle("Conserve Memory", loadedFSCoreSettings.ConserveMemory);
			EditorGUILayout.HelpBox("Conserve memory makes sure that objects are used by reference instead of cloned. When you give a vertices collection to a PolygonShape, it will by default copy the vertices instead of using the original reference. This is to ensure that objects modified outside the engine does not affect the engine itself, however, this uses extra memory. This behavior can be turned off by setting ConserveMemory to true.", MessageType.Info);
			EditorGUILayout.Separator();
			//MaxManifoldPoints
			loadedFSCoreSettings.MaxManifoldPoints = EditorGUILayout.IntField("Max Manifold Points", loadedFSCoreSettings.MaxManifoldPoints);
			EditorGUILayout.HelpBox("The maximum number of contact points between two convex shapes.", MessageType.Info);
			EditorGUILayout.Separator();
			//AABBExtension
			loadedFSCoreSettings.AABBExtension = EditorGUILayout.FloatField("AABB Extension", loadedFSCoreSettings.AABBExtension);
			EditorGUILayout.HelpBox("This is used to fatten AABBs in the dynamic tree. This allows proxies to move by a small amount without triggering a tree adjustment. This is in meters.", MessageType.Info);
			EditorGUILayout.Separator();
			//AABBMultiplier
			loadedFSCoreSettings.AABBMultiplier = EditorGUILayout.FloatField("AABB Multiplier", loadedFSCoreSettings.AABBMultiplier);
			EditorGUILayout.HelpBox("This is used to fatten AABBs in the dynamic tree. This is used to predict the future position based on the current displacement. This is a dimensionless multiplier.", MessageType.Info);
			EditorGUILayout.Separator();
			//LinearSlop
			loadedFSCoreSettings.LinearSlop = EditorGUILayout.FloatField("Linear Slop", loadedFSCoreSettings.LinearSlop);
			EditorGUILayout.HelpBox("A small length used as a collision and constraint tolerance. Usually it is chosen to be numerically significant, but visually insignificant.", MessageType.Info);
			EditorGUILayout.Separator();
			//AngularSlop
			loadedFSCoreSettings.AngularSlop = EditorGUILayout.FloatField("Angular Slop", loadedFSCoreSettings.AngularSlop);
			EditorGUILayout.HelpBox("A small angle used as a collision and constraint tolerance. Usually it is chosen to be numerically significant, but visually insignificant.", MessageType.Info);
			EditorGUILayout.Separator();
			//PolygonRadius
			loadedFSCoreSettings.PolygonRadius = EditorGUILayout.FloatField("Polygon Radius", loadedFSCoreSettings.PolygonRadius);
			EditorGUILayout.HelpBox("The radius of the polygon/edge shape skin. This should not be modified. Making this smaller means polygons will have an insufficient buffer for continuous collision. Making it larger may create artifacts for vertex collision.", MessageType.Info);
			EditorGUILayout.Separator();
			EditorGUILayout.EndVertical();
		}
		// Dynamics
		loadedFSCoreSettings.EdShowDynamics = EditorGUILayout.Foldout(loadedFSCoreSettings.EdShowDynamics, "Dynamics");
		if(loadedFSCoreSettings.EdShowDynamics)
		{
			EditorGUILayout.BeginVertical(GUI.skin.box);
			//MaxTOIContacts
			loadedFSCoreSettings.MaxTOIContacts = EditorGUILayout.IntField("Max T.O.I. Contacts", loadedFSCoreSettings.MaxTOIContacts);
			EditorGUILayout.HelpBox("Maximum number of contacts to be handled to solve a TOI impact.", MessageType.Info);
			EditorGUILayout.Separator();
			//VelocityThreshold
			loadedFSCoreSettings.VelocityThreshold = EditorGUILayout.FloatField("Velocity Threshold", loadedFSCoreSettings.VelocityThreshold);
			EditorGUILayout.HelpBox("A velocity threshold for elastic collisions. Any collision with a relative linear velocity below this threshold will be treated as inelastic.", MessageType.Info);
			EditorGUILayout.Separator();
			//MaxLinearCorrection
			loadedFSCoreSettings.MaxLinearCorrection = EditorGUILayout.FloatField("Max Linear Correction", loadedFSCoreSettings.MaxLinearCorrection);
			EditorGUILayout.HelpBox("The maximum linear position correction used when solving constraints. This helps to prevent overshoot.", MessageType.Info);
			EditorGUILayout.Separator();
			//MaxAngularCorrection
			loadedFSCoreSettings.MaxAngularCorrection = EditorGUILayout.FloatField("Max Angular Correction", loadedFSCoreSettings.MaxAngularCorrection);
			EditorGUILayout.HelpBox("The maximum angular position correction used when solving constraints. This helps to prevent overshoot.", MessageType.Info);
			EditorGUILayout.Separator();
			//Baumgarte
			loadedFSCoreSettings.Baumgarte = EditorGUILayout.FloatField("Baumgarte", loadedFSCoreSettings.Baumgarte);
			loadedFSCoreSettings.TOIBaumgarte = EditorGUILayout.FloatField("T.O.I. Baumgarte", loadedFSCoreSettings.TOIBaumgarte);
			EditorGUILayout.HelpBox("This scale factor controls how fast overlap is resolved. Ideally this would be 1 so that overlap is removed in one time step. However using values close to 1 often lead to overshoot.", MessageType.Info);
			EditorGUILayout.Separator();
			EditorGUILayout.EndVertical();
		}
		// Sleep
		loadedFSCoreSettings.EdShowSleep = EditorGUILayout.Foldout(loadedFSCoreSettings.EdShowSleep, "Sleep");
		if(loadedFSCoreSettings.EdShowSleep)
		{
			EditorGUILayout.BeginVertical(GUI.skin.box);
			//TimeToSleep
			loadedFSCoreSettings.TimeToSleep = EditorGUILayout.FloatField("Time To Sleep", loadedFSCoreSettings.TimeToSleep);
			EditorGUILayout.HelpBox("The time that a body must be still before it will go to sleep.", MessageType.Info);
			EditorGUILayout.Separator();
			//LinearSleepTolerance
			loadedFSCoreSettings.LinearSleepTolerance = EditorGUILayout.FloatField("Linear Sleep Tolerance", loadedFSCoreSettings.LinearSleepTolerance);
			EditorGUILayout.HelpBox("A body cannot sleep if its linear velocity is above this tolerance.", MessageType.Info);
			EditorGUILayout.Separator();
			//AngularSleepTolerance
			loadedFSCoreSettings.AngularSleepTolerance = EditorGUILayout.FloatField("Angular Sleep Tolerance", loadedFSCoreSettings.AngularSleepTolerance);
			EditorGUILayout.HelpBox("A body cannot sleep if its angular velocity is above this tolerance.", MessageType.Info);
			EditorGUILayout.Separator();
			//MaxTranslation
			loadedFSCoreSettings.MaxTranslation = EditorGUILayout.FloatField("Max Translation", loadedFSCoreSettings.MaxTranslation);
			EditorGUILayout.HelpBox("The maximum linear velocity of a body. This limit is very large and is used to prevent numerical problems. You shouldn't need to adjust this.", MessageType.Info);
			EditorGUILayout.Separator();
			//MaxRotation
			loadedFSCoreSettings.MaxRotation = EditorGUILayout.FloatField("Max Rotation", loadedFSCoreSettings.MaxRotation);
			EditorGUILayout.HelpBox("The maximum angular velocity of a body. This limit is very large and is used to prevent numerical problems. You shouldn't need to adjust this.", MessageType.Info);
			EditorGUILayout.Separator();
			EditorGUILayout.EndVertical();
		}
	}
	
	private void GUI_FSCategorySettings()
	{
		EditorGUILayout.BeginVertical(GUI.skin.box);
		loadedFSCategorySettings.CatAll = EditorGUILayout.TextField("All", loadedFSCategorySettings.CatAll);
		loadedFSCategorySettings.CatNone = EditorGUILayout.TextField("None", loadedFSCategorySettings.CatNone);
		for(int i = 0; i < loadedFSCategorySettings.Cat131.Length; i++)
		{
			loadedFSCategorySettings.Cat131[i] = EditorGUILayout.TextField("Cat" + (i + 1).ToString(), loadedFSCategorySettings.Cat131[i]);
		}
		EditorGUILayout.EndVertical();
	}
	
	private void OnDestroy()
	{
		Save ();
		
		// write and recompile
		string settings_f = "";
		settings_f += fpe_settings_header;
		settings_f += "#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX \n\n";
		FSSettings.Load(FSSettingsPlatform.Desktop);
		settings_f += FormattedSettings();
		settings_f += "\n#elif UNITY_WEBPLAYER\n\n";
		FSSettings.Load(FSSettingsPlatform.Web);
		settings_f += FormattedSettings();
		settings_f += "\n#elif UNITY_IPHONE\n\n";
		FSSettings.Load(FSSettingsPlatform.iOS);
		settings_f += FormattedSettings();
		settings_f += "\n#elif UNITY_ANDROID\n\n";
		FSSettings.Load(FSSettingsPlatform.Android);
		settings_f += FormattedSettings();
		settings_f += "\n#else\n\n";
		FSSettings.Load(FSSettingsPlatform.Default);
		settings_f += FormattedSettings();
		settings_f += "\n#endif\n\n";
		settings_f += fpe_settings_footer;
		
		string path = Application.dataPath + "/FarseerUnity/Base/FarseerPhysics";
		
		FileStream fs;
		StreamWriter sw0;
		
		fs = File.Open(path + "/Settings.cs", FileMode.Truncate);
		sw0 = new StreamWriter(fs);
		
		sw0.Write(settings_f);
		
		sw0.Flush();
		
		sw0.Close();
		
		AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
	}
	
	public void Save()
	{
		FSSettings.CategorySettings = loadedFSCategorySettings;
		FSSettings.CoreSettings = loadedFSCoreSettings;
		FSSettings.Save();
		EditorApplication.RepaintProjectWindow();
		//Microsoft.CSharp.CSharpCodeProvider prov = new Microsoft.CSharp.CSharpCodeProvider(
	}
	
	private string FormattedSettings()
	{
		return string.Format(fpe_settings_middle,
			FSSettings.CoreSettings.EnableDiagnostics.ToString().ToLower(),
			FSSettings.CoreSettings.VelocityIterations,
			FSSettings.CoreSettings.PositionIterations,
			FSSettings.CoreSettings.ContinuousPhysics.ToString().ToLower(),
			FSSettings.CoreSettings.TOIVelocityIterations,
			FSSettings.CoreSettings.TOIPositionIterations,
			FSSettings.CoreSettings.MaxSubSteps,
			FSSettings.CoreSettings.EnableWarmstarting.ToString().ToLower(),
			FSSettings.CoreSettings.AllowSleep.ToString().ToLower(),
			FSSettings.CoreSettings.MaxPolygonVertices,
			FSSettings.CoreSettings.UseFPECollisionCategories.ToString().ToLower(),
			"Category."+FSSettings.CoreSettings.DefaultFixtureCollisionCategories.ToString(),
			"Category."+FSSettings.CoreSettings.DefaultFixtureCollidesWith.ToString(),
			"Category."+FSSettings.CoreSettings.DefaultFixtureIgnoreCCDWith.ToString(),
			FSSettings.CoreSettings.ConserveMemory.ToString().ToLower(),
			FSSettings.CoreSettings.MaxManifoldPoints,
			FSSettings.CoreSettings.AABBExtension,
			FSSettings.CoreSettings.AABBMultiplier,
			FSSettings.CoreSettings.LinearSlop,
			FSSettings.CoreSettings.AngularSlop,
			FSSettings.CoreSettings.PolygonRadius,
			FSSettings.CoreSettings.MaxTOIContacts,
			FSSettings.CoreSettings.VelocityThreshold,
			FSSettings.CoreSettings.MaxLinearCorrection,
			FSSettings.CoreSettings.MaxAngularCorrection,
			FSSettings.CoreSettings.Baumgarte,
			FSSettings.CoreSettings.TOIBaumgarte,
			FSSettings.CoreSettings.TimeToSleep,
			FSSettings.CoreSettings.LinearSleepTolerance,
			FSSettings.CoreSettings.AngularSleepTolerance,
			FSSettings.CoreSettings.MaxTranslation,
			FSSettings.CoreSettings.MaxTranslation * FSSettings.CoreSettings.MaxTranslation,
			FSSettings.CoreSettings.MaxRotation,
			FSSettings.CoreSettings.MaxRotation * FSSettings.CoreSettings.MaxRotation,
			FSSettings.CoreSettings.FixedUpdate.ToString().ToLower());
	}
	
	private static string fpe_settings_middle = @"		// Common

		//GABS
		public static bool FixedUpdate = {34};

        /// <summary>
        /// Enabling diagnostics causes the engine to gather timing information.
        /// You can see how much time it took to solve the contacts, solve CCD
        /// and update the controllers.
        /// NOTE: If you are using a debug view that shows performance counters,
        /// you might want to enable this.
        /// </summary>
        public static bool EnableDiagnostics = {0};

        /// <summary>
        /// The number of velocity iterations used in the solver.
        /// </summary>
        public static int VelocityIterations = {1};

        /// <summary>
        /// The number of position iterations used in the solver.
        /// </summary>
        public static int PositionIterations = {2};

        /// <summary>
        /// Enable/Disable Continuous Collision Detection (CCD)
        /// </summary>
        public static bool ContinuousPhysics = {3};

        /// <summary>
        /// The number of velocity iterations in the TOI solver
        /// </summary>
        public static int TOIVelocityIterations = {4};

        /// <summary>
        /// The number of position iterations in the TOI solver
        /// </summary>
        public static int TOIPositionIterations = {5};

        /// <summary>
        /// Maximum number of sub-steps per contact in continuous physics simulation.
        /// </summary>
		public const int MaxSubSteps = {6};

        /// <summary>
        /// Enable/Disable warmstarting
        /// </summary>
        public static bool EnableWarmstarting = {7};

        /// <summary>
        /// Enable/Disable sleeping
        /// </summary>
        public static bool AllowSleep = {8};

        /// <summary>
        /// The maximum number of vertices on a convex polygon.
        /// </summary>
        public static int MaxPolygonVertices = {9};

        /// <summary>
        /// Farseer Physics Engine has a different way of filtering fixtures than Box2d.
        /// We have both FPE and Box2D filtering in the engine. If you are upgrading
        /// from earlier versions of FPE, set this to true and DefaultFixtureCollisionCategories
		/// to Category.All.
        /// </summary>
        public static bool UseFPECollisionCategories = {10};

		/// <summary>
		/// This is used by the Fixture constructor as the default value 
		/// for Fixture.CollisionCategories member. Note that you may need to change this depending
		/// on the setting of UseFPECollisionCategories, above.
		/// </summary>
		public static Category DefaultFixtureCollisionCategories = {11};

		/// <summary>
		/// This is used by the Fixture constructor as the default value 
		/// for Fixture.CollidesWith member.
		/// </summary>
		public static Category DefaultFixtureCollidesWith = {12};


		/// <summary>
		/// This is used by the Fixture constructor as the default value 
		/// for Fixture.IgnoreCCDWith member.
		/// </summary>
		public static Category DefaultFixtureIgnoreCCDWith = {13};

        /// <summary>
        /// Conserve memory makes sure that objects are used by reference instead of cloned.
        /// When you give a vertices collection to a PolygonShape, it will by default copy the vertices
        /// instead of using the original reference. This is to ensure that objects modified outside the engine
        /// does not affect the engine itself, however, this uses extra memory. This behavior
        /// can be turned off by setting ConserveMemory to true.
        /// </summary>
        public static bool ConserveMemory = {14};

        /// <summary>
        /// The maximum number of contact points between two convex shapes.
        /// </summary>
        public const int MaxManifoldPoints = {15};

        /// <summary>
        /// This is used to fatten AABBs in the dynamic tree. This allows proxies
        /// to move by a small amount without triggering a tree adjustment.
        /// This is in meters.
        /// </summary>
        public const float AABBExtension = {16}f;

        /// <summary>
        /// This is used to fatten AABBs in the dynamic tree. This is used to predict
        /// the future position based on the current displacement.
        /// This is a dimensionless multiplier.
        /// </summary>
        public const float AABBMultiplier = {17}f;

        /// <summary>
        /// A small length used as a collision and constraint tolerance. Usually it is
        /// chosen to be numerically significant, but visually insignificant.
        /// </summary>
        public const float LinearSlop = {18}f;

        /// <summary>
        /// A small angle used as a collision and constraint tolerance. Usually it is
        /// chosen to be numerically significant, but visually insignificant.
        /// </summary>
        public const float AngularSlop = {19}f;

        /// <summary>
        /// The radius of the polygon/edge shape skin. This should not be modified. Making
        /// this smaller means polygons will have an insufficient buffer for continuous collision.
        /// Making it larger may create artifacts for vertex collision.
        /// </summary>
        public const float PolygonRadius = {20}f;

        // Dynamics

        /// <summary>
        /// Maximum number of contacts to be handled to solve a TOI impact.
        /// </summary>
        public const int MaxTOIContacts = {21};

        /// <summary>
        /// A velocity threshold for elastic collisions. Any collision with a relative linear
        /// velocity below this threshold will be treated as inelastic.
        /// </summary>
        public const float VelocityThreshold = {22}f;

        /// <summary>
        /// The maximum linear position correction used when solving constraints. This helps to
        /// prevent overshoot.
        /// </summary>
        public const float MaxLinearCorrection = {23}f;

        /// <summary>
        /// The maximum angular position correction used when solving constraints. This helps to
        /// prevent overshoot.
        /// </summary>
        public const float MaxAngularCorrection = {24}f;

        /// <summary>
        /// This scale factor controls how fast overlap is resolved. Ideally this would be 1 so
        /// that overlap is removed in one time step. However using values close to 1 often lead
        /// to overshoot.
        /// </summary>
        public const float Baumgarte = {25}f;
        public const float TOIBaumgarte = {26}f;

        // Sleep

        /// <summary>
        /// The time that a body must be still before it will go to sleep.
        /// </summary>
        public const float TimeToSleep = {27}f;

        /// <summary>
        /// A body cannot sleep if its linear velocity is above this tolerance.
        /// </summary>
        public const float LinearSleepTolerance = {28}f;

        /// <summary>
        /// A body cannot sleep if its angular velocity is above this tolerance.
        /// </summary>
        public const float AngularSleepTolerance = {29}f;

        /// <summary>
        /// The maximum linear velocity of a body. This limit is very large and is used
        /// to prevent numerical problems. You shouldn't need to adjust this.
        /// </summary>
        public const float MaxTranslation = {30}f;

        public const float MaxTranslationSquared = {31}f;

        /// <summary>
        /// The maximum angular velocity of a body. This limit is very large and is used
        /// to prevent numerical problems. You shouldn't need to adjust this.
        /// </summary>
        public const float MaxRotation = {32}f;

        public const float MaxRotationSquared = {33}f;

		";
	
	private static string fpe_settings_header = @"/*
* Farseer Physics Engine based on Box2D.XNA port:
* Copyright (c) 2011 Ian Qvist
* 
* Original source Box2D:
* Copyright (c) 2006-2011 Erin Catto http://www.box2d.org 
* 
* This software is provided 'as-is', without any express or implied 
* warranty.  In no event will the authors be held liable for any damages 
* arising from the use of this software. 
* Permission is granted to anyone to use this software for any purpose, 
* including commercial applications, and to alter it and redistribute it 
* freely, subject to the following restrictions: 
* 1. The origin of this software must not be misrepresented; you must not 
* claim that you wrote the original software. If you use this software 
* in a product, an acknowledgment in the product documentation would be 
* appreciated but is not required. 
* 2. Altered source versions must be plainly marked as such, and must not be 
* misrepresented as being the original software. 
* 3. This notice may not be removed or altered from any source distribution. 
*/

using System;
using FarseerPhysics.Dynamics;

namespace FarseerPhysics
{
    public static class Settings
    {
        public const float MaxFloat = 3.402823466e+38f;
        public const float Epsilon = 1.192092896e-07f;
        public const float Pi = 3.14159265359f;
		
		";
	
	private static string fpe_settings_footer = @"
        /// <summary>
        /// Friction mixing law. Feel free to customize this.
        /// </summary>
        /// <param name=""friction1"">The friction1.</param>
        /// <param name=""friction2"">The friction2.</param>
        /// <returns></returns>
        public static float MixFriction(float friction1, float friction2)
        {
            return (float) Math.Sqrt(friction1 * friction2);
        }

        /// <summary>
        /// Restitution mixing law. Feel free to customize this.
        /// </summary>
        /// <param name=""restitution1"">The restitution1.</param>
        /// <param name=""restitution2"">The restitution2.</param>
        /// <returns></returns>
        public static float MixRestitution(float restitution1, float restitution2)
        {
            return restitution1 > restitution2 ? restitution1 : restitution2;
        }
    }
}";
	
}
