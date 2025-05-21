// ✅ vote.js — 與 main.js 結構整合、功能正確的投票頁腳本

// ------------------ 初始化 ------------------
const voteUser = localStorage.getItem("current_user");
const voteIndex = parseInt(localStorage.getItem("vote_index"));
const voteUsers = getData("vote_users");
const voteTopic = voteUsers[voteUser]?.votes[voteIndex];

// ------------------ 渲染投票畫面 ------------------
function renderVotePage() {
  const titleEl = document.getElementById("topicTitle");
  const hintEl = document.getElementById("voteHint");
  const sectionEl = document.getElementById("voteSection");
  const resultEl = document.getElementById("voteResult");

  // 清空所有欄位
  titleEl.innerText = "";
  hintEl.innerText = "";
  sectionEl.innerHTML = "";
  resultEl.innerHTML = "";

  if (!voteTopic) {
    titleEl.innerText = "找不到主題";
    return;
  }

  // 顯示主題與提示
  titleEl.innerText = voteTopic.topic;
  hintEl.innerText = voteTopic.anonymous
    ? "匿名投票（可重複投票）"
    : "記名投票（僅能投一次）";

  // 建立投票選項表單
  const form = document.createElement("form");
  form.id = "voteForm";
  Object.keys(voteTopic.options).forEach(opt => {
    const label = document.createElement("label");
    label.style.display = "block";
    const input = document.createElement("input");
    input.type = "radio";
    input.name = "opt";
    input.value = opt;
    label.appendChild(input);
    label.appendChild(document.createTextNode(" " + opt));
    form.appendChild(label);
  });
  sectionEl.appendChild(form);

  // 投票按鈕
  const submitBtn = document.createElement("button");
  submitBtn.innerText = "送出投票";
  submitBtn.onclick = (e) => {
    e.preventDefault();
    submitVote();
  };
  sectionEl.appendChild(submitBtn);

  showResult(voteTopic);
}

// ------------------ 投票處理 ------------------
function submitVote() {
  if (!voteTopic) return;

  if (!voteTopic.anonymous && voteTopic.voters[voteUser]) {
    return alert("你已投過票！");
  }

  const selected = document.querySelector('input[name="opt"]:checked');
  if (!selected) return alert("請選擇一個選項！");

  voteTopic.options[selected.value]++;

  if (!voteTopic.anonymous) {
    voteTopic.voters[voteUser] = selected.value;
  }

  setData("vote_users", voteUsers);
  renderVotePage();
}

// ------------------ 顯示投票結果 ------------------
function showResult(data) {
  const resultDiv = document.getElementById("voteResult");
  const total = Object.values(data.options).reduce((a, b) => a + b, 0);
  resultDiv.innerHTML = "<h4>投票結果</h4>";

  for (let opt in data.options) {
    const count = data.options[opt];
    const percent = total > 0 ? ((count / total) * 100).toFixed(1) : 0;
    const bar = `<div style='background:#4caf50;height:8px;margin:4px 0;width:${percent}%'></div>`;
    resultDiv.innerHTML += `<div>${opt}：${count} 票 (${percent}%)${bar}</div>`;
  }
}

// ------------------ 初始執行 ------------------
window.onload = () => {
  renderVotePage();
};
