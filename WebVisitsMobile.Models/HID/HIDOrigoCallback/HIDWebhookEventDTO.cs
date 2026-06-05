using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebVisitsMobile.Models.HID.HIDOrigoCallback
{
    // ─── ENVELOPE ────────────────────────────────────────────────────────────────
    // HID envía un array de estos objetos en cada POST
    // El campo "type" indica QUÉ recurso cambió
    // El status real del evento vive dentro de data.status
    public class HIDWebhookEventDTO
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("specversion")]
        public string? SpecVersion { get; set; }

        // "type" indica el recurso: "com.origo.mi.user", "origo.credential-management.events.pass", etc.
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("source")]
        public string? Source { get; set; }

        [JsonPropertyName("time")]
        public string? Timestamp { get; set; }

        [JsonPropertyName("datacontenttype")]
        public string? DataContentType { get; set; }

        [JsonPropertyName("organizationId")]
        public string? OrganizationId { get; set; }

        [JsonPropertyName("data")]
        public JsonElement Data { get; set; }
    }

    // ─── USUARIOS ─────────────────────────────────────────────────────────────────
    // Viene en eventos type: "com.origo.mi.user"
    //public class UserEventDTO
    //{
    //    [JsonPropertyName("userId")]
    //    public string? UserId { get; set; }

    //    [JsonPropertyName("externalId")]
    //    public string? ExternalId { get; set; }

    //    // ✅ Status real: USER_CREATED, USER_UPDATED, USER_DELETE_INITIATED, USER_DELETED
    //    // ✅ Status real: USER_CREATED: 20, USER_UPDATED: 21, USER_DELETE_INITIATED: 22, USER_DELETED: 23
    //    [JsonPropertyName("status")]
    //    public string? Status { get; set; }

    //    [JsonPropertyName("firstname")]
    //    public string? Firstname { get; set; }

    //    [JsonPropertyName("lastname")]
    //    public string? Lastname { get; set; }

    //    [JsonPropertyName("email")]
    //    public string? Email { get; set; }

    //    [JsonPropertyName("organizationId")]
    //    public string? OrganizationId { get; set; }

    //    [JsonPropertyName("sourceTimestamp")]
    //    public string? SourceTimestamp { get; set; }

    //    [JsonPropertyName("processedDate")]
    //    public string? ProcessedDate { get; set; }
    //}

    public class UserEventDTO
    {
        [JsonPropertyName("userId")]
        public string? UserId { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("givenName")]       // ← era "firstname"
        public string? Firstname { get; set; }

        [JsonPropertyName("familyName")]      // ← era "lastname"
        public string? Lastname { get; set; }

        [JsonPropertyName("userName")]
        public string? UserName { get; set; }

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("externalId")]
        public string? ExternalId { get; set; }

        [JsonPropertyName("organizationId")]
        public string? OrganizationId { get; set; }

        [JsonPropertyName("userReferenceId")]
        public string? UserReferenceId { get; set; }

        [JsonPropertyName("active")]
        public bool? Active { get; set; }

        [JsonPropertyName("emails")]
        public List<EmailDTO>? Emails { get; set; }

        public string? Email => Emails?.FirstOrDefault(e => e.Primary == true)?.Value
                             ?? Emails?.FirstOrDefault()?.Value;
    }

    public class EmailDTO
    {
        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("primary")]
        public bool? Primary { get; set; }
    }

    // ─── INVITACIONES ─────────────────────────────────────────────────────────────
    // Viene en eventos type: "com.origo.mi.invitation"
    public class InvitationEventDTO
    {
        [JsonPropertyName("userId")]
        public string? UserId { get; set; }

        [JsonPropertyName("invitationCode")]
        public string? InvitationCode { get; set; }

        // ✅ Status real: INVITATION_PENDING, INVITATION_ACKNOWLEDGED, INVITATION_CANCELLED, INVITATION_EXPIRED
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("expirationTime")]
        public string? ExpirationTime { get; set; }

        [JsonPropertyName("organizationId")]
        public string? OrganizationId { get; set; }
    }

    // ─── ENDPOINTS / DISPOSITIVOS ─────────────────────────────────────────────────
    // Viene en eventos type: "origo.credential-management.events.credentialcontainer"
    public class EndpointEventDTO
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("userId")]
        public string? UserId { get; set; }

        [JsonPropertyName("externalId")]
        public string? ExternalId { get; set; }

        // ✅ Status real: CREDENTIALCONTAINER_PERSONALIZED, CREDENTIALCONTAINER_INACTIVE
        // ✅ Status real: CREDENTIALCONTAINER_PERSONALIZED, CREDENTIALCONTAINER_INACTIVE
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("statusCode")]
        public int? StatusCode { get; set; }

        [JsonPropertyName("organizationId")]
        public string? OrganizationId { get; set; }

        [JsonPropertyName("lastUpdatedBy")]
        public string? LastUpdatedBy { get; set; }

        // Propiedades del dispositivo
        public string? Model => DeviceProperties?.Model;
        public string? DeviceType => DeviceProperties?.DeviceType;
        public string? Manufacturer => DeviceProperties?.Manufacturer;

        [JsonPropertyName("deviceProperties")]
        public DevicePropertiesDTO? DeviceProperties { get; set; }

        [JsonPropertyName("platform")]
        public PlatformDTO? Platform { get; set; }
    }

    public class DevicePropertiesDTO
    {
        [JsonPropertyName("deviceType")]
        public string? DeviceType { get; set; }

        [JsonPropertyName("manufacturer")]
        public string? Manufacturer { get; set; }

        [JsonPropertyName("model")]
        public string? Model { get; set; }
    }

    public class PlatformDTO
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("storageType")]
        public string? StorageType { get; set; }
    }

    // ─── CREDENCIALES ─────────────────────────────────────────────────────────────
    // Viene en eventos type: "origo.credential-management.events.credential"
    public class CredentialEventDTO
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("userId")]
        public string? UserId { get; set; }

        [JsonPropertyName("organizationId")]
        public string? OrganizationId { get; set; }

        [JsonPropertyName("credentialTemplateId")]
        public string? CredentialTemplateId { get; set; }

        // ✅ Status real: CREDENTIAL_RESERVED, CREDENTIAL_ISSUED, CREDENTIAL_REVOKED, etc.
        // ✅ Status real: CREDENTIAL_RESERVED: 30, CREDENTIAL_ISSUED: 31, CREDENTIAL_REVOKED: 32, etc.
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("statusCode")]
        public int? StatusCode { get; set; }

        [JsonPropertyName("lastUpdatedBy")]
        public string? LastUpdatedBy { get; set; }

        // CardNumber y partNumber viven en metaData
        public int? CardNumber => MetaData?.CardNumber;
        public string? PartNumber => MetaData?.PartNumber;

        [JsonPropertyName("metaData")]
        public CredentialMetaDataDTO? MetaData { get; set; }
    }

    public class CredentialMetaDataDTO
    {
        [JsonPropertyName("cardNumber")]
        public int? CardNumber { get; set; }

        [JsonPropertyName("partNumber")]
        public string? PartNumber { get; set; }

        [JsonPropertyName("keyset")]
        public string? Keyset { get; set; }
    }

    // ─── PASSES ───────────────────────────────────────────────────────────────────
    // Viene en eventos type: "origo.credential-management.events.pass"
    public class PassEventDTO
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("userId")]
        public string? UserId { get; set; }

        [JsonPropertyName("organizationId")]
        public string? OrganizationId { get; set; }

        [JsonPropertyName("passTemplateId")]
        public string? PassTemplateId { get; set; }

        [JsonPropertyName("credentialContainerId")]
        public string? CredentialContainerId { get; set; }

        // ✅ Status real: PASS_CREATED, PASS_ACTIVE, PASS_REVOKED, etc.
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("statusCode")]
        public int? StatusCode { get; set; }

        [JsonPropertyName("deviceType")]
        public string? DeviceType { get; set; }

        [JsonPropertyName("lastUpdatedBy")]
        public string? LastUpdatedBy { get; set; }

        [JsonPropertyName("updateStatus")]
        public string? UpdateStatus { get; set; }

        // PlatformType vive en el primer elemento de credentials
        public string? PlatformType => Credentials?.FirstOrDefault()?.PlatformType;
        public int? CardNumber => Credentials?.FirstOrDefault()?.CredentialIdentifiers?.CardNumber;

        [JsonPropertyName("credentials")]
        public List<PassCredentialDTO>? Credentials { get; set; }

        [JsonPropertyName("issuanceToken")]
        public IssuanceTokenDTO? IssuanceToken { get; set; }
    }

    public class PassCredentialDTO
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("platformType")]
        public string? PlatformType { get; set; }

        [JsonPropertyName("credentialIdentifiers")]
        public CredentialIdentifiersDTO? CredentialIdentifiers { get; set; }
    }

    public class CredentialIdentifiersDTO
    {
        [JsonPropertyName("cardNumber")]
        public int? CardNumber { get; set; }

        [JsonPropertyName("hexValue")]
        public string? HexValue { get; set; }
    }

    public class IssuanceTokenDTO
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("token")]
        public string? Token { get; set; }
    }
}