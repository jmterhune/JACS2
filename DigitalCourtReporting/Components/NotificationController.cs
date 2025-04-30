using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;
namespace tjc.Modules.DigitalCourtReporting.Components
{
    internal class NotificationController
    {
        public void CreateNotification(Notification t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Notification>();
                rep.Insert(t);
            }
        }
        public void DeleteNotification(int notificationId)
        {
            var t = GetNotification(notificationId);
            DeleteNotification(t);
        }
        public void DeleteNotification(Notification t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Notification>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Notification> GetNotifications()
        {
            IEnumerable<Notification> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Notification>();
                t = rep.Get();
            }
            return t;
        }
        public Notification GetNotification(int notificationId)
        {
            Notification t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Notification>();
                t = rep.GetById(notificationId);
            }
            return t;
        }
        public IEnumerable<Notification> GetNotificationsByProceeding(int proceedingId)
        {
            IEnumerable<Notification> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Notification>();
                t = rep.Find("Where ProceedingID=@0", proceedingId);
            }
            return t;
        }
        public Notification GetNotificationByProceeding(int proceedingId)
        {
            Notification t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Notification>();
                t = rep.Find("Where ProceedingID=@0",proceedingId).FirstOrDefault();
            }
            return t;
        }
        public void UpdateNotification(Notification t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Notification>();
                rep.Update(t);
            }
        }
    }
}