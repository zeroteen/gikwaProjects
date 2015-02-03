using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Grikwa.Models
{

    /// <summary>
    /// The FileSize attribute validator class
    /// </summary>
    public class FileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxSize;

        public FileSizeAttribute(int maxSize)
        {
            _maxSize = maxSize;
        }

        public override bool IsValid(object value)
        {
            if (value == null) return true;
            return _maxSize > (value as HttpPostedFileWrapper).ContentLength;
        }

        public override string FormatErrorMessage(string name)
        {

            string metric = "Byte";
            if (_maxSize >= 1049000)
            {
                metric = "MB";
            }
            else if (_maxSize >= 1024)
            {
                metric = "KB";
            }
     
            return string.Format("The file size should not exceed {0} "+metric, _maxSize);
        }

    }

    /// <summary>
    /// The FileType attribute validator class
    /// </summary>
    public class FileTypeAttribute : ValidationAttribute
    {

        private readonly List<string> _types;

        public FileTypeAttribute(string types)
        {
            _types = types.Split(',').ToList();
        }

        public override bool IsValid(object value)
        {
            if (value == null) return true;

            var fileExt = System.IO.Path.GetExtension((value as HttpPostedFileWrapper).FileName).Substring(1);
            return _types.Contains(fileExt, StringComparer.OrdinalIgnoreCase);
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format("Invalid file type. Only the following types {0} are supported.", string.Join(", ", _types));
        }
     
    }

    public class SellProductModel
    {

        [FileSize(5243000)]
        [FileType("jpg,jpeg,png")]
        [Display(Name="Product Image*")]
        [Required(ErrorMessage = "The product image is required")]
        public virtual HttpPostedFileBase ProductImage { get; set; }
        
        [MaxLength(50)]
        [Display(Name="Product Name*")]
        [Required(ErrorMessage = "The product name is required")]
        public virtual string Name { get; set; }

        [MaxLength(100)]
        [Display(Name = "Short Description*")]
        [Required(ErrorMessage = "The short description is required")]
        public virtual string ShortDescription { get; set; }

        [MaxLength(1500)]
        [Display(Name = "Long Description")]
        //[Required(ErrorMessage = "The long description is required")]
        public virtual string LongDescription { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Price in Rands (R)*")]
        [Required(ErrorMessage = "The Price is required")]
        public virtual decimal Price { get; set; }

        [MaxLength(250)]
        [Display(Name="Keywords*")]
        [Required(ErrorMessage = "At least one keyword is required")]
        public virtual string KeyWords { get; set; }

        [Required]
        [Display(Name = "Terms and Conditions*")]
        public virtual bool AcceptTerms { get; set; }

        [MaxLength(50)]
        [Display(Name = "Contact Email")]
        [DataType(DataType.EmailAddress)]
        public virtual string Email { get; set; }

        [MaxLength(15)]
        [Display(Name = "Contact Number")]
        [DataType(DataType.PhoneNumber)]
        public virtual string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please select at least one category")]
        [Display(Name = "Categories*")]
        public virtual ICollection<int> Categories { get; set; }

    }

    public class NotifyPosterModel
    {
        [FileSize(5243000)]
        [FileType("jpg,jpeg,png")]
        [Display(Name = "Poster Image*")]
        [Required(ErrorMessage = "The poster image is required")]
        public virtual HttpPostedFileBase PosterImage { get; set; }

        [MaxLength(50)]
        [Display(Name = "Poster Name*")]
        [Required(ErrorMessage = "The poster name is required")]
        public virtual string Name { get; set; }

        [MaxLength(1500)]
        [Display(Name = "Description")]
        //[Required(ErrorMessage = "The description is required")]
        public virtual string Description { get; set; }

        [MaxLength(250)]
        [Display(Name = "Keywords*")]
        [Required(ErrorMessage = "At least one keyword is required")]
        public virtual string KeyWords { get; set; }

        [Required]
        [Display(Name = "Terms and Conditions*")]
        public virtual bool AcceptTerms { get; set; }

        [MaxLength(200)]
        [Display(Name = "Website link (start with http:// or https://)")]
        public string WebsiteLink { get; set; }

        [MaxLength(50)]
        [Display(Name = "Contact Email")]
        [DataType(DataType.EmailAddress)]
        public virtual string Email { get; set; }

        [MaxLength(15)]
        [Display(Name = "Contact Number")]
        [DataType(DataType.PhoneNumber)]
        public virtual string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please select at least one category")]
        [Display(Name = "Categories*")]
        public virtual ICollection<int> Categories { get; set; }
    }

    public class EditProductModel
    {

        [Required]
        public virtual int ProductID { get; set; }

        [Required]
        public virtual string UserID { get; set; }

        [FileSize(5243000)]
        [FileType("jpg,jpeg,png")]
        [Display(Name = "Replace Product Image With")]
        public virtual HttpPostedFileBase ProductImage { get; set; }

        [MaxLength(50)]
        [Display(Name = "Product Name*")]
        [Required(ErrorMessage = "The product name is required")]
        public virtual string Name { get; set; }

        [MaxLength(100)]
        [Display(Name = "Short Description*")]
        [Required(ErrorMessage = "The short description is required")]
        public virtual string ShortDescription { get; set; }

        [MaxLength(1500)]
        [Display(Name = "Long Description")]
        //[Required(ErrorMessage = "The long description is required")]
        public virtual string LongDescription { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Price in Rands (R)*")]
        [Required(ErrorMessage = "The price is required")]
        public virtual decimal Price { get; set; }

        [MaxLength(250)]
        [Display(Name = "Keywords*")]
        [Required(ErrorMessage = "At least one keyword is required")]
        public virtual string KeyWords { get; set; }

        [MaxLength(50)]
        [Display(Name = "Contact Email")]
        [DataType(DataType.EmailAddress)]
        public virtual string Email { get; set; }

        [MaxLength(15)]
        [Display(Name = "Contact Number")]
        [DataType(DataType.PhoneNumber)]
        public virtual string PhoneNumber { get; set; }

        [Required(ErrorMessage="Please select at least one category")]
        [Display(Name = "Categories*")]
        public virtual ICollection<int> Categories { get; set; }
    }

    public class EditPosterModel
    {
        [Required]
        public virtual int PosterID { get; set; }

        [Required]
        public virtual string UserID { get; set; }

        [FileSize(5243000)]
        [FileType("jpg,jpeg,png")]
        [Display(Name = "Replace Poster Image With")]
        public virtual HttpPostedFileBase PosterImage { get; set; }

        [MaxLength(50)]
        [Display(Name = "Poster Name*")]
        [Required(ErrorMessage = "The poster name is required")]
        public virtual string Name { get; set; }

        [MaxLength(1500)]
        [Display(Name = "Description")]
        //[Required(ErrorMessage = "The long description is required")]
        public virtual string Description { get; set; }

        [MaxLength(250)]
        [Display(Name = "Keywords*")]
        [Required(ErrorMessage = "At least one keyword is required")]
        public virtual string KeyWords { get; set; }

        [MaxLength(200)]
        [Display(Name = "Website link (start with http:// or https://)")]
        public string WebsiteLink { get; set; }

        [MaxLength(50)]
        [Display(Name = "Contact Email")]
        [DataType(DataType.EmailAddress)]
        public virtual string Email { get; set; }

        [MaxLength(15)]
        [Display(Name = "Contact Number")]
        [DataType(DataType.PhoneNumber)]
        public virtual string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please select at least one category")]
        [Display(Name = "Categories*")]
        public virtual ICollection<int> Categories { get; set; }
    }

    public class CatalogProductModel
    {
        public int ProductID { get; set; }

        public string UserID { get; set; }

        public string UserName { get; set; }

        public string UserFullName { get; set; }

        public string Name { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public decimal Price { get; set; }

        public virtual string Email { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual string WebsiteLink { get; set; }

        public DateTime DatePosted { get; set; }

        public int Offers { get; set; }

        public ProductIntention ProductIntention { get; set; }

        public ProductStatus ProductStatus { get; set; }
    }

    public class BusinessCardModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Institution { get; set; }
        public string Qualification { get; set; }
        public bool HasPicture { get; set; }
        public IEnumerable<CatalogProductModel> Products { get; set; }
    }

    public class GetModel
    {
        [MaxLength(500)]
        public virtual string RequestMessage { get; set; }

        public virtual int ProductID { get; set; }
    }

    public class PendingUser
    {
        public string UserID { get; set; }
        public string FullName { get; set; }

    }

    public class SoldProductModel
    {
        [Required(ErrorMessage="Please select a product to sell")]
        [Display(Name="Product to sell")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Please select the person to sell to")]
        [Display(Name = "To")]
        public string CustomerID { get; set; }

        public SelectList Products { get; set; }
    }

    public class NegotiationProductModel
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
    }

    public class ConfirmProductSaleModel
    {
        [Required]
        public int ProductID { get; set; }
        public string ProductName { get; set; }

        [Required]
        public string CustomerID { get; set; }
        public string FullName { get; set; }
    }

    public class SendMessageModel
    {

        [Display(Name="Send To")]
        public virtual string ReceiverID { get; set; }
        public virtual ApplicationUser Receiver { get; set; }

        [Display(Name = "Message")]
        public virtual string Message { get; set; }
    }

    public class PaginationModel
    {
        public static int TotalItems { get; set; }
        public static string FilterURL { get; set; }
        public static int CurrentPage { get; set; }
        public static int CurrentStartPage { get; set; }
        public static int CurrentEndPage { get; set; }
        public const int PageSize = 12;
        public const int PosterPageSize = 6;
        public const int PaginationSize = 10;

        public static int TotalPages()
        {
            var pages = (int) Math.Ceiling(TotalItems * 1.0d / PageSize*1.0d);

            return pages;
        }

        public static bool AtStart()
        {
            return (CurrentPage == 1);
        }

        public static bool AtEnd()
        {
            return (CurrentPage == TotalPages());
        }

        public static int NextPage()
        {
            if (AtEnd())
            {
                return CurrentPage;
            }
            else
            {
                return (CurrentPage + 1);
            }
        }

        public static int PreviousPage()
        {
            if (AtStart())
            {
                return CurrentPage;
            }
            else
            {
                return (CurrentPage - 1);
            }
        }

        public static int GoToNextPage()
        {
            if (AtEnd())
            {
                return CurrentPage;
            }
            else
            {
                CurrentPage++;
                return (CurrentPage);
            }
        }

        public static int GoToNextPreviousPage()
        {
            if (AtStart())
            {
                return CurrentPage;
            }
            else
            {
                CurrentPage--;
                return (CurrentPage);
            }
        }

        public static int GoToEnd()
        {
            CurrentPage = TotalPages();
            return CurrentPage;
        }

        public static int GoToStart()
        {
            CurrentPage = 1;
            return CurrentPage;
        }

        public static int GoToPage(int page)
        {

            if (TotalItems == 0)
            {
                CurrentStartPage = 1;
                CurrentEndPage = 1;
                return 2;
            }

            if (page < 1)
            {
                CurrentPage = GoToStart();
            }
            else if (page > TotalPages())
            {
                CurrentPage = GoToEnd();
            }
            else
            {
                CurrentPage = page;
            }

            SetPaginationBounds(CurrentPage);

            return CurrentPage;
        }

        public static void SetPaginationBounds(int currentPage)
        {

            CurrentStartPage = currentPage - ((int)(PaginationSize / 2));
            CurrentEndPage = currentPage + ((int)(PaginationSize / 2) - 1);
            if (CurrentStartPage < 1)
            {
                CurrentStartPage = 1;
                CurrentEndPage += ((currentPage - ((int)(PaginationSize / 2))) * -1) + 1;
            }

            if (CurrentEndPage > TotalPages())
            {
                CurrentEndPage = TotalPages();
            }
        }
    }

    public class InstitutionViewModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class ContactUsModel
    {
        [Required]
        [Display(Name = "Your Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Your Email Address")]
        [DataType(DataType.EmailAddress)]
        public string From { get; set; }

        [Required]
        [Display(Name = "Message")]
        public string Message { get; set; }
    }

    public class ForbiddenMessageModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }
}