using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNetNuke.Common.Utilities;
using tjc.Modules.PretrialServices.Sarasota.Components;
using iTextSharp.text.pdf;
using iTextSharp.text;
using DotNetNuke.Entities.Modules;

namespace tjc.Modules.PretrialServices.Sarasota
{
    partial class PretrialReport : System.Web.UI.Page
    {
        private ReportType reportType;
        private DateTime reportDate = Null.NullDate;
        private int ModuleId = 0;
        private string ReportRootUrl = "~/portals/0/reports/pretrialservices/sarasota/";
        private List<DayTotal> colDefendantDayTotal = new List<DayTotal>();
        private List<DayTotal> colDefendantRunningTotal = new List<DayTotal>();
        private List<IntakeLogItem> colIntake = new List<IntakeLogItem>();
        private List<IntakeLogItem> colIntakeRunningTotal = new List<IntakeLogItem>();
        private TwoColumnHeaderFooter PageEventHandler = new TwoColumnHeaderFooter();
        private string reportTitle;
        private string reportTitleW;
        private string reportTitleM;
        private string reportTitleY;
        private Font normalFont = new Font(Font.FontFamily.HELVETICA, 5);
        private Font titleFont = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLDITALIC);
        private Font boldFont = new Font(Font.FontFamily.HELVETICA, 5, Font.BOLD);
        private string beginningDate = "";
        private string enddingDate = "";
        private Font captionFont = new Font(Font.FontFamily.HELVETICA, 5, Font.ITALIC, new BaseColor(0, 183, 183));
        private void AddBlankCells(PdfPTable table, int count, int colspan = 1)
        {
            for (int i = 0; i < count; i++)
            {
                PdfPCell blank = new PdfPCell
                {
                    Border = 0,
                    Colspan = colspan
                };
                table.AddCell(blank);
            }
        }


        private bool IsWeekend(DateTime inDate)
        {
            int weekDay = inDate.Day;
            int daysInMonth = System.DateTime.DaysInMonth(inDate.Year, inDate.Month);

            if (weekDay == 7 | weekDay == 14 | weekDay == 21 | weekDay == 28 | weekDay == daysInMonth)
                return true;
            return false;
        }

        private int GetWeekStartDay(DateTime inDate)
        {
            int weekDay = inDate.Day;
            if (1 <= weekDay && weekDay <= 7)
                return 1;
            else if (8 <= weekDay && weekDay <= 14)
                return 8;
            else if (15 <= weekDay && weekDay <= 21)
                return 15;
            else if (22 <= weekDay && weekDay <= 28)
                return 22;
            return 29;
        }

        private bool IsMonthEnd(DateTime inDate)
        {
            DateTime tempDate = GetLastDayOfMonth(inDate);
            if (tempDate == inDate)
                return true;
            return false;
        }

        private bool IsYearEnd(DateTime inDate)
        {
            if (inDate == reportDate)
                return true;
            return false;
        }

        private DateTime GetLastDayOfMonth(DateTime inDate)
        {
            return new DateTime(inDate.Year, inDate.Month, DateTime.DaysInMonth(inDate.Year, inDate.Month));
        }

        private string GetWeekTextValue(DateTime inDate)
        {
            string weekEndValue = inDate.ToString("MMM");
            int weekStartDay = inDate.Day;
            if (weekStartDay == 7)
                return "Week 1";
            else if (weekStartDay == 14)
                return "Week 2";
            else if (weekStartDay == 21)
                return "Week 3";
            else if (weekStartDay == 28)
                return "Week 4";
            else if (weekStartDay == DateTime.DaysInMonth(inDate.Year, inDate.Month))
                return "Week 5";
            return weekEndValue;
        }

        private PdfPTable getIntakelog(DateTime inDate)
        {
            PdfPTable table = new PdfPTable(7);
            table.DefaultCell.BackgroundColor = new BaseColor(255, 255, 204);
            table.DefaultCell.HorizontalAlignment = 1;
            table.HorizontalAlignment = 0;
            table.WidthPercentage = 60;
            table.HeaderRows = 1;
            table.DefaultCell.VerticalAlignment = Element.ALIGN_BOTTOM;
            table.AddCell(new Phrase("Date", boldFont));
            table.AddCell(new Phrase("# Interviewed", boldFont));
            table.AddCell(new Phrase("# Assessed", boldFont));
            table.AddCell(new Phrase(string.Format("# Recommended {0} for PTR", Environment.NewLine), boldFont));
            table.AddCell(new Phrase(string.Format("# PTR not {0} Recommended ", Environment.NewLine), boldFont));
            table.AddCell(new Phrase(string.Format("# Ordered {0} to PTR ", Environment.NewLine), boldFont));
            table.AddCell(new Phrase(string.Format("# Indigent {0} Int / Assessed ", Environment.NewLine), boldFont));
            table.DefaultCell.BackgroundColor = BaseColor.WHITE;
            var ctl = new IntakeLogItemController();
            IntakeLogItem intakeLogItem = ctl.GetIntakeLogItemByDate(inDate);
            if (intakeLogItem != null)
            {
                table.AddCell(new Phrase(GetWeekTextValue(inDate), normalFont));
                if (intakeLogItem.Interviewed.HasValue)
                    table.AddCell(new Phrase(intakeLogItem.Interviewed.ToString(), normalFont));
                if (intakeLogItem.Assessed.HasValue)
                    table.AddCell(new Phrase(intakeLogItem.Assessed.ToString(), normalFont));
                if (intakeLogItem.PtrRecommended.HasValue)
                    table.AddCell(new Phrase(intakeLogItem.PtrRecommended.ToString(), normalFont));
                if (intakeLogItem.PtrNotRecommended.HasValue)
                    table.AddCell(new Phrase(intakeLogItem.PtrNotRecommended.ToString(), normalFont));
                if (intakeLogItem.PtrOrdered.HasValue)
                    table.AddCell(new Phrase(intakeLogItem.PtrOrdered.ToString(), normalFont));
                if (intakeLogItem.IndigentAssessed.HasValue)
                    table.AddCell(new Phrase(intakeLogItem.IndigentAssessed.ToString(), normalFont));
                colIntake.Add(intakeLogItem);
            }
            return table;
        }

