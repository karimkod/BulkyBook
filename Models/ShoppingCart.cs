using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
    public class ShoppingCart
    {


        public ShoppingCart()
        {
            Count = 1;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        public int ProductId { get; set; }
        
        [ForeignKey("ProductId")]
        public Product Prodcut { get; set; }
        
        [Range(1,1000, ErrorMessage ="The count must be between 1 and 1000")]
        public int Count { get; set; }

        [NotMapped]
        public double Price { get; set; }

    }
}
