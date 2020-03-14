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
    class TagView : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            RevitApp revit = new RevitApp(commandData);
            try
            {
                //List<BuiltInCategory> cats = new List<BuiltInCategory>();
                //cats.Add(BuiltInCategory.OST_Doors);
                //cats.Add(BuiltInCategory.OST_Windows);
                ////ElementCategoryFilter filter = new ElementCategoryFilter();
                //ElementMulticategoryFilter filter = new ElementMulticategoryFilter(cats);
                //IList<Element> eles = new FilteredElementCollector(revit.Doc, revit.Doc.ActiveView.Id)
                //    .WherePasses(filter).WhereElementIsNotElementType().ToElements();
                //using (Transaction trans = new Transaction(revit.Doc, "Tag"))
                //{
                //    trans.Start();
                //    foreach (Element ele in eles)
                //    {
                //        Reference myRef = new Reference(ele);
                //        LocationPoint locp = ele.Location as LocationPoint;
                //        IndependentTag tag = IndependentTag.Create(revit.Doc, revit.Doc.ActiveView.Id,myRef, true, TagMode.TM_ADDBY_CATEGORY, TagOrientation.Horizontal, locp.Point);
                //    }
                //    trans.Commit();
                //}
                View tagView = revit.SelectElement<View>(BuiltInCategory.OST_Views, false, x => x.Name == "Level 1");
                revit.TagView(new List<BuiltInCategory> { BuiltInCategory.OST_Doors, BuiltInCategory.OST_Windows }, tagView.Id, true, TagMode.TM_ADDBY_CATEGORY, TagOrientation.Horizontal);
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

