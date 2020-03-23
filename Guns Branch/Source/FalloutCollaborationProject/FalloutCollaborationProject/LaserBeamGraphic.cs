using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace FalloutCollaborationProject
{
    class LaserBeamGraphic : Thing
    {
        new LaserBeamDef def => base.def as LaserBeamDef;

        int ticks;
        int colorIndex = 2;
        Vector3 a;
        Vector3 b;

        public Matrix4x4 drawingMatrix = default(Matrix4x4);
        Material materialBeam;
        Mesh mesh;


        public float Opacity => (float)Math.Sin(Math.Pow(1.0 - 1.0 * ticks / def.lifetime, def.impulse) * Math.PI);
        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref ticks, "ticks");
            Scribe_Values.Look(ref colorIndex, "colorIndex");
            Scribe_Values.Look(ref a, "a");
            Scribe_Values.Look(ref b, "b");
        }

        public override void Tick()
        {
            if (def == null || ticks++ > def.lifetime)
            {
                Destroy(DestroyMode.Vanish);
            }
        }

        void SetColor(Thing launcher)
        {
            IBeamColorThing gun = null;

            Pawn pawn = launcher as Pawn;
            if (pawn != null && pawn.equipment != null) gun = pawn.equipment.Primary as IBeamColorThing;
            if (gun == null) gun = launcher as IBeamColorThing;

            if (gun != null && gun.BeamColor != -1)
            {
                colorIndex = gun.BeamColor;
            }
        }

        public void Setup(Thing launcher, Vector3 origin, Vector3 destination)
        {
            SetColor(launcher);

            a = origin;
            b = destination;
        }

        public void SetupDrawing()
        {
            if (mesh != null) return;

            materialBeam = def.GetBeamMaterial(colorIndex) ?? def.graphicData.Graphic.MatSingle;

            if (this.def.graphicData.graphicClass == typeof(Graphic_Random))
            {
                materialBeam = def.GetBeamMaterial(Rand.RangeInclusive(0, this.def.materials.Count)) ?? def.graphicData.Graphic.MatSingle;
            }
            float beamWidth = def.beamWidth;
            Quaternion rotation = Quaternion.LookRotation(b - a);
            Vector3 dir = (b - a).normalized;
            float length = (b - a).magnitude;

            Vector3 drawingScale = new Vector3(beamWidth, 1f, length);
            Vector3 drawingPosition = (a + b) / 2;
            drawingMatrix.SetTRS(drawingPosition, rotation, drawingScale);

            float textureRatio = 1.0f * materialBeam.mainTexture.width / materialBeam.mainTexture.height;
            float seamTexture = def.seam < 0 ? textureRatio : def.seam;
            float capLength = beamWidth / textureRatio / 2f * seamTexture;
            float seamGeometry = length <= capLength * 2 ? 0.5f : capLength * 2 / length;

            this.mesh = MeshMakerLaser.Mesh(seamTexture, seamGeometry);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            if (def == null || def.decorations == null || respawningAfterLoad) return;


        }
    }
}
