namespace WebVisitsMobile.Models.Empresa.EmpresaCliente
{
    public class TestConnectionDTO
    {
        // CN01
        public string CustomerId { get; set; } = default!;
        public string ClientId { get; set; } = default!;
        public string ClientSecretOrCertificate { get; set; } = default!;

        // CN02
        public string IdpAuthenticationUrl { get; set; } = default!;
    }
}