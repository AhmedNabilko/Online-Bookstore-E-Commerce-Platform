using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerceEntity.Entity
{
    public class Category : IEntity
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        [ForeignKey(nameof(ParentCategory))]
        public int? ParentCategoryId { get; set; }

        public Category ParentCategory { get; set; }

        public ICollection<Product> Products { get; set; }
        public ICollection<Category> SubCategories { get; set; }
    }
}
