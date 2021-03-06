﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Verse;

namespace CooperateRim
{
    public class Zone_StockPileSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Zone_Stockpile sz = (Zone_Stockpile)obj;
            info.AddValue("zoneID", sz.ID);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            int zoneID = info.GetInt32("zoneID");
            return Find.CurrentMap.zoneManager.AllZones.First(u => u.ID == zoneID);
        }
    }
}
