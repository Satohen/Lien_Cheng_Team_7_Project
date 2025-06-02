function initLeaveForm() {
    const btnSubmit = document.getElementById('submitLeave');
    const btnCancel = document.getElementById('cancelLeave');

    if (!btnSubmit || !btnCancel) {
        console.warn('請假元件尚未載入，稍後重試...');
        return setTimeout(initLeaveForm, 100);
    }

    btnSubmit.addEventListener('click', async () => {
        const leaveType = document.getElementById('leaveType').value.trim();
        const fromDate = document.getElementById('fromDate').value;
        const toDate = document.getElementById('toDate').value;
        const reason = document.getElementById('reason').value.trim();
        const fileInput = document.getElementById('attachment');
        const file = fileInput?.files?.[0];

        // 檢查欄位必填
        if (!leaveType || !fromDate || !toDate) {
            alert('請填寫完整資料！');
            return;
        }

        // 檢查檔案格式與大小
        if (file) {
            const allowedTypes = ['application/pdf', 'image/png', 'image/jpeg'];
            const maxSize = 2 * 1024 * 1024; // 2MB

            if (!allowedTypes.includes(file.type)) {
                fileInput.classList.add('is-invalid');
                document.getElementById('attachmentFeedback').style.display = 'block';
                alert('檔案類型僅限 PDF、JPG、PNG');
                return;
            } else {
                fileInput.classList.remove('is-invalid');
            }

            if (file.size > maxSize) {
                fileInput.classList.add('is-invalid');
                document.getElementById('attachmentFeedback').style.display = 'block';
                alert('檔案大小不能超過 2MB');
                return;
            } else {
                fileInput.classList.remove('is-invalid');
                document.getElementById('attachmentFeedback').style.display = 'none';
            }
        }

        const formData = new FormData();
        formData.append('employeeId', 1); // TODO: 改為動態使用者 ID
        formData.append('leaveType', leaveType);
        formData.append('fromDate', fromDate);
        formData.append('toDate', toDate);
        formData.append('reason', reason);

        if (file) formData.append('attachment', file);

        try {
            const res = await fetch('/api/leave/apply', {
                method: 'POST',
                body: formData
            });

            const result = await res.json();
            alert(result.message || '申請成功！');
            document.getElementById('leaveType').value = '';
            document.getElementById('fromDate').value = '';
            document.getElementById('toDate').value = '';
            document.getElementById('reason').value = '';
            document.getElementById('attachment').value = '';
            document.getElementById('attachment').classList.remove('is-invalid');


        } catch (err) {
            const text = await res.text();
            throw new Error(text); // 👈 丟出來給 catch 處理
            console.error('請求發生錯誤', err);
            alert('申請失敗，請稍後再試。');
        }
    });

    btnCancel.addEventListener('click', () => {
        document.getElementById('leaveType').value = '';
        document.getElementById('fromDate').value = '';
        document.getElementById('toDate').value = '';
        document.getElementById('reason').value = '';
        document.getElementById('attachment').value = '';
        document.getElementById('attachment').classList.remove('is-invalid');
    });
    loadData();
}
console.log('載入完成');

setTimeout(initLeaveForm, 100);
