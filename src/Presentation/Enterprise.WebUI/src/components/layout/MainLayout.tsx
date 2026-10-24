import React from 'react';
import { Header } from '../common/Header';
import { CartDrawer } from '../cart/CartDrawer';

export const MainLayout: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  return (
    <div style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
      <Header />
      
      <main style={{ flex: 1, maxWidth: '1280px', width: '100%', margin: '0 auto', padding: '32px 24px' }}>
        {children}
      </main>

      <CartDrawer />

      <footer className="glass-panel" style={{ borderRadius: 0, borderBottom: 'none', borderLeft: 'none', borderRight: 'none', marginTop: 'auto' }}>
        <div style={{ maxWidth: '1280px', margin: '0 auto', padding: '24px', textAlign: 'center', color: '#9ca3af', fontSize: '0.85rem' }}>
          © 2026 Enterprise Commerce Platform — .NET Clean Architecture & React SPA
        </div>
      </footer>
    </div>
  );
};
