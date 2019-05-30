using System;

namespace verint_service.Models.Attributes
{
    /// <summary>
    /// This attribute indicated that null values should be written to the eform (as an empty string)
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class IncludeIfNull : Attribute 
    {
    }
}
