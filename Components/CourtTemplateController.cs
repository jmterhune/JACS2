using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.jacs.Services.ViewModels;

namespace tjc.Modules.jacs.Components
{
    internal class CourtTemplateController
    {
        private const string CONN_JACS = "jacs";

        public void CreateCourtTemplate(CourtTemplate t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTemplate>();
                t.created_at = DateTime.Now;
                t.updated_at = DateTime.Now;
                rep.Insert(t);

                // Create court template orders for calendar weeks
                var court = ctx.GetRepository<Court>().GetById(t.court_id);
                if (!court.auto_extension)
                {
                    for (int x = 0; x < court.calendar_weeks; x++)
                    {
                        var order = ctx.GetRepository<CourtTemplateOrder>()
                            .Find($"WHERE court_id = {court.id} AND date = '{DateTime.UtcNow.AddDays(x * 7).Date.ToString("yyyy-MM-dd")}'")
                            .FirstOrDefault();
                        if (order == null)
                        {
                            ctx.GetRepository<CourtTemplateOrder>().Insert(new CourtTemplateOrder
                            {
                                court_id = court.id,
                                date = DateTime.UtcNow.AddDays(x * 7).Date,
                                auto = false,
                                template_id = t.id,
                                created_at = DateTime.UtcNow,
                                updated_at = DateTime.UtcNow
                            });
                        }
                    }
                }
            }
        }

        public void DeleteCourtTemplate(long courttemplateId)
        {
            var t = GetCourtTemplate(courttemplateId);
            if (t != null)
            {
                DeleteCourtTemplate(t);
            }
        }

