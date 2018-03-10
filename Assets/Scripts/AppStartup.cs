using System.Collections;
using System.Collections.Generic;
using Terrain.Services;
using Terrain.Services.Concrete;
using TinyIoC;
using UnityEngine;

public class AppStartup : MonoBehaviour
{
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

    private void RegisterServices()
    {
        TinyIoCContainer.Current.Register<IContourSmoothingService>(new ContourSmoothingService());
        TinyIoCContainer.Current.Register<IDecompService>(new DecompService());
        TinyIoCContainer.Current.Register<IGroundGeneratorService>(new GroundGeneratorService());
        TinyIoCContainer.Current.Register<IMarchingService>(new MarchingService());
        TinyIoCContainer.Current.Register<IMeshService>(new MeshService());
        TinyIoCContainer.Current.Register<ITerrainService>(new TerrainService());
    }
}
