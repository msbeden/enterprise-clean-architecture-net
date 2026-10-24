import React, { useState } from 'react';
import { X, Trash2, Plus, Minus, ShoppingBag, CheckCircle, ShieldAlert } from 'lucide-react';
import { useCart } from '../../context/CartContext';
import { useAuth } from '../../context/AuthContext';
import { apiClient } from '../../services/api';
import { useNavigate } from 'react-router-dom';

export const CartDrawer: React.FC = () => {
  const { cartItems, isCartOpen, setIsCartOpen, removeFromCart, updateQuantity, clearCart, totalAmount } = useCart();
  const { isAuthenticated } = useAuth();
  const navigate = useNavigate();

  const [shippingAddress, setShippingAddress] = useState('TEİTECH Eskişehir Ar-Ge Binası No:4');
  const [loading, setLoading] = useState(false);
  const [orderSuccess, setOrderSuccess] = useState<string | null>(null);
  const [errorMsg, setErrorMsg] = useState<string | null>(null);

  if (!isCartOpen) return null;

  const handleCheckout = async () => {
    if (!isAuthenticated) {
      setIsCartOpen(false);
      navigate('/login');
      return;
    }

    if (cartItems.length === 0) return;

    setLoading(true);
    setErrorMsg(null);

    const orderCommand = {
      shippingAddress,
      items: cartItems.map(item => ({
        productId: item.product.id,
        quantity: item.quantity,
        unitPriceAmount: item.product.price.amount,
        currency: item.product.price.currency
      }))
    };

    const res = await apiClient.createOrder(orderCommand);
    setLoading(false);

    if (res.isSuccess && res.value) {
      setOrderSuccess(res.value);
      clearCart();
    } else {
      setErrorMsg(res.error?.description || 'Sipariş oluşturulurken bir hata oluştu.');
    }
  };

  return (
    <div style={{
      position: 'fixed',
      inset: 0,
      zIndex: 50,
      background: 'rgba(0, 0, 0, 0.7)',
      backdropFilter: 'blur(6px)',
      display: 'flex',
      justifyContent: 'flex-end'
    }}>
      <div style={{
        width: '100%',
        maxWidth: '480px',
        height: '100%',
        background: '#0f172a',
        borderLeft: '1px solid var(--border-glass)',
        display: 'flex',
        flexDirection: 'column',
        boxShadow: '-10px 0 30px rgba(0,0,0,0.5)'
      }}>
        
        {/* Header */}
        <div style={{ padding: '20px', borderBottom: '1px solid var(--border-glass)', display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
            <ShoppingBag color="#06b6d4" size={24} />
            <h3 style={{ fontSize: '1.2rem', fontWeight: 700 }}>Sipariş Sepeti</h3>
          </div>
          <button onClick={() => { setIsCartOpen(false); setOrderSuccess(null); }} style={{ background: 'none', border: 'none', color: '#9ca3af', cursor: 'pointer' }}>
            <X size={24} />
          </button>
        </div>

        {/* Content */}
        <div style={{ flex: 1, overflowY: 'auto', padding: '20px' }}>
          {orderSuccess ? (
            <div style={{ textAlign: 'center', padding: '40px 20px' }}>
              <div style={{ width: '64px', height: '64px', background: 'rgba(16,185,129,0.15)', color: '#10b981', borderRadius: '50%', display: 'flex', alignItems: 'center', justifyContent: 'center', margin: '0 auto 20px' }}>
                <CheckCircle size={36} />
              </div>
              <h4 style={{ fontSize: '1.3rem', fontWeight: 700, marginBottom: '10px' }}>Sipariş Başarıyla Oluşturuldu!</h4>
              <p style={{ color: '#9ca3af', fontSize: '0.9rem', marginBottom: '20px' }}>
                Sipariş Numaranız: <strong style={{ color: '#06b6d4' }}>#{orderSuccess}</strong>
              </p>
              <button onClick={() => { setOrderSuccess(null); setIsCartOpen(false); }} className="btn-primary" style={{ width: '100%', justifyContent: 'center' }}>
                Alışverişe Devam Et
              </button>
            </div>
          ) : cartItems.length === 0 ? (
            <div style={{ textAlign: 'center', padding: '60px 20px', color: '#9ca3af' }}>
              <ShoppingBag size={48} style={{ opacity: 0.3, marginBottom: '16px' }} />
              <p>Sepetiniz henüz boş.</p>
            </div>
          ) : (
            <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
              {cartItems.map(item => (
                <div key={item.product.id} className="glass-card" style={{ padding: '14px', display: 'flex', gap: '14px', alignItems: 'center' }}>
                  <img src={item.product.imageUrl} alt={item.product.name} style={{ width: '64px', height: '64px', objectFit: 'cover', borderRadius: '8px' }} />
                  <div style={{ flex: 1 }}>
                    <div style={{ fontSize: '0.9rem', fontWeight: 600, color: '#f3f4f6', marginBottom: '4px' }}>{item.product.name}</div>
                    <div style={{ fontSize: '0.8rem', color: '#06b6d4', fontWeight: 700 }}>
                      {item.product.price.amount.toLocaleString('tr-TR')} {item.product.price.currency}
                    </div>
                  </div>

                  <div style={{ display: 'flex', alignItems: 'center', gap: '8px', background: 'rgba(0,0,0,0.3)', padding: '4px', borderRadius: '6px' }}>
                    <button onClick={() => updateQuantity(item.product.id, item.quantity - 1)} style={{ background: 'none', border: 'none', color: '#9ca3af', cursor: 'pointer', padding: '2px' }}>
                      <Minus size={14} />
                    </button>
                    <span style={{ fontSize: '0.85rem', fontWeight: 700, minWidth: '18px', textAlign: 'center' }}>{item.quantity}</span>
                    <button onClick={() => updateQuantity(item.product.id, item.quantity + 1)} style={{ background: 'none', border: 'none', color: '#9ca3af', cursor: 'pointer', padding: '2px' }}>
                      <Plus size={14} />
                    </button>
                  </div>

                  <button onClick={() => removeFromCart(item.product.id)} style={{ background: 'none', border: 'none', color: '#f43f5e', cursor: 'pointer', padding: '4px' }}>
                    <Trash2 size={16} />
                  </button>
                </div>
              ))}

              <div style={{ marginTop: '20px' }}>
                <label style={{ display: 'block', fontSize: '0.85rem', fontWeight: 600, color: '#9ca3af', marginBottom: '8px' }}>
                  Teslimat / Teslim Alma Adresi
                </label>
                <textarea
                  className="form-control"
                  rows={2}
                  value={shippingAddress}
                  onChange={e => setShippingAddress(e.target.value)}
                />
              </div>

              {!isAuthenticated && (
                <div style={{ padding: '10px 14px', background: 'rgba(234,179,8,0.1)', border: '1px solid rgba(234,179,8,0.3)', borderRadius: '8px', color: '#fef08a', fontSize: '0.85rem', display: 'flex', alignItems: 'center', gap: '8px' }}>
                  <ShieldAlert size={18} />
                  Siparişi tamamlamak için oturum açmanız gerekmektedir.
                </div>
              )}

              {errorMsg && (
                <div style={{ padding: '10px 14px', background: 'rgba(244,63,94,0.1)', border: '1px solid rgba(244,63,94,0.3)', borderRadius: '8px', color: '#fda4af', fontSize: '0.85rem' }}>
                  {errorMsg}
                </div>
              )}
            </div>
          )}
        </div>

        {/* Footer */}
        {!orderSuccess && cartItems.length > 0 && (
          <div style={{ padding: '20px', borderTop: '1px solid var(--border-glass)', background: 'rgba(15,23,42,0.9)' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '16px' }}>
              <span style={{ color: '#9ca3af', fontSize: '0.95rem' }}>Toplam Tutar:</span>
              <span style={{ fontSize: '1.4rem', fontWeight: 800, color: '#06b6d4' }}>
                {totalAmount.toLocaleString('tr-TR')} TRY
              </span>
            </div>

            <button
              onClick={handleCheckout}
              disabled={loading}
              className="btn-primary"
              style={{ width: '100%', justifyContent: 'center', padding: '14px' }}
            >
              {loading ? 'Sipariş İşleniyor...' : isAuthenticated ? 'Siparişi Onayla ve Tamamla' : 'Giriş Yap ve Sipariş Ver'}
            </button>
          </div>
        )}

      </div>
    </div>
  );
};
