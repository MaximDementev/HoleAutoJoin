using Autodesk.Revit.DB;

namespace HoleAutoJoin.Core
{
    public interface IHoleFilter
    {
        bool IsValidHole(FamilyInstance familyInstance);
    }
}