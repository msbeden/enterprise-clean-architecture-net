import React, { useEffect, useState } from 'react';
import { AdminLayout } from '../../components/layout/AdminLayout';
import { Package, Grid, ShoppingBag, Users, Plus, Activity } from 'lucide-react';
import { apiClient } from '../../services/api';
import { useNavigate } from 'react-router-dom';

export const DashboardPage: React.FC = () => {
  const [productCount, setProductCount] = useState(0);
  const [categoryCount, setCategoryCount] = useState(0);
  const [orderCount, setOrderCount] = useState(0);
  const [userCount, setUserCount] = useState(0);

  const navigate = useNavigate();

  useEffect(() => {
    apiClient.getProducts({ pageSize: 100 }).then(res => {
      if (res.isSuccess && res.value) setProductCount(res.value.totalCount);
    });
    apiClient.getCategories().then(res => {
      if (res.isSuccess && res.value) setCategoryCount(res.value.length);
    });
    apiClient.getOrders().then(res => {
      if (res.isSuccess && res.value) setOrderCount(res.value.length);
    });
    apiClient.getUsers().then(res => {
      if (res.isSuccess && res.value) setUserCount(res.value.length);
    });
  }, []);

  const stats = [
    { title: 'Toplam Ürün', value: productCount, icon: Package, color: '#06b6d4', path: '/admin/products' },
    { title: 'Aktif Kategori', value: categoryCount, icon: Grid, color: '#6366f1', path: '/admin/categories' },
    { title: 'Toplam Sipariş', value: orderCount, icon: ShoppingBag, color: '#10b981', path: '/admin/orders' },
    { title: 'Kayıtlı Üye', value: userCount, icon: Users, color: '#f59e0b', path: '/admin/users' },
  ];

  return (
    <AdminLayout>
      <div style={{ marginBottom: '32px' }}>
        <h1 style={{ fontSize: '1.8rem', fontWeight: 800, color: '#f3f4f6', marginBottom: '6px' }}>Sistem Genel Bakış</h1>
        <p style={{ color: '#9ca3af', fontSize: '0.95rem' }}>Enterprise Clean Architecture - Yönetim ve Kontrol Paneli</p>
      </div>

      {/* Stats Cards */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(220px, 1fr))', gap: '20px', marginBottom: '32px' }}>
        {stats.map((stat, i) => {
          const Icon = stat.icon;
          return (
            <div
              key={i}
              className="glass-card"
              onClick={() => navigate(stat.path)}
              style={{ padding: '24px', cursor: 'pointer', display: 'flex', alignItems: 'center', gap: '16px' }}
            >
              <div style={{ background: `${stat.color}20`, border: `1px solid ${stat.color}40`, padding: '14px', borderRadius: '12px', color: stat.color }}>
                <Icon size={26} />
              </div>
              <div>
                <div style={{ fontSize: '0.85rem', color: '#9ca3af', fontWeight: 500 }}>{stat.title}</div>
                <div style={{ fontSize: '1.6rem', fontWeight: 800, color: '#f3f4f6' }}>{stat.value}</div>
              </div>
            </div>
          );
        })}
      </div>

      {/* Quick Actions & System Info */}
      <div style={{ display: 'grid', gridTemplateColumns: '2fr 1fr', gap: '24px' }}>
        
        <div className="glass-panel" style={{ padding: '24px' }}>
          <h3 style={{ fontSize: '1.1rem', fontWeight: 700, marginBottom: '16px', display: 'flex', alignItems: 'center', gap: '8px' }}>
            <Activity size={20} color="#06b6d4" />
            Hızlı Yönetim Eylemleri
          </h3>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(200px, 1fr))', gap: '16px' }}>
            <button onClick={() => navigate('/admin/products')} className="btn-primary" style={{ justifyContent: 'center', padding: '14px' }}>
              <Plus size={18} /> Yeni Ürün Ekle
            </button>
            <button onClick={() => navigate('/admin/categories')} className="btn-secondary" style={{ justifyContent: 'center', padding: '14px' }}>
              <Plus size={18} /> Yeni Kategori Ekle
            </button>
            <button onClick={() => navigate('/admin/users')} className="btn-secondary" style={{ justifyContent: 'center', padding: '14px' }}>
              <Plus size={18} /> Yeni Üye Tanımla
            </button>
          </div>
        </div>

        <div className="glass-panel" style={{ padding: '24px' }}>
          <h3 style={{ fontSize: '1.1rem', fontWeight: 700, marginBottom: '16px', color: '#f3f4f6' }}>
            Mimari Bilgisi
          </h3>
          <div style={{ fontSize: '0.85rem', color: '#9ca3af', display: 'flex', flexDirection: 'column', gap: '10px' }}>
            <div><strong>Backend:</strong> .NET 8/9 Clean Architecture & Web API</div>
            <div><strong>Auth:</strong> JWT Bearer Token</div>
            <div><strong>Database:</strong> EF Core 9 CodeFirst</div>
            <div><strong>Frontend:</strong> React 18+ TypeScript SPA</div>
          </div>
        </div>

      </div>
    </AdminLayout>
  );
};
