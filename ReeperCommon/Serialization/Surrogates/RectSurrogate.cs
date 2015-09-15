﻿using System;
using System.Reflection;
using System.Runtime.Serialization;
using ReeperCommon.Extensions;
using ReeperCommon.Serialization.Exceptions;
using UnityEngine;

namespace ReeperCommon.Serialization.Surrogates
{
    // ReSharper disable once UnusedMember.Global
    public class RectSurrogate : IConfigNodeItemSerializer<Rect>
    {
        public void Serialize(Type type, ref object target, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (key == null) throw new ArgumentNullException("key");
            if (config == null) throw new ArgumentNullException("config");
            if (serializer == null) throw new ArgumentNullException("serializer");
            if (!(target is Rect)) throw new WrongSerializerException(type, typeof(Rect));

            var cfg = config.AddNode(key);
            var r = (Rect)target;

            cfg.AddValue("x", r.x);
            cfg.AddValue("y", r.y);
            cfg.AddValue("width", r.width);
            cfg.AddValue("height", r.height);
        }


        public void Deserialize(Type type, ref object target, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (key == null) throw new ArgumentNullException("key");
            if (config == null) throw new ArgumentNullException("config");
            if (serializer == null) throw new ArgumentNullException("serializer");
            if (!(target is Rect)) throw new WrongSerializerException(type, typeof(Rect));

            if (!config.HasNode(key))
                return; // no changes; leave existing values intact


            var rectConfig = config.GetNode(key);
            var r = (Rect)target;

            r.x = rectConfig.Parse("x", r.x);
            r.y = rectConfig.Parse("y", r.y);
            r.width = rectConfig.Parse("width", r.width);
            r.height = rectConfig.Parse("height", r.height);

            target = r;
        }
    }
}
