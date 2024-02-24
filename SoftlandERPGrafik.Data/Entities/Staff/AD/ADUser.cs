using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace SoftlandERPGrafik.Data.Entities.Staff.AD
{
    public class ADUser
    {
        public Guid? Id { get; set; }

        [Display(Name = "Imię")]
        [Required(ErrorMessage = "Imię jest wymagane")]
        [RegularExpression("^[a-zA-ZąćęłńóśźżĄĘŁŃÓŚŹŻ]+$", ErrorMessage = "Dozwolone tylko litery")]
        public string? FirstName { get; set; }

        [Display(Name = "Nazwisko")]
        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        [RegularExpression("^[a-zA-ZąćęłńóśźżĄĘŁŃÓŚŹŻ-]+$", ErrorMessage = "Dozwolone tylko litery i znak \"-\"")]
        public string? LastName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email jest wymagany")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Adres email jest niepoprawny")]
        public string? EmailAddress { get; set; }

        [Display(Name = "Telefon stanowiskowy")]
        [RegularExpression(@"\(?\+[0-9]{1,3}\)? ?-?[0-9]{1,3} ?-?[0-9]{3,5} ?-?[0-9]{4}( ?-?[0-9]{3})? ?(\w{1,10}\s?\d{1,6})?/?[0-9]+$", ErrorMessage = "Telefon jest niepoprawny. Przykład poprawnego telefonu: +48123456798")]
        [DataType(DataType.PhoneNumber)]
        public string? Mobile { get; set; }

        [Display(Name = "Telefon działowy")]
        [RegularExpression(@"\(?\+[0-9]{1,3}\)? ?-?[0-9]{1,3} ?-?[0-9]{3,5} ?-?[0-9]{4}( ?-?[0-9]{3})? ?(\w{1,10}\s?\d{1,6})?/?[0-9]+$", ErrorMessage = "Telefon jest niepoprawny. Przykład poprawnego telefonu: +48123456798")]
        [DataType(DataType.PhoneNumber)]
        public string? DepartmentMobile { get; set; }

        [Display(Name = "Akronim")]
        [Required(ErrorMessage = "Akronim jest wymagany")]
        [RegularExpression("^[A-Z]{3}$", ErrorMessage = "Dozwolone tylko 3 duże litery alfabetu łacińskiego")]
        [Remote("CheckLogin", "AD", ErrorMessage = "Taki akronim już istnieje")]
        public string? Login { get; set; }

        [Display]
        public bool AccountExpirationDateCheck { get; set; } = false;

        [DataType(DataType.Date)]
        [Display(Name = "Data wygaśnieńcia konta")]
        public DateTime? AccountExpirationDate { get; set; }

        [Display(Name = "Hasło")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Czy jest aktywny?")]
        [Required(ErrorMessage = "Pole jest wymagany")]
        public bool Enabled { get; set; } = true;

        [Display(Name = "Haslo nigdy nie wygasa")]
        public bool PasswordNeverExpires { get; set; } = true;

        [Display(Name = "Użytkownik musi zmienić hasło przy następnym logowaniu")]
        public bool UserMustChangePassword { get; set; }

        [Display(Name = "Użytkownik nie może zmienić hasło")]
        public bool UserCannotChangePassword { get; set; }

        [Display(Name = "Firma")]
        public string? Company { get; set; }

        [Display(Name = "Dział")]
        public string? Department { get; set; }

        [Display(Name = "Stanowisko")]
        public string? JobTitle { get; set; }

        [Display(Name = "Przyłożony")]
        public string? Manager { get; set; }
    }
}