using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BulkyBook.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]

        public string ISBN { get; set; }

        [Required]

        public string Author { get; set; }

        public string Description { get; set; }

        [Required]
        [Range(0,10000)]
        public double ListPrice { get; set; }

        [Required]
        [Range(0, 10000)]
        public double Price { get; set; }

        [Required]
        [Range(0, 10000)]
        public double Price50 { get; set; }

        [Required]
        [Range(0, 10000)]
        public double Price100 { get; set; }

        public string ImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Required]
        public int CoverTypeId { get; set; }

        [ForeignKey("CoverTypeId")]
        public CoverType CoverType { get; set; }

    }
}
