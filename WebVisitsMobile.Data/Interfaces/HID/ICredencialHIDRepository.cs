using System.Linq.Expressions;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.HID;

namespace WebVisitsMobile.Data.Interfaces.HID
{
    public interface ICredencialHIDRepository : IRepository<CredencialHid>
    {
        Task<CredencialHid> GetCredentialHID(Expression<Func<CredencialHid, bool>> predicate);
        Task<IEnumerable<CredencialHid>> GetAllCredentialHID();
        Task<string?> GetCredencialWalletMasReciente(Guid licenciaHidUserId);
        Task<string?> GetCredencialOrigoMasReciente(Guid licenciaHidUserId);

        Task<CredencialHid> GetCredentialHIDExternalId(Guid id);

        Task<string?> GetCredencialWalletMasRecienteWatch(Guid licenciaHidUserId);


    }
}