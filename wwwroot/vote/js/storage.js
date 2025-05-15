// ----------------------
// 從 localStorage 取得資料
// key：字串，例如 "vote_users"
// 若不存在，則回傳空物件 {}
// ----------------------
function getData(key) {
  return JSON.parse(localStorage.getItem(key) || "{}");
}

// ----------------------
// 將資料存入 localStorage
// key：要存的名稱
// value：JavaScript 物件，會轉成 JSON 字串儲存
// ----------------------
function setData(key, value) {
  localStorage.setItem(key, JSON.stringify(value));
}
