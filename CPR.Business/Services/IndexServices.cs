using System;
using System.Collections.Generic;
using CPR.Business.Helpers;
using CPR.Core;
using CPR.Core.Enums;
using CPR.Core.Interfaces;

namespace CPR.Business.Services
{
    public class IndexServices : IIndexServices
    {
        public string[] Titles { private get; set; }

        public IDictionary<FileTitles, int> Indexes
        {
            get
            {
                if (Titles == null)
                {
                    throw new ArgumentNullException(GlobalConstants.TITLESERVICE_SETTITLE_ERROR);
                }

                if (Titles.Length != 4)
                {
                    throw new FormatException(GlobalConstants.TITLE_OUTNUMBER_ERROR);
                }

                return new Dictionary<FileTitles, int>
                {
                    {FileTitles.Product, Titles.GetIndexByName(FileTitles.Product.ToString())},
                    {FileTitles.Origin, Titles.GetIndexByName(FileTitles.Origin.ToString())},
                    {FileTitles.Development, Titles.GetIndexByName(FileTitles.Development.ToString())},
                    {FileTitles.Value, Titles.GetIndexByName(FileTitles.Value.ToString())}
                };
            }
        }
    }
}