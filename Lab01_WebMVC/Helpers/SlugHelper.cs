using System.Text.RegularExpressions;

namespace Lab01_WebMVC.Helpers;

public static class SlugHelper {
    public static string Generate(string text)
    {
        text = text.ToLowerInvariant().Trim();
        var map = new Dictionary<string,string> {
            {"à|á|ả|ã|ạ|ă|ắ|ặ|ẳ|ẵ|ằ|â|ấ|ầ|ẩ|ẫ|ậ","a"},
            {"è|é|ẻ|ẽ|ẹ|ê|ế|ề|ể|ễ|ệ","e"},
            {"ì|í|ỉ|ĩ|ị","i"},
            {"ò|ó|ỏ|õ|ọ|ô|ố|ồ|ổ|ỗ|ộ|ơ|ớ|ờ|ở|ỡ|ợ","o"},
            {"ù|ú|ủ|ũ|ụ|ư|ứ|ừ|ử|ữ|ự","u"},
            {"ỳ|ý|ỷ|ỹ|ỵ","y"},{"đ","d"},
        };
        foreach (var kv in map)
            foreach (var ch in kv.Key.Split('|'))
                text = text.Replace(ch, kv.Value);
        text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
        text = Regex.Replace(text, @"[\s-]+", "-");
        return text.Trim('-');
    }
}
