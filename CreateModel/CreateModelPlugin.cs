using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateModel
{

    [TransactionAttribute(TransactionMode.Manual)]
    public class CreateModelPlugin : IExternalCommand

    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            Transaction transaction = new Transaction(doc, "Построение стены");
            transaction.Start();
            List<Wall> newWalls = WallBuilder.CreateWallsFromRectangle(doc, "Уровень 1", "Уровень 2", 10000, 5000);
            FamilyInstance door = CreateDoorInMiddle.CreateSpecificDoor(doc, "Уровень 1", newWalls[0]);
            FamilyInstance window1 = WindowBuilder.CreateWindowInMiddle(doc, "Уровень 1", newWalls[1], 800);
            FamilyInstance window2 = WindowBuilder.CreateWindowInMiddle(doc, "Уровень 1", newWalls[2], 800);
            FamilyInstance window3 = WindowBuilder.CreateWindowInMiddle(doc, "Уровень 1", newWalls[3], 800);
            ExtrusionRoof roof = RoofBuilder.CreateRoof(doc, "Уровень 2", newWalls[0], newWalls[1], 1000);

            transaction.Commit();

            return Result.Succeeded;
        }
    }
}
