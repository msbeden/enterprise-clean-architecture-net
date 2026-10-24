import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { ShoppingBag, Shield, LogOut, UserCheck, Layers, PackageCheck } from 'lucide-react';
import { useAuth } from '../../context/AuthContext';
import { useCart } from '../../context/CartContext';

export const Header: React.FC = () => {
  const { user, isAuthenticated, logout } = useAuth();
  const { totalCount, setIsCartOpen } = useCart();
  const navigate = useNavigate();

  return (
    <header className="glass-panel" style={{ borderRadius: 0, borderTop: 'none', borderLeft: 'none', borderRight: 'none', position: 'sticky', top: 0, zIndex: 40 }}>
      <div style={{ maxWidth: '1280px', margin: '0 auto', padding: '14px 24px', display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
        
        {/* Brand Logo */}
        <Link to="/" style={{ display: 'flex', alignItems: 'center', gap: '12px', textDecoration: 'none', color: 'inherit' }}>
          <div style={{
            background: 'linear-gradient(135deg, #06b6d4, #6366f1)',
            padding: '10px',
            borderRadius: '12px',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            boxShadow: '0 0 15px rgba(6, 182, 212, 0.4)'
          }}>
            <PackageCheck size={24} color="#ffffff" />
          </div>
          <div>
            <div style={{ fontSize: '1.25rem', fontWeight: 800, letterSpacing: '0.02em', background: 'linear-gradient(90deg, #ffffff, #67e8f9)', WebkitBackgroundClip: 'text', WebkitTextFillColor: 'transparent' }}>
              ENTERPRISE
            </div>
            <div style={{ fontSize: '0.7rem', color: '#9ca3af', fontWeight: 500, letterSpacing: '0.08em' }}>
              COMMERCE PORTAL
            </div>
          </div>
        </Link>

        {/* Navigation Links */}
        <nav style={{ display: 'flex', alignItems: 'center', gap: '24px' }}>
          <Link to="/" style={{ textDecoration: 'none', color: '#e5e7eb', fontWeight: 600, fontSize: '0.95rem', display: 'flex', alignItems: 'center', gap: '6px' }}>
            <Layers size={18} color="#06b6d4" />
            Ürün Kataloğu
          </Link>

          {isAuthenticated && (
            <Link to="/my-orders" style={{ textDecoration: 'none', color: '#9ca3af', fontWeight: 500, fontSize: '0.95rem', display: 'flex', alignItems: 'center', gap: '6px' }}>
              <PackageCheck size={18} color="#10b981" />
              Siparişlerim
            </Link>
          )}
          
          {isAuthenticated && user?.role === 'Admin' && (
            <Link to="/admin" style={{ textDecoration: 'none', color: '#9ca3af', fontWeight: 500, fontSize: '0.95rem', display: 'flex', alignItems: 'center', gap: '6px' }}>
              <Shield size={18} color="#6366f1" />
              Admin Paneli
            </Link>
          )}
        </nav>

        {/* Right Actions */}
        <div style={{ display: 'flex', alignItems: 'center', gap: '16px' }}>
          
          {/* Cart Button */}
          <button
            onClick={() => setIsCartOpen(true)}
            className="btn-secondary"
            style={{ position: 'relative' }}
          >
            <ShoppingBag size={20} color="#06b6d4" />
            <span>Sepetim</span>
            {totalCount > 0 && (
              <span style={{
                position: 'absolute',
                top: '-6px',
                right: '-6px',
                background: '#06b6d4',
                color: '#090d16',
                fontWeight: 800,
                fontSize: '0.75rem',
                width: '22px',
                height: '22px',
                borderRadius: '50%',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                boxShadow: '0 0 10px rgba(6, 182, 212, 0.6)'
              }}>
                {totalCount}
              </span>
            )}
          </button>

          {/* User / Login Button */}
          {isAuthenticated && user ? (
            <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: '8px', padding: '6px 12px', background: 'rgba(255,255,255,0.05)', borderRadius: '8px', border: '1px solid var(--border-glass)' }}>
                <UserCheck size={18} color="#10b981" />
                <div style={{ fontSize: '0.85rem' }}>
                  <div style={{ fontWeight: 600, color: '#f3f4f6' }}>{user.firstName} {user.lastName}</div>
                  <div style={{ fontSize: '0.7rem', color: '#9ca3af' }}>{user.role || 'Kullanıcı'}</div>
                </div>
              </div>

              <button
                onClick={logout}
                title="Çıkış Yap"
                className="btn-danger"
                style={{ padding: '8px', borderRadius: '8px' }}
              >
                <LogOut size={18} />
              </button>
            </div>
          ) : (
            <button onClick={() => navigate('/login')} className="btn-primary">
              Oturum Aç
            </button>
          )}

        </div>

      </div>
    </header>
  );
};
