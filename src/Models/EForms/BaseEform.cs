using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using verint_service.Models.Attributes;

namespace verint_service.Models.EForms
{
    public abstract class BaseEform : IBaseEform
    {
        // Explicit parameterless constructor
        public BaseEform()
        {
        }

        public virtual void Populate(Case crmCase)
        {
            // Nothing required in base form
        }

        /// <summary>
        /// This method checks for mandatory values in the form not being null/empty and for invalid types.
        /// It throws an exception if the form is not valid.
        /// Any invalid are added to the list Invalid Properties, which is then exposed in the exception message
        /// </summary>
        public virtual void Validate()
        {
            var classProperties = this.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            var invalidProperties = new List<string>();

            foreach (var classProperty in classProperties)
            {
                Type t = classProperty.PropertyType;
                bool mandatory = classProperty.GetCustomAttributes(typeof(Mandatory), true).Any();
                int maxLength = classProperty.GetCustomAttributes(typeof(MaxLength), true).Any() ? ((MaxLength)classProperty.GetCustomAttributes(typeof(MaxLength), true).First()).Length : 0;

                if (t == typeof(int) || t == typeof(bool) || t == typeof(decimal))
                    continue; // These are allowed types and default values of 0 and false are permitted so no further validation required

                // If it a string and has a max length attribute
                if (t == typeof(string) && maxLength > 0)
                {
                    string val = (string)classProperty.GetValue(this, null);

                    if (val != null && val.Length > maxLength)
                    {
                        invalidProperties.Add(classProperty.Name);
                        continue; // If it exceeds max length, add to invalid properties and continue loop
                    }
                }

                // Mandatory fields require extra validation
                if (mandatory && (t == typeof(string) || t == typeof(DateTime)))
                {
                    // String cannot be null or empty
                    if (classProperty.PropertyType == typeof(string))
                    {
                        string val = (string)classProperty.GetValue(this, null);
                        if (!string.IsNullOrEmpty(val))
                            continue;
                    }

                    // DateTime cannot be null
                    if (classProperty.PropertyType == typeof(DateTime) && classProperty.GetValue(this, null) != null)
                    {
                        continue;
                    }
                }
                else if (classProperty.PropertyType == typeof(string) || classProperty.PropertyType == typeof(DateTime))
                {
                    continue; // Property is allowed type, but is not validated
                }

                invalidProperties.Add(classProperty.Name); // If loop has reached this point property is not of allowed type, so any value is not valid
            }

            if (invalidProperties.Count() > 0)
            {
                throw new Exception("Invalid Properties in eForm: " + string.Join(",", invalidProperties));
            }
        }
    }
}
