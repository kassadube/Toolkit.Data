namespace Toolkit.Model
{
    public class BaseREQ
    {
        public int? AccountId { get; set; }
        public int? UserId { get; set; }
        public int? LangId { get; set; }
        public int? UserType { get; set; }
        // public decimal? Switches { get; set; } // Removed Switches claims from Token claims
        public long? Permissions { get; set; }
        public string ClientId { get; set; }
        public bool UseCache { get; set; } = true;
        public string TimeZoneName { get; set; }
        public bool DayLightSaving { get; set; }
        public string UserName { get; set; }

        public BaseREQ()
        {

        }

        public BaseREQ(BaseREQ req)
        {
            AccountId = req.AccountId;
            UserId = req.UserId;
            LangId = req.LangId;
            UserType = req.UserType;
            // Switches = req.Switches;
            Permissions = req.Permissions;
            ClientId = req.ClientId;
            UseCache = req.UseCache;
            TimeZoneName = req.TimeZoneName;
            UserName = req.UserName;
        }
    }
}
