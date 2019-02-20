namespace CPR.Core
{
    public static class GlobalConstants
    {
        public const char SEPERATOR = ',';

        #region Error messages

        public const string TITLE_ERROR_MESSAGE = "Can not find title {titleName}, please verify the file!";
        public const string TITLE_OUTNUMBER_ERROR = "The data source text file title is not correct, please verify that!";
        public const string TITLESERVICE_SETTITLE_ERROR = "Please set the titles property first!";

        public const string YEAR_FORMAT_WRONG_MESSAGE =
            "The years format in this line is not correct, please verify it and try again! The Line is -- {0}";

        public const string TEXT_HEADER_FORMAT_ERROR = "Text header error, please verify that.";
        #endregion
    }
}
