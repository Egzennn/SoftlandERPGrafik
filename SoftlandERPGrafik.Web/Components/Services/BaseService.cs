using SoftlandERPGrafik.Core.Repositories.Interfaces;

namespace SoftlandERPGrafik.Web.Components.Services
{
    public class BaseService : ServiceCollection
    {
        protected readonly IADRepository adRepository;
        protected readonly ILogger<BaseService> logger;

        public BaseService(IADRepository adRepository, ILogger<BaseService> logger)
            : base()
        {
            this.adRepository = adRepository;
            this.logger = logger;
        }

        //        protected bool CheckPermission(string module)
        //        {
        //#if DEBUG
        //            return true;
        //#else
        //            ClearCookiesOnExit();
        //            return this.adRepository.CheckMembership(this.User?.Identity?.Name ?? string.Empty, module);
        //#endif
        //        }

        protected string GetSignedInDisplayName(string? username)
        {
            if (username == null)
            {
                return string.Empty;
            }

            return adRepository.GetUserAcronym(username);
        }

        protected string GetSignedInFirstLastName(string? username)
        {
            if (username == null)
            {
                return string.Empty;
            }

            return adRepository.GetUserFirstLastName(username);
        }
    }
}
