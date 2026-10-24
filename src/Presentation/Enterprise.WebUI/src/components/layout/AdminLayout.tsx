import React from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { LayoutDashboard, Package, Grid, ShoppingBag, Users, ArrowLeft, Shield, LogOut } from 'lucide-react';
import { useAuth } from '../../context/AuthContext';

export const AdminLayout: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const location = useLocation();
  const navigate = useNavigate();
  const { user, logout } = useAuth();

  const menuItems = [
    { path: '/admin', label: 'Genel Bakış', icon: LayoutDashboard },
    { path: '/admin/products', label: 'Ürün Yönetimi', icon: Package },
    { path: '/admin/categories', label: 'Kategori Yönetimi', icon: Grid },
    { path: '/admin/orders', label: 'Sipariş Yönetimi', icon: ShoppingBag },
    { path: '/admin/users', label: 'Üye Yönetimi', icon: Users },
  ];

  return (
    <div style={{ display: 'flex', minHeight: '100vh', background: 'var(--bg-primary)' }}>
      
      {/* Admin Sidebar */}
      <aside style={{
        width: '260px',
        background: 'rgba(15, 23, 42, 0.95)',
        borderRight: '1px solid var(--border-glass)',
        display: 'flex',
        flexDirection: 'column',
        padding: '24px 16px',
        position: 'sticky',
        top: 0,
        height: '100vh'
      }}>
        {/* Sidebar Brand Header */}
        <div style={{ display: 'flex', alignItems: 'center', gap: '12px', padding: '0 12px 24px', borderBottom: '1px solid var(--border-glass)', marginBottom: '24px' }}>
          <div style={{ background: 'linear-gradient(135deg, #6366f1, #06b6d4)', padding: '8px', borderRadius: '10px' }}>
            <Shield size={22} color="#ffffff" />
          </div>
          <div>
            <div style={{ fontWeight: 800, fontSize: '1.05rem', color: '#f3f4f6' }}>Yönetim Paneli</div>
            <div style={{ fontSize: '0.75rem', color: '#6366f1', fontWeight: 600 }}>ENTERPRISE ADMIN</div>
          </div>
        </div>

        {/* Navigation Items */}
        <nav style={{ flex: 1, display: 'flex', flexDirection: 'column', gap: '6px' }}>
          {menuItems.map(item => {
            const Icon = item.icon;
            const isActive = location.pathname === item.path;
            return (
              <Link
                key={item.path}
                to={item.path}
                style={{
                  display: 'flex',
                  alignItems: 'center',
                  gap: '12px',
                  padding: '12px 16px',
                  borderRadius: '10px',
                  textDecoration: 'none',
                  fontSize: '0.9rem',
                  fontWeight: isActive ? 700 : 500,
                  color: isActive ? '#67e8f9' : '#9ca3af',
                  background: isActive ? 'rgba(6, 182, 212, 0.15)' : 'transparent',
                  border: isActive ? '1px solid rgba(6, 182, 212, 0.3)' : '1px solid transparent',
                  transition: 'all 0.2s ease'
                }}
              >
                <Icon size={18} color={isActive ? '#06b6d4' : '#9ca3af'} />
                <span>{item.label}</span>
              </Link>
            );
          })}
        </nav>

        {/* Return to Store & Logout */}
        <div style={{ borderTop: '1px solid var(--border-glass)', paddingTop: '16px', display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <button
            onClick={() => navigate('/')}
            className="btn-secondary"
            style={{ width: '100%', justifyContent: 'flex-start', padding: '10px 14px', fontSize: '0.85rem' }}
          >
            <ArrowLeft size={16} />
            <span>Mağazaya Dön</span>
          </button>

          {user && (
            <button
              onClick={logout}
              className="btn-danger"
              style={{ width: '100%', display: 'flex', alignItems: 'center', gap: '8px', justifyContent: 'flex-start', padding: '10px 14px', fontSize: '0.85rem' }}
            >
              <LogOut size={16} />
              <span>Çıkış Yap ({user.firstName})</span>
            </button>
          )}
        </div>
      </aside>

      {/* Main Viewport Area */}
      <main style={{ flex: 1, padding: '32px 40px', overflowY: 'auto' }}>
        {children}
      </main>

    </div>
  );
};
