using SoftlandERPGrafik.Data.Entities.Staff.AD;

namespace SoftlandERPGrafik.Core.Repositories.Interfaces
{
    public interface IADRepository
    {
        IEnumerable<ADUser>? GetAllADUsers();

        IEnumerable<string>? GetAllADUserLogins();

        IEnumerable<string>? GetAllADUserAcronyms();

        IEnumerable<ADGroup>? GetAllADGroups();

        ADUser? GetADUsersById(Guid? id);

        ADUser? GetADUsersBySamAccountName(string? acronym);

        bool CreateUser(ADUser? adUser);

        bool UpdateUser(ADUser? adUser);

        IEnumerable<string>? GetAllADGroupsName();

        ADGroup? GetADGroupById(Guid? id);

        List<string> GetAllADGroupsByUser(string? login);

        bool CheckGroup(string? login, List<string> groups);

        //bool CheckMembership(string? login, string? applicationModule);

        bool CheckLogin(string? login);

        bool ResetPassword(ADUser? adUser);

        bool ChangeStatus(ADUser? adUser);

        int GetUsersCount();

        int GetComputersCount();

        string GetUserAcronym(string username);

        string GetUserFirstLastName(string username);
    }
}