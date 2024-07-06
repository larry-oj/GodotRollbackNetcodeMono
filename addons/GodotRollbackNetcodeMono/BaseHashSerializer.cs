using GDC = Godot.Collections;
using Godot;
using System;

namespace GodotRollbackNetcode
{
    public interface IHashSerializer
    {
        object Serialize(object value);

        object Unserialize(object value);
    }

    public partial class BaseHashSerializer : Godot.RefCounted, IHashSerializer
    {
        private object serialize(object value) => Serialize(value);

        public virtual object Serialize(object value)
        {
            if (value is GDC.Dictionary dict)
                return SerializeDictionary(dict);
            else if (value is GDC.Array array)
                return SerializeArray(array);
            else if (value is Resource resource)
                return SerializeResource(resource);
            else if (value is Godot.GodotObject obj)
                return SerializeObject(obj);

            return SerializeOther(value);
        }

        protected virtual GDC.Dictionary SerializeDictionary(GDC.Dictionary value)
        {
            var serialized = new GDC.Dictionary();
            foreach (var key in value.Keys)
                serialized[key] = Variant.From(Serialize(value[key]));
            return serialized;
        }

        protected virtual GDC.Array SerializeArray(GDC.Array array)
        {
            var serialized = new GDC.Array();
            foreach (var item in array)
                serialized.Add(Variant.From(Serialize(item)));
            return serialized;
        }

        protected virtual GDC.Dictionary SerializeResource(Resource value)
        {
            return new GDC.Dictionary()
            {
                ["_"] = nameof(Resource),
                ["path"] = value.ResourcePath
            };
        }

        protected virtual GDC.Dictionary SerializeObject(Godot.GodotObject value)
        {
            return new GDC.Dictionary()
            {
                ["_"] = nameof(Godot.GodotObject),
                ["str"] = value.ToString(),
            };
        }

        protected virtual object SerializeOther(object value)
        {
            if (value is Vector2 vector2)
                return new GDC.Dictionary()
                {
                    ["_"] = nameof(Vector2),
                    ["x"] = vector2.X,
                    ["y"] = vector2.Y
                };
            else if (value is Vector3 vector3)
                return new GDC.Dictionary()
                {
                    ["_"] = nameof(Vector3),
                    ["x"] = vector3.X,
                    ["y"] = vector3.Y,
                    ["z"] = vector3.Z
                };
            else if (value is Transform2D transform2D)
                return new GDC.Dictionary()
                {
                    ["_"] = nameof(Transform2D),
                    ["x"] = new GDC.Dictionary()
                    {
                        ["x"] = transform2D.X.X,
                        ["y"] = transform2D.X.Y
                    },
                    ["y"] = new GDC.Dictionary()
                    {
                        ["x"] = transform2D.Y.X,
                        ["y"] = transform2D.Y.Y
                    },
                    ["origin"] = new GDC.Dictionary()
                    {
                        ["x"] = transform2D.Origin.X,
                        ["y"] = transform2D.Origin.Y
                    }
                };
            else if (value is Transform3D transform)
                return new GDC.Dictionary()
                {
                    ["_"] = nameof(Transform3D),
                    ["x"] = new GDC.Dictionary()
                    {
                        ["x"] = transform.Basis.X.X,
                        ["y"] = transform.Basis.X.Y,
                        ["z"] = transform.Basis.X.Z,
                    },
                    ["y"] = new GDC.Dictionary()
                    {
                        ["x"] = transform.Basis.Y.X,
                        ["y"] = transform.Basis.Y.Y,
                        ["z"] = transform.Basis.Y.Z
                    },
                    ["z"] = new GDC.Dictionary()
                    {
                        ["x"] = transform.Basis.Z.X,
                        ["y"] = transform.Basis.Z.Y,
                        ["z"] = transform.Basis.Z.Z
                    },
                    ["origin"] = new GDC.Dictionary()
                    {
                        ["x"] = transform.Origin.X,
                        ["y"] = transform.Origin.Y,
                        ["z"] = transform.Origin.Z,
                    }
                };
            return value;
        }

        private object unserialize(object value) => Unserialize(value);

        readonly string[] SerializedOthers = new[] { nameof(Vector2), nameof(Vector3), nameof(Transform2D), nameof(Transform3D) };

        public virtual object Unserialize(object value)
        {
            if (value is GDC.Dictionary dictionary)
            {
                if (!dictionary.ContainsKey("_"))
                    return UnserializeDictionary(dictionary);
                var type = dictionary["_"].ToString();
                if (type == nameof(Resource))
                    return UnserializeResource(dictionary);
                else if (Array.IndexOf(SerializedOthers, type) > -1)
                    return UnserializeOther(dictionary);
            }
            else if (value is GDC.Array array)
            {
                return UnserializeArray(array);
            }
            return value;
        }

        protected virtual GDC.Dictionary UnserializeDictionary(GDC.Dictionary value)
        {
            var unserialized = new GDC.Dictionary();
            foreach (var key in value.Keys)
                unserialized[key] = Variant.From(Unserialize(value[key]));
            return unserialized;
        }

        protected virtual GDC.Array UnserializeArray(GDC.Array value)
        {
            var unserialized = new GDC.Array();
            foreach (var item in value)
                unserialized.Add(Variant.From(Unserialize(item)));
            return unserialized;
        }

        protected virtual Resource UnserializeResource(GDC.Dictionary value)
        {
            return GD.Load<Resource>((string)value["path"]);
        }

        protected virtual string UnserializeObject(GDC.Dictionary value)
        {
            // experimental
            // if (value["_"] is Godot.Object)
                return (string)value["string"];
            // return null;
        }

        protected virtual object UnserializeOther(GDC.Dictionary value)
        {
            // converted to switch expression
            return (string)value["_"] switch
            {
                nameof(Vector2) => new Vector2((float)value["x"], (float)value["y"]),
                nameof(Vector3) => new Vector3((float)value["x"], (float)value["y"], (float)value["z"]),
                nameof(Transform2D) => new Transform2D(new Vector2((float)value["x.x"], (float)value["x.y"]),
                    new Vector2((float)value["y.x"], (float)value["y.y"]),
                    new Vector2((float)value["origin.x"], (float)value["origin.y"])),
                nameof(Transform3D) => new Transform3D(
                    new Vector3((float)value["x.x"], (float)value["x.y"], (float)value["x.z"]),
                    new Vector3((float)value["y.x"], (float)value["y.y"], (float)value["y.z"]),
                    new Vector3((float)value["z.x"], (float)value["z.y"], (float)value["z.z"]),
                    new Vector3((float)value["origin.x"], (float)value["origin.y"], (float)value["origin.z"])),
                _ => null
            };
        }
    }
}
