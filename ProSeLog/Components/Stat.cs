using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tjc.Modules.ProSeLog.Components
{
    internal class Stat
    {
        public Stat()
        {
        }

        public Stat(string fieldName, int simpDom, int dom, int domch, int nc, int spa, int cust, int modif, int cont, int pat, int cs, int other)
        {
            this.FieldName = fieldName;
            this.SimpDom = simpDom;
            this.DOM = dom;
            this.DOMCH = domch;
            this.NC = nc;
            this.SPA = spa;
            this.CUST = cust;
            this.MODIF = modif;
            this.CONT = cont;
            this.PAT = pat;
            this.CS = cs;
            this.Other = other;
        }
        public int GroupId
        {
            get;set;
        }
        public int Total
        {
            get; set;
        }
        public int Other
        {
            get; set;
        }
        public int CS
        {
            get; set;
        }
        public int PAT
        {
            get; set;
        }
        public int CONT
        {
            get; set;
        }
        public int MODIF
        {
            get; set;
        }
        public int CUST
        {
            get; set;
        }
        public int SPA
        {
            get; set;
        }
        public int NC
        {
            get; set;
        }
        public int DOMCH
        {
            get; set;
        }
        public int DOM
        {
            get; set;
        }
        public string FieldName
        {
            get; set;
        }
        public int SimpDom
        {
            get; set;
        }
    }
}