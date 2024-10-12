﻿namespace OnlineShop.DataLayer.Entities;

public class Category : IEntity
{
    public string Name { get; set; }
    public Guid? ParentId { get; set; } // foreign key for parent category (null for root category)
    public Category ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; }
    public ICollection<Item> Items { get; set; } = new List<Item>();
    public ICollection<Trait> Traits { get; set; }
}
