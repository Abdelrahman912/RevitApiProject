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
    class PlaceView : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            RevitApp revit = new RevitApp(commandData);


            try
            {

                ViewSheet sheet = revit.SelectElement<ViewSheet>(BuiltInCategory.OST_Sheets, false, x => x.SheetNumber == "404");
                Element plan = revit.SelectElement<View>(BuiltInCategory.OST_Views, false, x => x.Name == "Automated plan");
                BoundingBoxUV boundingBox = sheet.Outline;
                UV min = boundingBox.Min;
                UV max = boundingBox.Max;
                UV avg = (min + max) / 2;
                using (Transaction trans = new Transaction(revit.Doc, "aaa"))
                {
                    trans.Start();
                    Viewport viewport = Viewport.Create(revit.Doc, sheet.Id, plan.Id, new XYZ(avg.U, avg.V, 0.0));
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
    }
}
