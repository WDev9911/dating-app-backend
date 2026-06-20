using SameMess.Application.Interfaces.Services;

namespace SameMess.Infrastructure.Services;

/// <summary>Dựng nội dung email hóa đơn + voucher combo hẹn hò.</summary>
public static class VoucherEmailTemplate
{
    public static (string Subject, string Html) Build(VoucherEmailModel m)
    {
        var price = m.AmountVnd.ToString("N0") + "đ";
        var expires = m.ExpiresAt.ToString("dd/MM/yyyy");
        var subject = $"Voucher hẹn hò SameMess — {m.ComboTitle}";

        var html = $@"
<div style=""font-family:Segoe UI,Arial,sans-serif;max-width:480px;margin:0 auto;background:#fff;border:1px solid #f0e6ec;border-radius:16px;overflow:hidden"">
  <div style=""background:linear-gradient(135deg,#ff4f8b,#b14bff);padding:24px;text-align:center;color:#fff"">
    <div style=""font-size:22px;font-weight:800"">SameMess 💕</div>
    <div style=""opacity:.9;margin-top:4px"">Voucher hẹn hò của bạn</div>
  </div>
  <div style=""padding:24px"">
    <h2 style=""margin:0 0 4px;color:#1a1a2e"">{m.ComboTitle}</h2>
    <p style=""margin:0 0 16px;color:#6b7280"">tại <b>{m.VenueName}</b></p>

    <div style=""text-align:center;margin:18px 0"">
      <img src=""{m.QrUrl}"" alt=""QR voucher"" width=""200"" height=""200"" style=""border:1px solid #eee;border-radius:12px"" />
      <div style=""margin-top:10px;font-size:18px;font-weight:800;letter-spacing:2px;color:#c9184a"">{m.VoucherCode}</div>
      <div style=""font-size:12px;color:#9ca3af"">Đưa mã QR này cho quán quét khi đến</div>
    </div>

    <table style=""width:100%;font-size:14px;color:#374151;border-collapse:collapse"">
      <tr><td style=""padding:6px 0;color:#6b7280"">Số tiền</td><td style=""text-align:right;font-weight:700"">{price}</td></tr>
      <tr><td style=""padding:6px 0;color:#6b7280"">Hạn dùng</td><td style=""text-align:right;font-weight:700"">{expires}</td></tr>
    </table>

    <p style=""margin:18px 0 0;font-size:12px;color:#9ca3af"">Voucher dùng 1 lần cho buổi hẹn của bạn. Chúc hai bạn có buổi hẹn vui vẻ! 🌳</p>
  </div>
</div>";
        return (subject, html);
    }
}
