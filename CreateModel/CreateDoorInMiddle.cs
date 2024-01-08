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
    public static class CreateDoorInMiddle
    {
        public static FamilyInstance CreateSpecificDoor(Document doc, string level1Name, Wall wall)
        {
            List<Level> listLevel = new FilteredElementCollector(doc)
                 .OfClass(typeof(Level))
                 .OfType<Level>()
                 .ToList();

            Level level1 = listLevel
                  .Where(x => x.Name.Equals("Уровень 1"))
                  .FirstOrDefault();
            if (level1 == null)
            {
                TaskDialog.Show("Ошибка", "Уровня с таким именем не существует");
                return null;
            }

            FamilySymbol doorType = new FilteredElementCollector(doc)
                 .OfClass(typeof(FamilySymbol))
                 .OfCategory(BuiltInCategory.OST_Doors)
                 .OfType<FamilySymbol>()
                 .Where(x => x.Name.Equals("0915 x 2134 мм"))
                 .Where(x => x.FamilyName.Equals("Одиночные-Щитовые"))
                 .FirstOrDefault();

            LocationCurve hostCurve = wall.Location as LocationCurve;

            XYZ point1 = hostCurve.Curve.GetEndPoint(0);
            XYZ point2 = hostCurve.Curve.GetEndPoint(1);
            XYZ point = (point1 + point2) / 2;
           
            if (!doorType.IsActive)
            {
                doorType.Activate();
            }

            FamilyInstance door = doc.Create.NewFamilyInstance(point, doorType, wall, level1, StructuralType.NonStructural);
            return door;

        }


    }
}
