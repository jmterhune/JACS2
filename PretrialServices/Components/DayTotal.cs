using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tjc.Modules.PretrialServices.Components
{
    public class DayTotal
    {
        public int IndigentYes
        {
            get; set;
        }
        public int Defendants
        {
            get; set;
        }
        public int IndigentNo
        {
            get; set;
        }
        public int IndigentUkn
        {
            get; set;
        }
        public int FcDangerous
        {
            get; set;
        }
        public int FcDangerous_h
        {
            get; set;
        }
        public int FcNonDangerous
        {
            get; set;
        }
        public int FcNonDangerous_h
        {
            get; set;
        }
        public int McDangerous
        {
            get; set;
        }
        public string MostSeriousOffense { get; set; }

        public int McDangerous_h
        {
            get; set;
        }
        public int McNonDangerous
        {
            get; set;
        }
        public int McNonDangerous_h
        {
            get; set;
        }
        public int NoPriors
        {
            get; set;
        }
        public int BwOrderedYes
        {
            get; set;
        }
        public int Mso907
        {
            get; set;
        }
        public int MsoNonDangerous
        {
            get; set;
        }
        public int MsoMisd
        {
            get; set;
        }
        public int MsoTotals
        {
            get
            {
                return Mso907 + MsoNonDangerous + MsoMisd;
            }
        }
        public int NonCompNewArrest
        {
            get; set;
        }
        public int NonCompViolCalls
        {
            get; set;
        }
        public int NonCompContact
        {
            get; set;
        }
        public int NonCompOther
        {
            get; set;
        }
        public int NonCompUa
        {
            get; set;
        }
        public int NonCompTotals
        {
            get
            {
                return NonCompNewArrest + NonCompContact + NonCompViolCalls + NonCompOther + NonCompUa;
            }
        }
        public int Day
        {
            get; set;
        }
        public int CourtAppearances
        {
            get; set;
        }
        public int BwOrderedNo
        {
            get; set;
        }
        public int Revoked
        {
            get; set;
        }
        public int Successfull
        {
            get; set;
        }
        public int UnSuccessfull
        {
            get; set;
        }
        public int DaysSpr
        {
            get; set;
        }
        public int FtaCount
        {
            get; set;
        }
    }
}