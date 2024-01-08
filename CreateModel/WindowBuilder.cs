using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateModel
{
   public static class WindowBuilder
    {
        public static FamilyInstance CreateWindowInMiddle(Document doc, string level1Name, Wall wall, double sillHeightInMm)
        {
            List<Level> listLevel = new FilteredElementCollector(doc)
            .OfClass(typeof(Level))
            .OfType<Level>()
            .ToList();

            Level level1 = listLevel
                .Where(x => x.Name.Equals(level1Name))
                .FirstOrDefault();

            if (level1 == null)
            {
                TaskDialog.Show("Ошибка", "Базового уровня с таким именем не существует");
                return null;
            }

            FamilySymbol windowType = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_Windows)
                .OfType<FamilySymbol>()
                .Where(x => x.Name.Equals("0915 x 1830 мм"))
                .Where(x => x.FamilyName.Equals("Фиксированные"))
                .FirstOrDefault();

            LocationCurve locationCurve = wall.Location as LocationCurve;
            XYZ point1 = locationCurve.Curve.GetEndPoint(0);
            XYZ point2 = locationCurve.Curve.GetEndPoint(1);
            XYZ pointTemp = (point1 + point2) / 2;

            double sillHeight = UnitUtils.ConvertToInternalUnits(sillHeightInMm, UnitTypeId.Millimeters);

            XYZ point = new XYZ(pointTemp.X, pointTemp.Y, sillHeight);

            if (!windowType.IsActive)
            {
                windowType.Activate();
            }

            FamilyInstance window = doc.Create.NewFamilyInstance(point, windowType, wall, level1, StructuralType.NonStructural);

            return window;


        }

    }
}
