﻿using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace FalloutCollaborationProject.HarmonyPatches
{
    [StaticConstructorOnStartup]
    public class Main
    {
        static Main()
        {
            new Harmony("FalloutCollaborationProject").PatchAll(Assembly.GetExecutingAssembly());
            /*
            var harmony = new Harmony("com.ogliss.rimworld.mod.VanillaWeaponsExpandedLaser");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            */
        }
    }
}