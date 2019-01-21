﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADTeam5.Models
{
    public partial class DisbursementList
    {
        [Required]
        public string Dlid { get; set; }
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Display(Name = "Estimate Deliver Date")]
        [Required, DataType(DataType.Date)]
        [EndLaterThanV("StartDate")]
        public DateTime? EstDeliverDate { get; set; }
        [Display(Name = "Complete Date")]
        [Required, DataType(DataType.Date)]
        [EndLaterThanV("StartDate")]
        public DateTime? CompleteDate { get; set; }
        [Display(Name ="Department")]
        [Required(ErrorMessage = "*This field is required")]
        [MaxLength(4), RegularExpression(@"(^[A-Z]+[a-zA-Z'\s]*$)", ErrorMessage = "Alphabets only")]
        public string DepartmentCode { get; set; }
        [Display(Name ="Rep ID")]
        [Required]
        public int RepId { get; set; }
        [Range(0, 6, ErrorMessage = "Not a valid collection point"), RegularExpression(@"^[0-9]*$", ErrorMessage = "Numbers only")]
        public int CollectionPointId { get; set; }
        [Display(Name = "Disbursement Status")]
        [Required(ErrorMessage = "*This field is required")]
        [MaxLength(50), RegularExpression(@"(^[A-Z]+[a-zA-Z'\s]*$)", ErrorMessage = "Alphabets only")]
        public string Status { get; set; }
        

        public virtual CollectionPoint CollectionPointNavigation { get; set; }
        public virtual Department DepartmentCodeNavigation { get; set; }
        public virtual User RepNavigation { get; set; }

    }
}
