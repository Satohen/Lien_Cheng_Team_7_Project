// --------------------- 使用者名稱顯示（防呆處理） ---------------------
const name = localStorage.getItem("current_user");
const el = document.getElementById("userDisplay");
if (el && name) el.innerText = name;
// ---初始化主題列表---
renderTopics();
// --------------------- 跳轉到投票頁面 ---------------------
function goToVote(index) {
  localStorage.setItem("current_vote_index", index);  // 設定目前選擇的主題 index
  window.location.href = "vote.html";  // 跳轉到投票頁
}
// --------------------- 建立新主題 ---------------------
function createTopic() {
  const user = localStorage.getItem("current_user");
  const topic = document.getElementById("newTopic").value.trim();
  const opts = Array.from(document.querySelectorAll(".newOpt"))
                    .map(e => e.value.trim())
                    .filter(Boolean);  // 篩掉空選項
  const isAnon = document.getElementById("anonCheckbox").checked;
  if (!topic || opts.length < 2) return alert("請填寫主題與至少2個選項");

  const users = getData("vote_users");
  const options = {};
  opts.forEach(o => options[o] = 0);  // 每個選項初始票數設為 0

  users[user].votes.push({ topic, options, voters: {}, anonymous: isAnon });
  setData("vote_users", users);  // 儲存到 localStorage
  renderTopics();  // 重新顯示主題列表
}
// --------------------- 渲染所有主題列表 ---------------------
function renderTopics() {
  const user = localStorage.getItem("current_user");
  const users = getData("vote_users");
  const list = document.getElementById("topicList");
  list.innerHTML = "";

  users[user].votes.forEach((t, i) => {
    const li = document.createElement("li");

    li.innerHTML = `
      <div class="topic-card">
        <div style="font-size: 1.3em; font-weight: bold; cursor: pointer;" onclick="toggleMenu(${i})">
          ${t.topic}
        </div>
        <div id="menu${i}" style="display: none; margin-top: 8px;">
          <button onclick="goToVote(${i})">投票</button>
          <button onclick="showEditForm(${i})">編輯</button>
          <button onclick="deleteTopic(${i})">刪除</button>
          <div id="editForm${i}" style="display:none; margin-top:10px;"></div>
        </div>
      </div>
    `;

    list.appendChild(li);
  });
}
// --------------------- 展開/收合 主題功能選單 ---------------------
function toggleMenu(index) {
  const menu = document.getElementById("menu" + index);
  menu.style.display = menu.style.display === "none" ? "block" : "none";
}
// --------------------- 刪除主題 ---------------------
function deleteTopic(index) {
  const user = localStorage.getItem("current_user");
  const users = getData("vote_users");
  if (confirm("確定要刪除這個主題嗎？")) {
    users[user].votes.splice(index, 1);
    setData("vote_users", users);
    renderTopics();
  }
}
// --------------------- 快速編輯主題名稱 ---------------------
function editTopic(index) {
  const user = localStorage.getItem("current_user");
  const users = getData("vote_users");
  const newTitle = prompt("請輸入新的主題名稱：", users[user].votes[index].topic);
  if (newTitle) {
    users[user].votes[index].topic = newTitle;
    setData("vote_users", users);
    renderTopics();
  }
}
// --------------------- 顯示完整編輯表單 ---------------------
function showEditForm(index) {
  const user = localStorage.getItem("current_user");
  const users = getData("vote_users");
  const topic = users[user].votes[index];
  const container = document.getElementById("editForm" + index);
  container.innerHTML = "";
  // 編輯主題名稱
  const titleInput = document.createElement("input");
  titleInput.value = topic.topic;
  container.appendChild(document.createTextNode("主題："));
  container.appendChild(titleInput);
  // 編輯選項
  const optionInputs = [];
  Object.keys(topic.options).forEach((opt, idx) => {
    const input = document.createElement("input");
    input.value = opt;
    optionInputs.push({ old: opt, input });
    container.appendChild(document.createTextNode(`選項${idx + 1}：`));
    container.appendChild(input);
  });
  // 新增選項按鈕
  const addBtn = document.createElement("button");
  addBtn.innerText = "新增選項";
  addBtn.onclick = () => {
    const newInput = document.createElement("input");
    container.insertBefore(document.createTextNode(`新增選項：`), addBtn);
    container.insertBefore(newInput, addBtn);
    optionInputs.push({ old: null, input: newInput });
  };
  // 儲存修改按鈕
  const saveBtn = document.createElement("button");
  saveBtn.innerText = "儲存修改";
  saveBtn.onclick = () => {
    const newTitle = titleInput.value.trim();
    const newOptions = {};
    optionInputs.forEach(({ input }) => {
      const val = input.value.trim();
      if (val) {newOptions[val] = 0;}
    });
    topic.topic = newTitle;
    topic.options = newOptions;
    topic.voters = {}; // 編輯時清除舊投票紀錄
    setData("vote_users", users);
    renderTopics();
  };

  container.appendChild(document.createElement("br"));
  container.appendChild(addBtn);
  container.appendChild(saveBtn);
  container.style.display = "block";
}
// --------------------- 新增選項欄位（建立主題時） ---------------------
function addOption() {
  const container = document.getElementById("optionContainer");
  const count = container.querySelectorAll("input").length + 1;
  const input = document.createElement("input");
  input.type = "text";
  input.className = "newOpt";
  input.placeholder = `選項${count}`;
  container.appendChild(input);
}
