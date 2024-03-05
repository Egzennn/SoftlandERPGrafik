using SoftlandERPGrafik.Core.Repositories.Interfaces;
using SoftlandERPGrafik.Data.DB;
using SoftlandERPGrafik.Data.Entities.Forms.Data;
using SoftlandERPGrafik.Data.Entities.Views;
using SoftlandERPGrafik.Data.Entities.Vocabularies.Forms.Ogolne;

namespace SoftlandERPGrafik.Web.Components.Services
{
    public class BaseService : ServiceCollection
    {
        public readonly MainContext mainContext;
        public readonly IADRepository adRepository;
        public readonly ILogger<BaseService> logger;
        public readonly UserDetailsService userDetailsService;
        public readonly IRepository<OrganizacjaLokalizacje> lokalizacjeRepository;
        public readonly IRepository<ZatrudnieniDzialy> dzialyRepository;
        public readonly IRepository<ZatrudnieniZrodlo> zrodloRepository;
        public readonly IRepository<OgolneStan> stanRepository;
        public readonly IRepository<OgolneStatus> statusRepository;

        public BaseService(MainContext mainContext, IADRepository adRepository, ILogger<BaseService> logger, UserDetailsService userDetailsService, IRepository<OrganizacjaLokalizacje> lokalizacjeRepository, IRepository<ZatrudnieniDzialy> dzialyRepository, IRepository<ZatrudnieniZrodlo> zrodloRepository, IRepository<OgolneStan> stanRepository, IRepository<OgolneStatus> statusRepository)
            : base()
        {
            this.mainContext = mainContext;
            this.adRepository = adRepository;
            this.logger = logger;
            this.userDetailsService = userDetailsService;
            this.lokalizacjeRepository = lokalizacjeRepository;
            this.dzialyRepository = dzialyRepository;
            this.zrodloRepository = zrodloRepository;
            this.stanRepository = stanRepository;
            this.statusRepository = statusRepository;
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

        public async Task<List<OsobaData>> GetEmployeesAsync()
        {
            var employees = await this.zrodloRepository.GetAllAsync();

            return employees.Select(zatrudniony => new OsobaData(zatrudniony)).OrderBy(dzial => dzial.DZL_Kod).ThenBy(osoba => osoba.Imie_Nazwisko).ToList();
        }

        public async Task<IEnumerable<ZatrudnieniDzialy>> GetDzialyAsync() => await this.dzialyRepository.GetAllAsync();

        public async Task<IEnumerable<OrganizacjaLokalizacje>> GetLocalizationAsync() => await this.lokalizacjeRepository.GetAllAsync();

        public async Task<List<string>> GetStanAsync()
        {
            var stany = await this.stanRepository.GetAllAsync();

            var ogolneStany = stany
                .OfType<OgolneStan>()
                .Where(s => (s as OgolneStan)?.Stan == "Aktywny")
                .OrderBy(s => s?.Wartosc)
                .Select(s => s?.Wartosc)
                .ToList();

            return ogolneStany;
        }

        public async Task<List<string>> GetStatusAsync()
        {
            var statusy = await this.statusRepository.GetAllAsync();

            var ogolneStany = statusy
                .OfType<OgolneStatus>()
                .Where(s => (s as OgolneStatus)?.Stan == "Aktywny")
                .OrderBy(s => s?.Wartosc)
                .Select(s => s?.Wartosc)
                .ToList();

            return ogolneStany;
        }

    }
}
