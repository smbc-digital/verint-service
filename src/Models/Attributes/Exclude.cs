using System;

namespace verint_service.Models.Attributes
{
    /// <summary>
    /// Custom attribute to denote excluded properties of classes for adding to Lagan via strongly typed Eform.
    /// It doesn't require any extra information as it is just used to flag fields NOT to write.
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class Exclude : Attribute
    {

    }
}
