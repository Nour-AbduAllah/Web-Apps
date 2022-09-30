using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Models
{
    public class Order
    {
        public Order()
        {
            this.OrderDetails = new List<OrderDetails>();
        }
        public int ID { get; set; }
        [Display(Name ="Serial Number")]
        public string SerialNo { get; set; }
        [Required]
        public string Name { get; set; }
        [Required, DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        [Required, EmailAddress]
        public string EMail { get; set; }
        [Required]
        public string Address { get; set; }
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }
        public virtual List<OrderDetails> OrderDetails { get; set; }
    }
}
