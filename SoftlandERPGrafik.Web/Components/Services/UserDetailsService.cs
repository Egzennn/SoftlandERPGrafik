// UserDetailsService.cs
using Microsoft.AspNetCore.Components.Authorization;
using SoftlandERPGrafik.Data.Configurations;
using SoftlandERPGrafik.Data.Entities.Staff.AD;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SoftlandERPGrafik.Web.Components.Services
{
    public class UserDetailsService
    {
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly ADConfiguration adConfiguration;

        public UserDetailsService(AuthenticationStateProvider authenticationStateProvider, ADConfiguration adConfiguration)
        {
            this.authenticationStateProvider = authenticationStateProvider;
            this.adConfiguration = adConfiguration;
        }

        public async Task<UserDetails> GetUserDetailsAsync()
        {
            try
            {
                var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;

                using var context = new PrincipalContext(ContextType.Domain, adConfiguration?.ServerIP, adConfiguration?.SearchBase, adConfiguration?.Username, adConfiguration?.Password);
                using var adUser = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, user?.Identity?.Name);

                var userDetails = new UserDetails
                {
                    DisplayName = adUser.DisplayName.ToString(),
                    JobTitle = string.Empty,
                };

                if (adUser?.GetUnderlyingObjectType() == typeof(DirectoryEntry))
                {
                    using var entry = (DirectoryEntry?)adUser?.GetUnderlyingObject();
                    userDetails.JobTitle = entry?.Properties["title"]?.Value?.ToString() ?? string.Empty;
                }

                return userDetails;
            }
            catch (Exception ex)
            {
                var user = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
                return new UserDetails
                {
                    DisplayName = user?.Identity?.Name,
                    JobTitle = "Developer"
                };
            }
        }

        public async Task<UserPrincipal?> GetUserAllDetailsAsync()
        {
            try
            {
                var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;

                using var context = new PrincipalContext(ContextType.Domain);
                var adUser = UserPrincipal.FindByIdentity(context, user?.Identity?.Name);

                return adUser;
            }
            catch
            {
                return null;
            }
        }
    }

    public class UserDetails
    {
        public string? DisplayName { get; set; }
        public string? JobTitle { get; set; }
    }
}