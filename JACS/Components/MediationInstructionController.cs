using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class MediationInstructionController
    {
        public void CreateMediationInstruction(MediationInstruction t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationInstruction>();
                rep.Insert(t);
            }
        }
        public void DeleteMediationInstruction(int mediationinstructionId)
        {
            var t = GetMediationInstruction(mediationinstructionId);
            DeleteMediationInstruction(t);
        }
        public void DeleteMediationInstruction(MediationInstruction t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationInstruction>();
                rep.Delete(t);
            }
        }
        public IEnumerable<MediationInstruction> GetMediationInstructions()
        {
            IEnumerable<MediationInstruction> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationInstruction>();
                t = rep.Get();
            }
            return t;
        }
        public MediationInstruction GetMediationInstruction(int mediationinstructionId)
        {
            MediationInstruction t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationInstruction>();
                t = rep.GetById(mediationinstructionId);
            }
            return t;
        }
        public void UpdateMediationInstruction(MediationInstruction t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationInstruction>();
                rep.Update(t);
            }
        }
    }
}