using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using System.Linq;
using Revit;
using ExcelIO;
using System.IO;
using OfficeOpenXml;

namespace RevitCommands
{
    [Transaction(TransactionMode.Manual)]
    class CreateLevelsFromExcel : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            RevitApp revit = new RevitApp(commandData);

            FileOpenDialog openDialog = new FileOpenDialog("Excel Files(*.xlsx) | *.xlsx");
            string path = "";
            if (openDialog.Show() == ItemSelectionDialogResult.Confirmed)
            {
                path = ModelPathUtils.ConvertModelPathToUserVisiblePath(openDialog.GetSelectedModelPath());
                

                    Excel excel = new Excel(path);
                    List<ExcelData> data = new List<ExcelData>();
                    data = excel.ReadFromTable<ExcelData>("second sheet", "RevitData", (v, i) =>
                    {
                        return new ExcelData(v[i, 0].ToString(), v[i, 1].ToString(), v[i, 2].ToString());
                    });

                    Level level;
                    ViewFamilyType viewType = revit.SelectClassBased<ViewFamilyType>( true, x => x.ViewFamily == ViewFamily.FloorPlan);
                    try
                    {
                        using (Transaction trans = new Transaction(revit.Doc, "create level"))
                        {
                            trans.Start();
                            foreach (var d in data)
                            {
                                level = Level.Create(revit.Doc, UnitUtils.ConvertToInternalUnits(Convert.ToDouble(d.Level), DisplayUnitType.DUT_METERS));

                                revit.CreatePlanView(level.Name, d.ViewPlan);
                            }

                            trans.Commit();
                        }

                        return Result.Succeeded;
                    }
                    catch (Exception e)
                    {

                        message = e.Message;
                        return Result.Failed;
                    }
                }
            
            else
            {
                return Result.Cancelled;
            }
        }
    }
}
