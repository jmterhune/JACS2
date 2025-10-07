// DataController.cs (Repository-like class using DAL2/PetaPoco)
using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Data;

public static class DataController
{
    public static IEnumerable<CourtTemplateOrder> GetAutoCourtTemplateOrders(int courtId)
    {
        using (IDataContext ctx = DataContext.Instance())
        {
            var repo = ctx.GetRepository<CourtTemplateOrder>();
            return repo.Find("WHERE court_id = @0 AND auto = 1 ORDER BY [order]", courtId);
        }
    }

    public static CourtTemplateOrder GetAutoCourtTemplateOrderByOrder(int courtId, int order)
    {
        using (IDataContext ctx = DataContext.Instance())
        {
            var repo = ctx.GetRepository<CourtTemplateOrder>();
            return repo.Find("WHERE court_id = @0 AND auto = 1 AND [order] = @1", courtId, order).FirstOrDefault();
        }
    }

    public static Template GetTemplate(int templateId)
    {
        using (IDataContext ctx = DataContext.Instance())
        {
            var repo = ctx.GetRepository<Template>();
            return repo.GetById(templateId);
        }
    }

    public static IEnumerable<TemplateTimeslot> GetTemplateTimeslots(int templateId)
    {
        using (IDataContext ctx = DataContext.Instance())
        {
            var repo = ctx.GetRepository<TemplateTimeslot>();
            return repo.Find("WHERE court_template_id = @0", templateId);
        }
    }

    public static IEnumerable<Holiday> GetHolidays()
    {
        using (IDataContext ctx = DataContext.Instance())
        {
            var repo = ctx.GetRepository<Holiday>();
            return repo.Get();
        }
    }

    public static Timeslot GetLastTemplateTimeslot(int courtId)
    {
        using (IDataContext ctx = DataContext.Instance())
        {
            return ctx.ExecuteQuery<Timeslot>(System.Data.CommandType.Text,
                @"SELECT TOP 1 t.* FROM timeslots t 
                  INNER JOIN court_timeslots ct ON ct.timeslot_id = t.id 
                  WHERE ct.court_id = @0 AND t.template_id IS NOT NULL 
                  ORDER BY t.start DESC",
                courtId).FirstOrDefault();
        }
    }

    public static int CreateTimeslot(Timeslot ts)
    {
        using (IDataContext ctx = DataContext.Instance())
        {
            var repo = ctx.GetRepository<Timeslot>();
            repo.Insert(ts);
            return ts.id; // Assumes identity insert
        }
    }

    public static void CreateCourtTimeslot(CourtTimeslot cts)
    {
        using (IDataContext ctx = DataContext.Instance())
        {
            var repo = ctx.GetRepository<CourtTimeslot>();
            repo.Insert(cts);
        }
    }

    // Additional helper if needed
    public static Court GetCourt(int courtId)
    {
        using (IDataContext ctx = DataContext.Instance())
        {
            var repo = ctx.GetRepository<Court>();
            return repo.GetById(courtId);
        }
    }
}
