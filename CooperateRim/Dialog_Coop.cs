﻿using RimWorld;
using Verse;
using System;
using UnityEngine;
using RimWorld.Planet;
using Harmony;
using CooperateRim.Utilities;
using System.Collections.Generic;

namespace CooperateRim
{
    [HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new Type[] { typeof(PawnGenerationRequest) })]
    public class pwnd
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            getValuePatch.GuardedPush();
        }

        [HarmonyPostfix]
        public static void Postfix()
        {
            getValuePatch.GuardedPop();
        }
    }
    /*
    [HarmonyPatch(typeof(PawnApparelGenerator), "GenerateStartingApparelFor")]
    public class generator_patch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return false;
        }
    }*/

    public class ThingRegistry
    {
        static Dictionary<int, WeakReference<Thing>> registry = new Dictionary<int, WeakReference<Thing>>();

        public static void AddThing(Thing t, int id)
        {
            if (id == -1)
            {
                //RimLog.Message("uhm, -1 id for " + t + ", skipping");
            }
            else
            {
                if (registry.ContainsKey(id))
                {
                    RimLog.Error("attempt to add something with id " + id + " for thing " + t);
                }
                else
                {
                    registry.Add(id, new WeakReference<Thing>(t));
                }
            }
        }

        public static Thing tryGetThing(int id)
        {
            if (registry.ContainsKey(id))
            {
                return registry[id].Target;
            }
            else
            {
                return null;
            }
        }
    }

    [HarmonyPatch(typeof(ThingIDMaker), "GiveIDTo")]
    public class ThingIDMakerPatch
    {
        public static bool stopID;

        [HarmonyPostfix]
        public static void Postfix(Thing t)
        {
            ThingRegistry.AddThing(t, t.thingIDNumber);

            if (!stopID)
            {
                if (t is Pawn)
                {
                    Utilities.RimLog.Message("Made id for " + (t as Pawn) + " | " + Rand.Int + "|" + System.Threading.Thread.CurrentThread.ManagedThreadId);
                }
                else
                {
                    Utilities.RimLog.Message("Made id for " + (t) + " | " + Rand.Int + "|" + System.Threading.Thread.CurrentThread.ManagedThreadId);
                }
            }
        }
    }
    
    public partial class CooperateRimming
    {
        

        public class Dialog_Coop : Window
        {
            public override void DoWindowContents(Rect inRect)
            {
                int size = 52;
                Rect r = inRect;
                r.height = size - 1;
                r.width = 150;
                if (Widgets.ButtonText(r, "Connect to "))
                {
                    NetDemo.WaitForConnection(hostName);
                }
                r.y += size;
                Widgets.ButtonText(r, "MEANINGLESS BUTTON"); r.y += size;
                hostName = Widgets.TextArea(r, hostName);
                //Widgets.Label(r, "hey, motherfuckers!");
            }

            static void ErrorHandler(System.Exception ex)
            {
                RimLog.Message(ex.ToString());
            }

            static void GenerateMap()
            {
                if (Find.Root)
                {
                    //Find.Root.Start();
                }
                else
                {
                    RimLog.Message("Null root");
                }
            }
        }
    }
}