using Godot;
using GDC = Godot.Collections;

namespace GodotRollbackNetcode
{
    public partial class HashSerializerWrapper : GDScriptWrapper, IHashSerializer
    {
        public HashSerializerWrapper() { }
        public HashSerializerWrapper(Godot.GodotObject source) : base(source) { }

        public object Serialize(object value) => (GDC.Dictionary)Source.Call("serialize", Variant.From(value));

        public object Unserialize(object value) => (GDC.Dictionary)Source.Call("unserialize", Variant.From(value));
    }
}
