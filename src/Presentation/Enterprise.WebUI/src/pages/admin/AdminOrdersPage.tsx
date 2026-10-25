import React, { useState, useEffect } from 'react';
import { AdminLayout } from '../../components/layout/AdminLayout';
import type { OrderDto } from '../../types/order.types';
import { apiClient } from '../../services/api';
import { ShoppingBag, Eye, X, MapPin, Calendar, UserCheck } from 'lucide-react';

export const AdminOrdersPage: React.FC = () => {
  const [orders, setOrders] = useState<OrderDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedOrder, setSelectedOrder] = useState<OrderDto | null>(null);

  const loadData = () => {
    setLoading(true);
    apiClient.getOrders().then(res => {
      setLoading(false);
      if (res.isSuccess && res.value) {
        setOrders(res.value);
      }
    });
  };

  useEffect(() => {
    loadData();
  }, []);

  return (
    <AdminLayout>
      <div style={{ marginBottom: '28px' }}>
        <h1 style={{ fontSize: '1.8rem', fontWeight: 800, color: '#f3f4f6', marginBottom: '4px' }}>Sipariş Yönetimi</h1>
        <p style={{ color: '#9ca3af', fontSize: '0.9rem' }}>Müşteri tarafından verilen siparişler ve teslimat detayları</p>
      </div>

      <div className="glass-panel" style={{ overflow: 'hidden' }}>
        {loading ? (
          <div style={{ padding: '40px', textAlign: 'center', color: '#9ca3af' }}>Siparişler yükleniyor...</div>
        ) : orders.length === 0 ? (
          <div style={{ padding: '40px', textAlign: 'center', color: '#9ca3af' }}>Henüz sipariş bulunmamaktadır.</div>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>Sipariş No</th>
                <th>Müşteri</th>
                <th>Tarih</th>
                <th>Toplam Tutar</th>
                <th>Durum</th>
                <th>İşlemler</th>
              </tr>
            </thead>
            <tbody>
              {orders.map(ord => (
                <tr key={ord.id}>
                  <td>
                    <span className="badge badge-cyan">#{ord.id}</span>
                  </td>
                  <td>
                    <strong style={{ color: '#f3f4f6' }}>{ord.customerName || 'Kullanıcı'}</strong>
                  </td>
                  <td style={{ color: '#9ca3af' }}>{ord.orderDate}</td>
                  <td>
                    <strong style={{ color: '#10b981' }}>{ord.totalAmount.amount.toLocaleString('tr-TR')} {ord.totalAmount.currency}</strong>
                  </td>
                  <td>
                    <span className="badge badge-emerald">{ord.status}</span>
                  </td>
                  <td>
                    <button
                      onClick={() => setSelectedOrder(ord)}
                      className="btn-secondary"
                      style={{ padding: '6px 12px', fontSize: '0.8rem' }}
                    >
                      <Eye size={14} /> Detay Gör
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {/* Order Detail Modal */}
      {selectedOrder && (
        <div style={{ position: 'fixed', inset: 0, zIndex: 50, background: 'rgba(0,0,0,0.75)', backdropFilter: 'blur(8px)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '20px' }}>
          <div className="glass-panel" style={{ width: '100%', maxWidth: '560px', padding: '32px', background: '#0f172a' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px' }}>
              <h3 style={{ fontSize: '1.3rem', fontWeight: 700, display: 'flex', alignItems: 'center', gap: '10px' }}>
                <ShoppingBag color="#10b981" /> Sipariş Detayı #{selectedOrder.id}
              </h3>
              <button onClick={() => setSelectedOrder(null)} style={{ background: 'none', border: 'none', color: '#9ca3af', cursor: 'pointer' }}>
                <X size={24} />
              </button>
            </div>

            <div style={{ display: 'flex', flexDirection: 'column', gap: '20px' }}>
              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px', background: 'rgba(255,255,255,0.03)', padding: '16px', borderRadius: '10px' }}>
                <div>
                  <div style={{ fontSize: '0.75rem', color: '#6b7280', display: 'flex', alignItems: 'center', gap: '4px' }}>
                    <UserCheck size={12} /> Müşteri
                  </div>
                  <div style={{ fontWeight: 600, color: '#f3f4f6' }}>{selectedOrder.customerName}</div>
                </div>
                <div>
                  <div style={{ fontSize: '0.75rem', color: '#6b7280', display: 'flex', alignItems: 'center', gap: '4px' }}>
                    <Calendar size={12} /> Sipariş Tarihi
                  </div>
                  <div style={{ fontWeight: 600, color: '#f3f4f6' }}>{selectedOrder.orderDate}</div>
                </div>
              </div>

              <div>
                <div style={{ fontSize: '0.75rem', color: '#6b7280', marginBottom: '4px', display: 'flex', alignItems: 'center', gap: '4px' }}>
                  <MapPin size={12} /> Teslimat Adresi
                </div>
                <div style={{ background: 'rgba(255,255,255,0.03)', padding: '12px', borderRadius: '8px', fontSize: '0.85rem', color: '#e5e7eb' }}>
                  {selectedOrder.shippingAddress}
                </div>
              </div>

              <div>
                <div style={{ fontSize: '0.85rem', fontWeight: 600, color: '#9ca3af', marginBottom: '10px' }}>Sipariş Kalemleri</div>
                <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
                  {selectedOrder.items.map((item, idx) => (
                    <div key={idx} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '10px 14px', background: 'rgba(255,255,255,0.02)', border: '1px solid var(--border-glass)', borderRadius: '8px' }}>
                      <div>
                        <div style={{ fontWeight: 600, fontSize: '0.9rem', color: '#f3f4f6' }}>{item.productName}</div>
                        <div style={{ fontSize: '0.75rem', color: '#9ca3af' }}>Birim: {item.unitPrice.amount.toLocaleString('tr-TR')} TRY x {item.quantity} Adet</div>
                      </div>
                      <div style={{ fontWeight: 700, color: '#06b6d4', fontSize: '0.95rem' }}>
                        {item.totalPrice.amount.toLocaleString('tr-TR')} TRY
                      </div>
                    </div>
                  ))}
                </div>
              </div>

              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', borderTop: '1px solid var(--border-glass)', paddingTop: '16px', marginTop: '8px' }}>
                <span style={{ color: '#9ca3af' }}>Toplam Tutar:</span>
                <span style={{ fontSize: '1.4rem', fontWeight: 800, color: '#10b981' }}>
                  {selectedOrder.totalAmount.amount.toLocaleString('tr-TR')} TRY
                </span>
              </div>
            </div>

          </div>
        </div>
      )}
    </AdminLayout>
  );
};
