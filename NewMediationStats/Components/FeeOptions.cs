using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace tjc.Modules.NewMediationStats.Components
{
    /// <summary>Fills and selects the fee and income drop-downs from fee schedule tiers.</summary>
    internal static class FeeOptions
    {
        /// <summary>
        /// Replaces the items with a placeholder plus the tiers. A current selection is kept only if it is
        /// still one of the tiers, so values from another fee schedule do not carry over silently.
        /// </summary>
        public static void Fill(DropDownList drp, IEnumerable<FeeTier> tiers, string placeholder)
        {
            string previous = drp.SelectedValue;
            drp.Items.Clear();
            drp.Items.Add(new ListItem(placeholder, string.Empty));
            foreach (FeeTier tier in tiers)
                drp.Items.Add(new ListItem(tier.Label, tier.Value));
            Select(drp, previous, false);
        }

        /// <summary>
        /// Selects a stored value. A value that is not in the list (for example one entered under an older
        /// schedule) is added and marked, so opening an old record never fails or loses its value.
        /// </summary>
        public static void Select(DropDownList drp, string value, bool keepIfMissing = true)
        {
            if (string.IsNullOrEmpty(value))
            {
                drp.SelectedIndex = 0;
                return;
            }
            ListItem item = drp.Items.FindByValue(value);
            if (item == null)
            {
                if (!keepIfMissing)
                {
                    drp.SelectedIndex = 0;
                    return;
                }
                item = new ListItem(value + " (not in this schedule)", value);
                drp.Items.Add(item);
            }
            drp.ClearSelection();
            item.Selected = true;
        }

        /// <summary>Null for the "nothing selected" placeholder, otherwise the selected value.</summary>
        public static string ValueOrNull(DropDownList drp)
        {
            return string.IsNullOrEmpty(drp.SelectedValue) ? null : drp.SelectedValue;
        }
    }
}
