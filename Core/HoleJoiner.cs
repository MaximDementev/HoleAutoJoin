using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HoleAutoJoin.Core
{
    public class HoleJoiner
    {
        #region Fields
        private readonly Document _document;
        private readonly IHoleFilter _holeFilter;
        #endregion

        #region Constructor
        public HoleJoiner(Document document, IHoleFilter holeFilter)
        {
            _document = document ?? throw new ArgumentNullException(nameof(document));
            _holeFilter = holeFilter ?? throw new ArgumentNullException(nameof(holeFilter));
        }
        #endregion

        #region Public Methods
        public JoinResult JoinHolesToStructure()
        {
            var result = new JoinResult();

            try
            {
                var elementsToJoin = GetHolesToJoin();

                if (!elementsToJoin.Any())
                {
                    result.Message = "Не найдено отверстий в плитах для соединения";
                    return result;
                }

                using (Transaction trans = new Transaction(_document, "Автоматическое соединение отверстий с плитой"))
                {
                    trans.Start();

                    foreach (FamilyInstance familyInstance in elementsToJoin)
                    {
                        if (!IsValidForJoining(familyInstance))
                            continue;

                        Element host = familyInstance.Host;
                        if (host == null || !host.IsValidObject)
                            continue;

                        if (!JoinGeometryUtils.AreElementsJoined(_document, familyInstance, host))
                        {
                            try
                            {
                                JoinGeometryUtils.JoinGeometry(_document, familyInstance, host);
                                result.JoinedCount++;
                            }
                            catch
                            {
                                result.FailedCount++;
                            }
                        }
                    }

                    trans.Commit();

                    result.Success = true;
                    if (result.JoinedCount > 0)
                    {
                        result.Message = $"Соединено {result.JoinedCount} отверстий с плитами";
                    }
                    else
                    {
                        result.Message = "Нет новых элементов для соединения";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Произошла ошибка при соединении элементов: {ex.Message}";
            }

            return result;
        }
        #endregion

        #region Private Methods
        private IEnumerable<FamilyInstance> GetHolesToJoin()
        {
            return new FilteredElementCollector(_document)
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .Where(fi => _holeFilter.IsValidHole(fi))
                .ToList();
        }

        private bool IsValidForJoining(FamilyInstance familyInstance)
        {
            return familyInstance != null &&
                   familyInstance.IsValidObject &&
                   familyInstance.Host != null;
        }
        #endregion
    }
}