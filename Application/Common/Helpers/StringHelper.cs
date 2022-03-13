using System.Text;
using System.Text.RegularExpressions;

namespace Application.Common.Helpers
{
    public static class StringHelper
    {
        public static string NormalizeVietnameseDiacritics(this string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, string.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
    }
}