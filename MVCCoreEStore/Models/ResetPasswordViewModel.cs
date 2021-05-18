﻿using System.ComponentModel.DataAnnotations;

namespace MVCCoreEStore.Models
{
    public class ResetPasswordViewModel
    {
        [Display(Name = "E-Posta")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        [EmailAddress(ErrorMessage = "Lütfen geçerli bir e-posta adresi yazınız!")]
        [DataType(DataType.EmailAddress)]
        public string UserName { get; set; }
    }
}
