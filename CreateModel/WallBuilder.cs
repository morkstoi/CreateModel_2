using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateModel
{
    public static class WallBuilder
    {
        public static List<Wall> CreateWallsFromRectangle(Document doc, string level1Name, string level2Name,int widthinitial,int depthInitial)
        {
            List <Wall> walls = new List<Wall>();
            if (widthinitial == 0 || depthInitial == 0)
            {
                TaskDialog.Show("Ошибка", "Размеры некорректны");
                return walls;
            }

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
                return walls;
            }

            Level level2 = listLevel
                  .Where(x => x.Name.Equals("Уровень 2"))
                  .FirstOrDefault();
            if (level2 == null)
            {
                TaskDialog.Show("Ошибка", "Уровня с таким именем не существует");
                return walls;
            }
            double width = UnitUtils.ConvertToInternalUnits(widthinitial, UnitTypeId.Millimeters);
            double depth = UnitUtils.ConvertToInternalUnits(depthInitial, UnitTypeId.Millimeters);
            double dx = width / 2;
            double dy = depth / 2;

            List<XYZ> points = new List<XYZ>();
            points.Add(new XYZ(-dx, -dy, 0));
            points.Add(new XYZ(dx, -dy, 0));
            points.Add(new XYZ(dx, dy, 0));
            points.Add(new XYZ(-dx, dy, 0));
            points.Add(new XYZ(-dx, -dy, 0));

            for (int i = 0; i < 4; i++)
            {
                Line line = Line.CreateBound(points[i], points[i + 1]);
                Wall wall = Wall.Create(doc, line, level1.Id, false);
                walls.Add(wall);
                wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(level2.Id);

            }

            return walls;



        }


    }
}
