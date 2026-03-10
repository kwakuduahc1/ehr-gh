namespace SHIMS.Context
{
    public interface IAppFeatures
    {
        public string AppName { get; set; }

        public string Key { get; set; }

        public string Audience { get; set; }

        public string Issuer { get; set; }

        public DateTime Expiry { get; set; }

        public byte Hours { get; set; }
    }

    public class AppFeatures : IAppFeatures
    {
        public string AppName { get; set; }

        public string Key { get; set; }

        public string Audience { get; set; }

        public string Issuer { get; set; }

        public DateTime Expiry { get; set; }

        public byte Hours { get; set; }

        public AppFeatures(IConfiguration config)
        {
            var con = config.GetSection("AppFeatures").Get<AppModel>();
            if (con is not null)
            {
                var date = DateTime.UtcNow;
                AppName = con.AppName;
                Key = con.Key;
                Audience = con.Audience;
                Issuer = con.Issuer;
                Expiry = date.AddDays(con.Hours);
                Hours = con.Hours;
            }
            else throw new Exception("Application features were not found in the store");
        }
    }

    public record AppModel(string AppName, string Key, string Audience, string Issuer, byte Hours);
}
