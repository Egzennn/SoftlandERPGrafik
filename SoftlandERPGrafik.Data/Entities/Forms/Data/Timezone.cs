namespace SoftlandERPGrafik.Data.Entities.Forms.Data
{
    public class Timezone
    {
        public string Name { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public Timezone GetSystemTimeZone()
        {
            TimeZoneInfo systemTimeZone = TimeZoneInfo.Local;
            return new Timezone
            {
                Name = systemTimeZone.DisplayName,
                Key = systemTimeZone.Id,
                Value = systemTimeZone.StandardName,
            };
        }
    }
}
