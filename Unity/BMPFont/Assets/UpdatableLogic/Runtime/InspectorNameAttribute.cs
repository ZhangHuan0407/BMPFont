using System;

namespace Encoder
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class InspectorNameAttribute : Attribute
    {
        /* field */
        public string Name;

        /* ctor */
        public InspectorNameAttribute()
        {

        }
    }
}