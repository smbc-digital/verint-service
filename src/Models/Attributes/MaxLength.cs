using System;

namespace verint_service.Models.Attributes
{
    /// <summary>
    /// specifies the maximum length of a string allowed
    /// </summary>
    public class MaxLength : Attribute
    {
        public MaxLength(int length)
        {
            Length = length;
        }
        
        public int Length { get; set; }
    }
}
