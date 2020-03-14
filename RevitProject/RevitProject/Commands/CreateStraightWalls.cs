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
    class CreateStraightWalls : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            RevitApp revit = new RevitApp(commandData);

            XYZ p1 = new XYZ(-10.0, -10.0, 0.0);
            XYZ p2 = new XYZ(-10.0, 10.0, 0.0);
            XYZ p3 = new XYZ(10.0, 10.0, 0.0);
            XYZ p4 = new XYZ(10.0, -10.0, 0.0);

            XYZ[] points = new XYZ[4] { p1, p2, p3, p4 };
            try
            {
                revit.DrawWall(points, "Level 1");

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
