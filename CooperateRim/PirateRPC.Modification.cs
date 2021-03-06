﻿using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace PirateRPC
{
    public partial class PirateRPC
    {
        [Serializable]
        public class Modification : ISerializable
        {
            Action<Stream> action;
            
            internal Modification(Action<Stream> action)
            {
                this.action = action;
            }

            internal Modification(SerializationInfo info, StreamingContext context)
            {
                // Simply retrieve the action if it is serializable
                if (info.GetBoolean("isSerializable"))
                    this.action = (Action<Stream>)info.GetValue("action", typeof(Action<Stream>));
                // Otherwise, recreate the action based on its serialized components
                else
                {
                    // Retrieve the serialized method reference
                    MethodInfo method = (MethodInfo)info.GetValue("method", typeof(MethodInfo));

                    // Create an instance of the anonymous delegate class
                    object target = System.Activator.CreateInstance(method.DeclaringType);// ReflectionUtil.CreateObject(method.DeclaringType, BindingFlags.NonPublic | BindingFlags.Instance);

                    // Initialize the fields of the anonymous instance
                    foreach (FieldInfo field in method.DeclaringType.GetFields())
                        field.SetValue(target, info.GetValue(field.Name, field.FieldType));

                    // Recreate the action delegate
                    action = (Action<Stream>)Delegate.CreateDelegate(typeof(Action<Stream>), target, method.Name);
                }
            }

            void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
            {
                // Serialize the action delegate directly if the target is serializable
                if (action.Target == null || action.Target.GetType().GetCustomAttributes(typeof(SerializableAttribute), false).Length > 0)
                {
                    info.AddValue("isSerializable", true);
                    info.AddValue("action", action);
                }
                // Otherwise, serialize information necessary to recreate the action delegate
                else
                {
                    info.AddValue("isSerializable", false);
                    info.AddValue("method", action.Method);
                    foreach (FieldInfo field in action.Method.DeclaringType.GetFields())
                        info.AddValue(field.Name, field.GetValue(action.Target));
                }
            }

            public void Apply(Stream s)
            {
                action(s);
            }
        }
    }
}