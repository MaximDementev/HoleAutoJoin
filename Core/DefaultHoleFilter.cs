using Autodesk.Revit.DB;

namespace HoleAutoJoin.Core
{
    public class DefaultHoleFilter : IHoleFilter
    {
        public bool IsValidHole(FamilyInstance familyInstance)
        {
            if (familyInstance == null || !familyInstance.IsValidObject)
                return false;

            if (familyInstance.Symbol == null || familyInstance.Symbol.Family == null)
                return false;

            return familyInstance.Symbol.Family.Name.StartsWith("Отверстие_Плита_") &&
                   familyInstance.Host != null;
        }
    }
}