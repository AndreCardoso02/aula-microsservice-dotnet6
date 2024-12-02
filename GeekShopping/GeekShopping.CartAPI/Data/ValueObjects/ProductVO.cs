using GeekShopping.CartAPI.Model.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeekShopping.CartAPI.Data.ValueObjects
{
    public class ProductVO
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? CategoryName { get; set; }
        public string? ImageUrl { get; set; }
    }
}
