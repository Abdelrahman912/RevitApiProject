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
    class PlanView : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get Acess to revit
            RevitApp revit = new RevitApp(commandData);

            //ViewFamilyType viewFamily = revit.Collector.OfClass(typeof(ViewFamilyType))
            //    .WhereElementIsElementType()
            //    .Cast<ViewFamilyType>()
            //    .First(x => x.ViewFamily == ViewFamily.FloorPlan);

            //Level level = revit.SelectElement<Level>(BuiltInCategory.OST_Levels, false, x => x.Name == "Level 1");
            try
            {
                //using (Transaction trans = new Transaction(revit.Doc,"Create View"))
                //{
                //    trans.Start();
                //    ViewPlan viewPlan = ViewPlan.Create(revit.Doc, viewFamily.Id, level.Id);
                //    viewPlan.Name = "Our  Plan";
                //    trans.Commit();
                //}
                revit.CreatePlanView("Level 1", "Automated plan");
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

