# GAM106 – ASM 1 Game API

ASP.NET Core Web API quản lý tài khoản và nhân vật, sử dụng .NET 8, EF Core, MySQL và Swagger UI.

## Chạy trên máy local

Cài .NET 8 SDK và MySQL. Từ thư mục chứa `GameApi.csproj`, cấu hình kết nối và khóa JWT bằng .NET User Secrets. Thay các giá trị trong ví dụ bằng cấu hình MySQL của máy. Khóa JWT cần ít nhất 32 byte cho HS256. Ví dụ tạo khóa ngẫu nhiên 32 byte bằng PowerShell 7.

```powershell
$connection = Read-Host 'Nhập chuỗi kết nối MySQL' -MaskInput
$key = [Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(32))
@{ 'ConnectionStrings:Default' = $connection; 'Jwt:Key' = $key } |
    ConvertTo-Json -Compress | dotnet user-secrets set

dotnet restore
dotnet build
```

Mẫu chuỗi kết nối `Server=localhost;Port=3306;Database=game_asm1;User=TEN_USER;Password=MAT_KHAU;`. Có thể cấp cấu hình qua biến môi trường `ConnectionStrings__Default` và `Jwt__Key`. User Secrets được đọc khi môi trường là `Development`.

Chỉ cài công cụ EF nếu máy chưa có. Dùng migration đã lưu trong dự án để tạo bảng cho database mới hoặc áp dụng migration còn thiếu. Với database đã có, kiểm tra đúng tên database và lịch sử migration trước khi cập nhật.

```powershell
dotnet tool install --global dotnet-ef --version 8.0.31
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet ef migrations list
dotnet ef database update
dotnet run --launch-profile http
```

Mở [Swagger UI](http://localhost:5080/swagger). Đăng ký, đăng nhập rồi dán token vào **Authorize**. Swagger tự thêm tiền tố `Bearer`. Mỗi lần kiểm thử nên dùng username riêng. Để thử quyền sở hữu, tạo một nhân vật bằng tài khoản thứ hai rồi dùng token của tài khoản thứ nhất để sửa ID đó.

| API | Hành vi |
| --- | --- |
| `POST /api/auth/register` | Tạo tài khoản (201), trùng username (409), dữ liệu sai (400) |
| `POST /api/auth/login` | Trả JWT có hiệu lực một ngày (200), sai thông tin (401) |
| `POST /api/characters` | Yêu cầu JWT, tạo nhân vật Level 1 và HP 100 (201) |
| `PUT /api/characters/{id}` | Cập nhật nhân vật của tài khoản đang đăng nhập (200), dữ liệu sai (400), ID ngoài quyền sở hữu hoặc không tồn tại (404) |

## Các file chính

- `Program.cs` cấu hình MySQL, JWT, Swagger, các service và controller.
- `Models/Account.cs` và `Models/Character.cs` định nghĩa dữ liệu tài khoản, nhân vật và chỉ số mặc định.
- `Data/AppDbContext.cs` định nghĩa ánh xạ bảng, chỉ mục username duy nhất và khóa ngoại `AccountId`.
- `DTOs/Requests.cs` kiểm tra dữ liệu đầu vào. `DTOs/Responses.cs` định nghĩa dữ liệu trả về.
- `Services/AuthService.cs` băm và xác minh mật khẩu với `PasswordHasher<Account>`, sau đó cấp JWT khi đăng nhập.
- `Services/CharacterService.cs` tạo nhân vật cho chủ tài khoản và lọc theo cả ID nhân vật lẫn ID tài khoản khi cập nhật.
- `Controllers/AuthController.cs` và `Controllers/CharactersController.cs` khai báo route, mã phản hồi HTTP và yêu cầu xác thực.
- `Migrations/` lưu migration ban đầu và snapshot của EF Core.
- `appsettings.json` lưu cấu hình chung. `Properties/launchSettings.json` đặt profile local tại cổng 5080.

## Bằng chứng kiểm thử

Xem thư mục [Evidence](Evidence/) để đối chiếu ảnh Swagger TC01–TC12 và kết quả kiểm thử.
