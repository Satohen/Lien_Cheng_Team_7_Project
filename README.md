# Lien_Cheng_Team_7_Project
# 第七小組 ASP.NET Core 專題

這是一個採用 ASP.NET Core MVC 架構（前後分離）的專題作品，共包含三個模組：
- ✅ 打卡模組（由「亨」負責）
- 🔐 登入模組（由「東」負責）
- 🗳️ 投票模組（由「Tim」負責）

📂 位置：`wwwroot/vote`

此模組為獨立的前端投票系統，採用 HTML + JavaScript 建構，並透過 localStorage 暫存資料，模擬後端功能。

### 🔹 檔案說明

- `index.html`：主題列表、建立與編輯功能頁面
- `vote.html`：使用者進行投票與顯示結果頁面
- `js/main.js`：主畫面操作邏輯
- `js/vote.js`：投票流程與畫面邏輯
- `js/storage.js`：資料的本地儲存與讀取功能（模擬資料庫）

### 🔸 操作方式

1. 開啟 `vote/index.html` 進入主題列表
2. 點擊「投票」跳轉至 `vote/vote.html` 進行投票
3. 投票完成後會立即顯示統計結果（含百分比與長條圖）
4. 匿名投票與記名投票皆支援（由 checkbox 控制）

### ⚠️ 注意事項

- 尚未與資料庫串接，資料暫存於使用者端瀏覽器
- 若需與登入模組整合請通知 Tim 協作


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
