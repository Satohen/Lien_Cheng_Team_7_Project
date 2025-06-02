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
                employeeId: '1' // 改成登入者 ID
            })
        });

        const rawData = await res.json();
        const labels = ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'];

        const totalDays = Array(12).fill(0);
        const presentDays = Array(12).fill(0);
        const leaveDays = Array(12).fill(0);
        const absentDays = Array(12).fill(0);
        const personalLeave = Array(12).fill(0);
        const sickLeave = Array(12).fill(0);
        const annualLeave = Array(12).fill(0);

        rawData.forEach(item => {
            const i = item.month - 1;
            totalDays[i] = item.totalDays;
            presentDays[i] = item.presentDays;
            leaveDays[i] = item.leaveDays;
            absentDays[i] = item.absentDays;
            personalLeave[i] = item.personalLeaveDays;
            sickLeave[i] = item.sickLeaveDays;
            annualLeave[i] = item.annualLeaveDays;
        });

        const config = {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: '出勤',
                        data: presentDays,
                        backgroundColor: 'rgba(54, 162, 235, 0.8)',
                    },
                    {
                        label: '事假',
                        data: personalLeave,
                        backgroundColor: 'rgba(255, 205, 86, 0.8)',
                    },
                    {
                        label: '病假',
                        data: sickLeave,
                        backgroundColor: 'rgba(255, 99, 132, 0.8)',
                    },
                    {
                        label: '特休',
                        data: annualLeave,
                        backgroundColor: 'rgba(75, 192, 192, 0.8)',
                    },
                    {
                        label: '曠職',
                        data: absentDays,
                        backgroundColor: 'rgba(201, 203, 207, 0.8)',
                    }
                ]
            },
            options: {
                responsive: true,
                plugins: {
                    tooltip: {
                        mode: 'index',
                        intersect: false,
                    },
                    title: {
                        display: true,
                        text: `${year} 年出勤統計圖（堆疊）`
                    }
                },
                interaction: {
                    mode: 'nearest',
                    axis: 'x',
                    intersect: false
                },
                scales: {
                    x: {
                        stacked: true
                    },
                    y: {
                        stacked: true,
                        beginAtZero: true,
                        stepSize: 1,
                        title: {
                            display: true,
                            text: '天數'
                        }
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
