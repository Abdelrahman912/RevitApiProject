using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using System.Linq;
using System.IO;
using OfficeOpenXml;
using RevitLogic;

namespace RevitCommands
{
    [Transaction(TransactionMode.Manual)]
    class CreateLevelsFromExcel : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                FromExcelToRevit e2r = new FromExcelToRevit(commandData, "revitSheet", "revitData");
                using (Transaction trans = new Transaction(e2r.Revit.Doc, "Create Levels and views"))
                {
                    bool flag = true;
                    trans.Start();
                    flag = e2r.CreateLevelsAndViewPlans();
                    trans.Commit();
                    if (!flag)
                    {
                        return Result.Failed;
                    }

                    return Result.Succeeded;

                }
            }
            catch (Exception e)
            {

                message = e.Message;
                return Result.Failed;
            }
          
        }
    }
}
