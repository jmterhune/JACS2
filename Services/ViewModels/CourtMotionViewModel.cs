using Newtonsoft.Json;
using System.Collections.Generic;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class CourtMotionViewModel
    {
        public CourtMotionViewModel() { }
        public CourtMotionViewModel(CourtMotion courtMotion)
        {
            id = courtMotion.id;
            court_id = courtMotion.court_id;
            motion_id = courtMotion.motion_id;
            allowed = courtMotion.allowed;
        }
        public long id { get; set; }
        public long court_id { get; set; }
        public long motion_id { get; set; }
        public bool allowed { get; set; }

        // Fix for CS0117: Adding the missing 'FromCourtMotions' method
        public static List<long> FromCourtMotions(IEnumerable<CourtMotion> courtMotions)
        {
            var viewModels = new List<long>();
            foreach (var motion in courtMotions)
            {
                viewModels.Add(motion.motion_id);
            }
            return viewModels;
        }
    }
}