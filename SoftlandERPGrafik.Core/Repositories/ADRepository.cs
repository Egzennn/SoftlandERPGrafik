using System.Collections.Concurrent;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using Microsoft.Extensions.Options;
using SoftlandERPGrafik.Core.Repositories.Interfaces;
using SoftlandERPGrafik.Data.Configurations;
using SoftlandERPGrafik.Data.Entities.Staff.AD;

namespace SoftlandERPGrafik.Core.Repositories
{
    public class ADRepository : IADRepository
    {
        private readonly ADConfiguration adConfiguration;

        public ADRepository(IOptions<ADConfiguration> adConfiguration)
        {
            this.adConfiguration = adConfiguration.Value;
        }

        public IEnumerable<ADUser>? GetAllADUsers()
        {
            try
            {
                ConcurrentBag<ADUser> adUsers = new ();

                using var context = new PrincipalContext(ContextType.Domain, this.adConfiguration?.ServerIP, this.adConfiguration?.SearchBase, this.adConfiguration?.Username, this.adConfiguration?.Password);
                using var principal = new UserPrincipal(context);
                using var searcher = new PrincipalSearcher(principal);

                Parallel.ForEach(searcher.FindAll(), user =>
                {
                    if (user?.GetUnderlyingObjectType() == typeof(DirectoryEntry))
                    {
                        using var entry = (DirectoryEntry?)(user as UserPrincipal)?.GetUnderlyingObject();

                        if (entry != null)
                        {
                            ADUser adUser = new ()
                            {
                                Id = (user as UserPrincipal)?.Guid ?? null,
                                LastName = (user as UserPrincipal)?.Surname ?? string.Empty,
                                FirstName = (user as UserPrincipal)?.GivenName ?? string.Empty,
                                Login = (user as UserPrincipal)?.SamAccountName ?? string.Empty,
                                Enabled = (user as UserPrincipal)?.Enabled ?? false,
                                EmailAddress = (user as UserPrincipal)?.EmailAddress ?? string.Empty,
                                Mobile = (user as UserPrincipal)?.VoiceTelephoneNumber ?? string.Empty,
                                PasswordNeverExpires = (user as UserPrincipal)?.PasswordNeverExpires ?? false,
                                UserCannotChangePassword = (user as UserPrincipal)?.UserCannotChangePassword ?? false,
                                AccountExpirationDate = (user as UserPrincipal)?.AccountExpirationDate ?? null,
                                UserMustChangePassword = (user as UserPrincipal)?.LastPasswordSet == null,
                                Company = entry?.Properties["company"]?.Value?.ToString() ?? string.Empty,
                                Department = entry?.Properties["department"]?.Value?.ToString() ?? string.Empty,
                                JobTitle = entry?.Properties["title"]?.Value?.ToString() ?? string.Empty,
                                DepartmentMobile = entry?.Properties["homePhone"]?.Value?.ToString() ?? string.Empty,
                                Manager = !string.IsNullOrEmpty(entry?.Properties["manager"]?.Value?.ToString()) ? UserPrincipal.FindByIdentity(context, IdentityType.DistinguishedName, entry?.Properties["manager"]?.Value?.ToString()).DisplayName : null
                            };
                            adUsers.Add(adUser);
                        }
                    }
                });

                foreach (var adUser in adUsers)
                {
                    var userprincipal = UserPrincipal.FindByIdentity(context, IdentityType.Guid, adUser.Id.ToString());
                    if (userprincipal?.GetUnderlyingObjectType() == typeof(DirectoryEntry))
                    {
                        using var entry = (DirectoryEntry?)(userprincipal as UserPrincipal)?.GetUnderlyingObject();
                    }
                }

                return adUsers.OrderBy(x => x.FirstName).ToList();
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<string>? GetAllADUserLogins()
        {
            try
            {
                List<string> logins = new ();
                using var context = new PrincipalContext(ContextType.Domain, this.adConfiguration?.ServerIP, this.adConfiguration?.SearchBase, this.adConfiguration?.Username, this.adConfiguration?.Password);
                using var principal = new UserPrincipal(context);
                using var searcher = new PrincipalSearcher(principal);

                logins.AddRange(from UserPrincipal user in searcher.FindAll()
                                where !string.IsNullOrEmpty(user?.DisplayName)
                                select user?.DisplayName);
                logins.Sort();
                return logins;
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<string>? GetAllADUserAcronyms()
        {
            try
            {
                List<string> acronyms = new ();
                using var context = new PrincipalContext(ContextType.Domain, this.adConfiguration?.ServerIP, this.adConfiguration?.SearchBase, this.adConfiguration?.Username, this.adConfiguration?.Password);
                using var principal = new UserPrincipal(context);
                using var searcher = new PrincipalSearcher(principal);

                acronyms.AddRange(from UserPrincipal user in searcher.FindAll()
                                  where !string.IsNullOrEmpty(user?.SamAccountName)
                                  select user?.SamAccountName);
                acronyms.Sort();
                return acronyms;
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<ADGroup>? GetAllADGroups()
        {
            try
            {
                var adGroups = new List<ADGroup>();

                using var context = new PrincipalContext(ContextType.Domain, this.adConfiguration?.ServerIP, this.adConfiguration?.GroupsSearchBase, this.adConfiguration?.Username, this.adConfiguration?.Password);
                using var principal = new GroupPrincipal(context);
                using var searcher = new PrincipalSearcher(principal);

                foreach (GroupPrincipal group in searcher.FindAll())
                {
                    var adGroup = new ADGroup
                    {
                        Name = group.Name,
                        Members = new List<string>()
                    };

                    foreach (var member in group.GetMembers())
                    {
                        if (member is UserPrincipal)
                        {
                            if (((UserPrincipal)member).Enabled == true)
                            {
                                adGroup.Members.Add(member.Name);
                            }
                            else
                            {
                                adGroup.Members.Add(member.Name);
                            }
                        }
                    }

                    adGroup.Members.Sort();

                    adGroups.Add(adGroup);
                }

                return adGroups.OrderBy(x => x.Name).ToList();
            }
            catch
            {
                throw;
            }
        }

        public ADUser? GetADUsersById(Guid? id)
        {
            try
            {
                if (id == null)
                {
                    return null;
                }

                using var context = new PrincipalContext(ContextType.Domain, this.adConfiguration?.ServerIP, this.adConfiguration?.SearchBase, this.adConfiguration?.Username, this.adConfiguration?.Password);
                using var user = UserPrincipal.FindByIdentity(context, IdentityType.Guid, id.ToString());

                if (user?.GetUnderlyingObjectType() == typeof(DirectoryEntry))
                {
                    using var entry = (DirectoryEntry?)(user as UserPrincipal)?.GetUnderlyingObject();

                    if (entry != null)
                    {
                        return new ADUser()
                        {
                            Id = (user as UserPrincipal)?.Guid ?? null,
                            LastName = (user as UserPrincipal)?.Surname ?? string.Empty,
                            FirstName = (user as UserPrincipal)?.GivenName ?? string.Empty,
                            Login = (user as UserPrincipal)?.SamAccountName ?? string.Empty,
                            Enabled = (user as UserPrincipal)?.Enabled ?? false,
                            EmailAddress = (user as UserPrincipal)?.EmailAddress ?? string.Empty,
                            Mobile = (user as UserPrincipal)?.VoiceTelephoneNumber ?? string.Empty,
                            PasswordNeverExpires = (user as UserPrincipal)?.PasswordNeverExpires ?? false,
                            UserCannotChangePassword = (user as UserPrincipal)?.UserCannotChangePassword ?? false,
                            AccountExpirationDate = (user as UserPrincipal)?.AccountExpirationDate ?? null,
                            UserMustChangePassword = (user as UserPrincipal)?.LastPasswordSet == null,
                            Company = entry.Properties["company"]?.Value?.ToString() ?? string.Empty,
                            Department = entry.Properties["department"]?.Value?.ToString() ?? string.Empty,
                            JobTitle = entry.Properties["title"]?.Value?.ToString() ?? string.Empty,
                            DepartmentMobile = entry.Properties["homePhone"]?.Value?.ToString() ?? string.Empty,
                            Manager = !string.IsNullOrEmpty(entry.Properties["manager"]?.Value?.ToString()) ? UserPrincipal.FindByIdentity(context, IdentityType.DistinguishedName, entry.Properties["manager"]?.Value?.ToString()).DisplayName : null
                        };
                    }
                }

                return null;
            }
            catch
            {
                throw;
            }
        }

        public ADUser? GetADUsersBySamAccountName(string? acronym)
        {
            try
            {
                if (acronym == null)
                {
                    return null;
                }

                using var context = new PrincipalContext(ContextType.Domain, this.adConfiguration?.ServerIP, this.adConfiguration?.SearchBase, this.adConfiguration?.Username, this.adConfiguration?.Password);
                using var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, acronym);


                if (user?.GetUnderlyingObjectType() == typeof(DirectoryEntry))
                {
                    using var entry = (DirectoryEntry?)(user as UserPrincipal)?.GetUnderlyingObject();

                    if (entry != null)
                    {
                        return new ADUser()
                        {
                            Id = (user as UserPrincipal)?.Guid ?? null,
                            LastName = (user as UserPrincipal)?.Surname ?? string.Empty,
                            FirstName = (user as UserPrincipal)?.GivenName ?? string.Empty,
                            Login = (user as UserPrincipal)?.SamAccountName ?? string.Empty,
                            Enabled = (user as UserPrincipal)?.Enabled ?? false,
                            EmailAddress = (user as UserPrincipal)?.EmailAddress ?? string.Empty,
                            Mobile = (user as UserPrincipal)?.VoiceTelephoneNumber ?? string.Empty,
                            PasswordNeverExpires = (user as UserPrincipal)?.PasswordNeverExpires ?? false,
                            UserCannotChangePassword = (user as UserPrincipal)?.UserCannotChangePassword ?? false,
                            AccountExpirationDate = (user as UserPrincipal)?.AccountExpirationDate ?? null,
                            UserMustChangePassword = (user as UserPrincipal)?.LastPasswordSet == null,
                            Company = entry.Properties["company"]?.Value?.ToString() ?? string.Empty,
                            Department = entry.Properties["department"]?.Value?.ToString() ?? string.Empty,
                            JobTitle = entry.Properties["title"]?.Value?.ToString() ?? string.Empty,
                            DepartmentMobile = entry.Properties["homePhone"]?.Value?.ToString() ?? string.Empty,
                            //Address = this.userAddressRepository.FindAddressByCountryCodeAsync(entry.Properties["c"]?.Value?.ToString() ?? string.Empty, entry.Properties["l"]?.Value?.ToString() ?? string.Empty, entry.Properties["streetAddress"]?.Value?.ToString() ?? string.Empty).Result ?? new UserAddress(),
                            Manager = !string.IsNullOrEmpty(entry.Properties["manager"]?.Value?.ToString()) ? UserPrincipal.FindByIdentity(context, IdentityType.DistinguishedName, entry.Properties["manager"]?.Value?.ToString()).DisplayName : null
                        };
                    }
                }

                return null;
            }
            catch
            {
                throw;
            }
        }

        public bool CreateUser(ADUser? adUser)
        {
            try
            {
                using var context = new PrincipalContext(ContextType.Domain, this.adConfiguration?.ServerIP, this.adConfiguration?.SearchBase, this.adConfiguration?.Username, this.adConfiguration?.Password);
                using var user = new UserPrincipal(context);

                if (adUser != null)
                {
                    user.Surname = adUser.LastName;
                    user.GivenName = adUser.FirstName;
                    user.Enabled = adUser.Enabled;
                    user.EmailAddress = adUser.EmailAddress;
                    user.VoiceTelephoneNumber = adUser.Mobile;
                    user.PasswordNeverExpires = adUser.PasswordNeverExpires;
                    user.UserCannotChangePassword = adUser.UserCannotChangePassword;
                    if (adUser.AccountExpirationDateCheck)
                    {
                        user.AccountExpirationDate = adUser.AccountExpirationDate;
                    }

                    user.SamAccountName = adUser.Login;
                    user.UserPrincipalName = adUser.Login + "@SOFTLAND20.PL";

                    user.Name = adUser.FirstName + " " + adUser.Login + " " + adUser.LastName;
                    user.DisplayName = adUser.FirstName + " " + adUser.Login + " " + adUser.LastName;

                    if (adUser.UserCannotChangePassword)
                    {
                        user.ExpirePasswordNow();
                    }

                    if (this.adConfiguration?.DefaultPassword != null)
                    {
                        adUser.Password ??= this.adConfiguration?.DefaultPassword;
                    }

                    user.SetPassword(adUser.Password);
                    user.Save();

                    using var entry = (DirectoryEntry)user.GetUnderlyingObject();

                    entry.Properties["company"].Value = adUser.Company;
                    entry.Properties["department"].Value = adUser.Department;
                    entry.Properties["title"].Value = adUser.JobTitle;
                    entry.Properties["initials"].Value = adUser.Login;
                    entry.Properties["department"].Value = adUser.Department;
                    entry.Properties["mobile"].Value = adUser.Mobile;
                    entry.Properties["homePhone"].Value = adUser.DepartmentMobile;
                    entry.Properties["manager"].Value = UserPrincipal.FindByIdentity(context, adUser.Manager)?.DistinguishedName;
                    entry.CommitChanges();
                    user.Save();

                    return true;
                }

                return false;
            }
            catch
            {
                throw;
            }
        }

        public bool UpdateUser(ADUser? adUser)
        {
            try
            {
                using var context = new PrincipalContext(ContextType.Domain, this.adConfiguration?.ServerIP, this.adConfiguration?.SearchBase, this.adConfiguration?.Username, this.adConfiguration?.Password);
                using var user = UserPrincipal.FindByIdentity(context, IdentityType.Guid, adUser?.Id.ToString());

                if (adUser != null)
                {
                    user.Surname = adUser.LastName;
                    user.GivenName = adUser.FirstName;
                    user.Enabled = adUser.Enabled;
                    user.EmailAddress = adUser.EmailAddress;
                    user.VoiceTelephoneNumber = adUser.Mobile;
                    user.PasswordNeverExpires = adUser.PasswordNeverExpires;
                    user.UserCannotChangePassword = adUser.UserCannotChangePassword;
                    if (adUser.AccountExpirationDateCheck)
                    {
                        user.AccountExpirationDate = adUser.AccountExpirationDate;
                    }

                    user.DisplayName = adUser.FirstName + " " + user.SamAccountName + " " + adUser.LastName;

                    if (adUser.UserCannotChangePassword)
                    {
                        user.ExpirePasswordNow();
                    }

                    user.Save();

                    using var entry = (DirectoryEntry)user.GetUnderlyingObject();

                    entry.Rename("CN=" + adUser.FirstName + " " + user.SamAccountName + " " + adUser.LastName);
                    entry.Properties["company"].Value = adUser.Company;
                    entry.Properties["department"].Value = adUser.Department;
                    entry.Properties["title"].Value = adUser.JobTitle;
                    entry.Properties["department"].Value = adUser.Department;
                    entry.Properties["mobile"].Value = adUser.Mobile;
                    entry.Properties["homePhone"].Value = adUser.DepartmentMobile;
                    entry.Properties["manager"].Value = UserPrincipal.FindByIdentity(context, adUser.Manager)?.DistinguishedName;
                    entry.CommitChanges();
                    user.Save();

                    return true;
                }

                return false;
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<string>? GetAllADGroupsName()
        {
            List<string> groups = new ();
            try
            {
                using var context = new PrincipalContext(ContextType.Domain, this.adConfiguration?.ServerIP, this.adConfiguration?.GroupsSearchBase, this.adConfiguration?.Username, this.adConfiguration?.Password);
                using var principal = new GroupPrincipal(context);
                using var searcher = new PrincipalSearcher(principal);
                groups.AddRange(from GroupPrincipal item in searcher.FindAll() where item?.Name != null && item?.Name.StartsWith("O_", StringComparison.InvariantCulture) == false select item.Name);
                groups.Sort();
            }
            catch
            {
                throw;
            }

            return groups;
        }

        public ADGroup? GetADGroupById(Guid? id)
        {
            throw new NotImplementedException();
        }

        public List<string> GetAllADGroupsByUser(string? login)
        {
            try
            {
                var groupsForUser = new List<string>();

                using var context = new PrincipalContext(ContextType.Domain, this.adConfiguration?.ServerIP, this.adConfiguration?.Username, this.adConfiguration?.Password);
                using var user = UserPrincipal.FindByIdentity(context, login);

                if (user != null)
                {
                    var allGroups = this.GetAllADGroups();

                    foreach (var group in allGroups)
                    {
                        if (group.Members.Contains(user.DisplayName))
                        {
                            groupsForUser.Add(group.Name);
                        }
                    }
                }

                return groupsForUser;
            }
            catch
            {
                throw;
            }
        }

        public bool CheckGroup(string? login, List<string> groups)
        {
            try
            {
                using var context = new PrincipalContext(ContextType.Domain, this.adConfiguration?.ServerIP, this.adConfiguration?.Username, this.adConfiguration?.Password);
                using var user = UserPrincipal.FindByIdentity(context, login);

                if (user != null)
                {
                    foreach (string group in groups)
                    {
                        if (user.IsMemberOf(GroupPrincipal.FindByIdentity(context, group)))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            catch
            {
                throw;
            }
        }

        //public bool CheckMembership(string? login, string? applicationModule)
        //{
        //    try
        //    {
        //        using var context = new PrincipalContext(ContextType.Domain, this.adConfiguration?.ServerIP, this.adConfiguration?.Username, this.adConfiguration?.Password);
        //        using var user = UserPrincipal.FindByIdentity(context, login);

        //        if (user != null)
        //        {
        //            if (user.IsMemberOf(GroupPrincipal.FindByIdentity(context, "S_ADM_IT")))
        //            {
        //                return true;
        //            }

        //            List<ApplicationPermissionsRule>? permissions = this.permissionRulesRepository.GetAllAsync().Result?.Where(x => x.Module == applicationModule).ToList();

        //            if (permissions?.Any() == true)
        //            {
        //                foreach (var permission in permissions)
        //                {
        //                    if (permission.ManagerOnly)
        //                    {
        //                        var group = GroupPrincipal.FindByIdentity(context, permission.ADGroupDisplayName);

        //                        using var entry = (DirectoryEntry)group.GetUnderlyingObject();

        //                        var manager = entry?.Properties["managedBy"]?.Value?.ToString();

        //                        if (user.DistinguishedName == manager)
        //                        {
        //                            return true;
        //                        }
        //                    }
        //                    else if (user.IsMemberOf(GroupPrincipal.FindByIdentity(context, permission.ADGroupDisplayName)))
        //                    {
        //                        return true;
        //                    }
        //                }
        //            }
        //        }

        //        return false;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        public bool CheckLogin(string? login)
        {
            try
            {
                using var context = new PrincipalContext(ContextType.Domain, this.adConfiguration?.ServerIP, this.adConfiguration?.SearchBase, this.adConfiguration?.Username, this.adConfiguration?.Password);
                return UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, login) == null;
            }
            catch
            {
                throw;
            }
        }

        public bool ResetPassword(ADUser? adUser)
        {
            try
            {
                using var context = new PrincipalContext(ContextType.Domain, this.adConfiguration?.ServerIP, this.adConfiguration?.SearchBase, this.adConfiguration?.Username, this.adConfiguration?.Password);
                using var user = UserPrincipal.FindByIdentity(context, IdentityType.Guid, adUser?.Id.ToString());

                if (this.adConfiguration?.DefaultPassword != null)
                {
                    user.SetPassword(this.adConfiguration?.DefaultPassword);
                }
                else
                {
                    return false;
                }

                if (adUser?.PasswordNeverExpires == true)
                {
                    user.PasswordNeverExpires = true;
                }

                if (adUser?.UserMustChangePassword == true && adUser.PasswordNeverExpires != true)
                {
                    user.ExpirePasswordNow();
                }

                user.Save();

                return true;
            }
            catch
            {
                throw;
            }
        }

        public bool ChangeStatus(ADUser? adUser)
        {
            try
            {
                using var context = new PrincipalContext(ContextType.Domain, this.adConfiguration?.ServerIP, this.adConfiguration?.SearchBase, this.adConfiguration?.Username, this.adConfiguration?.Password);
                using var user = UserPrincipal.FindByIdentity(context, IdentityType.Guid, adUser?.Id.ToString());

                user.Enabled = !user.Enabled;

                user.Save();

                return true;
            }
            catch
            {
                throw;
            }
        }

        public int GetUsersCount()
        {
            try
            {
                using var context = new PrincipalContext(ContextType.Domain, this.adConfiguration?.ServerIP, this.adConfiguration?.SearchBase, this.adConfiguration?.Username, this.adConfiguration?.Password);
                using var principal = new UserPrincipal(context);
                using var searcher = new PrincipalSearcher(principal);

                return searcher.FindAll().Count();
            }
            catch
            {
                throw;
            }
        }

        public int GetComputersCount()
        {
            try
            {
                using var context = new PrincipalContext(ContextType.Domain, this.adConfiguration?.ServerIP, this.adConfiguration?.Username, this.adConfiguration?.Password);
                using var principal = new ComputerPrincipal(context);
                using var searcher = new PrincipalSearcher(principal);

                return searcher.FindAll().Count();
            }
            catch
            {
                throw;
            }
        }

        public string GetUserAcronym(string username)
        {
            try
            {
                using var context = new PrincipalContext(ContextType.Domain, this.adConfiguration?.ServerIP, this.adConfiguration?.SearchBase, this.adConfiguration?.Username, this.adConfiguration?.Password);
                using var user = UserPrincipal.FindByIdentity(context, username);

                return user?.SamAccountName ?? string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string GetUserFirstLastName(string username)
        {
            try
            {
                using var context = new PrincipalContext(ContextType.Domain, this.adConfiguration?.ServerIP, this.adConfiguration?.SearchBase, this.adConfiguration?.Username, this.adConfiguration?.Password);
                using var user = UserPrincipal.FindByIdentity(context, username);

                return user?.GivenName + ' ' + user?.Surname ?? string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}