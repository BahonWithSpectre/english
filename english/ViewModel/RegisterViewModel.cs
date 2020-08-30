using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace english.ViewModel
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Номер телефона")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }




        [Required]
        [MinLength(6, ErrorMessage = "Құпиясөз 6 символдан кем болмауы тиіс!")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Құпиясөздер сәйкес келмейді!")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]


        public string PasswordConfirm { get; set; }
    }
}
