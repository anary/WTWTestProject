using System;
using CPR.Core;

namespace CPR.Business.Helpers
{
    public static class TitlesExtensions
    {
        public static int GetIndexByName(this string[] line, string titleName)
        {
            var index = Array.FindIndex<string>(line, row => row.ToLower().Contains(titleName.ToLower()));

            if (index == -1)
                throw new FormatException(string.Format(GlobalConstants.TITLE_ERROR_MESSAGE, titleName));

            return index;
        }


    }
}