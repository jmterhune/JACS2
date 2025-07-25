using DotNetNuke.Data;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
namespace tjc.Modules.jacs.Components
{
    internal class CategoryController
    {
        private const string CONN_JACS = "jacs"; //Connection

        public void CreateCategory(Category t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Category>();
                t.created_at = System.DateTime.Now;
                t.updated_at = System.DateTime.Now;
                rep.Insert(t);
            }
        }
        public void DeleteCategory(long categoryId)
        {
            var t = GetCategory(categoryId);
            DeleteCategory(t);
        }
        public void DeleteCategory(Category t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Category>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Category> GetCategorys()
        {
            IEnumerable<Category> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Category>();
                t = rep.Get();
            }
            return t;
        }
        public List<KeyValuePair<long, string>> GetCategoryDropDownItems(string searchTerm)
        {
            try
            {
                // Normalize search term
                searchTerm = string.IsNullOrWhiteSpace(searchTerm) ? string.Empty : searchTerm.Trim();

                using (IDataContext ctx = DataContext.Instance("jacs"))
                {
                    var rep = ctx.GetRepository<Category>();
                    var results = rep.Find("WHERE description LIKE @0", $"%{searchTerm}%")
                        .Select(c => new KeyValuePair<long, string>(c.id, c.description)).ToList();
                    return results ?? new List<KeyValuePair<long, string>>();
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return new List<KeyValuePair<long, string>>();
            }
        }
        public Category GetCategory(long categoryId)
        {
            Category t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Category>();
                t = rep.GetById(categoryId);
            }
            return t;
        }
        public void UpdateCategory(Category t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Category>();
                t.updated_at = System.DateTime.Now;
                rep.Update(t);
            }
        }
        public IEnumerable<Category> GetCategoriesPaged(string searchTerm, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<Category>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_category_paged",
                    searchTerm ?? string.Empty,
                    rowOffset,
                    pageSize,
                    sortOrder ?? "description",
                    sortDesc ?? "asc"
                );
            }
        }
        public int GetCategoriesCount(string searchTerm)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_category_count",
                    searchTerm ?? string.Empty
                );
            }
        }

    }
}