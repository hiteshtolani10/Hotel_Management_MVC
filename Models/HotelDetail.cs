using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace Hotel_Management_MVC.Models;

public class HotelDetail
{
    public string Id { get; set; }

    //public int HotelId { get; set; }

    [Required(ErrorMessage = "Please Enter Hotel Name")]
    [Display(Name = "Hotel Name")]
    public string HotelName { get; set; } = null!;

    [Required(ErrorMessage = "Please Enter Hotel Email")]
    [Display(Name = "Hotel Email")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Please Enter Your Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    //[Required(ErrorMessage = "Please Confirm your Password")]
    //[DataType(DataType.Password)]
    //[Display(Name = "Confirm Password")]
    //[Compare("Password")]
    //public string ConfirmPassword { get; set; } = null!;

    public bool EmailVerified { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreationDate { get; set; }

    [Display(Name = "Single-Bed")]
    public int SingleBedAc { get; set; }

    [Display(Name = "Single-Bed")]
    public int SingleBedNonAc { get; set; }

    [Display(Name = "Double-Bed")]
    public int DoubleBedAc { get; set; }

    [Display(Name = "Double-Bed")]
    public int DoubleBedNonAc { get; set; }

    public int MaharajaSuite { get; set; }

    public string? CoverImageUrl { get; set; }

    public string? SingleBedAcImgUrl { get; set; }

    public string? DoubleBedAcImgUrl { get; set; }

    public string? SingleBedNonAcImgUrl { get; set; }

    public string? DoubleBedNonAcImgUrl { get; set; }

    public double SingleBedAcPrice { get; set; }

    public double DoubleBedAcPrice { get; set; }

    public double SingleBedNonAcPrice { get; set; }

    public double DoubleBedNonAcPrice { get; set; }

}
