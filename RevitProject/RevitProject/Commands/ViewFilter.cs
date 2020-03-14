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
    class ViewFilter : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            RevitApp revit = new RevitApp(commandData);
            List<ElementId> cats = new List<ElementId>();
            cats.Add(new ElementId(BuiltInCategory.OST_Sections));
            ElementParameterFilter filter = new ElementParameterFilter(ParameterFilterRuleFactory.CreateContainsRule(new ElementId(BuiltInParameter.VIEW_NAME), "WIP", false));
            try
            {
                using (Transaction trans = new Transaction(revit.Doc, "Create Filter"))
                {
                    trans.Start();
                    ParameterFilterElement filterElement = ParameterFilterElement.Create(revit.Doc, "Section Filter", cats, filter);
                    revit.Doc.ActiveView.AddFilter(filterElement.Id);
                    revit.Doc.ActiveView.SetFilterVisibility(filterElement.Id, false);
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

