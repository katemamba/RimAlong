﻿using Verse;
using System.Runtime.Serialization;

public class Rot4Surrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Rot4 r = (Rot4)obj;
        info.AddValue("rot4", r.AsInt);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Rot4 r = new Rot4();
        r.AsInt = info.GetInt32("rot4");
        return r;
    }
}
