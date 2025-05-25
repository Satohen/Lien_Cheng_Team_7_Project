let attendanceChart;

function initAttendanceStats() {
    const yearSelect = document.getElementById('yearSelect');
    const canvas = document.getElementById('attendanceChart');

    if (!yearSelect || !canvas) {
        console.warn("元素還沒載入完成，稍後重試...");
        return setTimeout(initAttendanceStats, 100);
    }

    initYearOptions();
    loadAttendanceChart();
}

async function loadAttendanceChart() {
    const year = document.getElementById('yearSelect').value;
    const ctx = document.getElementById('attendanceChart').getContext('2d');

    try {
        const res = await fetch('/api/attendance/yearly', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                year: parseInt(year),
                employeeId: '1' // TODO 你可以改成登入後的變數
            })
        });

        const data = await res.json();
        const labels = ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'];

        const config = {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: '出勤天數',
                    data: data,
                    backgroundColor: 'rgba(54, 162, 235, 0.6)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                scales: {
                    y: {
                        beginAtZero: true,
                        stepSize: 1
                    }
                }
            }
        };

        if (attendanceChart && typeof attendanceChart.destroy === 'function') {
            attendanceChart.destroy();
        }
        attendanceChart = new Chart(ctx, config);
    } catch (err) {
        console.error('讀取出勤資料失敗：', err);
    }
}

function initYearOptions() {
    const yearSelect = document.getElementById('yearSelect');
    const thisYear = new Date().getFullYear();

    // 清空舊的（避免重新執行時重複）
    yearSelect.innerHTML = '';

    for (let y = thisYear; y >= thisYear - 3; y--) {
        const opt = document.createElement('option');
        opt.value = y;
        opt.textContent = `${y} 年`;
        yearSelect.appendChild(opt);
    }

    // 預設選擇今年
    yearSelect.value = thisYear;

    // 綁定 onchange 事件（選擇年份會重新載入圖表）
    yearSelect.addEventListener('change', loadAttendanceChart);
}

setTimeout(initAttendanceStats, 100);
