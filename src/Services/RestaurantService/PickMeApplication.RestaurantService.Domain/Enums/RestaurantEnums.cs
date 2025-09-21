namespace PickMeApplication.RestaurantService.Domain.Enums;

public enum CuisineType
{
    Vietnamese = 0,
    Chinese = 1,
    Japanese = 2,
    Korean = 3,
    Thai = 4,
    Western = 5,
    Italian = 6,
    Indian = 7,
    Mexican = 8,
    FastFood = 9,
    Seafood = 10,
    Vegetarian = 11,
    Dessert = 12,
    Coffee = 13,
    Other = 99
}

public enum RestaurantStatus
{
    Pending = 0,      // Đang chờ duyệt
    Active = 1,       // Hoạt động
    Inactive = 2,     // Tạm ngừng
    Suspended = 3,    // Bị đình chỉ
    Closed = 4        // Đóng cửa vĩnh viễn
}

public enum BusinessDayStatus
{
    Open = 0,
    Closed = 1,
    Holiday = 2
}