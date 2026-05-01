using Newtonsoft.Json;

namespace WebVisitsMobile.Models.HID.UserHID
{
    public class CustomerDTO
    {
        [JsonProperty("schemas")]
        public List<string> Schemas { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("mobileKeysets")]
        public string MobileKeysets { get; set; }

        [JsonProperty("subscriptionMode")]
        public string SubscriptionMode { get; set; }

        [JsonProperty("activeUsers")]
        public int ActiveUsers { get; set; }

        [JsonProperty("pendingInvitations")]
        public int PendingInvitations { get; set; }

        [JsonProperty("totalUsers")]
        public int TotalUsers { get; set; }

        [JsonProperty("urn:hid:scim:api:ma:2.2:LicenseInfo")]
        public List<LicenseDetail> LicenseDetails { get; set; }
    }

    public class LicenseDetail
    {
        [JsonProperty("partNumber")]
        public string PartNumber { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("availableQty")]
        public int AvailableQty { get; set; }

        [JsonProperty("consumedQty")]
        public int ConsumedQty { get; set; }

        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }

        [JsonProperty("licenseState")]
        public string LicenseState { get; set; }

        [JsonProperty("periodState")]
        public string PeriodState { get; set; }

        [JsonProperty("statusMessage")]
        public string StatusMessage { get; set; }
    }
}