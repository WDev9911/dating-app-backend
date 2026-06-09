namespace SameMess.Application.Common;

/// <summary>
/// Tính toán địa lý cho Discovery. Giai đoạn đầu dùng Haversine + bounding box trên cột
/// Latitude/Longitude. Khi scale lớn nên chuyển sang kiểu geography + spatial index.
/// </summary>
public static class GeoCalculator
{
    private const double EarthRadiusKm = 6371.0;
    private const double KmPerLatDegree = 111.0; // ~111km cho mỗi độ vĩ độ

    /// <summary>Khoảng cách giữa 2 tọa độ (km) theo công thức Haversine.</summary>
    public static double DistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                + Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2))
                * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        return EarthRadiusKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    /// <summary>
    /// Hình vuông bao quanh điểm gốc với bán kính cho trước (lọc thô, dùng được index).
    /// Sau đó cần lọc tinh lại bằng <see cref="DistanceKm"/> để ra hình tròn chính xác.
    /// </summary>
    public static (double MinLat, double MaxLat, double MinLon, double MaxLon) BoundingBox(
        double lat, double lon, int radiusKm)
    {
        var latDelta = radiusKm / KmPerLatDegree;

        // Càng gần cực, mỗi độ kinh độ càng ngắn → chia cho cos(vĩ độ). Chặn cos rất nhỏ để tránh chia 0.
        var cosLat = Math.Max(Math.Cos(ToRad(lat)), 0.00001);
        var lonDelta = radiusKm / (KmPerLatDegree * cosLat);

        return (lat - latDelta, lat + latDelta, lon - lonDelta, lon + lonDelta);
    }

    private static double ToRad(double degrees) => degrees * Math.PI / 180.0;
}