        private PdfPTable GetDefendantLog(DateTime InDate)
        {
            PdfPTable table = new PdfPTable(17);
            table.DefaultCell.BackgroundColor = new BaseColor(255, 204, 153); // ORANGE
            table.DefaultCell.HorizontalAlignment = 1;
            table.HorizontalAlignment = 0;
            table.WidthPercentage = 100;
            table.HeaderRows = 1;
            table.DefaultCell.VerticalAlignment = Element.ALIGN_BOTTOM;
            table.AddCell(new Phrase("Name", boldFont));
            table.AddCell(new Phrase("Case Number(s)", boldFont));
            table.AddCell(new Phrase(string.Format("Arrest{0}Charges", Environment.NewLine), boldFont));
            table.AddCell(new Phrase("Indigent", boldFont));
            table.AddCell(new Phrase(string.Format("# Fel Conv{0}Dangerous", Environment.NewLine), boldFont));
            table.AddCell(new Phrase(string.Format("# Fel Conv Non-{0}Dangerous", Environment.NewLine), boldFont));
            table.AddCell(new Phrase(string.Format("# Misd Conv{0}Dangerous", Environment.NewLine), boldFont));
            table.AddCell(new Phrase(string.Format("# Misd Conv Non-{0}Dangerous", Environment.NewLine), boldFont));
            table.AddCell(new Phrase("FTA Date", boldFont));
            table.AddCell(new Phrase(string.Format("# Court{0}Appearances", Environment.NewLine), boldFont));
            table.AddCell(new Phrase("BW Ordered", boldFont));
            table.AddCell(new Phrase("Bond Paid", boldFont));
            table.AddCell(new Phrase(string.Format("Most{0}Serious{0}Offense", Environment.NewLine), boldFont));
            table.AddCell(new Phrase(string.Format("Non-Compliance{0}New Arrest/Tech", Environment.NewLine), boldFont));
            table.AddCell(new Phrase(string.Format("Recommendation{0}Revoked", Environment.NewLine), boldFont));
            table.AddCell(new Phrase(string.Format("Successful/Non-{0}Successful{0}Completion", Environment.NewLine), boldFont));
            table.AddCell(new Phrase(string.Format("Days SPR{0}Completion", Environment.NewLine), boldFont));
            table.DefaultCell.BackgroundColor = BaseColor.WHITE;

            string defendantName = "";
            string caseNumber = "";
            string arrestCharges = "";
            string indigent = "";
            string fcvDangerous = "";
            string fcvNonDangerous = "";
            string mcvDangerous = "";
            string mcvNonDangerous = "";
            string ftaDate = "";
            string courtAppearance = "";
            string bwOrdered = "";
            string bondPaid = "";
            string mostSeriosOffense = "";
            string nonCompViolation = "";
            string revoked = "";
            string successfull = "";
            string daysSPR = "";
            int indigentYes = 0;
            int indigentNo = 0;
            int indigentUk = 0;
            int FcDangerousTT = 0;
            int FcDangerous_h = 0;
            int FcNonDangerousTT = 0;
            int FcNonDangerous_h = 0;
            int dayssprTT = 0;
            int McDangerousTT = 0;
            int McDangerous_h = 0;
            int McNonDangerousTT = 0;
            int McNonDangerous_h = 0;
            int noPriors_h = 0;
            int bwOrderedYes = 0;
            int bwOrderedNo = 0;
            int bondPaidYes = 0;
            int bondPaidNo = 0;
            int Mso907TT = 0;
            int MsoNdFelonyTT = 0;
            int MsoMisdTT = 0;
            int MsoTotal = 0;
            int revokedTT = 0;
            int successfulTT = 0;
            int unSuccessfulTT = 0;
            int newArrestTT = 0;
            int violCallsTT = 0;
            int contactTT = 0;
            int otherTT = 0;
            int UATT = 0;
            int ViolationTT = 0;
            int defendantCount = 0;
            int courtAppearanceTT = 0;
            int FTACountTT = 0;
            bool isFcDangerous = false;
            bool isFcNonDangerous = false;
            bool isMcDangerous = false;
            bool isMcNonDangerous = false;
            var ctl = new DefendantInProgramController();
            IEnumerable<DefendantInProgram> defendantsInProgram = ctl.GetDefendantsInProgramByDate(InDate);

            defendantCount = defendantsInProgram.Count();
            foreach (DefendantInProgram defendantInProgram in defendantsInProgram)
            {
                isFcDangerous = false;
                isFcNonDangerous = false;
                isMcDangerous = false;
                isMcNonDangerous = false;
                defendantName = "";
                caseNumber = "";
                arrestCharges = "";
                indigent = "";
                fcvDangerous = "";
                fcvNonDangerous = "";
                mcvDangerous = "";
                mcvNonDangerous = "";
                ftaDate = "";
                courtAppearance = "";
                bwOrdered = "";
                nonCompViolation = "";
                mostSeriosOffense = "";
                bondPaid = "";
                revoked = "";
                successfull = "";
                daysSPR = "";
                defendantName = defendantInProgram.DefendantName;
                caseNumber = defendantInProgram.CaseNumber;
                arrestCharges = defendantInProgram.ArrestCharges;
                fcvDangerous = defendantInProgram.FcDangerous.ToString();
                if (defendantInProgram.FcDangerous > 0)
                {
                    isFcDangerous = true;
                    FcDangerousTT += defendantInProgram.FcDangerous;
                }
                fcvNonDangerous = defendantInProgram.FcNonDangerous.ToString();
                if (defendantInProgram.FcNonDangerous > 0)
                {
                    isFcNonDangerous = true;
                    FcNonDangerousTT += defendantInProgram.FcNonDangerous;
                }
                mcvDangerous = defendantInProgram.McDangerous.ToString();
                if (defendantInProgram.McDangerous > 0)
                {
                    isMcDangerous = true;
                    McDangerousTT += defendantInProgram.McDangerous;
                }
                mcvNonDangerous = defendantInProgram.McNonDangerous.ToString();
                if (defendantInProgram.McNonDangerous > 0)
                {
                    isMcNonDangerous = true;
                    McNonDangerousTT += defendantInProgram.McNonDangerous;
                }
                if (isFcDangerous)
                {
                    FcDangerous_h++;
                }
                else if (isFcNonDangerous)
                {
                    FcNonDangerous_h++;
                }
                else if (isMcDangerous)
                {
                    McDangerous_h++;
                }
                else if (isMcNonDangerous)
                {
                    McNonDangerous_h++;
                }
                else
                    noPriors_h++;
                if (defendantInProgram.CourtAppearances > 0)
                    courtAppearanceTT += defendantInProgram.CourtAppearances;
                if (defendantInProgram.Indigent == true)
                {
                    indigent = "Yes";
                    indigentYes++;
                }
                else if (defendantInProgram.Indigent == false)
                {
                    indigent = "No";
                    indigentNo++;
                }
                if (defendantInProgram.BwOrdered == true)
                {
                    bwOrdered = "Yes";
                    bwOrderedYes++;
                }
                else if (defendantInProgram.BwOrdered == false)
                {
                    bwOrdered = "No";
                    bwOrderedNo++;
                }
                if (defendantInProgram.BondPaid == true)
                {
                    bondPaid = "Yes";
                    bondPaidYes++;
                }
                else if (defendantInProgram.BondPaid == false)
                {
                    bondPaid = "No";
                    bondPaidNo++;
                }
                if (defendantInProgram.Completion.HasValue)
                {
                    if (defendantInProgram.Completion == 1)
                    {
                        successfull = "Successful";
                        successfulTT++;
                    }
                    else if (defendantInProgram.Completion == 0)
                    {
                        successfull = "Unsuccessful";
                        unSuccessfulTT++;
                    }
                }

                if (defendantInProgram.FtaDate.HasValue)
                {
                    ftaDate = defendantInProgram.FtaDate.Value.ToShortDateString();
                    FTACountTT++;
                }
                courtAppearance = defendantInProgram.CourtAppearances.ToString();
                nonCompViolation = defendantInProgram.NonCompArrestViolation;
                switch (nonCompViolation)
                {
                    case "New Arrest":
                        {
                            newArrestTT++;
                            ViolationTT++;
                            break;
                        }

                    case "Viol Calls":
                        {
                            violCallsTT++;
                            ViolationTT++;
                            break;
                        }

                    case "Contact":
                        {
                            contactTT++;
                            ViolationTT++;
                            break;
                        }

                    case "Other":
                        {
                            otherTT++;
                            ViolationTT++;
                            break;
                        }

                    case "UA":
                        {
                            UATT++;
                            ViolationTT++;
                            break;
                        }
                }
                mostSeriosOffense = defendantInProgram.MostSeriousOffense;
                switch (mostSeriosOffense)
                {
                    case "907.041 (Incl Domestic)":
                        {
                            Mso907TT++;
                            MsoTotal++;
                            break;
                        }
                    case "Non-Dangerous Felony":
                        {
                            MsoNdFelonyTT++;
                            MsoTotal++;
                            break;
                        }
                    case "Misd Only (Not Domestic)":
                        {
                            MsoMisdTT++;
                            MsoTotal++;
                            break;
                        }
                }
                if (defendantInProgram.IsRevoked)
                {
                    revoked = "Yes";
                    revokedTT++;
                }
                if (defendantInProgram.DaysSpr > 0)
                {
                    daysSPR = defendantInProgram.DaysSpr.ToString();
                    dayssprTT += defendantInProgram.DaysSpr;
                }
                table.AddCell(new Phrase(defendantName, normalFont));
                table.AddCell(new Phrase(caseNumber, normalFont));
                table.AddCell(new Phrase(arrestCharges, normalFont));
                table.AddCell(new Phrase(indigent, normalFont));
                table.AddCell(new Phrase(fcvDangerous, normalFont));
                table.AddCell(new Phrase(fcvNonDangerous, normalFont));
                table.AddCell(new Phrase(mcvDangerous, normalFont));
                table.AddCell(new Phrase(mcvNonDangerous, normalFont));
                table.AddCell(new Phrase(ftaDate, normalFont));
                table.AddCell(new Phrase(courtAppearance, normalFont));
                table.AddCell(new Phrase(bwOrdered, normalFont));
                table.AddCell(new Phrase(bondPaid, normalFont));
                table.AddCell(new Phrase(mostSeriosOffense, normalFont));
                table.AddCell(new Phrase(nonCompViolation, normalFont));
                table.AddCell(new Phrase(revoked, normalFont));
                table.AddCell(new Phrase(successfull, normalFont));
                table.AddCell(new Phrase(daysSPR, normalFont));
            }

            table.DefaultCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            AddBlankCells(table, 3);
            table.AddCell(new Phrase("YES", boldFont));
            table.AddCell(new Phrase("DANGEROUS", boldFont));
            table.AddCell(new Phrase("NON-DANGEROUS", boldFont));
            table.AddCell(new Phrase("DANGEROUS", boldFont));
            table.AddCell(new Phrase("NON-DANGEROUS", boldFont));
            table.AddCell(new Phrase("FTA COUNT", boldFont));
            table.AddCell(new Phrase("APPEARANCES", boldFont));
            table.AddCell(new Phrase("YES", boldFont));
            table.AddCell(new Phrase("YES", boldFont));
            table.AddCell(new Phrase("907.041", boldFont));
            table.AddCell(new Phrase("NEW ARREST", boldFont));
            table.AddCell(new Phrase("REVOKED", boldFont));
            table.AddCell(new Phrase("SUCCESSFUL", boldFont));
            AddBlankCells(table, 1);
            table.DefaultCell.BackgroundColor = BaseColor.WHITE;
            AddBlankCells(table, 3);
            table.AddCell(new Phrase(indigentYes.ToString(), normalFont));
            table.AddCell(new Phrase(FcDangerousTT.ToString(), normalFont));
            table.AddCell(new Phrase(FcNonDangerousTT.ToString(), normalFont));
            table.AddCell(new Phrase(McDangerousTT.ToString(), normalFont));
            table.AddCell(new Phrase(McNonDangerousTT.ToString(), normalFont));
            table.AddCell(new Phrase(FTACountTT.ToString(), normalFont)); // FTA Date Count
            table.AddCell(new Phrase(courtAppearanceTT.ToString(), normalFont)); // Court Appearances
            table.AddCell(new Phrase(bwOrderedYes.ToString(), normalFont));
            table.AddCell(new Phrase(bondPaidYes.ToString(), normalFont));
            table.AddCell(new Phrase(Mso907TT.ToString(), normalFont));
            table.AddCell(new Phrase(newArrestTT.ToString(), normalFont));
            table.AddCell(new Phrase(revokedTT.ToString(), normalFont));
            table.AddCell(new Phrase(successfulTT.ToString(), normalFont));
            AddBlankCells(table, 1);
            table.DefaultCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            AddBlankCells(table, 3);
            table.AddCell(new Phrase("NO", boldFont));
            AddBlankCells(table, 6);
            table.AddCell(new Phrase("NO", boldFont));
            table.AddCell(new Phrase("NO", boldFont));
            table.AddCell(new Phrase("NON-DANGEROUS", boldFont));
            table.AddCell(new Phrase("VIOL CALLS", boldFont));
            AddBlankCells(table, 1);
            table.DefaultCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.DefaultCell.NoWrap = true;
            table.AddCell(new Phrase("UNSUCCESSFUL", boldFont));
            table.DefaultCell.BackgroundColor = BaseColor.WHITE;
            AddBlankCells(table, 4);
            table.AddCell(new Phrase(indigentNo.ToString(), normalFont));
            AddBlankCells(table, 6);
            table.AddCell(new Phrase(bwOrderedNo.ToString(), normalFont));
            table.AddCell(new Phrase(bondPaidNo.ToString(), normalFont));
            table.AddCell(new Phrase(MsoNdFelonyTT.ToString(), normalFont));
            table.AddCell(new Phrase(violCallsTT.ToString(), normalFont));
            AddBlankCells(table, 1);
            table.AddCell(new Phrase(unSuccessfulTT.ToString(), normalFont));
            AddBlankCells(table, 1);

            table.DefaultCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            AddBlankCells(table, 4);
            AddBlankCells(table, 8);
            table.AddCell(new Phrase("MISD ONLY", boldFont));
            table.AddCell(new Phrase("CONTACT", boldFont));
            AddBlankCells(table, 3);
            table.DefaultCell.BackgroundColor = BaseColor.WHITE;
            AddBlankCells(table, 4);
            PdfPCell caption = new PdfPCell(new Phrase("Chart below is based on the defendants criminal histories. " + Environment.NewLine + "It categorizes each defendant by their most serious offense.", captionFont))
            {
                Colspan = 4,
                Border = 0
            };
            table.AddCell(caption);
            AddBlankCells(table, 4);
            table.AddCell(new Phrase(MsoMisdTT.ToString(), normalFont));
            table.AddCell(new Phrase(contactTT.ToString(), normalFont));
            AddBlankCells(table, 7);
            table.DefaultCell.BackgroundColor = new BaseColor(0, 183, 183);
            PdfPCell hdrCell = new PdfPCell(new Phrase("OFFENSE TYPE", boldFont));
            hdrCell.Colspan = 3;
            hdrCell.BackgroundColor = new BaseColor(0, 183, 183);
            table.AddCell(hdrCell);
            table.AddCell(new Phrase("TOTAL", boldFont));
            table.DefaultCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            AddBlankCells(table, 4);
            table.AddCell(new Phrase("MSO TOTALS", boldFont));
            table.AddCell(new Phrase("OTHER", boldFont));
            AddBlankCells(table, 3);
            table.DefaultCell.BackgroundColor = BaseColor.WHITE;
            AddBlankCells(table, 4);
            PdfPCell ttlCell = new PdfPCell(new Phrase("Dangerous Felony", boldFont));
            ttlCell.Colspan = 3;
            table.AddCell(ttlCell);
            table.AddCell(new Phrase(FcDangerous_h.ToString(), normalFont));
            AddBlankCells(table, 4);
            table.AddCell(new Phrase(MsoTotal.ToString(), normalFont));
            table.AddCell(new Phrase(otherTT.ToString(), normalFont));
            AddBlankCells(table, 7);
            PdfPCell ttlCell2 = new PdfPCell(new Phrase("Non-Dangerous Felony", boldFont));
            ttlCell2.Colspan = 3;
            table.AddCell(ttlCell2);
            table.AddCell(new Phrase(FcNonDangerous_h.ToString(), normalFont));
            table.DefaultCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            AddBlankCells(table, 5);
            table.AddCell(new Phrase("UA", boldFont));
            AddBlankCells(table, 3);
            table.DefaultCell.BackgroundColor = BaseColor.WHITE;
            AddBlankCells(table, 4);
            PdfPCell ttlCell3 = new PdfPCell(new Phrase("Dangerous Misdemeanor", boldFont));
            ttlCell3.Colspan = 3;
            table.AddCell(ttlCell3);
            table.AddCell(new Phrase(McDangerous_h.ToString(), normalFont));
            AddBlankCells(table, 5);
            table.AddCell(new Phrase(UATT.ToString(), normalFont));
            AddBlankCells(table, 7);
            PdfPCell ttlCell4 = new PdfPCell(new Phrase("Non-Dangerous Misdemeanor", boldFont));
            ttlCell4.Colspan = 3;
            table.AddCell(ttlCell4);
            table.AddCell(new Phrase(McNonDangerous_h.ToString(), normalFont));

            table.DefaultCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            AddBlankCells(table, 5);
            table.AddCell(new Phrase("VIOL TOTALS", boldFont));
            AddBlankCells(table, 3);
            table.DefaultCell.BackgroundColor = BaseColor.WHITE;
            AddBlankCells(table, 4);
            PdfPCell ttlCell5 = new PdfPCell(new Phrase("No prior offense", boldFont))
            {
                Colspan = 3
            };
            table.AddCell(ttlCell5);
            table.AddCell(new Phrase(noPriors_h.ToString(), normalFont));

            AddBlankCells(table, 5);
            table.AddCell(new Phrase(ViolationTT.ToString(), normalFont));
            AddBlankCells(table, 3);
            DayTotal dayTotal = new DayTotal();
            dayTotal.Defendants = defendantCount;
            dayTotal.Day = InDate.Day;
            dayTotal.BwOrderedNo = bwOrderedNo;
            dayTotal.BwOrderedYes = bwOrderedYes;
            dayTotal.BondPaidNo = bondPaidNo;
            dayTotal.BondPaidYes = bondPaidYes;
            dayTotal.Mso907 = Mso907TT;
            dayTotal.MsoMisd = MsoMisdTT;
            dayTotal.MsoNonDangerous = MsoNdFelonyTT;
            dayTotal.FcDangerous = FcDangerousTT;
            dayTotal.FcNonDangerous = FcNonDangerousTT;
            dayTotal.IndigentNo = indigentNo;
            dayTotal.IndigentUkn = indigentUk;
            dayTotal.IndigentYes = indigentYes;
            dayTotal.McDangerous = McDangerousTT;
            dayTotal.McNonDangerous = McNonDangerousTT;
            dayTotal.NonCompContact = contactTT;
            dayTotal.NonCompNewArrest = newArrestTT;
            dayTotal.NonCompOther = otherTT;
            dayTotal.NonCompUa = UATT;
            dayTotal.NonCompViolCalls = violCallsTT;
            dayTotal.Successfull = successfulTT;
            dayTotal.UnSuccessfull = unSuccessfulTT;
            dayTotal.FtaCount = FTACountTT;
            dayTotal.CourtAppearances = courtAppearanceTT;
            dayTotal.Revoked = revokedTT;
            dayTotal.FcDangerous_h = FcDangerous_h;
            dayTotal.FcNonDangerous_h = FcNonDangerous_h;
            dayTotal.McDangerous_h = McDangerous_h;
            dayTotal.McNonDangerous_h = McNonDangerous_h;
            dayTotal.NoPriors = noPriors_h;
            dayTotal.DaysSpr = dayssprTT;
            colDefendantDayTotal.Add(dayTotal);
            return table;
        }

