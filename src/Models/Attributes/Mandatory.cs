using System;

namespace verint_service.Models.Attributes
{
    /// <summary>
    /// Custom attribute to denote Mandatory properties of classes for adding to Lagan.
    /// It doesn't require any extra information as it is just used to flag fields for the Validate Method.
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class Mandatory : Attribute
    {
    }
}
