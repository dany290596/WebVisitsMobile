using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Models.Configuracion.Configuraciones;
using WebVisitsMobile.Models.HID.UserHID;
using WebVisitsMobile.Services.Interfaces.HID;

namespace WebVisitsMobile.Services.Services.HID
{
    public class HIDService : IHIDService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public HIDService(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<TokenHID?> GetTokenHIDAsync(AppSettingDTO configuracionHID)
        {
            try
            {
                CancellationToken ct = default;
                var urlHid = $"{configuracionHID.IdpAuthenticationUrl}/authentication/customer/{configuracionHID.CustomerId}/token";
                var body = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", configuracionHID.ClientId),
                    new KeyValuePair<string, string>("client_secret", configuracionHID.ClientSecretOrCertificate),
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                });

                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(30);
                var response = await client.PostAsync(urlHid, body, ct);
                var content = await response.Content.ReadAsStringAsync(ct);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                if (!string.IsNullOrWhiteSpace(content))
                {
                    return JsonSerializer.Deserialize<TokenHID>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                return null;
            }
            catch (HttpRequestException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<UserMapperHIDDTO?> GetAllUserAsync(AppSettingDTO configuracionHID, int userHIDId, string tokenHID)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                var url = $"{configuracionHID.ApiUrl}/customer/{configuracionHID.CustomerId}/users/{userHIDId}";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenHID);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.assaabloy.ma.credential-management-2.2+json"));
                client.DefaultRequestHeaders.Add("Application-Id", configuracionHID.ApplicationId);
                client.DefaultRequestHeaders.Add("Application-Version", configuracionHID.ApplicationVersion);

                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error en la API: {response.StatusCode} - {content}");
                    return null;
                }

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<UserMapperHIDDTO>(content, jsonOptions);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetAllUserAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteCredentialAsync(AppSettingDTO configuracionHID, int credentialHIDId, string tokenHID)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Delete, $"{configuracionHID.ApiUrl}/customer/{configuracionHID.CustomerId}/credential/{credentialHIDId}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenHID);
                request.Headers.Add("Application-Version", configuracionHID.ApplicationVersion);
                request.Headers.Add("Application-Id", configuracionHID.ApplicationId);
                request.Headers.TryAddWithoutValidation(
                    "Content-Type",
                    "application/vnd.assaabloy.ma.credential-management-2.2+json"
                );

