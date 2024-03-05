using CUnity.Shared.CustomValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CUnity.Shared.Models
{
    public class Subscribtion : IValidatableObject
    {
        [Required]
        public string ClientName { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        //public ProductNames ProductName { get; set; }
        [Required(ErrorMessage = "Please Select Product Name")]
        public string ProductName { get; set; }
        public string EncryptedText { get; set; }
        public string DecryptedText { get; set; }
     

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate.Date < DateTime.Now.Date)
            {
                yield return new ValidationResult(
                   errorMessage: "Start Date must be after or equal to current date",
                   memberNames: new[] { "StartDate" }
              );
            }


            if (EndDate < StartDate)
            {
                yield return new ValidationResult(
                    errorMessage: "EndDate must be greater than StartDate",
                    memberNames: new[] { "EndDate" }
               );
            }
        }
    }
    public class Client : BaseEntity, IValidatableObject
    {
        public string Name { get; set; }
        [Required(ErrorMessage ="Please Enter Client Email")]
        public string Email { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Name==string.Empty)
            {
                yield return new ValidationResult(
                   errorMessage: "ClientName is Required",
                   memberNames: new[] { "Name" }
              );
            }
        }
    }

    public class ClientListView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public string AzureGroupName { get; set; }
        public string AzureObjectId { get; set; }
        public string PowerBIWorkSpaceId { get; set; }

        public bool IsReportCopied { get; set; }
    }
    }
