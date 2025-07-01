using System;
using System.Collections.Generic;
using Godot;

namespace demo.addons.GDTask.Internal
{
    internal static class GodotEqualityComparer
    {
        public static readonly IEqualityComparer<Vector2> Vector2 = new Vector2EqualityComparer();
        public static readonly IEqualityComparer<Vector3> Vector3 = new Vector3EqualityComparer();
        public static readonly IEqualityComparer<Color> Color = new ColorEqualityComparer();
        public static readonly IEqualityComparer<Rect2> Rect2 = new Rect2EqualityComparer();
        public static readonly IEqualityComparer<Aabb> Aabb = new AabbEqualityComparer();
        public static readonly IEqualityComparer<Quaternion> Quaternion = new QuatEqualityComparer();

        private static readonly RuntimeTypeHandle Vector2Type = typeof(Vector2).TypeHandle;
        private static readonly RuntimeTypeHandle Vector3Type = typeof(Vector3).TypeHandle;
        private static readonly RuntimeTypeHandle ColorType = typeof(Color).TypeHandle;
        private static readonly RuntimeTypeHandle RectType = typeof(Rect2).TypeHandle;
        private static readonly RuntimeTypeHandle AabbType = typeof(Aabb).TypeHandle;
        private static readonly RuntimeTypeHandle QuaternionType = typeof(Quaternion).TypeHandle;

        private static class Cache<T>
        {
            public static readonly IEqualityComparer<T> Comparer;

            static Cache()
            {
                var comparer = GetDefaultHelper(typeof(T));
                if (comparer == null)
                {
                    Comparer = EqualityComparer<T>.Default;
                }
                else
                {
                    Comparer = (IEqualityComparer<T>)comparer;
                }
            }
        }

        public static IEqualityComparer<T> GetDefault<T>()
        {
            return Cache<T>.Comparer;
        }

        private static object GetDefaultHelper(Type type)
        {
            var t = type.TypeHandle;

            if (t.Equals(Vector2Type)) return (object)GodotEqualityComparer.Vector2;
            if (t.Equals(Vector3Type)) return (object)GodotEqualityComparer.Vector3;
            if (t.Equals(ColorType)) return (object)GodotEqualityComparer.Color;
            if (t.Equals(RectType)) return (object)GodotEqualityComparer.Rect2;
            if (t.Equals(AabbType)) return (object)GodotEqualityComparer.Aabb;
            if (t.Equals(QuaternionType)) return (object)GodotEqualityComparer.Quaternion;

            return null;
        }

        private sealed class Vector2EqualityComparer : IEqualityComparer<Vector2>
        {
            public bool Equals(Vector2 self, Vector2 vector)
            {
                return self.X.Equals(vector.X) && self.Y.Equals(vector.Y);
            }

            public int GetHashCode(Vector2 obj)
            {
                return obj.X.GetHashCode() ^ obj.Y.GetHashCode() << 2;
            }
        }

        private sealed class Vector3EqualityComparer : IEqualityComparer<Vector3>
        {
            public bool Equals(Vector3 self, Vector3 vector)
            {
                return self.X.Equals(vector.X) && self.Y.Equals(vector.Y) && self.Z.Equals(vector.Z);
            }

            public int GetHashCode(Vector3 obj)
            {
                return obj.X.GetHashCode() ^ obj.Y.GetHashCode() << 2 ^ obj.Z.GetHashCode() >> 2;
            }
        }

        private sealed class ColorEqualityComparer : IEqualityComparer<Color>
        {
            public bool Equals(Color self, Color other)
            {
                return self.R.Equals(other.R) && self.G.Equals(other.G) && self.B.Equals(other.B) && self.A.Equals(other.A);
            }

            public int GetHashCode(Color obj)
            {
                return obj.R.GetHashCode() ^ obj.G.GetHashCode() << 2 ^ obj.B.GetHashCode() >> 2 ^ obj.A.GetHashCode() >> 1;
            }
        }

        private sealed class Rect2EqualityComparer : IEqualityComparer<Rect2>
        {
            public bool Equals(Rect2 self, Rect2 other)
            {
                return self.Size.Equals(other.Size) && self.Position.Equals(other.Position);
            }

            public int GetHashCode(Rect2 obj)
            {
                return obj.Size.GetHashCode() ^ obj.Position.GetHashCode() << 2;
            }
        }

        private sealed class AabbEqualityComparer : IEqualityComparer<Aabb>
        {
            public bool Equals(Aabb self, Aabb vector)
            {
                return self.Position.Equals(vector.Position) && self.Size.Equals(vector.Size);
            }

            public int GetHashCode(Aabb obj)
            {
                return obj.Position.GetHashCode() ^ obj.Size.GetHashCode() << 2;
            }
        }

        private sealed class QuatEqualityComparer : IEqualityComparer<Quaternion>
        {
            public bool Equals(Quaternion self, Quaternion vector)
            {
                return self.X.Equals(vector.X) && self.Y.Equals(vector.Y) && self.Z.Equals(vector.Z) && self.W.Equals(vector.W);
            }

            public int GetHashCode(Quaternion obj)
            {
                return obj.X.GetHashCode() ^ obj.Y.GetHashCode() << 2 ^ obj.Z.GetHashCode() >> 2 ^ obj.W.GetHashCode() >> 1;
            }
        }
    }
}