        private PdfPTable GetCombinedTotalsTop(DateTime inDate, bool IsMonthEnd, bool IsYearEnd)
        {
            List<IntakeLogItem> colIntakeTemp = null;
            PdfPTable table = new PdfPTable(13);
            table.DefaultCell.BackgroundColor = new BaseColor(255, 255, 204); // YELLOW
            table.DefaultCell.HorizontalAlignment = 1;
            table.HorizontalAlignment = 0;
            table.WidthPercentage = 90;
            table.HeaderRows = 1;
            table.AddCell(new Phrase("Date", boldFont));
            table.AddCell(new Phrase("# Interviewed", boldFont));
            table.AddCell(new Phrase("# Assessed", boldFont));
            table.AddCell(new Phrase("# Recommended" + Environment.NewLine + " for PTR", boldFont));
            table.AddCell(new Phrase("# PTR not" + Environment.NewLine + " Recommended", boldFont));
            table.AddCell(new Phrase("# Accepted" + Environment.NewLine + " into Program", boldFont));
            table.AddCell(new Phrase("# Indigent" + Environment.NewLine + " Int/Assessed", boldFont));

            AddBlankCells(table, 6);
            table.DefaultCell.BackgroundColor = BaseColor.WHITE;
            table.DefaultCell.VerticalAlignment = Element.ALIGN_BOTTOM;
            int startDay = inDate.Day - 6;
            if (inDate.Day > 28)
                startDay = 29;
            int endDay = inDate.Day;
            if (IsMonthEnd)
                startDay = 1;

            string dateText = GetWeekTextValue(inDate);
            if (IsMonthEnd)
                dateText = "Month End";

            if (IsYearEnd)
            {
                dateText = "Year End";
                colIntakeTemp = colIntakeRunningTotal;
            }
            else
                colIntakeTemp = colIntake;
            var query = from i in colIntakeTemp
                        where i.IntakeDay >= startDay & i.IntakeDay <= endDay
                        select i;
            int interviewTT = query.Sum(i => i.Interviewed.Value);

            int assessedTT = query.Sum(i => i.Assessed.Value);
            int recommendPtrTT = query.Sum(i => i.PtrRecommended.Value);
            int notRecommednPtrTT = query.Sum(i => i.PtrNotRecommended.Value);
            int acceptedTT = query.Sum(i => i.PtrOrdered.Value);
            int indigentAssessedTT = query.Sum(i => i.IndigentAssessed.Value);

            table.AddCell(new Phrase(dateText, boldFont));
            table.AddCell(new Phrase(interviewTT.ToString(), normalFont));
            table.AddCell(new Phrase(assessedTT.ToString(), normalFont));
            table.AddCell(new Phrase(recommendPtrTT.ToString(), normalFont));
            table.AddCell(new Phrase(notRecommednPtrTT.ToString(), normalFont));
            table.AddCell(new Phrase(acceptedTT.ToString(), normalFont));
            table.AddCell(new Phrase(indigentAssessedTT.ToString(), normalFont));
            AddBlankCells(table, 4);
            AddBlankCells(table, 11);
            if (IsMonthEnd)
            {
                colIntakeRunningTotal.AddRange(colIntake);
                colIntake.Clear();
            }
            return table;
        }

