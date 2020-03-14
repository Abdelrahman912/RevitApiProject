using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExcelIO;
using Revit;

namespace RevitLogic
{
  public  class FromExcelToRevit
    {
        #region Member Variables
        private Excel excel;
        private  List<ExcelData> data;
        RevitApp revit;
        #endregion

        #region Constructors
        public FromExcelToRevit(ExternalCommandData commandData,string workSheetName,string tableName)
        {
            Revit = new RevitApp(commandData);
            FileOpenDialog dialog = new FileOpenDialog("Excel Files(*.xlsx) | *.xlsx");
            if (dialog.Show() == ItemSelectionDialogResult.Confirmed)
            {
                string path = ModelPathUtils.ConvertModelPathToUserVisiblePath(dialog.GetSelectedModelPath());
                 excel = new Excel(path);
                data = new List<ExcelData>();
                data = excel.ReadFromTable<ExcelData>(workSheetName, tableName, (v, i) =>
                {
                    return new ExcelData(v[i, 0].ToString(), v[i, 1].ToString(), v[i, 2].ToString());
                });
            }
            else
            {
                data = null;
            }
        }

        #endregion

        #region Properties

        public RevitApp Revit { get => revit; set => revit = value; }
        #endregion

        #region Methods
        /// <summary>
        /// Create Levels and Floor Plans from excel sheet
        /// </summary>
        /// <returns></returns>
        public bool CreateLevelsAndViewPlans()
        {
            bool flag = true;

            if (data != null)
            {
                Level level;
                ViewFamilyType viewType = Revit.SelectClassBased<ViewFamilyType>(true, x => x.ViewFamily == ViewFamily.FloorPlan); //select floor plan type only
                foreach (var d in data)
                {
                    level = Level.Create(Revit.Doc, UnitUtils.ConvertToInternalUnits(Convert.ToDouble(d.Level), DisplayUnitType.DUT_METERS));

                    Revit.CreatePlanView(level.Name, d.ViewPlan);
                }

            }
            else
            {
                flag = false;
            }



            return flag;
        }
        #endregion

    }
}
