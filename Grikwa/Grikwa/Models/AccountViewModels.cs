using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
//using System.Web.Mvc;

namespace Grikwa.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
    }

    public class ManageProfileViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Title")]
        public Title Title { get; set; }

        [Required]
        [MaxLength(3)]
        [Display(Name = "Initials")]
        public string Initials { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Required]
        public bool HasPicture { get; set; }

        public System.Web.Mvc.SelectList TitleSelectList { get; set; }

        [FileSize(5243000)]
        [FileType("jpg,jpeg,png")]
        [Display(Name = "Replace Profile Picture With")]
        public virtual HttpPostedFileBase ProfileImage { get; set; }
    }

    public class ManagePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RequestTokenModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Eneter Student Email")]
        public string Email { get; set; }
    }

    public class SendBulkTokenEmailModel
    {
        public string TitleID { get; set; }
        public string Intials { get; set; }
        public string Surname { get; set; }
        public string UserName { get; set; }
        public string TokenID { get; set; }
        public string Email { get; set; }
        public DateTime IssueDate { get; set; }
    }

    public class ResetPasswordViewModel
    {

        [Required]
        public string Token { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {

        public IEnumerable<Institution> Institutions { get; set; }
        public IEnumerable<Qualification> Qualifications { get; set; }
        public IEnumerable<Title> Titles { get; set; }
        public System.Web.Mvc.SelectList InstitutionSelectList { get; set; }
        public System.Web.Mvc.SelectList QualificationSelectList { get; set; }
        public System.Web.Mvc.SelectList TitleSelectList { get; set; }

        [Required]
        [Display(Name = "Institution")]
        public int InstitutionID { get; set; }

        //[Required]
        [Display(Name = "Qualification")]
        public int QualificationID { get; set; }

        [Required]
        [Display(Name = "Title")]
        public Title Title { get; set; }

        [Required]
        [Display(Name = "Initials")]
        public string Initials { get; set; }

        [Required]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Student Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public bool AcceptTerms { get; set; }

    }

    public class UserViewModel
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
    
    }
}
