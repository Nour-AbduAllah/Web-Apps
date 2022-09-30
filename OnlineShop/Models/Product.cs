using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Models
{
    public class Product
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required, DataType(DataType.Currency)]
        public decimal Price { get; set; }
        
        public string Image { get; set; }
        [Required]
        [Display(Name ="Color")]
        public string ProductColor { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required, Display(Name = "Product Type")]
        public int ProductTypeId { get; set; }
        [Display(Name ="Product Type")]
        public ProductTypes ProductType { get; set; }
        [Required, Display(Name = "Product Tag")]
        public int SpecialTagId { get; set; }
        [Display(Name ="Product Tag")]
        public SpecialTag ProductState { get; set; }
    }
}
