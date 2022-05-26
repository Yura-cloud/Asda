namespace LinnworksMacroHelpers.Helpers
{
    public static class ConvertorDateTimeHelper
    {
        public static string ParseToCurrentCulture(string inputDate)
        {
             var arr = inputDate.Split('.');
             
             var outputDate = arr[1][0] == '0' ? $"{arr[1][1]}/" : $"{arr[1]}/";
             
             if (arr[0][0] == '0')
                 outputDate += $"{arr[0][1]}/";
             else
                 outputDate += $"{arr[0]}/";
             
             outputDate += $"{arr[2]}";
             
             return outputDate;

            //return inputDate;
        }
    }
}