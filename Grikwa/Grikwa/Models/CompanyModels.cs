using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Grikwa.Models
{

    public enum VisibilitySatus
    {
        SHOW,
        HIDE
    }

    public enum ItemType 
    {
        SPECIAL,
        COMMON
    }

    public class Company
    {
        public virtual int CompanyID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string Telephone { get; set; }
        public virtual string AdditionalTelephone { get; set; }
        public virtual byte[] Logo { get; set; }
        public virtual byte[] Background { get; set; }
        public VisibilitySatus VisibilitySatus { get; set; }
    }

    public class Location
    {
        public virtual string LocationID { get; set; }
        public virtual string Name { get; set; }
    }

    public class InstitutionCompany
    {
        public virtual int InstitutionCompanyID { get; set; }
        public virtual int CompanyID { get; set; }
        public virtual Company Company { get; set; }
        public virtual int InstitutionID { get; set; }
        public virtual Institution Institution { get; set; }
        public virtual string LocationID { get; set; }
        public virtual Location Location { get; set; }
    }

    public class CompanyEmployee
    {
        public virtual int CompanyEmployeeID { get; set; }
        public virtual string EmployeeID { get; set; }
        public virtual ApplicationUser Employee { get; set; } 
        public virtual int CompanyID { get; set; }
        public virtual Company Company { get; set; }
    }

    public class Item
    {
        public virtual int ItemID { get; set; }
        public virtual string Name { get; set; }
        public virtual string ShortDescription { get; set; }
        public virtual string LongDescription { get; set; }
        public virtual double Price { get; set; }
        public virtual string UnitID { get; set; }
        public virtual Unit Unit { get; set; }
        public virtual byte[] ItemImage { get; set; }
        public virtual byte[] ItemImage2 { get; set; }
        public virtual byte[] ItemImage3 { get; set; }
        public virtual ItemType ItemType { get; set; }
        public VisibilitySatus VisibilitySatus { get; set; }
        
    }

    public class CompanyItem
    {
        public virtual int CompanyItemID { get; set; }
        public virtual int CompanyID { get; set; }
        public virtual Company Company { get; set; }
        public virtual int ItemID { get; set; }
        public virtual Item Item { get; set; }
    }

    public class Unit
    {
        public virtual string UnitID { get; set; }
        public virtual string Description { get; set; }
    }

    public class ItemAdjustment
    {
        public virtual int ItemAdjustmentID { get; set; }
        public virtual int Quantity { get; set; }
        public virtual float Discount { get; set; }
        public virtual int ItemID { get; set; }
        public virtual Item Item { get; set; }
        public virtual string EmployeeID { get; set; }
        public virtual ApplicationUser Employee { get; set; }
        public virtual string Reason { get; set; }
        public virtual DateTime EffectDate { get; set; }
    }

    public class Order
    {
        public virtual int OrderID { get; set; }
        public virtual string OrderNumber { get; set; }
        public virtual string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual int CompanyID { get; set; }
        public virtual Company Company { get; set; }
        public virtual DateTime OrderTime { get; set; }
        public virtual DateTime CollectionTime { get; set; }
    }

    public class OrderItem
    {
        public virtual int OrderItemID { get; set; }
        public virtual int OrderID { get; set; }
        public virtual Order Order { get; set; }
        public virtual int ItemID { get; set; }
        public virtual Item Item { get; set; }
        public virtual int Discount { get; set; }
    }

    public class CompanySale
    {
        public virtual int CompanySaleID { get; set; }
        public virtual string EmployeeID { get; set; }
        public virtual ApplicationUser Employee { get; set; }
        public virtual int OrderID { get; set; }
        public virtual Order Order { get; set; }
        public virtual DateTime SaleTime { get; set; }
    }

    public class SlotSetting
    {
        public virtual int SlotSettingID { get; set; }
        public virtual DateTime StartTime { get; set; }
        public virtual DateTime EndTime { get; set; }
        public virtual int MinutesRange { get; set; }
        public virtual int MaxQuantity { get; set; }
    }

    public class CompanySlotSetting
    {
        public virtual int CompanySlotSettingID { get; set; }
        public virtual int CompanyID { get; set; }
        public virtual Company Company { get; set; }
        public virtual int SlotSettingID { get; set; }
        public virtual SlotSetting SlotSetting { get; set; }
        public virtual DateTime ActiveDate { get; set; }
    }
}