        private PdfPTable GetCombinedTotalsBottom(DateTime inDate, bool IsMonthEnd, bool IsYearEnd)
        {
            List<DayTotal> colDefendantDayTotalTemp = null;
            PdfPTable table = new PdfPTable(13);
            table.DefaultCell.BackgroundColor = new BaseColor(255, 255, 204); // YELLOW
            table.DefaultCell.HorizontalAlignment = 1;
            table.HorizontalAlignment = 0;
            table.WidthPercentage = 90;
            table.HeaderRows = 1;
            int startDay = inDate.Day - 6;
            if (inDate.Day > 28)
                startDay = 29;
            int endDay = inDate.Day;
            if (IsMonthEnd)
                startDay = 1;
            if (IsYearEnd)
                colDefendantDayTotalTemp = colDefendantRunningTotal;
            else
                colDefendantDayTotalTemp = colDefendantDayTotal;
            var query2 = from d in colDefendantDayTotalTemp
                         where d.Day >= startDay & d.Day <= endDay
                         select d;
            int itemCount = query2.Sum(c => c.Defendants);
            int indigentYesTT = query2.Sum(c => c.IndigentYes);
            int indigentNoTT = query2.Sum(c => c.IndigentNo);
            int indigentUnkTT = query2.Sum(c => c.IndigentUkn);
            int FcDangerousTT = query2.Sum(c => c.FcDangerous);
            int FcDangerousTT_h = query2.Sum(c => c.FcDangerous_h);
            int FcNonDangerousTT = query2.Sum(c => c.FcNonDangerous);
            int FcNonDangerousTT_h = query2.Sum(c => c.FcNonDangerous_h);
            int McDangerousTT = query2.Sum(c => c.McDangerous);
            int McDangerousTT_h = query2.Sum(c => c.McDangerous_h);
            int McNonDangerousTT = query2.Sum(c => c.McNonDangerous);
            int McNonDangerousTT_h = query2.Sum(c => c.McNonDangerous_h);
            int NoPriorsTT = query2.Sum(c => c.NoPriors);
            int bworderedYesTT = query2.Sum(c => c.BwOrderedYes);
            int bwOrderedNoTT = query2.Sum(c => c.BwOrderedNo);
            int bondPaidYesTT = query2.Sum(c => c.BondPaidYes);
            int bondPaidNoTT = query2.Sum(c => c.BondPaidNo);
            int mso907TT = query2.Sum(c => c.Mso907);
            int msoMisdTT = query2.Sum(c => c.MsoMisd);
            int msoTotal = query2.Sum(c => c.MsoTotals);
            int msoNonDangerousTT = query2.Sum(c => c.MsoNonDangerous);
            int ncArrestTT = query2.Sum(c => c.NonCompNewArrest);
            int ncViolCallsTT = query2.Sum(c => c.NonCompViolCalls);
            int ncContactTT = query2.Sum(c => c.NonCompContact);
            int ncOtherTT = query2.Sum(c => c.NonCompOther);
            int ncUATT = query2.Sum(c => c.NonCompUa);
            int ncTotalsTT = query2.Sum(c => c.NonCompTotals);
            int revokedTT = query2.Sum(c => c.Revoked);
            int successfullTT = query2.Sum(c => c.Successfull);
            int daysSprTT = 0;
            if (itemCount > 0)
                daysSprTT = query2.Sum(c => c.DaysSpr) / itemCount;// Modified 11/7/2016 Ticket:42051
            int ftaCountTT = query2.Sum(c => c.FtaCount);
            int unsuccessfulTT = query2.Sum(c => c.UnSuccessfull);
            int courtAppearanceTT = query2.Sum(c => c.CourtAppearances);
            table.DefaultCell.BackgroundColor = new BaseColor(255, 204, 153); // ORANGE
            table.AddCell(new Phrase("Defendants", boldFont));
            table.AddCell(new Phrase("Indigent PD Appointed", boldFont));
            table.AddCell(new Phrase("# Felony Convictions", boldFont));
            table.AddCell(new Phrase("# Misdemeanor Convictions", boldFont));
            table.AddCell(new Phrase("FTA Dates", boldFont));
            table.AddCell(new Phrase("Court Appearances", boldFont));
            table.AddCell(new Phrase("BW Ordered", boldFont));
            table.AddCell(new Phrase("Bond Paid", boldFont));
            table.AddCell(new Phrase("Most Serious Offense", boldFont));
            table.AddCell(new Phrase("Non-Compliance", boldFont));
            table.AddCell(new Phrase("Recommendation", boldFont));
            table.AddCell(new Phrase("Completion", boldFont));
            table.AddCell(new Phrase("Days SPR", boldFont));
            table.DefaultCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(new Phrase("COUNT", boldFont));
            table.AddCell(new Phrase("YES", boldFont));
            table.AddCell(new Phrase("DANGEROUS", boldFont));
            table.AddCell(new Phrase("DANGEROUS", boldFont));
            table.AddCell(new Phrase("COUNT", boldFont));
            table.AddCell(new Phrase("COUNT", boldFont));
            table.AddCell(new Phrase("YES", boldFont));
            table.AddCell(new Phrase("YES", boldFont));
            table.AddCell(new Phrase("907.041", boldFont));
            table.AddCell(new Phrase("NEW ARREST", boldFont));
            table.AddCell(new Phrase("REVOKED", boldFont));
            table.AddCell(new Phrase("SUCCESSFUL", boldFont));
            table.AddCell(new Phrase("AVERAGE", boldFont));
            table.DefaultCell.BackgroundColor = BaseColor.WHITE;
            table.AddCell(new Phrase(itemCount.ToString(), normalFont));
            table.AddCell(new Phrase(indigentYesTT.ToString(), normalFont));
            table.AddCell(new Phrase(FcDangerousTT.ToString(), normalFont));
            table.AddCell(new Phrase(McDangerousTT.ToString(), normalFont));
            table.AddCell(new Phrase(ftaCountTT.ToString(), normalFont));
            table.AddCell(new Phrase(courtAppearanceTT.ToString(), normalFont));
            table.AddCell(new Phrase(bworderedYesTT.ToString(), normalFont));
            table.AddCell(new Phrase(bondPaidYesTT.ToString(), normalFont));
            table.AddCell(new Phrase(mso907TT.ToString(), normalFont));
            table.AddCell(new Phrase(ncArrestTT.ToString(), normalFont));
            table.AddCell(new Phrase(revokedTT.ToString(), normalFont));
            table.AddCell(new Phrase(successfullTT.ToString(), normalFont));
            table.AddCell(new Phrase(daysSprTT.ToString(), normalFont));
            table.DefaultCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            AddBlankCells(table, 1);
            table.AddCell(new Phrase("NO", boldFont));
            table.AddCell(new Phrase("NON-DANGEROUS", boldFont));
            table.AddCell(new Phrase("NON-DANGEROUS", boldFont));
            AddBlankCells(table, 2);
            table.AddCell(new Phrase("NO", boldFont));
            table.AddCell(new Phrase("NO", boldFont));
            table.AddCell(new Phrase("NON-DANGEROUS", boldFont));
            table.AddCell(new Phrase("VIOL CALLS", boldFont));
            AddBlankCells(table, 1);
            table.AddCell(new Phrase("UNSUCCESSFUL", boldFont));
            AddBlankCells(table, 1);
            table.DefaultCell.BackgroundColor = BaseColor.WHITE;
            AddBlankCells(table, 1);
            table.AddCell(new Phrase(indigentNoTT.ToString(), normalFont));
            table.AddCell(new Phrase(FcNonDangerousTT.ToString(), normalFont));
            table.AddCell(new Phrase(McNonDangerousTT.ToString(), normalFont));
            AddBlankCells(table, 2);
            table.AddCell(new Phrase(bwOrderedNoTT.ToString(), normalFont));
            table.AddCell(new Phrase(bondPaidNoTT.ToString(), normalFont));
            table.AddCell(new Phrase(msoNonDangerousTT.ToString(), normalFont));
            table.AddCell(new Phrase(ncViolCallsTT.ToString(), normalFont));
            AddBlankCells(table, 1);
            table.AddCell(new Phrase(unsuccessfulTT.ToString(), normalFont));
            AddBlankCells(table, 1);
            table.DefaultCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            AddBlankCells(table, 2);
            AddBlankCells(table, 6);
            table.AddCell(new Phrase("MISD ONLY", boldFont));
            table.AddCell(new Phrase("CONTACT", boldFont));
            AddBlankCells(table, 4);
            table.DefaultCell.BackgroundColor = BaseColor.WHITE;
            AddBlankCells(table, 7);
            table.AddCell(new Phrase(msoMisdTT.ToString(), normalFont));
            table.AddCell(new Phrase(ncContactTT.ToString(), normalFont));
            AddBlankCells(table, 3);
            table.DefaultCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            AddBlankCells(table, 8);
            table.AddCell(new Phrase("MSO TOTALS", boldFont));
            table.AddCell(new Phrase("OTHER", boldFont));
            AddBlankCells(table, 3);
            table.DefaultCell.BackgroundColor = BaseColor.WHITE;
            AddBlankCells(table, 8);
            table.AddCell(new Phrase(msoTotal.ToString(), normalFont));
            table.AddCell(new Phrase(ncOtherTT.ToString(), normalFont));
            AddBlankCells(table, 3);
            table.DefaultCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            AddBlankCells(table, 9);
            table.AddCell(new Phrase("UA", boldFont));
            AddBlankCells(table, 3);
            table.DefaultCell.BackgroundColor = BaseColor.WHITE;
            AddBlankCells(table, 9);
            table.AddCell(new Phrase(ncUATT.ToString(), normalFont));
            AddBlankCells(table, 3);
            table.DefaultCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            AddBlankCells(table, 9);
            table.AddCell(new Phrase("VIOL TOTALS", boldFont));
            AddBlankCells(table, 3);
            table.DefaultCell.BackgroundColor = BaseColor.WHITE;
            AddBlankCells(table, 9);
            table.AddCell(new Phrase(ncTotalsTT.ToString(), normalFont));
            AddBlankCells(table, 3);
            PdfPCell caption = new PdfPCell(new Phrase("Chart below is based on the defendants criminal histories. " + Environment.NewLine + "It categorizes each defendant by their most serious offense.", captionFont));
            caption.Colspan = 4;
            caption.Border = 0;
            table.AddCell(caption);
            AddBlankCells(table, 9);
            table.DefaultCell.BackgroundColor = new BaseColor(0, 183, 183);
            PdfPCell hdrCell = new PdfPCell(new Phrase("OFFENSE TYPE", boldFont));
            hdrCell.Colspan = 3;
            hdrCell.BackgroundColor = new BaseColor(0, 183, 183);
            table.AddCell(hdrCell);
            table.AddCell(new Phrase("TOTAL", boldFont));
            table.DefaultCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            AddBlankCells(table, 9);
            PdfPCell ttlCell = new PdfPCell(new Phrase("Dangerous Felony", boldFont));
            ttlCell.Colspan = 3;
            table.AddCell(ttlCell);
            table.AddCell(new Phrase(FcDangerousTT_h.ToString(), normalFont));
            AddBlankCells(table, 9);
            PdfPCell ttlCell2 = new PdfPCell(new Phrase("Non-Dangerous Felony", boldFont));
            ttlCell2.Colspan = 3;
            table.AddCell(ttlCell2);
            table.AddCell(new Phrase(FcNonDangerousTT_h.ToString(), normalFont));
            AddBlankCells(table, 9);
            PdfPCell ttlCell3 = new PdfPCell(new Phrase("Dangerous Misdemeanor", boldFont));
            ttlCell3.Colspan = 3;
            table.AddCell(ttlCell3);
            table.AddCell(new Phrase(McDangerousTT_h.ToString(), normalFont));
            AddBlankCells(table, 9);
            PdfPCell ttlCell4 = new PdfPCell(new Phrase("Non-Dangerous Misdemeanor", boldFont));
            ttlCell4.Colspan = 3;
            table.AddCell(ttlCell4);
            table.AddCell(new Phrase(McNonDangerousTT_h.ToString(), normalFont));
            AddBlankCells(table, 9);
            PdfPCell ttlCell5 = new PdfPCell(new Phrase("No prior offense", boldFont));
            ttlCell5.Colspan = 3;
            table.AddCell(ttlCell5);
            table.AddCell(new Phrase(NoPriorsTT.ToString(), normalFont));
            AddBlankCells(table, 9);
            if (IsMonthEnd)
            {
                colDefendantRunningTotal.AddRange(colDefendantDayTotal);
                colDefendantDayTotal.Clear();
            }

            return table;
        }

