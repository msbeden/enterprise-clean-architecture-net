import React, { useEffect, useState } from 'react';
import { MainLayout } from '../../components/layout/MainLayout';
import { useAuth } from '../../context/AuthContext';
import { apiClient } from '../../services/api';
import type { OrderDto } from '../../types/order.types';
import { ShoppingBag, PackageCheck, MapPin, Calendar, ArrowLeft, ChevronDown, ChevronUp } from 'lucide-react';
import { useNavigate } from 'react-router-dom';

export const MyOrdersPage: React.FC = () => {
  const { user } = useAuth();
  const [orders, setOrders] = useState<OrderDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [expandedOrderId, setExpandedOrderId] = useState<string | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    if (user) {
      setLoading(true);
      apiClient.getMyOrders(user.userId).then(res => {
        setLoading(false);
        if (res.isSuccess && res.value) {
          setOrders(res.value);
          if (res.value.length > 0) {
            setExpandedOrderId(res.value[0].id);
          }
        }
      });
    } else {
      setLoading(false);
    }
  }, [user]);

  const toggleExpand = (id: string) => {
    setExpandedOrderId(prev => (prev === id ? null : id));
  };

  return (
    <MainLayout>
      <div style={{ maxWidth: '1000px', margin: '0 auto' }}>
        
        {/* Header Section */}
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '28px' }}>
          <div>
            <h1 style={{ fontSize: '1.8rem', fontWeight: 800, color: '#f3f4f6', marginBottom: '4px', display: 'flex', alignItems: 'center', gap: '10px' }}>
              <PackageCheck color="#06b6d4" size={28} /> Sipariş Geçmişim
            </h1>
            <p style={{ color: '#9ca3af', fontSize: '0.9rem' }}>
              Geçmişte verdiğiniz siparişlerinizin durumunu ve kargo detaylarını takip edebilirsiniz.
            </p>
          </div>

          <button onClick={() => navigate('/')} className="btn-secondary">
            <ArrowLeft size={16} />
            <span>Alışverişe Devam Et</span>
          </button>
        </div>

        {/* Content Section */}
        {loading ? (
          <div className="glass-panel" style={{ padding: '60px', textAlign: 'center', color: '#9ca3af' }}>
            Siparişleriniz yükleniyor...
          </div>
        ) : orders.length === 0 ? (
          <div className="glass-panel" style={{ padding: '60px', textAlign: 'center' }}>
            <div style={{ width: '64px', height: '64px', background: 'rgba(6,182,212,0.15)', borderRadius: '50%', display: 'flex', alignItems: 'center', justifyContent: 'center', margin: '0 auto 16px', color: '#06b6d4' }}>
              <ShoppingBag size={32} />
            </div>
            <h3 style={{ fontSize: '1.2rem', fontWeight: 700, color: '#f3f4f6', marginBottom: '8px' }}>
              Henüz Siparişiniz Bulunmamaktadır
            </h3>
            <p style={{ color: '#9ca3af', fontSize: '0.9rem', marginBottom: '24px' }}>
              Kurumsal ürün kataloğumuzu inceleyerek ilk siparişinizi hemen verebilirsiniz.
            </p>
            <button onClick={() => navigate('/')} className="btn-primary">
              Kataloğu İncele
            </button>
          </div>
        ) : (
          <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
            {orders.map(order => {
              const isExpanded = expandedOrderId === order.id;
              return (
                <div key={order.id} className="glass-panel" style={{ overflow: 'hidden', padding: 0 }}>
                  
                  {/* Order Header / Summary Row */}
                  <div
                    onClick={() => toggleExpand(order.id)}
                    style={{
                      padding: '20px 24px',
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'space-between',
                      cursor: 'pointer',
                      background: isExpanded ? 'rgba(255,255,255,0.03)' : 'transparent',
                      transition: 'background 0.2s ease'
                    }}
                  >
                    <div style={{ display: 'flex', alignItems: 'center', gap: '20px', flexWrap: 'wrap' }}>
                      <div>
                        <div style={{ fontSize: '0.75rem', color: '#9ca3af', fontWeight: 600 }}>SİPARİŞ KODU</div>
                        <div style={{ fontWeight: 800, color: '#67e8f9', fontSize: '1.05rem' }}>#{order.id}</div>
                      </div>

                      <div style={{ height: '30px', width: '1px', background: 'var(--border-glass)' }} />

                      <div style={{ display: 'flex', alignItems: 'center', gap: '6px', color: '#9ca3af', fontSize: '0.88rem' }}>
                        <Calendar size={16} color="#06b6d4" />
                        <span>{order.orderDate}</span>
                      </div>

                      <div style={{ height: '30px', width: '1px', background: 'var(--border-glass)' }} />

                      <div>
                        <span className={order.status === 'Tamamlandı' ? 'badge badge-emerald' : 'badge badge-cyan'}>
                          {order.status}
                        </span>
                      </div>
                    </div>

                    <div style={{ display: 'flex', alignItems: 'center', gap: '16px' }}>
                      <div style={{ textAlign: 'right' }}>
                        <div style={{ fontSize: '0.75rem', color: '#9ca3af', fontWeight: 600 }}>TOPLAM TUTAR</div>
                        <strong style={{ fontSize: '1.1rem', color: '#f3f4f6' }}>
                          {order.totalAmount.amount.toLocaleString('tr-TR')} {order.totalAmount.currency}
                        </strong>
                      </div>

                      <div style={{ color: '#9ca3af' }}>
                        {isExpanded ? <ChevronUp size={20} /> : <ChevronDown size={20} />}
                      </div>
                    </div>
                  </div>

                  {/* Expanded Order Items Detail */}
                  {isExpanded && (
                    <div style={{ padding: '20px 24px', borderTop: '1px solid var(--border-glass)', background: 'rgba(15,23,42,0.6)' }}>
                      
                      {/* Shipping Address */}
                      <div style={{ display: 'flex', alignItems: 'flex-start', gap: '8px', marginBottom: '20px', color: '#9ca3af', fontSize: '0.88rem', background: 'rgba(255,255,255,0.03)', padding: '12px 16px', borderRadius: '8px' }}>
                        <MapPin size={18} color="#6366f1" style={{ marginTop: '2px', flexShrink: 0 }} />
                        <div>
                          <strong style={{ color: '#e5e7eb', display: 'block', marginBottom: '2px' }}>Teslimat Adresi:</strong>
                          {order.shippingAddress}
                        </div>
                      </div>

                      {/* Items List */}
                      <div style={{ fontWeight: 700, fontSize: '0.9rem', color: '#f3f4f6', marginBottom: '12px' }}>
                        Sipariş İçeriği ({order.items.length} Kalem)
                      </div>

                      <div style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
                        {order.items.map((item, idx) => (
                          <div key={idx} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '12px 16px', background: 'rgba(255,255,255,0.02)', borderRadius: '8px', border: '1px solid rgba(255,255,255,0.05)' }}>
                            <div>
                              <div style={{ fontWeight: 600, color: '#f3f4f6', fontSize: '0.92rem' }}>{item.productName}</div>
                              <div style={{ fontSize: '0.8rem', color: '#9ca3af' }}>
                                Birim Fiyat: {item.unitPrice.amount.toLocaleString('tr-TR')} {item.unitPrice.currency} × {item.quantity} Adet
                              </div>
                            </div>
                            <strong style={{ color: '#06b6d4', fontSize: '0.95rem' }}>
                              {item.totalPrice.amount.toLocaleString('tr-TR')} {item.totalPrice.currency}
                            </strong>
                          </div>
                        ))}
                      </div>

                    </div>
                  )}

                </div>
              );
            })}
          </div>
        )}

      </div>
    </MainLayout>
  );
};
