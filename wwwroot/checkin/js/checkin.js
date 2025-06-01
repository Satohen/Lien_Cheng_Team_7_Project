function initCheckinPage() {
    const nameInput = document.getElementById('nameInput');
    const clock = document.getElementById('clock-display');
    const checkinList = document.getElementById('checkinList');
    const monthSelector = document.getElementById('monthSelector');
    const btnCheckin = document.getElementById('btnCheckin');
    const btnCheckout = document.getElementById('btnCheckout');
    const btnExportExcel = document.getElementById('btnExportExcel');

    if (!nameInput || !clock || !checkinList || !monthSelector) {
        console.warn('元素尚未準備好，稍後再試...');
        return setTimeout(initCheckinPage, 100);
    }

    // ➤ 建立月份下拉選單（近六個月）
    function initMonthSelector() {
        const now = new Date();
        for (let i = 0; i < 6; i++) {
            const date = new Date(now.getFullYear(), now.getMonth() - i, 1);
            const value = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`;
            const label = `${date.getFullYear()}年 ${date.getMonth() + 1}月`;
            const option = new Option(label, value);
            monthSelector.appendChild(option);
        }
        monthSelector.value = `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}`;
    }

    // 時鐘更新
    function updateInnerClock() {
        const now = new Date();
        const timeString = now.toLocaleTimeString('zh-Hant-TW', { hour12: false });
        clock.textContent = timeString;
    }
    setInterval(updateInnerClock, 1000);
    updateInnerClock();

    // ➤ 載入指定月份的歷史紀錄
    async function loadCheckins() {
        const selectedMonth = monthSelector.value;
        const res = await fetch('/api/checkin/history', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                employeeId: 1,
                month: selectedMonth
            })
        });

        const data = await res.json();
        checkinList.innerHTML = '';

        if (data.length === 0) {
            checkinList.innerHTML = `<tr><td colspan="3" class="text-center">本月無打卡紀錄</td></tr>`;
        } else {
            data.forEach(item => {
                const date = item.date?.split('T')[0] || '--';
                const checkinTime = item.checkInTime?.split('T')[1]?.substring(0, 5) || '';
                const checkoutTime = item.checkOutTime?.split('T')[1]?.substring(0, 5) || '--';
                const isLate = item.isLate === true;
                const isLeave = !!item.leaveType?.trim();
                const isWorkday = item.isWorkday === true;

                let tdClass = '';
                if (isLate) tdClass += ' text-danger fw-bold';
                if (isLeave) tdClass += ' text-info fst-italic';

                let checkinDisplay = '';

                if (!isWorkday) {
                    checkinDisplay = '<span class="text-secondary">假日</span>';
                } else if (checkinTime) {
                    checkinDisplay = checkinTime;
                } else if (isLeave) {
                    checkinDisplay = `${item.leaveType}（請假）`;
                } else {
                    checkinDisplay = '<span class="text-warning fw-bold">未出勤</span>'; // 曠班
                }

                const tr = document.createElement('tr');
                tr.innerHTML = `
        <td>${date}</td>
        <td class="${tdClass.trim()}">${checkinDisplay}</td>
        <td>${checkoutTime}</td>
    `;
                checkinList.appendChild(tr);
            });
        }
        await updateCheckinButtonStatus(1);
    }


    // 綁定出勤按鈕事件
    btnCheckin.addEventListener('click', async () => {
        const now = new Date();
        const hour = now.getHours();
        if (hour >= 12) {
            const confirmLate = confirm("現在已超過中午 12 點，是否仍要打卡出勤？");
            if (!confirmLate) return;
        }
        const res = await fetch('/api/checkin/checkin', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ employeeId: 1 })  // ⚠️ 之後改成動態值
        });
        const result = await res.json();
        alert(result.message);
        loadCheckins();
    });

    // 綁定退勤按鈕事件
    btnCheckout.addEventListener('click', async () => {
        const res = await fetch('/api/checkin/checkout', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ employeeId: 1 })  // ⚠️ 同上
        });
        const result = await res.json();
        alert(result.message);
        loadCheckins();
    });

    //下載事件
    btnExportExcel.addEventListener('click', async () => {
        const selectedMonth = monthSelector.value;

        try {
            const res = await fetch('/api/checkin/export-csv', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    employeeId: 1,
                    month: selectedMonth
                })
            });

            if (!res.ok) throw new Error('下載失敗');

            const blob = await res.blob();
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `出勤紀錄_${selectedMonth}.csv`;
            document.body.appendChild(a);
            a.click();
            a.remove();
            window.URL.revokeObjectURL(url);
        } catch (err) {
            alert('下載失敗，請稍後再試');
            console.error(err);
        }
    });



    // ➤ 月份變更事件
    monthSelector.addEventListener('change', loadCheckins);

    // 初始載入
    initMonthSelector();
    loadCheckins();
}


async function updateCheckinButtonStatus(employeeId) {
    const res = await fetch('/api/checkin/today', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ employeeId })
    });

    const todayRecord = await res.json();

    btnCheckin.disabled = false;
    btnCheckout.disabled = false;
    btnCheckin.title = '';
    btnCheckout.title = '';

    if (todayRecord) {
        if (todayRecord.checkInTime) {
            btnCheckin.disabled = true;
            btnCheckin.title = "您今天已經出勤過囉～";
        }
        if (!todayRecord.checkInTime) {
            btnCheckout.disabled = true;
            btnCheckout.title = "請先完成出勤，才能退勤～";
        }
        if (todayRecord.checkOutTime) {
            btnCheckout.disabled = true;
            btnCheckout.title = "您今天已經退勤過囉～";
        }
    } else {
        btnCheckout.disabled = true;
        btnCheckout.title = "請先完成出勤，才能退勤～";
    }
}

// 檔案最下方
setTimeout(initCheckinPage, 100);
