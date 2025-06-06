using DotNetNuke.Data;
using System.Collections.Generic;
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
            IEnumerable<Category> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                t = ctx.ExecuteQuery<Category>(System.Data.CommandType.StoredProcedure, "tjc_jacs_get_category_paged", searchTerm, rowOffset, pageSize, sortOrder, sortDesc);
            }
            return t;
        }
        public int GetCategoriesCount(string searchTerm)
        {
            int t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                t = ctx.ExecuteScalar<int>(System.Data.CommandType.StoredProcedure, "tjc_jacs_get_category_count", searchTerm);
            }
            return t;
        }

    }
}