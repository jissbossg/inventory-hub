// Botones "+10 stock"
document.querySelectorAll('.btn-restock').forEach(btn => {
  btn.addEventListener('click', async () => {
    const id = parseInt(btn.dataset.id, 10);
    const resp = await fetch(`/api/products/${id}/stock`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ delta: 10, reason: 'Restock from UI' })
    });
    if (!resp.ok) {
      const err = await resp.json();
      alert(`Error: ${err.message || 'No se pudo actualizar'}`);
    }
  });
});

// SignalR: actualizaciones en vivo del stock
(function () {
  if (!window.signalR) return;
  const connection = new signalR.HubConnectionBuilder()
    .withUrl('/hubs/inventory')
    .withAutomaticReconnect()
    .build();

  connection.on('stockUpdated', payload => {
    const card = document.querySelector(`[data-product-id="${payload.productId}"] .stock-value`);
    if (card) card.textContent = payload.newStock;
    const detail = document.getElementById(`stock-${payload.productId}`);
    if (detail) detail.textContent = payload.newStock;
  });

  connection.start().then(() => {
    const live = document.getElementById('live-indicator');
    if (live) live.classList.add('active');
  }).catch(() => {});
})();