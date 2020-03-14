using System;
using System.Collections.Generic;
using System.Text;

namespace ExcelIO
{
    public class ExcelData
    {
        private string viewPlan;
        private string level;
        private string sheet;
        public ExcelData(string viewPlan, string level, string sheet)
        {
            this.viewPlan = viewPlan;
            this.level = level;
            this.sheet = sheet;
        }
        public string ViewPlan { get => viewPlan; set => viewPlan = value; }
        public string Level { get => level; set => level = value; }
        public string Sheet { get => sheet; set => sheet = value; }
    }
}