        public void WriteDaily(ref Document doc, DateTime Indate)
        {
            Paragraph pDates = new Paragraph(reportTitle + Environment.NewLine + "For " + beginningDate + " to " + enddingDate + Environment.NewLine + " ", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD));
            pDates.Alignment = Element.ALIGN_CENTER;
            doc.Add(pDates);
            Paragraph pCurrentDate = new Paragraph(Indate.ToShortDateString() + Environment.NewLine + " ", new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL));
            doc.Add(pCurrentDate);
            doc.Add(getIntakelog(Indate));
            Paragraph pDefendant = new Paragraph("Defendants Ordered into Program " + Environment.NewLine + " ", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLDITALIC));
            pDefendant.Alignment = Element.ALIGN_CENTER;
            doc.Add(pDefendant);
            doc.Add(GetDefendantLog(Indate));
            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(string.Format("* Sarasota PTS does not make recommendations @ 1st appearance."), new Font(Font.FontFamily.HELVETICA, 5)));
            doc.Add(new Paragraph("* In accordance with the FCIC/NCIC rules, only convictions in the State of Florida are reflected.", new Font(Font.FontFamily.HELVETICA, 5)));
            if (reportType == ReportType.weekly | reportType == ReportType.monthly | reportType == ReportType.yearly)
            {
                if (IsWeekend(Indate))
                {
                    doc.NewPage();
                    Paragraph pTitleW = new Paragraph(reportTitleW + Environment.NewLine + "For " + GetWeekTextValue(Indate).Replace("Week", Indate.Year.ToString() + " Week") + Environment.NewLine + " ", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD));
                    pTitleW.Alignment = Element.ALIGN_CENTER;
                    doc.Add(pTitleW);
                    doc.Add(GetCombinedTotalsTop(Indate, false, false));
                    doc.Add(pDefendant);
                    doc.Add(GetCombinedTotalsBottom(Indate, false, false));

                }
            }
            if (reportType == ReportType.monthly | reportType == ReportType.yearly)
            {
                if (IsMonthEnd(Indate))
                {
                    doc.NewPage();
                    Paragraph pTitleM = new Paragraph(reportTitleM + Environment.NewLine + "For " + Indate.ToString("MMMM") + " " + Indate.Year.ToString() + Environment.NewLine + " ", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD));
                    pTitleM.Alignment = Element.ALIGN_CENTER;
                    doc.Add(pTitleM);
                    doc.Add(GetCombinedTotalsTop(Indate, true, false));
                    doc.Add(pDefendant);
                    doc.Add(GetCombinedTotalsBottom(Indate, true, false));
                    doc.NewPage();
                }
            }
            if (reportType == ReportType.yearly)
            {
                if (IsYearEnd(Indate))
                {
                    Paragraph pTitleM = new Paragraph(reportTitleY + Environment.NewLine + "For " + beginningDate + " to " + enddingDate + Environment.NewLine + " ", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD));
                    pTitleM.Alignment = Element.ALIGN_CENTER;
                    doc.Add(pTitleM);
                    doc.Add(GetCombinedTotalsTop(Indate, true, true));
                    doc.Add(pDefendant);
                    doc.Add(GetCombinedTotalsBottom(Indate, true, true));

                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Document doc = new Document(PageSize.LETTER.Rotate(), 10, 10, 42, 35);
            string reportName = "";
            string reportTypeQSValue = "";
            doc.SetMargins(20, 20, 30, 50);
            if (Request.QueryString["mid"] != null)
                Int32.TryParse(Request.QueryString["mid"].ToString(), out ModuleId);
            if (Request.QueryString["rid"] != null)
                reportTypeQSValue = Request.QueryString["rid"].ToString();
            if (Request.QueryString["indate"] != null)
                reportDate = DateTime.Parse(Request.QueryString["indate"]);

            if (ModuleId > 0)
            {
                var mCtl = new ModuleController();
                ModuleInfo moduleInfo = mCtl.GetModule(ModuleId);
                if (moduleInfo.TabModuleSettings.Contains("ReportDirectory"))
                {
                    ReportRootUrl = moduleInfo.TabModuleSettings["ReportDirectory"].ToString();
                }
            }
            if (reportTypeQSValue == "daily")
            {
                reportType = ReportType.daily;
            }
            if (reportTypeQSValue == "monthly")
            {
                reportType = ReportType.monthly;
            }
            if (reportTypeQSValue == "weekly")
            {
                reportType = ReportType.weekly;
            }
            if (reportTypeQSValue == "yearly")
            {
                reportType = ReportType.yearly;
            }
            string reportHeader = string.Format("Sarasota County Board of County Commissioners Pretrial Services");
            reportTitle = string.Format("{0} Daily Register 12th Judicial Circuit", reportHeader);
            reportTitleW = string.Format("{0} Weekly Register 12th Judicial Circuit", reportHeader);
            reportTitleM = string.Format("{0} Monthly Register 12th Judicial Circuit", reportHeader);
            reportTitleY = string.Format("{0} Register 12th Judicial Circuit Yearly Totals", reportHeader);
            string appPath = Request.MapPath(ReportRootUrl);

            if (reportType == ReportType.daily)
            {
                beginningDate = reportDate.ToShortDateString();
                enddingDate = beginningDate;
                reportName = string.Format("pts-CRTK-{0}-{1}-{2}.pdf", reportDate.Month.ToString(), reportDate.Day.ToString(), reportDate.Year.ToString());
                PdfWriter pdfWriter1 = PdfWriter.GetInstance(doc, new FileStream(appPath + reportName, FileMode.Create));
                pdfWriter1.PageEvent = PageEventHandler;
                pdfWriter1.SetFullCompression();
                pdfWriter1.StrictImageSequence = true;
                pdfWriter1.SetLinearPageMode();
                // Define the page header

                PageEventHandler.Title = " ";
                PageEventHandler.HeaderFont = FontFactory.GetFont(BaseFont.COURIER_BOLD, 10, Font.BOLD);
                doc.Open();
                WriteDaily(ref doc, reportDate);
                doc.Close();

            }
            else if (reportType == ReportType.monthly)
            {
                int firstDay = 1;
                int lastDay = GetLastDayOfMonth(reportDate).Day;
                beginningDate = new DateTime(reportDate.Year, reportDate.Month, firstDay).ToShortDateString();
                enddingDate = new DateTime(reportDate.Year, reportDate.Month, lastDay).ToShortDateString();
                reportName = string.Format("pts-CRTK-Monthly-{0}-{1}.pdf", reportDate.Month.ToString(), reportDate.Year.ToString());
                PdfWriter pdfWriter1 = PdfWriter.GetInstance(doc, new FileStream(appPath + reportName, FileMode.Create));

                pdfWriter1.PageEvent = PageEventHandler;
                pdfWriter1.SetFullCompression();
                pdfWriter1.StrictImageSequence = true;
                pdfWriter1.SetLinearPageMode();

                // Define the page header
                PageEventHandler.Title = " ";
                PageEventHandler.HeaderFont = FontFactory.GetFont(BaseFont.COURIER_BOLD, 10, Font.BOLD);
                doc.Open();
                for (int cDay = firstDay; cDay <= lastDay; cDay++)
                {
                    DateTime indate = new DateTime(reportDate.Year, reportDate.Month, cDay);
                    WriteDaily(ref doc, indate);
                    if (cDay != lastDay)
                        doc.NewPage();
                }
                doc.Close();

            }
            else if (reportType == ReportType.weekly)
            {
                int firstDay = GetWeekStartDay(reportDate);
                int lastDayMonth = GetLastDayOfMonth(reportDate).Day;
                int lastDay = firstDay + 6;
                if (lastDay > lastDayMonth)
                    lastDay = lastDayMonth;
                beginningDate = new DateTime(reportDate.Year, reportDate.Month, firstDay).ToShortDateString();
                enddingDate = new DateTime(reportDate.Year, reportDate.Month, lastDay).ToShortDateString();
                reportName = string.Format("pts-CRTK-{0}-{1}-{2}.pdf", GetWeekTextValue(reportDate).Replace(" ", "-"), reportDate.Month.ToString(), reportDate.Year.ToString());
                PdfWriter pdfWriter1 = PdfWriter.GetInstance(doc, new FileStream(appPath + reportName, FileMode.Create));
                pdfWriter1.PageEvent = PageEventHandler;
                pdfWriter1.SetFullCompression();
                pdfWriter1.StrictImageSequence = true;
                pdfWriter1.SetLinearPageMode();

                // Define the page header
                PageEventHandler.Title = " ";
                PageEventHandler.HeaderFont = FontFactory.GetFont(BaseFont.COURIER_BOLD, 10, Font.BOLD);

                doc.Open();
                for (int cDay = firstDay; cDay <= lastDay; cDay++)
                {
                    DateTime indate = new DateTime(reportDate.Year, reportDate.Month, cDay);
                    WriteDaily(ref doc, indate);
                    if (cDay != lastDay)
                        doc.NewPage();
                }
                doc.Close();

            }
            else if (reportType == ReportType.yearly)
            {
                DateTime EndDateMonth = new DateTime(reportDate.Year, reportDate.Month, GetLastDayOfMonth(reportDate).Day);
                DateTime BeginDateMonth = EndDateMonth.AddMonths(-12).AddDays(1);
                beginningDate = BeginDateMonth.ToShortDateString();
                enddingDate = EndDateMonth.ToShortDateString();
                reportName = string.Format("pts-CRTK-Yearly-{0}-{1}.pdf", reportDate.Month.ToString(), reportDate.Year.ToString());
                PdfWriter pdfWriter1 = PdfWriter.GetInstance(doc, new FileStream(appPath + reportName, FileMode.Create));
                pdfWriter1.PageEvent = PageEventHandler;
                pdfWriter1.SetFullCompression();
                pdfWriter1.StrictImageSequence = true;
                pdfWriter1.SetLinearPageMode();

                // Define the page header
                PageEventHandler.Title = " ";
                PageEventHandler.HeaderFont = FontFactory.GetFont(BaseFont.COURIER_BOLD, 10, Font.BOLD);
                doc.Open();

                while (BeginDateMonth < EndDateMonth)
                {
                    int firstDay = 1;
                    int lastDay = GetLastDayOfMonth(BeginDateMonth).Day;
                    for (int cDay = firstDay; cDay <= lastDay; cDay++)
                    {
                        DateTime indate = new DateTime(BeginDateMonth.Year, BeginDateMonth.Month, cDay);
                        WriteDaily(ref doc, indate);
                        if (cDay != lastDay)
                            doc.NewPage();
                    }
                    BeginDateMonth = BeginDateMonth.AddMonths(1);
                }
                doc.Close();

            }

            Response.Redirect(string.Format("{0}{1}?ver={2}", ReportRootUrl, reportName, System.Web.HttpUtility.UrlEncode(DateTime.Now.ToString("MMddyyyhhmmss"))));

        }
    }
}
