namespace DataTester
{
    public class AccountUserInfo
    {
        public int UserId { get; set; }
        public int? ExternalUserId { get; set; }
        public int? AccountId { get; set; }
        public string AccountName { get; set; }
        public string SSOID { get; set; }
        public string Name { get; set; }
        public int UserType { get; set; }
        public string UserName { get; set; }
        public string Login { get; set; }
        public long? Switches { get; set; }
        public int? ParentAccount { get; set; }
        public string Email { get; set; }
        public string Remarks { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string UserViewConfiguration { get; set; }
        public int Status { get; set; }
        public int IsDefault { get; set; }
        public int UseMapCluster { get; set; }
        public int UseMapArrows { get; set; }
        public int UseMapRemark { get; set; }
        public int UseIdling { get; set; }
        public int UseTraceInTrack { get; set; }
        public int UseVehicleGroup { get; set; }
        public long? Permissions { get; set; }
        public long? TspPermissions { get; set; }
         public string PermittedIPs { get; set; }
        public string Settings { get; set; } = string.Empty;
        public int TimeZoneId { get; set; }
        public int DayLightSaving { get; set; }
    }
}

