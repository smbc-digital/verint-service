using Newtonsoft.Json;

namespace verint_service.Models
{
    public class CustomField
    {
        [JsonConstructor]
        public CustomField(string fieldName, string fieldValue, bool active = true)
        {
            this.Name = fieldName;
            this.Value = fieldValue;
            this.IsActive = active;
        }

        public CustomField(string fieldName, string fieldLabel, string fieldValue, bool active = true)
        {
            this.Name = fieldName;
            this.Label = fieldLabel;
            this.Value = fieldValue;
            this.IsActive = active;
        }

        public string Name { get; set; }

        public string Label { get; set; }

        public string Value { get; set; }

        public bool IsActive { get; set; }
    }
}
