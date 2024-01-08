using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateModel
{
    public static class RoofBuilder
    {
        public static ExtrusionRoof CreateRoof(Document doc, string level2Name, Wall wall0, Wall wall1, double heightInMm)
        {
            List<Level> listLevel = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .OfType<Level>()
                .ToList();

            Level level2 = listLevel
                .Where(x => x.Name.Equals(level2Name))
                .FirstOrDefault();

            if (level2 == null)
            {
                TaskDialog.Show("Ошибка", "Базового уровня с таким именем не существует");
                return null;
            }

            RoofType roofType = new FilteredElementCollector(doc)
                .OfClass(typeof(RoofType))
                .OfType<RoofType>()
                .Where(x => x.Name.Equals("Типовой - 400мм"))
                .Where(x => x.FamilyName.Equals("Базовая крыша"))
                .FirstOrDefault();

            LocationCurve locationCurve = wall0.Location as LocationCurve;
            XYZ pointStart = locationCurve.Curve.GetEndPoint(0);
            XYZ pointEnd = locationCurve.Curve.GetEndPoint(1);
            XYZ pointMiddleInLine = (pointStart + pointEnd) / 2;

            XYZ vect = pointEnd - pointStart;
            double wall0HalfWidth = wall0.Width / 2;
            XYZ vectScaled = vect * wall0HalfWidth / vect.GetLength();

            pointStart = pointStart.Add(vectScaled.Negate());
            pointEnd = pointEnd.Add(vectScaled);

            double height = UnitUtils.ConvertToInternalUnits(heightInMm, UnitTypeId.Millimeters);
            XYZ pointMiddle = new XYZ(pointMiddleInLine.X, pointMiddleInLine.Y, pointMiddleInLine.Z + height);

            double level2Elev = level2.Elevation;
            XYZ levelOffset = new XYZ(0, 0, level2Elev);

            pointStart = pointStart.Add(levelOffset);
            pointMiddle = pointMiddle.Add(levelOffset);
            pointEnd = pointEnd.Add(levelOffset);

            double roofThick = roofType.get_Parameter(BuiltInParameter.ROOF_ATTR_DEFAULT_THICKNESS_PARAM).AsDouble();
            XYZ roofOffset = new XYZ(0, 0, roofThick);

            CurveArray curveArray = new CurveArray();

            curveArray.Append(Line.CreateBound(pointStart, pointMiddle));
            curveArray.Append(Line.CreateBound(pointMiddle, pointEnd));

            LocationCurve locationCurveWall1 = wall1.Location as LocationCurve;
            double roofLength = locationCurveWall1.Curve.Length;

            LocationCurve locationCurveWall0 = wall0.Location as LocationCurve;
            Line lineNormal = locationCurveWall0.Curve as Line;
            XYZ normal = lineNormal.Direction.Negate();

            ReferencePlane plane = doc.Create.NewReferencePlane(pointMiddleInLine, pointMiddle, normal, doc.ActiveView);
            ExtrusionRoof roof = doc.Create.NewExtrusionRoof(curveArray, plane, level2, roofType, -wall0HalfWidth, roofLength + wall0HalfWidth);

            return roof;

        }
    }
}
