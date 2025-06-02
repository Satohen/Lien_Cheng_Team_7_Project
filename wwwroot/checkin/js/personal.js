function initPersonalPage() {
    const tabLeave = document.getElementById('leave-tab');
    const leaveTableBody = document.getElementById('leaveTableBody');
    const pageSizeSelect = document.getElementById('pageSizeSelect');
    const paginationContainer = document.getElementById('pagination');
    
    let currentPage = 1;
    let pageSize = 10;
    let totalPages = 1;

    if (!tabLeave || !leaveTableBody) {
        console.warn('員工資訊元件尚未載入，稍後重試...');
        return setTimeout(initPersonalPage, 100);
    }

    tabLeave.addEventListener('click', loadData);
    pageSizeSelect.addEventListener('change', () => {
        pageSize = parseInt(pageSizeSelect.value);
        currentPage = 1;
        loadData();
    });

    async function loadData() {
        try {
            const res = await fetch('/api/leave/my-records', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    employeeId: 1, // TODO: 改為動態登入 ID
                    page: currentPage,
                    pageSize: pageSize
                })
            });

            const result = await res.json();
            const data = result.data || [];
            totalPages = Math.ceil(result.total / pageSize);
            renderPage(data);
        } catch (err) {
            console.error('取得請假紀錄失敗', err);
            leaveTableBody.innerHTML = '<tr><td colspan="6" class="text-danger text-center">載入失敗</td></tr>';
        }
    }

    function renderPage(data) {
        leaveTableBody.innerHTML = '';

        if (!Array.isArray(data) || data.length === 0) {
            leaveTableBody.innerHTML = '<tr><td colspan="6" class="text-center text-muted">無請假紀錄</td></tr>';
            paginationContainer.innerHTML = '';
            return;
        }

        data.forEach(item => {
            const tr = document.createElement('tr');
            const attachment = item.attachmentPath
                ? `<a href="${item.attachmentPath}" target="_blank">檢視</a>`
                : '--';

            tr.innerHTML = `
        <td>${item.fromDate?.split('T')[0]}</td>
        <td>${item.toDate?.split('T')[0]}</td>
        <td>${item.leaveType}</td>
        <td>${item.reason || '--'}</td>
        <td>${item.status}</td>
        <td>${attachment}</td>
      `;
            leaveTableBody.appendChild(tr);
        });

        renderPagination();
    }

    function renderPagination() {
        paginationContainer.innerHTML = '';

        for (let i = 1; i <= totalPages; i++) {
            const btn = document.createElement('button');
            btn.className = `btn btn-sm ${i === currentPage ? 'btn-primary' : 'btn-outline-primary'} me-1`;
            btn.textContent = i;
            btn.addEventListener('click', () => {
                currentPage = i;
                loadData();
            });
            paginationContainer.appendChild(btn);
        }
    }
}

setTimeout(initPersonalPage, 100);
