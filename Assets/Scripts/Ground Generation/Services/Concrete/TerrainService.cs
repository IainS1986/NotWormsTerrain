using System.Collections;
using System.Collections.Generic;
using TinyIoC;
using UnityEngine;

namespace Terrain.Services.Concrete
{
    public class TerrainService : ITerrainService
    {
        private readonly IGroundGeneratorService m_groundGeneratorService;
        private readonly IMarchingService m_marchingSquaresService;
        private readonly IContourSmoothingService m_contourSmoothingService;
        private readonly IDecompService m_decompService;
        private readonly IMeshService m_meshService;

        public Ground Ground
        {
            get;
            private set;
        }

        public TerrainService()
        {
            m_groundGeneratorService = TinyIoCContainer.Current.Resolve<IGroundGeneratorService>();
            m_contourSmoothingService = TinyIoCContainer.Current.Resolve<IContourSmoothingService>();
            m_marchingSquaresService = TinyIoCContainer.Current.Resolve<IMarchingService>();
            m_decompService = TinyIoCContainer.Current.Resolve<IDecompService>();
            m_meshService = TinyIoCContainer.Current.Resolve<IMeshService>();
        }

        public void SetDimensions(int width, int height)
        {
            Ground = new Ground(width, height);
        }

        public bool GroundChangeSelectiveRebuild(int x, int y, int s, int type)
        {
            bool change = m_groundGeneratorService.SafeGroundFillForGenerator(x, y, s, type, Ground);

            if (!change)
                return change;

            if (Ground.CurrentStage == GroundStage.NONE) return change;
            if (Ground.CurrentStage == GroundStage.DOTS) return change;

            int border = 2;//Extra range to check for nearby ground that *may* be effected
            int minx = int.MaxValue;
            int miny = int.MaxValue;

            //Check and remove effected Chunks
            Dictionary<int, bool> chunkIdsToRemove = new Dictionary<int, bool>();
            for (int i = 0; i < s + (border * 2); i++)
            {
                int xx = x - border + i;
                for (int j = 0; j < s + (border * 2); j++)
                {
                    int yy = y - border + j;

                    //Ensure we don't go out of bounds
                    if (xx >= 0 && xx < Ground.Width && yy >= 0 && yy < Ground.Height)
                    {
                        if (xx < minx) minx = xx;
                        if (yy < miny) miny = yy;

                        if (Ground.Dots[yy, xx].Chunk != 0 && !chunkIdsToRemove.ContainsKey(Ground.Dots[yy, xx].Chunk))
                        {
                            chunkIdsToRemove.Add(Ground.Dots[yy, xx].Chunk, true);
                        }
                    }
                }
            }

            //Clear ChunkID lookups
            foreach (var id in chunkIdsToRemove)
            {
                var chunk = Ground.Chunks[id.Key];
                chunk.Dispose();
                Ground.Chunks.Remove(id.Key);
            }
            //Clear GroundToChunk values (Quicker way to do this?)
            for (int a = 0; a < Ground.Width; a++)
                for (int b = 0; b < Ground.Height; b++)
                    if (chunkIdsToRemove.ContainsKey(Ground.Dots[b, a].Chunk))
                        Ground.Dots[b, a].Chunk = 0;

            //Preprocess
            m_groundGeneratorService.DotRemoval(minx, miny, s + (border * 2), s + (border * 2), Ground);
            m_groundGeneratorService.RemoveDiagonals(minx, miny, s + (border * 2), s + (border * 2), Ground);

            Dictionary<int, GroundChunk> chunks = new Dictionary<int, GroundChunk>();
            m_marchingSquaresService.March(0, 0, Ground.Width, Ground.Height, Ground, out chunks);

            if (Ground.CurrentStage >= GroundStage.SMOOTHED)
            {
                m_contourSmoothingService.SmoothContours(chunks);
            }
            if (Ground.CurrentStage >= GroundStage.VERTEX_REMOVAL)
            {
                m_contourSmoothingService.RemoveVertices(chunks);
            }
            if (Ground.CurrentStage >= GroundStage.DECOMP)
            {
                m_decompService.Decomp(chunks);
            }
            if (Ground.CurrentStage >= GroundStage.MESH)
            {
                m_meshService.BuildMesh(chunks);
            }
            if (Ground.CurrentStage >= GroundStage.LIPS)
            {
                m_meshService.BuildLips(chunks);
            }

            return change;
        }

        public void Generate()
        {
            m_groundGeneratorService.Generate(Ground);

            Ground.CurrentStage = GroundStage.DOTS;
        }

        public void March()
        {
            var newChunks = new Dictionary<int, GroundChunk>();

            Ground.ResetChunks();
            m_marchingSquaresService.March(0, 0, Ground.Width, Ground.Height, Ground, out newChunks);

            Ground.CurrentStage = GroundStage.MARCHING;
        }

        public void SmoothContours()
        {
            if (Ground == null)
                return;

            m_contourSmoothingService.SmoothContours(Ground);

            Ground.CurrentStage = GroundStage.SMOOTHED;
        }

        public void RemoveVertices()
        {
            if (Ground == null)
                return;

            m_contourSmoothingService.RemoveVertices(Ground);

            Ground.CurrentStage = GroundStage.VERTEX_REMOVAL;
        }

        public void Decomp()
        {
            if (Ground == null)
                return;

            m_decompService.Decomp(Ground);

            Ground.CurrentStage = GroundStage.DECOMP;
        }

        public void Mesh()
        {
            if (Ground == null || Ground.CurrentStage < GroundStage.DECOMP)
                return;

            m_meshService.BuildMesh(Ground);

            Ground.CurrentStage = GroundStage.MESH;
        }

        public void Lips()
        {
            if (Ground == null || Ground.CurrentStage < GroundStage.MESH)
                return;

            m_meshService.BuildLips(Ground);

            Ground.CurrentStage = GroundStage.LIPS;
        }
    }
}
