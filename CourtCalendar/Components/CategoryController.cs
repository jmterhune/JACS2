/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.CourtCalendar.Components
{
    /// <summary>
    /// Per-division category data access (Jud12 connection). Every division is
    /// seeded with four built-ins when it is created; judge/JA may recolor them
    /// (except default) and add custom categories.
    /// </summary>
    public class CategoryController
    {
        private const string CONN_JUD12 = "Jud12";
        private readonly IHostSettings _hostSettings;

        public CategoryController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public IEnumerable<Category> GetCategories(int divisionId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<Category>();
                return rep.Find("WHERE DivisionId = @0 AND Active = 1 ORDER BY SortOrder, Name", divisionId).ToList();
            }
        }

        public Category GetCategory(int categoryId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.GetRepository<Category>().GetById(categoryId);
            }
        }

        public void CreateCategory(Category c)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                ctx.GetRepository<Category>().Insert(c);
            }
        }

        public void UpdateCategory(Category c)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                ctx.GetRepository<Category>().Update(c);
            }
        }

        public void DeleteCategory(int categoryId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<Category>();
                var c = rep.GetById(categoryId);
                // Never delete a built-in; mark inactive instead.
                if (c == null) return;
                if (c.IsSystem) { c.Active = false; rep.Update(c); }
                else rep.Delete(c);
            }
        }

        /// <summary>Seeds the four built-in categories for a newly created division.</summary>
        public void SeedDefaults(int divisionId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<Category>();
                if (rep.Find("WHERE DivisionId = @0", divisionId).Any()) return;
                rep.Insert(new Category { DivisionId = divisionId, Name = "Default", Color = null, IsSystem = true, IsDefault = true, SortOrder = 0, Active = true });
                rep.Insert(new Category { DivisionId = divisionId, Name = "Vacation", Color = "#fff3cd", IsSystem = true, IsDefault = false, SortOrder = 1, Active = true });
                rep.Insert(new Category { DivisionId = divisionId, Name = "No Court", Color = "#e2e3e5", IsSystem = true, IsDefault = false, SortOrder = 2, Active = true });
                rep.Insert(new Category { DivisionId = divisionId, Name = "Holiday", Color = "#f8d7da", IsSystem = true, IsDefault = false, SortOrder = 3, Active = true });
            }
        }
    }
}
