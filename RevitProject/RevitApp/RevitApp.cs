using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using System.Linq;

namespace Revit
{
    public class RevitApp
    {
        private UIDocument uidoc;
        private Document doc;
        //private FilteredElementCollector collector;

        public UIDocument Uidoc { get => uidoc; }
        public Document Doc { get => doc; }
        public FilteredElementCollector Collector
        {
            get
            {
                return new FilteredElementCollector(doc);
            }

        }
        #region Constructor

        public RevitApp(ExternalCommandData commandData)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;
        }
        #endregion

        /// <summary>
        /// Generate Plan View 
        /// </summary>
        /// <param name="levelName"></param>
        /// <param name="viewName"></param>
        public void CreatePlanView(string levelName, string viewName)
        {
            ViewFamilyType viewType = Collector.OfClass(typeof(ViewFamilyType))
                .WhereElementIsElementType()
                .Cast<ViewFamilyType>()
                .First(x => x.ViewFamily == ViewFamily.FloorPlan);

            Level level = SelectElement<Level>(BuiltInCategory.OST_Levels, false, x => x.Name == levelName);

                ViewPlan plan = ViewPlan.Create(doc, viewType.Id, level.Id);
                plan.Name = viewName;
                
            
        }

        /// <summary>
        /// Create Tag for specific View
        /// </summary>
        /// <param name="cats"></param>
        /// <param name="viewId"></param>
        /// <param name="leader"></param>
        /// <param name="tagMode"></param>
        /// <param name="orientation"></param>
        public void TagView(ICollection<BuiltInCategory> cats, ElementId viewId, bool leader, TagMode tagMode, TagOrientation orientation)
        {
            //create multiCategory Filter
            ElementMulticategoryFilter filter = new ElementMulticategoryFilter(cats);
            //retrieve filtered elements
            IList<Element> elements = new FilteredElementCollector(doc, viewId).WherePasses(filter).WhereElementIsNotElementType().ToElements(); //note: i am tagging only some elements 
            //in t a specific view so i don't need to filter all elements in the documnet.
            Reference eleRef;
            IndependentTag tag;
            using (Transaction trans = new Transaction(doc, "Tag Elements"))
            {
                trans.Start();
                foreach (Element ele in elements)
                {
                    eleRef = new Reference(ele);
                    tag = IndependentTag.Create(doc, viewId, eleRef, leader, tagMode, orientation, (ele.Location as LocationPoint).Point);
                }
                trans.Commit();
            }
        }

        public Element PickElement()
        {
            Reference eleRef = uidoc.Selection.PickObject(ObjectType.Element);
            Element ele = null;
            if (eleRef != null)
            {
                ele = doc.GetElement(eleRef);
            }
            return ele;
        }

        /// <summary>
        /// Select a specific FamilyType(Family Symbol)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public FamilySymbol SelectFamilyType(string name)
        {
            return Collector.OfClass(typeof(FamilySymbol)).WhereElementIsElementType().Cast<FamilySymbol>().FirstOrDefault(x => x.Name == name);
        }
        public T SelectClassBased<T>(bool isElmentType,Func<T,bool> predicate)
        {
            T result = default;
            switch (isElmentType)
            {
                case true:
                result= Collector.OfClass(typeof(T)).WhereElementIsElementType().Cast<T>().First(predicate);
                    break;
                case false:
                    result= Collector.OfClass(typeof(T)).WhereElementIsElementType().Cast<T>().First(predicate);
                    break;
            }
            return result;
        }
        /// <summary>
        /// Create Sheet
        /// </summary>
        /// <param name="titleBlockName"></param>
        /// <param name="sheetName"></param>
        /// <param name="sheetNumber"></param>
        /// <returns></returns>
        public ViewSheet CreateSheet(string titleBlockName, string sheetName, string sheetNumber)
        {
            FamilySymbol titleBlock = SelectElement<FamilySymbol>(BuiltInCategory.OST_TitleBlocks, true, x => x.Name == titleBlockName);
            ViewSheet sheet = null;
            using (Transaction trans = new Transaction(doc, string.Format($"Create Sheet: {sheetName}")))
            {
                trans.Start();
                sheet = ViewSheet.Create(doc, titleBlock.Id);
                sheet.Name = sheetName;
                sheet.SheetNumber = sheetNumber;
                trans.Commit();
            }
            return sheet;
        }

        /// <summary>
        /// Select a specific element based on its builtin category
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builtInCategory"></param>
        /// <param name="isElementType"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T SelectElement<T>(BuiltInCategory builtInCategory, bool isElementType, Func<T, bool> predicate)
        {
            T ele = default; ;
            switch (isElementType)
            {
                case true:
                    ele = Collector.OfCategory(builtInCategory).WhereElementIsElementType()
                        .Cast<T>().First(predicate);
                    break;
                case false:
                    ele = Collector.OfCategory(builtInCategory).WhereElementIsNotElementType()
                        .Cast<T>().First(predicate);
                    break;
            }
            return ele;
        }

        /// <summary>
        /// Select a specific element based on its builtin category
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builtInCategory"></param>
        /// <param name="isElementType"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T SelectElement<T>(BuiltInCategory builtInCategory, bool isElementType)
        {
            T ele=default;
            switch (isElementType)
            {
                case true:
                    ele = Collector.OfCategory(builtInCategory).WhereElementIsElementType()
                        .Cast<T>().First();
                    break;
                case false:
                    ele = Collector.OfCategory(builtInCategory).WhereElementIsNotElementType()
                        .Cast<T>().First();
                    break;
            }
            return ele;
        }
        /// <summary>
        /// Draw Straight Walls
        /// </summary>
        /// <param name="points"></param>
        /// <param name="_level"></param>
        public void DrawWall(XYZ[] points, string _level)
        {

            Level level = SelectElement<Level>(BuiltInCategory.OST_Levels, false, x => x.Name == _level);

            List<Curve> curves = new List<Curve>();
            for (int i = 0; i < points.Length - 1; i++)
            {
                curves.Add(Line.CreateBound(points[i], points[i + 1]));
            }
            try
            {
                using (Transaction trans = new Transaction(doc, "Create Walls"))
                {
                    trans.Start();
                    foreach (var curve in curves)
                    {
                        Wall.Create(doc, curve, level.Id, false);
                    }
                    trans.Commit();
                }
            }
            catch (Exception e)
            {

                TaskDialog.Show("Error", e.Message);
            }


        }
    }
}
