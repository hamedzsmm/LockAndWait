# LockAndWait (v1.1.1)

- لاک توزیع‌شده با ردیس
- APIها: `AcquireAsync`, `ReleaseAsync`, `WaitAsync`
- اکستنشن DI: `AddLockAndWaitService(connectionString, database = -1)`
- اصلاح تبدیل نتیجه‌ی Lua (`ScriptEvaluateAsync`) برای سازگاری کامل با StackExchange.Redis

## نصب
```powershell
dotnet add package LockAndWait
```

## رجیستر در DI
```csharp
builder.Services.AddLockAndWaitService("localhost:6379"); // فقط کانکشن‌استرینگ
```

## استفاده
```csharp
public class MySvc(ILockAndWaitService lockSvc) { ... }
```
