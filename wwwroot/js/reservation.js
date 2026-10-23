// 預約表單：依儀器與時段即時試算費用、提示時數上限
(function () {
    const select = document.getElementById('instrumentSelect');
    const startInput = document.getElementById('startInput');
    const endInput = document.getElementById('endInput');
    const feeBox = document.getElementById('feeBox');
    const info = document.getElementById('instrumentInfo');
    if (!select || !startInput || !endInput || !feeBox) return;

    const list = window.labInstruments || [];
    const fmt = n => n.toLocaleString('zh-TW', { maximumFractionDigits: 0 });

    function update() {
        const inst = list.find(i => i.id === parseInt(select.value, 10));
        info.innerHTML = inst
            ? `<i class="bi bi-geo-alt"></i> ${inst.location}　·　每小時 NT$ ${fmt(inst.rate)}　·　單次最多 ${inst.max} 小時` +
              (inst.training ? '　·　<span class="text-danger">需完成教育訓練</span>' : '')
            : '';

        const start = new Date(startInput.value);
        const end = new Date(endInput.value);
        if (!inst || isNaN(start) || isNaN(end)) {
            feeBox.className = 'fee-box';
            feeBox.textContent = '請選擇儀器與時段';
            return;
        }
        const hours = (end - start) / 36e5;
        if (hours <= 0) {
            feeBox.className = 'fee-box text-danger';
            feeBox.textContent = '結束時間必須晚於開始時間';
            return;
        }
        const over = hours > inst.max;
        feeBox.className = 'fee-box' + (over ? ' text-danger' : '');
        feeBox.innerHTML = over
            ? `共 ${hours} 小時，超過單次上限 ${inst.max} 小時`
            : `共 <strong>${hours}</strong> 小時 × NT$ ${fmt(inst.rate)} = <strong class="fs-5">NT$ ${fmt(hours * inst.rate)}</strong>`;
    }

    // 開始時間改變時，若結束時間早於開始，自動往後帶 2 小時
    startInput.addEventListener('change', () => {
        const start = new Date(startInput.value);
        const end = new Date(endInput.value);
        if (!isNaN(start) && (isNaN(end) || end <= start)) {
            const e = new Date(start.getTime() + 2 * 36e5);
            const pad = n => String(n).padStart(2, '0');
            endInput.value = `${e.getFullYear()}-${pad(e.getMonth() + 1)}-${pad(e.getDate())}T${pad(e.getHours())}:${pad(e.getMinutes())}`;
        }
        update();
    });
    [select, endInput].forEach(el => el.addEventListener('change', update));
    update();
})();
