namespace TikiShop.Model.Configurations;

public class VnPayConfig
{
    public static readonly string SectionName = "VnPayConfig";
    public string vnp_TmnCode { get; set; }
    public string vnp_HashSecret { get; set; }
    public string vnp_Returnurl { get; set; }
    public string vnp_Url { get; set; }
}