using System.ComponentModel;
using System.Security.Cryptography;

namespace TikiShop.Core.Utils;

public static class Helper
{
    public static string HashHMACSHA512(string input, string secretKey)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
        var hmac512 = new HMACSHA512(secretKeyBytes);
        var hashBytes = hmac512.ComputeHash(inputBytes);
        var sb = new StringBuilder();
        foreach (var x in hashBytes) sb.Append($"{x:x2}");
        return sb.ToString(); // The resulting string is 64 characters long
    }

    public static string DescriptionAttr<T>(this T source)
    {
        if (source is null) throw new NullReferenceException(nameof(source));

        var fi = source.GetType().GetField(source.ToString()!);

        if (fi?.GetCustomAttributes(typeof(DescriptionAttribute), false) is
            DescriptionAttribute[]
            {
                Length: > 0
            } attributes)
            return attributes[0].Description;
        return source.ToString()!;
    }

    /// <summary>
    ///     Convert To Date Int yyyyMMddHHmmss
    /// </summary>
    /// <param name="input"></param>
    /// <returns>yyyyMMddHHmmss Format String</returns>
    public static string ConvertToDateIntString(DateTime dateTime)
    {
        return dateTime.ToString("yyyyMMddHHmmss");
    }

    public static string RemoveUnicode(string text)
    {
        var arr1 = new[]
        {
            "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
            "đ",
            "é", "è", "ẻ", "ẽ", "ẹ", "ê", "ế", "ề", "ể", "ễ", "ệ",
            "í", "ì", "ỉ", "ĩ", "ị",
            "ó", "ò", "ỏ", "õ", "ọ", "ô", "ố", "ồ", "ổ", "ỗ", "ộ", "ơ", "ớ", "ờ", "ở", "ỡ", "ợ",
            "ú", "ù", "ủ", "ũ", "ụ", "ư", "ứ", "ừ", "ử", "ữ", "ự",
            "ý", "ỳ", "ỷ", "ỹ", "ỵ"
        };
        var arr2 = new[]
        {
            "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
            "d",
            "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e",
            "i", "i", "i", "i", "i",
            "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o",
            "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u",
            "y", "y", "y", "y", "y"
        };
        for (var i = 0; i < arr1.Length; i++) text = text.Replace(arr1[i], arr2[i]);
        return text;
    }
}