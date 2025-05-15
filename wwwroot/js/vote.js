// ---------------資料初始化（從 localStorage 讀取）---------------
const user = localStorage.getItem("current_user");  // 取得目前使用者名稱
const index = localStorage.getItem("current_vote_index");  // 目前選擇的投票主題索引
const users = getData("vote_users")  // 所有用戶與其投票資料
const topic = users[user]?.votes[index];  // 取得該使用者目前投票主題物件

// ---------------主要畫面渲染函式---------------
function renderVotePage() {
  if (!topic) {
    // 若找不到投票主題，顯示錯誤訊息
    document.getElementById("voteSection").innerHTML = "<p>找不到主題</p>";
    return;
  }
  // 顯示投票主題與投票提示
  document.getElementById("topicTitle").innerText = topic.topic;
  document.getElementById("voteHint").innerText =
    topic.anonymous ? "匿名投票（可重複投票）" : "記名投票（僅能投一次）";

  const container = document.getElementById("voteSection");
  container.innerHTML = "";   // 清空畫面內容
  // 動態產生所有選項的 radio button    
  Object.keys(topic.options).forEach(opt => {
    container.innerHTML += `<label><input type="radio" name="opt" value="${opt}"> ${opt}</label><br>`;
  });
  // 加入投票按鈕與結果區塊    
  container.innerHTML += '<button onclick="submitVote()">送出投票</button>';
  container.innerHTML += '<div id="voteResult"></div>';

  showResult(topic);  // 顯示目前的投票結果
}

// ---------------投票處理函式---------------
function submitVote() {
  if (!topic) return;
  // 記名投票時檢查是否已投過票
  if (!topic.anonymous && topic.voters[user]) {
    return alert("你已投過票！");
  }
  // 檢查是否有選取選項
  const selected = document.querySelector('input[name="opt"]:checked');
  if (!selected) return alert("請選擇一個選項！");
  // 投票數 +1
  topic.options[selected.value]++;
  // 若為記名投票，紀錄使用者投的選項
  if (!topic.anonymous) {
    topic.voters[user] = selected.value;
  }
  // 呼叫 storage.js
  setData("vote_users", users);
  renderVotePage();  // 重新渲染畫面（會連結果一起更新）
}

// ---------------顯示投票結果函式---------------
function showResult(data) {
  const resultDiv = document.getElementById("voteResult");
  const total = Object.values(data.options).reduce((a, b) => a + b, 0);  // 計算總票數
  resultDiv.innerHTML = "<h4>投票結果</h4>";
    
  // 顯示每個選項的票數與百分比長條圖    
  for (let opt in data.options) {
    const count = data.options[opt];
    const percent = total > 0 ? ((count / total) * 100).toFixed(1) : 0;
    resultDiv.innerHTML += `
      <div>${opt}：${count} 票 (${percent}%)
        <div class='bar' style='width:${percent}%'></div>
      </div>
    `;
  }
}
