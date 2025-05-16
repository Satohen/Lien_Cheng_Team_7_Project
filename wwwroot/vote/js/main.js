// --------------------- 使用者初始化 ---------------------
let user = localStorage.getItem("current_user") || "test";
localStorage.setItem("current_user", user);
let users = getData("vote_users") || {};
if (!users[user]) users[user] = { votes: [] };
setData("vote_users", users);

// --------------------- 樣式模組化 ---------------------
function styleAsCenteredButton(button) {
  button.style.fontSize = "16px";
  button.style.padding = "6px 14px";
  button.style.display = "flex";
  button.style.alignItems = "center";
  button.style.justifyContent = "center";
  button.style.lineHeight = "1";
}

function styleTopicHeaderRow(container) {
  container.style.display = "flex";
  container.style.justifyContent = "space-between";
  container.style.alignItems = "center";
  container.style.marginBottom = "8px";
}

function styleCheckboxRow(labelText) {
  const row = document.createElement("div");
  row.style.display = "flex";
  row.style.alignItems = "center";
  row.style.gap = "8px";

  const label = document.createElement("label");
  label.textContent = labelText;

  const checkbox = document.createElement("input");
  checkbox.type = "checkbox";
  checkbox.style.width = "18px";
  checkbox.style.height = "18px";

  row.appendChild(label);
  row.appendChild(checkbox);
  return { row, checkbox };
}

function createButtonRow(buttons) {
  const row = document.createElement("div");
  row.style.display = "flex";
  row.style.gap = "12px";
  buttons.forEach(btn => row.appendChild(btn));
  return row;
}

// --------------------- 工具函式：共用選項行 ---------------------
function createOptionRow(value = "", onDelete, parent) {
  const div = document.createElement("div");
  div.className = "option-row";
  div.style.display = "flex";
  div.style.gap = "8px";
  div.style.marginBottom = "6px";

  const input = document.createElement("input");
  input.type = "text";
  input.className = "newOpt";
  input.placeholder = `選項${parent.querySelectorAll("input.newOpt").length + 1}`;
  if (value) input.value = value;

  const btn = document.createElement("button");
  btn.innerText = "刪除";
  styleAsCenteredButton(btn);
  btn.onclick = () => {
    parent.removeChild(div);
    if (onDelete) onDelete();
  };

  div.appendChild(input);
  div.appendChild(btn);
  parent.appendChild(div);
  return input;
}

// --------------------- 建立主題流程 ---------------------
function addOption() {
  const container = document.getElementById("optionContainer");
  createOptionRow("", renumberOptions, container);
}

function renumberOptions() {
  const inputs = document.querySelectorAll("#optionContainer input.newOpt");
  inputs.forEach((input, i) => {
    input.placeholder = `選項${i + 1}`;
  });
}

function createTopic() {
  const title = document.getElementById("newTopic").value.trim();
  const anonymous = document.getElementById("anonCheckbox").checked;
  const inputs = document.querySelectorAll("#optionContainer input.newOpt");
  const options = {};
  inputs.forEach(input => {
    const val = input.value.trim();
    if (val) options[val] = 0;
  });
  if (!title || Object.keys(options).length < 2) {
    alert("主題與選項不能為空，且至少需要兩個選項");
    return;
  }
  users[user].votes.push({ topic: title, options, anonymous, voters: {} });
  setData("vote_users", users);
  document.getElementById("newTopic").value = "";
  document.getElementById("anonCheckbox").checked = false;
  document.getElementById("optionContainer").innerHTML = "";
  addOption();
  addOption();
  renderTopics();
}