        public void DeleteCourtTemplate(CourtTemplate t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                // Delete associated timeslots and their motions
                var timeslots = ctx.GetRepository<TemplateTimeslot>().Find($"WHERE court_template_id = {t.id}");
                foreach (var timeslot in timeslots)
                {
                    var motions = ctx.GetRepository<TimeslotMotion>()
                        .Find($"WHERE timeslotable_type = 'App\\Models\\TemplateTimeslot' AND timeslotable_id = {timeslot.id}");
                    foreach (var motion in motions)
                    {
                        ctx.GetRepository<TimeslotMotion>().Delete(motion);
                    }
                    ctx.GetRepository<TemplateTimeslot>().Delete(timeslot);
                }

                // Delete associated court template orders
                var orders = ctx.GetRepository<CourtTemplateOrder>().Find($"WHERE template_id = {t.id}");
                foreach (var order in orders)
                {
                    ctx.GetRepository<CourtTemplateOrder>().Delete(order);
                }

                var rep = ctx.GetRepository<CourtTemplate>();
                rep.Delete(t);
            }
        }

        public void BulkDeleteCourtTemplates(IEnumerable<long> templateIds)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                foreach (var id in templateIds)
                {
                    var template = ctx.GetRepository<CourtTemplate>().GetById(id);
                    if (template != null)
                    {
                        // Delete associated timeslots and their motions
                        var timeslots = ctx.GetRepository<TemplateTimeslot>().Find($"WHERE court_template_id = {id}");
                        foreach (var timeslot in timeslots)
                        {
                            var motions = ctx.GetRepository<TimeslotMotion>()
                                .Find($"WHERE timeslotable_type = 'App\\Models\\TemplateTimeslot' AND timeslotable_id = {timeslot.id}");
                            foreach (var motion in motions)
                            {
                                ctx.GetRepository<TimeslotMotion>().Delete(motion);
                            }
                            ctx.GetRepository<TemplateTimeslot>().Delete(timeslot);
                        }

                        // Delete associated court template orders
                        var orders = ctx.GetRepository<CourtTemplateOrder>().Find($"WHERE template_id = {id}");
                        foreach (var order in orders)
                        {
                            ctx.GetRepository<CourtTemplateOrder>().Delete(order);
                        }

                        ctx.GetRepository<CourtTemplate>().Delete(template);
                    }
                }
            }
        }

        public IEnumerable<CourtTemplate> GetCourtTemplates()
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTemplate>();
                return rep.Get();
            }
        }

        public List<KeyValuePair<long, string>> GetCourtTemplateDropDownItems()
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTemplate>();
                var templates = rep.Get();
                return templates.Select(temp => new KeyValuePair<long, string>(temp.id, temp.name)).ToList();
            }
        }

        public CourtTemplate GetCourtTemplate(long courttemplateId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTemplate>();
                var t = rep.GetById(courttemplateId);
                if (t != null)
                {
                    var court = ctx.GetRepository<Court>().GetById(t.court_id);
                    t.court_description = court?.description;
                    var judge = ctx.GetRepository<Judge>().Find($"WHERE court_id = {t.court_id}").FirstOrDefault();
                    t.judge_name = judge?.name;
                }
                return t;
            }
        }

        public void UpdateCourtTemplate(CourtTemplate t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTemplate>();
                t.updated_at = DateTime.Now;
                rep.Update(t);
            }
        }

        public IEnumerable<CourtTemplateViewModel> GetCourtTemplatesPaged(string searchTerm, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<CourtTemplateViewModel>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_court_template_paged",
                    searchTerm ?? string.Empty,
                    rowOffset,
                    pageSize,
                    sortOrder ?? "name",
                    sortDesc ?? "asc"
                );
            }
        }

        public int GetCourtTemplatesCount(string searchTerm)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_court_template_count",
                    searchTerm ?? string.Empty
                );
            }
        }

        public IEnumerable<TemplateTimeslot> GetTemplateTimeslots(long templateId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TemplateTimeslot>();
                return rep.Find($"WHERE court_template_id = {templateId}");
            }
        }

        public TemplateTimeslot GetTemplateTimeslot(long timeslotId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TemplateTimeslot>();
                var timeslot = rep.GetById(timeslotId);
                return timeslot;
            }
        }

        public void CreateTemplateTimeslot(TemplateTimeslot timeslot)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                timeslot.created_at = DateTime.UtcNow;
                timeslot.updated_at = DateTime.UtcNow;
                var rep = ctx.GetRepository<TemplateTimeslot>();
                rep.Insert(timeslot);

                if (timeslot.motions != null && timeslot.motions.Any())
                {
                    var motionRep = ctx.GetRepository<TimeslotMotion>();
                    foreach (var motion in timeslot.motions)
                    {
                        motionRep.Insert(new TimeslotMotion
                        {
                            timeslotable_type = "App\\Models\\TemplateTimeslot",
                            timeslotable_id = timeslot.id,
                            motion_id = motion.id,
                            created_at = DateTime.UtcNow,
                            updated_at = DateTime.UtcNow
                        });
                    }
                }
            }
        }

        public void UpdateTemplateTimeslot(TemplateTimeslot timeslot)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                timeslot.updated_at = DateTime.UtcNow;
                var rep = ctx.GetRepository<TemplateTimeslot>();
                rep.Update(timeslot);

                // Update motions
                var motionRep = ctx.GetRepository<TimeslotMotion>();
                var existingMotions = motionRep.Find($"WHERE timeslotable_type = 'App\\Models\\TemplateTimeslot' AND timeslotable_id = {timeslot.id}");
                foreach (var motion in existingMotions)
                {
                    motionRep.Delete(motion);
                }

                if (timeslot.motions != null && timeslot.motions.Any())
                {
                    foreach (var motion in timeslot.motions)
                    {
                        motionRep.Insert(new TimeslotMotion
                        {
                            timeslotable_type = "App\\Models\\TemplateTimeslot",
                            timeslotable_id = timeslot.id,
                            motion_id = motion.id,
                            created_at = DateTime.UtcNow,
                            updated_at = DateTime.UtcNow
                        });
                    }
                }
            }
        }

        public void DeleteTemplateTimeslot(long timeslotId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var timeslot = ctx.GetRepository<TemplateTimeslot>().GetById(timeslotId);
                if (timeslot != null)
                {
                    var motions = ctx.GetRepository<TimeslotMotion>()
                        .Find($"WHERE timeslotable_type = 'App\\Models\\TemplateTimeslot' AND timeslotable_id = {timeslotId}");
                    foreach (var motion in motions)
                    {
                        ctx.GetRepository<TimeslotMotion>().Delete(motion);
                    }
                    ctx.GetRepository<TemplateTimeslot>().Delete(timeslot);
                }
            }
        }

        public void CloneTemplate(long id)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var oldTemplate = GetCourtTemplate(id);
                if (oldTemplate == null)
                {
                    throw new Exception("Template not found.");
                }

                var newTemplate = new CourtTemplate
                {
                    name = oldTemplate.name + " (Copy)",
                    court_id = oldTemplate.court_id,
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow
                };
                var templateRep = ctx.GetRepository<CourtTemplate>();
                templateRep.Insert(newTemplate);

                var timeslots = ctx.GetRepository<TemplateTimeslot>().Find($"WHERE court_template_id = {id}");
                foreach (var timeslot in timeslots)
                {
                    var newTimeslot = new TemplateTimeslot
                    {
                        start = timeslot.start,
                        end = timeslot.end,
                        duration = timeslot.duration,
                        quantity = timeslot.quantity,
                        allDay = timeslot.allDay,
                        day = timeslot.day,
                        court_template_id = newTemplate.id,
                        description = timeslot.description,
                        category_id = timeslot.category_id,
                        blocked = timeslot.blocked,
                        public_block = timeslot.public_block,
                        block_reason = timeslot.block_reason,
                        created_at = DateTime.UtcNow,
                        updated_at = DateTime.UtcNow
                    };
                    var timeslotRep = ctx.GetRepository<TemplateTimeslot>();
                    timeslotRep.Insert(newTimeslot);

                    var motions = ctx.GetRepository<TimeslotMotion>()
                        .Find($"WHERE timeslotable_type = 'App\\Models\\TemplateTimeslot' AND timeslotable_id = {timeslot.id}");
                    foreach (var motion in motions)
                    {
                        ctx.GetRepository<TimeslotMotion>().Insert(new TimeslotMotion
                        {
                            timeslotable_type = "App\\Models\\TemplateTimeslot",
                            timeslotable_id = newTimeslot.id,
                            motion_id = motion.motion_id,
                            created_at = DateTime.UtcNow,
                            updated_at = DateTime.UtcNow
                        });
                    }
                }
            }
        }
    }
}