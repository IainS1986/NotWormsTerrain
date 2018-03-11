using System.Collections;
using System.Collections.Generic;
using Terrain.Services;
using Terrain.Services.Concrete;
using Terrain.Utility.Services;
using Terrain.Utility.Services.Concrete;
using TinyIoC;
using UnityEngine;

/// <summary>
/// AppStartup class is used to have functions
/// we want to run at Application start, or at
/// Scene start.
/// </summary>
public class AppStartup : MonoBehaviour
{
    /// <summary>
    /// Used to ensure functionality we only
    /// want to run at Application Startup is done once
    /// during the course of the session.
    /// </summary>
    public static bool sFirstRun  = true;

    void Awake()
    {
        if(sFirstRun)
        {
            RegisterServices();    
        }

        sFirstRun = false;
        DestroyObject(gameObject);
    }


    /// <summary>
    /// Registers singleton services with their interfaces in TinyIoC. This will only ever
    /// trigger once in the first AppStartup object instantiated.
    /// </summary>
    private void RegisterServices()
    {
        TinyIoCContainer.Current.Register<ILoggingService>(new LoggingService());
        TinyIoCContainer.Current.Register<IContourOptimiserService>(new ContourOptimiserService());
        TinyIoCContainer.Current.Register<IContourSmoothingService>(new ContourSmoothingService());
        TinyIoCContainer.Current.Register<IDecompService>(new DecompService());
        TinyIoCContainer.Current.Register<IGroundGeneratorService>(new GroundGeneratorService());
        TinyIoCContainer.Current.Register<IMarchingService>(new MarchingService());
        TinyIoCContainer.Current.Register<IMeshService>(new MeshService());
        TinyIoCContainer.Current.Register<ITerrainService>(new TerrainService());
    }
}