// --------------------- 主題編輯邏輯 ---------------------
function showEditForm(index) {
  const container = document.getElementById("editForm" + index);
  container.innerHTML = "";
  const topic = users[user].votes[index];
  const optionInputs = [];

  const { row: anonRow, checkbox: anonCheckbox } = styleCheckboxRow("匿名投票：");
  container.appendChild(anonRow);
  anonCheckbox.checked = topic.anonymous;

  const titleInput = document.createElement("input");
  titleInput.value = topic.topic;
  titleInput.style.margin = "6px 0";
  container.appendChild(titleInput);

  const editOptionContainer = document.createElement("div");
  container.appendChild(editOptionContainer);

function renumberEdit() {
  const inputs = editOptionContainer.querySelectorAll("input.newOpt");
  inputs.forEach((input, i) => {
    input.placeholder = `選項${i + 1}`;
  });
}

function addEditOption(value = "") {
  const input = createOptionRow(value, () => {
    // ❗ 當刪除時，同步從 optionInputs 裡移除對應的項目
    const idx = optionInputs.findIndex(item => item.input === input);
    if (idx !== -1) optionInputs.splice(idx, 1);
    renumberEdit();
  }, editOptionContainer);
  optionInputs.push({ old: null, input });
  renumberEdit();
}

  Object.keys(topic.options).forEach(opt => addEditOption(opt));

  const addBtn = document.createElement("button");
  addBtn.innerText = "新增選項";
  styleAsCenteredButton(addBtn);
  addBtn.onclick = () => addEditOption();

  const saveBtn = document.createElement("button");
  saveBtn.innerText = "儲存修改";
  styleAsCenteredButton(saveBtn);
  saveBtn.onclick = () => {
    const newTitle = titleInput.value.trim();
    const newOptions = {};
    optionInputs.forEach(({ input }) => {
      const val = input.value.trim();
      if (val) newOptions[val] = 0;
    });
    if (!newTitle || Object.keys(newOptions).length < 2) {
      alert("主題與選項不能為空，且至少兩項");
      return;
    }
    topic.topic = newTitle;
    topic.options = newOptions;
    topic.anonymous = anonCheckbox.checked;
    topic.voters = {};
    setData("vote_users", users);
    renderTopics();
  };

  container.appendChild(createButtonRow([addBtn, saveBtn]));
  container.style.display = "block";
}

// --------------------- 主題操作區域 ---------------------
function createTopicCard(topic, index) {
  const li = document.createElement("li");
  li.style.listStyleType = "none";
  li.style.maxWidth = "720px";
  li.style.margin = "0 auto 20px";

  const card = document.createElement("div");
  card.className = "topic-card";
  card.style.display = "flex";
  card.style.justifyContent = "space-between";
  card.style.alignItems = "center";
  card.style.border = "1px solid #ccc";
  card.style.borderRadius = "12px";
  card.style.padding = "16px 20px";

  const title = document.createElement("div");
  title.innerText = topic.topic;
  title.style.fontWeight = "bold";
  title.style.fontSize = "18px";

  const btnGroup = document.createElement("div");
  btnGroup.style.display = "flex";
  btnGroup.style.gap = "10px";

  const voteBtn = createCardButton("投票", () => goToVote(index));
  const editBtn = createCardButton("編輯", () => showEditForm(index));
  const delBtn = createCardButton("刪除", () => deleteTopic(index));

  btnGroup.append(voteBtn, editBtn, delBtn);
  card.append(title, btnGroup);
  li.appendChild(card);
    
  const editForm = document.createElement("div");
  editForm.id = `editForm${index}`;
  editForm.style.marginTop = "10px";
  li.appendChild(editForm);  

  return li;
}

function createCardButton(label, onClick) {
  const btn = document.createElement("button");
  btn.innerText = label;
  btn.onclick = onClick;
  styleAsCenteredButton(btn);  // 可重用你原本的樣式函式
  btn.style.minWidth = "60px"; // 一致寬度
  return btn;
}


function renderTopics() {
  const list = document.getElementById("topicList");
  list.innerHTML = "";
  users[user].votes.forEach((t, i) => {
    const card = createTopicCard(t, i);
    list.appendChild(card);
  });
}

function toggleMenu(index) {
  const menu = document.getElementById("menu" + index);
  menu.style.display = menu.style.display === "none" ? "block" : "none";
}

function deleteTopic(index) {
  if (!confirm("確定要刪除這個主題？")) return;
  users[user].votes.splice(index, 1);
  setData("vote_users", users);
  renderTopics();
}

function goToVote(index) {
  localStorage.setItem("vote_index", index);
  location.href = "vote.html";
}

function centerTopicHeader() {
  const header = document.getElementById("topicHeader");
  if (header) {
    header.style.textAlign = "center";
    header.style.fontSize = "28px";
    header.style.fontWeight = "bold";
    header.style.margin = "30px 0 20px 0";
  }
}


// --------------------- 初始執行 ---------------------
window.onload = () => {
  addOption();
  addOption();
  renderTopics();
  centerTopicHeader();
  document.getElementById("topicList").style.maxWidth = "720px";
  document.getElementById("topicList").style.margin = "0 auto";
};
