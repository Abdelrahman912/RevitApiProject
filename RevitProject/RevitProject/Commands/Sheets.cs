using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using System.Linq;
using Revit;

namespace RevitCommands
{
    [Transaction(TransactionMode.Manual)]
    class Sheets : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            RevitApp revit = new RevitApp(commandData);


            try
            {

                //FamilySymbol tBlock =  revit.SelectElement<FamilySymbol>(BuiltInCategory.OST_TitleBlocks, true,x=>x.Name=="A0 metric");
                //  using (Transaction trans = new Transaction(revit.Doc,"Create Sheets"))
                //  {
                //      trans.Start();
                //      ViewSheet sheet = ViewSheet.Create(revit.Doc, tBlock.Id);
                //      sheet.Name = "Automated Sheet";
                //      sheet.SheetNumber = "201";
                //      trans.Commit();
                //  }
                revit.CreateSheet("A0 metric", "Very Automated Sheet", "404");
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
        }
    }
}
