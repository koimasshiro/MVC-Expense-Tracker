using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_Expense_Tracker.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        //categoryId
        [Range(1, int.MaxValue, ErrorMessage = "Please Select a category")]
        public int CategoryId { get; set; }

        //navigation property
        public Category? Category { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Amount should be greater than 0.")]
        public int Amount { get; set; }

        [Column(TypeName = "nvarchar(65)")]
        public string? Description { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public string? CategoryTitleAndIcon {
            get
            {
                return Category == null? "" : Category.TitleAndIcon;
            }
        }

        //create a property for differentiating between expense and income by using substraction in amount field
        [NotMapped]
        public string? FormatAmount { 
            get 
            { 
                return ((Category == null || Category.Type == "Expense")? "- " : "+ " ) + Amount.ToString("C0");
            }
        }
    }
}
