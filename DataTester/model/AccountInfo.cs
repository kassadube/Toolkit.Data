using System;

namespace DataTester
{
    public class AccountInfo
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public int? ExternalId { get; set; }
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public long Switches { get; set; }
        public string Email { get; set; }
        public short? MapServerId { get; set; }
        public int LangId { get; set; }
        public int MeasurementSystemId { get; set; }
        public int? TimeZone { get; set; }
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
        public int Zoom { get; set; }
        public int AlertsHistory { get; set; } = 12; // default 12 hours
        public int AccountIconId { get; set; }
        public string AccountCustomStates { get; set; }
        public string WhiteLabelSubdomain { get; set; }
        public string WhiteLabelAppTitle { get; set; }
        public int WhiteLabelImageId { get; set; }
        public int BusinessDepartmentId { get; set; }
        public string PolygonColor { get; set; }
        public int AccountManagerId { get; set; }

        public int ParentAccount { get; set; }
        public int VehicleUniqueId { get; set; }
        public short Status { get; set; }
        public int LoginAuthenticatorId { get; set; }
        public bool IsFallback { get; set; }
        public string SMS { get; set; }
    }
}