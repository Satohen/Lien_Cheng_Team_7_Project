# Lien_Cheng_Team_7_Project
# 第七小組 ASP.NET Core 專題

這是一個採用 ASP.NET Core MVC 架構（前後分離）的專題作品，共包含三個模組：
- ✅ 打卡模組（由「亨」負責）
- 🔐 登入模組（由「東」負責）
- 🗳️ 投票模組（由「Tim」負責）

---

## 🛠 開發前準備

### ✅ Step 1：建立本地 SQL Server 資料庫

使用 SSMS 或 DBeaver 執行以下指令：

```sql
CREATE DATABASE Team_7_DB;
GO

USE Team_7_DB;
GO

-- 🔽 以下為打卡模組建立用資料表（可擴充）
CREATE TABLE CheckinRecords (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeName NVARCHAR(100),
    CheckinTime DATETIME
);
