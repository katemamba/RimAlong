﻿using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CooperateRim
{
    [HarmonyPatch(typeof(BillStack))]
    [HarmonyPatch("Delete")]
    public class bill_delete_patch
    {
        static bool avoid_loop_internal = false;

        public static void RemoveAt(BillStack stack, int index)
        {
            avoid_loop_internal = true;
            try
            {
                stack.Delete(stack.Bills[index]);
            }
            finally
            {
                avoid_loop_internal = false;
            }
        }

        [HarmonyPrefix]
        public static bool Delete(ref Bill bill, BillStack __instance)
        {
            if (avoid_loop_internal)
            {
                return true;
            }
            else
            {
                RemoveAt(__instance, __instance.Bills.IndexOf(bill));
                return false;
            }
        }
    }
}
