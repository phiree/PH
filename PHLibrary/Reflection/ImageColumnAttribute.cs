using System;

namespace PHLibrary.Reflection
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class ImageColumnAttribute : Attribute
    {
    }
}