                var response = await client.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteInvitationAsync: " + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("Inner: " + ex.InnerException.Message);
                return false;
            }
        }

        public async Task<bool> DeleteInvitationAsync(AppSettingDTO configuracionHID, int invitationHIDId, string tokenHID)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Delete, $"{configuracionHID.ApiUrl}/customer/{configuracionHID.CustomerId}/invitation/{invitationHIDId}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenHID);
                request.Headers.Add("Application-Version", configuracionHID.ApplicationVersion);
                request.Headers.Add("Application-Id", configuracionHID.ApplicationId);
                request.Headers.TryAddWithoutValidation(
                    "Content-Type",
                    "application/vnd.assaabloy.ma.credential-management-2.2+json"
                );

                var response = await client.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteInvitationAsync: " + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("Inner: " + ex.InnerException.Message);
                return false;
            }
        }

        public async Task<InvitacionYCredencialHIDDTO?> AddInvitationAsync(AppSettingDTO configuracionHID, int userHIDId, string tokenHID)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                var requestBody = new InvitacionYCredencialHIDCrearDTO
                {
                    Schemas = new[] { "urn:hid:scim:api:ma:2.2:UserAction" },
                    UserActionDetails = new UserActionDetails
                    {
                        CreateInvitationCode = "Y",
                        SendInvitationEmail = "N",
                        AssignCredential = "Y",
                        PartNumber = configuracionHID.PartNumberField,
                        Credential = ""
                    },
                    Meta = new MetaIYC
                    {
                        ResourceType = "PACSUser"
                    }
                };

                var url = $"{configuracionHID.ApiUrl}/customer/{configuracionHID.CustomerId}/users/{userHIDId}/invitation";
                using var request = new HttpRequestMessage(HttpMethod.Post, url);

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenHID);
                request.Headers.Add("Application-Id", configuracionHID.ApplicationId);
                request.Headers.Add("Application-Version", configuracionHID.ApplicationVersion);

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                };

                request.Content = new StringContent(
                    JsonSerializer.Serialize(requestBody, jsonOptions),
                    Encoding.UTF8,
                    "application/vnd.assaabloy.ma.credential-management-2.2+json"
                );

                var mediaType = request.Content.Headers.ContentType;
                mediaType!.Parameters.Clear();

                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error API: {response.StatusCode} - {content}");
                    return null;
                }

                var responseDto = JsonSerializer.Deserialize<InvitacionYCredencialHIDDTO>(
                    content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                return responseDto ?? null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AddInvitationAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SendInvitationCodeByEmailAsync(AppSettingDTO configuracionHID, int invitationHIDId, string tokenHID)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                var url = $"{configuracionHID.ApiUrl}/customer/{configuracionHID.CustomerId}/invitation/{invitationHIDId}/email";
                using var request = new HttpRequestMessage(HttpMethod.Post, url);

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenHID);
                request.Headers.Add("Application-Id", configuracionHID.ApplicationId);
                request.Headers.Add("Application-Version", configuracionHID.ApplicationVersion);

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                };

                request.Content = new StringContent(
                    JsonSerializer.Serialize(jsonOptions),
                    Encoding.UTF8,
                    "application/vnd.assaabloy.ma.credential-management-2.2+json"
                );

                var mediaType = request.Content.Headers.ContentType;
                mediaType!.Parameters.Clear();

                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error API: {response.StatusCode} - {content}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AddInvitationAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(AppSettingDTO configuracionHID, UserHIDEliminarDTO licenciaUserHIDEliminar, string tokenHID)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Delete, $"{configuracionHID.ApiUrl}/customer/{configuracionHID.CustomerId}/users/{licenciaUserHIDEliminar.UserId}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenHID);
                request.Headers.Add("Application-Version", configuracionHID.ApplicationVersion);
                request.Headers.Add("Application-Id", configuracionHID.ApplicationId);
                request.Headers.TryAddWithoutValidation(
                    "Content-Type",
                    "application/vnd.assaabloy.ma.credential-management-2.2+json"
                );

                var response = await client.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteInvitationAsync: " + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("Inner: " + ex.InnerException.Message);
                return false;
            }
        }

        public async Task<CustomerDTO?> GetCustomerAsync(AppSettingDTO configuracionHID, string tokenHID)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                var url = $"{configuracionHID.ApiUrl}/customer/{configuracionHID.CustomerId}";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenHID);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.assaabloy.ma.credential-management-2.2+json"));
                client.DefaultRequestHeaders.Add("Application-Id", configuracionHID.ApplicationId);
                client.DefaultRequestHeaders.Add("Application-Version", configuracionHID.ApplicationVersion);

                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error en la API: {response.StatusCode} - {content}");
                    return null;
                }

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<CustomerDTO>(content);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetAllUserAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<SDKVersionDTO?> GetSdkVersionAsync(AppSettingDTO configuracionHID, string tokenHID)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                var url = $"{configuracionHID.ApiUrl}/customer/{configuracionHID.CustomerId}/sdk-version";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenHID);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.assaabloy.ma.credential-management-2.2+json"));
                client.DefaultRequestHeaders.Add("Application-Id", configuracionHID.ApplicationId);
                client.DefaultRequestHeaders.Add("Application-Version", configuracionHID.ApplicationVersion);

                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error en la API: {response.StatusCode} - {content}");
                    return null;
                }

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<SDKVersionDTO>(content);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetAllUserAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<UserMapperRespHIDDTO?> CreateUserAsync(AppSettingDTO settingHID, LicenciaHidUser userHID, string tokenHID)
        {
            using var client = _httpClientFactory.CreateClient();
            var userHIDRequest = new UserMapperCrearHIDDTO
            {
                ExternalId = userHID.Id.ToString(), // Usar el ID de la tarea o generar uno único
                Name = new UserName
                {
                    FamilyName = userHID.Apellidos,
                    GivenName = userHID.Nombre
                },
                Emails = new List<UserEmail>
                {
                    new UserEmail { Value = userHID.Email }
                },
                UserAction = new UserActionSchema
                {
                    PartNumber = settingHID.PartNumberField // MID-SUB-CRD_FTPN_644745
                }
            };

            var userHIDUrl = $"{settingHID.ApiUrl}/customer/{settingHID.CustomerId}/users";
            using var userHIDHttpClientRequest = new HttpRequestMessage(HttpMethod.Post, userHIDUrl);

            userHIDHttpClientRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenHID);
            userHIDHttpClientRequest.Headers.Add("Application-Version", settingHID.ApplicationVersion);
            userHIDHttpClientRequest.Headers.Add("Application-Id", settingHID.ApplicationId);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            userHIDHttpClientRequest.Content = new StringContent(
            JsonSerializer.Serialize(userHIDRequest, jsonOptions),
            Encoding.UTF8,
            "application/vnd.assaabloy.ma.credential-management-2.2+json");

            var mediaType = userHIDHttpClientRequest.Content.Headers.ContentType;
            mediaType!.Parameters.Clear();

            try
            {
                var userHIDResponse = await client.SendAsync(userHIDHttpClientRequest);
                var userHIDResponseContent = await userHIDResponse.Content.ReadAsStringAsync();
                if (!userHIDResponse.IsSuccessStatusCode)
                {
                    return null;
                }

                var userHIDModel = JsonSerializer.Deserialize<UserMapperRespHIDDTO>(userHIDResponseContent);
                if (userHIDModel != null)
                {
                    return userHIDModel != null ? userHIDModel : null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return null;
        }

        public async Task<UserHIDAtributosDTO?> UpdateUserAsync(AppSettingDTO settingHID, UserHIDEditarAtributosDTO attributesDTO, string tokenHID)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                var requestBody = new UserMapperEditarHIDDTO
                {
                    Schemas = new[] { "urn:ietf:params:scim:schemas:core:2.0:User" },
                    ExternalId = attributesDTO.UserExternalId,
                    Name = new Name
                    {
                        FamilyName = attributesDTO.UserFamilyName,
                        GivenName = attributesDTO.UserGivenName
                    },
                    Emails = new List<Email>
                    {
                        new Email { Value = attributesDTO.UserEmail }
                    },
                    Meta = new MetaUserHIDEditar
                    {
                        ResourceType = "PACSUser"
                    }
                };

                var url = $"{settingHID.ApiUrl}/customer/{settingHID.CustomerId}/users/{attributesDTO.UserId}";
                using var request = new HttpRequestMessage(HttpMethod.Put, url);

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenHID);
                request.Headers.Add("Application-Id", settingHID.ApplicationId);
                request.Headers.Add("Application-Version", settingHID.ApplicationVersion);

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                string jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
                Console.WriteLine("JSON generado (para pruebas):");
                Console.WriteLine(jsonString);

                var utf8WithoutBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

                var content = new StringContent(jsonString, utf8WithoutBom);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.assaabloy.ma.credential-management-2.2+json");

                request.Content = content;

                var response = await client.SendAsync(request);
                var contentString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error API: {response.StatusCode} - {contentString}");
                    return null;
                }

                var responseDto = JsonSerializer.Deserialize<UserHIDAtributosDTO>(
                    contentString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return responseDto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateUserAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<int> GetUserByEmailAsync(AppSettingDTO settingHID, string email, string tokenHID)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                var requestBody = new SearchUserHIDDTO
                {
                    Schemas = new List<string> { "urn:ietf:params:scim:api:messages:2.0:SearchRequest" },
                    Attributes = new List<string>
                    {
                        "urn:ietf:params:scim:schemas:core:2.0:User:emails",
                        "name.familyName",
                        "name.givenName"
                    },
                    Filter = $"emails eq \"{email}\"",
                    SortOrder = "descending",
                    StartIndex = 1,
                    Count = 10
                };
                var url = $"{settingHID.ApiUrl}/customer/{settingHID.CustomerId}/users/.search";
                using var request = new HttpRequestMessage(HttpMethod.Post, url);

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenHID);
                request.Headers.Add("Application-Id", settingHID.ApplicationId);
                request.Headers.Add("Application-Version", settingHID.ApplicationVersion);

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                string jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);

                var utf8WithoutBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

                var content = new StringContent(jsonString, utf8WithoutBom);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.assaabloy.ma.credential-management-2.2+json");

                request.Content = content;

                var response = await client.SendAsync(request);
                var contentString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error API: {response.StatusCode} - {contentString}");
                    return 0;
                }

                var responseDto = JsonSerializer.Deserialize<SearchUserHIDDTO>(
                    contentString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (responseDto == null)
                {
                    Console.WriteLine("[HID] Error al deserializar la respuesta.");
                    return 0;
                }

                if (responseDto.Resources == null || responseDto.Resources.Count() == 0)
                {
                    Console.WriteLine("[HID] No se encontraron usuarios con ese email.");
                    return 0;
                }

                return responseDto.Resources.Count();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HID] Excepción en GetUserCountByEmailAsync: {ex.Message}");
                return 0;
            }
        }
    }
}