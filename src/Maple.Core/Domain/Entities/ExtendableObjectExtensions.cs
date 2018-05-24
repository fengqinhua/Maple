﻿using Maple.Core.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Domain.Entities
{
    public static class ExtendableObjectExtensions
    {
        public static T GetData<T>(this IExtendableObject extendableObject, string name, bool handleType = false)
        {
            return extendableObject.GetData<T>(
                name,
                handleType
                    ? new JsonSerializer { TypeNameHandling = TypeNameHandling.All }
                    : JsonSerializer.CreateDefault()
            );
        }

        public static T GetData<T>(this IExtendableObject extendableObject, string name, JsonSerializer jsonSerializer)
        {
            Check.NotNull(extendableObject, nameof(extendableObject));
            Check.NotNull(name, nameof(name));

            if (extendableObject.ExtensionData == null)
            {
                return default(T);
            }

            var json = JObject.Parse(extendableObject.ExtensionData);

            var prop = json[name];
            if (prop == null)
            {
                return default(T);
            }

            if (typeof(T).IsPrimitiveExtendedIncludingNullable())
            {
                return prop.Value<T>();
            }
            else
            {
                return (T)prop.ToObject(typeof(T), jsonSerializer ?? JsonSerializer.CreateDefault());
            }
        }

        public static void SetData<T>(this IExtendableObject extendableObject, string name, T value, bool handleType = false)
        {
            extendableObject.SetData(
                name,
                value,
                handleType
                    ? new JsonSerializer { TypeNameHandling = TypeNameHandling.All }
                    : JsonSerializer.CreateDefault()
            );
        }

        public static void SetData<T>( this IExtendableObject extendableObject,  string name, T value, JsonSerializer jsonSerializer)
        {
            Check.NotNull(extendableObject, nameof(extendableObject));
            Check.NotNull(name, nameof(name));

            if (jsonSerializer == null)
            {
                jsonSerializer = JsonSerializer.CreateDefault();
            }

            if (extendableObject.ExtensionData == null)
            {
                if (EqualityComparer<T>.Default.Equals(value, default(T)))
                {
                    return;
                }

                extendableObject.ExtensionData = "{}";
            }

            var json = JObject.Parse(extendableObject.ExtensionData);

            if (value == null || EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                if (json[name] != null)
                {
                    json.Remove(name);
                }
            }
            else if (value.GetType().IsPrimitiveExtendedIncludingNullable())
            {
                json[name] = new JValue(value);
            }
            else
            {
                json[name] = JToken.FromObject(value, jsonSerializer);
            }

            var data = json.ToString(Formatting.None);
            if (data == "{}")
            {
                data = null;
            }

            extendableObject.ExtensionData = data;
        }

        public static bool RemoveData(this IExtendableObject extendableObject, string name)
        {
            Check.NotNull(extendableObject, nameof(extendableObject));

            if (extendableObject.ExtensionData == null)
            {
                return false;
            }

            var json = JObject.Parse(extendableObject.ExtensionData);

            var token = json[name];
            if (token == null)
            {
                return false;
            }

            json.Remove(name);

            var data = json.ToString(Formatting.None);
            if (data == "{}")
            {
                data = null;
            }

            extendableObject.ExtensionData = data;

            return true;
        }

        //TODO: string[] GetExtendedPropertyNames(...)
    }
}